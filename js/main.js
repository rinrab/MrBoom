let canvas;
let ctx;

let images;

let bg;
let banana;
let tree;
let igloo;

let controllersList = [];
let sprites = [];

let boomSpriteMid;
let boomSpriteHor;
let boomSpriteVert;
let boomSpriteLeft;
let boomSpriteRight;
let boomSpriteTop;
let boomSpriteBottom;
let assets;

const spriteWidth = 24;
const spriteHeight = 24;

let cheats = {
    noClip: false
};

const TerrainType =
{
    Free: 0,
    PermanentWall: 1,
    TemporaryWall: 2
};

const Direction =
{
    Up: 0,
    Left: 1,
    Right: 2,
    Down: 3
};

const PlayerKeys =
{
    Up: 0,
    Left: 1,
    Right: 2,
    Down: 3,
    Bomb: 4
};

class Terrain {
    data;
    width;
    height;

    get width() {
        return this.width;
    }

    get height() {
        return this.height;
    }

    constructor(initial) {
        this.width = initial[0].length;
        this.height = initial.length;

        this.data = new Uint8Array(this.width * this.height);

        for (let y = 0; y < this.height; y++) {
            for (let x = 0; x < this.width; x++) {
                let src = initial[y][x];

                if (src == '#') {
                    this.data[y * this.width + x] = TerrainType.PermanentWall;
                } else if (src == '-') {
                    this.data[y * this.width + x] = TerrainType.TemporaryWall;
                } else {
                    this.data[y * this.width + x] = TerrainType.Free;
                }
            }
        }
    }

    get(x, y) {
        if (x >= 0 && x < this.width && y >= 0 && y < this.height) {
            return this.data[y * this.width + x];
        }
        else {
            return TerrainType.PermanentWall;
        }
    }

    set(x, y, type) {
        if (x >= 0 && x < this.width && y >= 0 && y < this.height) {
            this.data[y * this.width + x] = type;
            return true;
        }
        else {
            return false;
        }
    }

    isWalkable(x, y) {
        let cellType = this.get(x, y);

        switch (cellType) {
            case TerrainType.Free:
                return true;

            case TerrainType.PermanentWall:
                return false;

            case TerrainType.TemporaryWall:
                return cheats.noClip;

            default:
                return true;
        }
    }
}

class Int {
    static mod(val, divider) {
        return Math.floor(val) % divider;
    }

    static divFloor(val, divider) {
        return Math.floor(val / divider);
    }

    static divCeil(val, divider) {
        return Math.floor((val + divider - 1) / divider);
    }

    static divRound(val, divider) {
        return Math.round(val / divider);
    }
}

let map;

let gridImage;

const FPS = 60;

let keys = {};

let bombSprite;
let bombs = [];

let distroingTiles = [];

addEventListener("load", function () {
    init();
});

function init() {
    map = new Terrain([
        "###################",
        "#  -------------  #",
        "# #-# #-#-#-#-#-# #",
        "#---  ------  ----#",
        "#-#-#-###-#-# # #-#",
        "#-----###-----   -#",
        "#-#-#-###-#-#-# #-#",
        "#---------  ------#",
        "#-#-#-# #-# #-#-#-#",
        "#-----  ----------#",
        "# #-#-#-#-#-#-# ###",
        "#  -----------  ###",
        "###################"
    ]);

    assets = loadAssets();

    bombSprite = new AnimatedImage(assets.bomb, 10)

    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage(assets.niegeBg, 1000 / FPS * 8);

    banana = new AnimatedImage(assets.banana, 1000 / FPS * 5);

    boomSpriteMid = new AnimatedImage(assets.boomMid, -1);
    boomSpriteHor = new AnimatedImage(assets.boomHor, -1);
    boomSpriteLeft = new AnimatedImage(assets.boomLeftEnd, -1);
    boomSpriteRight = new AnimatedImage(assets.boomRightEnd, -1);
    boomSpriteVert = new AnimatedImage(assets.boomVert, -1);
    boomSpriteTop = new AnimatedImage(assets.boomTopEnd, -1);
    boomSpriteBottom = new AnimatedImage(assets.boomBottomEnd, -1);
    igloo = new AnimatedImage(assets.niegeIgloo, -1);

    tree = new AnimatedImage(assets.niegeTree, 1000 / FPS * 15);

    gridImage = new AnimatedImage(assets.niegeWall, -1);

    sprite = new Sprite(1);
    sprites.push(sprite);

    let ctrl = new KeyboardController("KeyW", "KeyS", "KeyA", "KeyD", "Space");
    ctrl.setSprite(sprite);
    controllersList.push(ctrl);

    MainLoop.setBegin(begin);
    MainLoop.setUpdate(update);
    MainLoop.setDraw(drawAll);
    MainLoop.setEnd(end);
    MainLoop.start();

    addEventListener("keydown", function (e) {
        keys[e.code] = true;
    })

    addEventListener("keyup", function (e) {
        keys[e.code] = false;
    })

    addEventListener("gamepadconnected", function (e) {
        let sprite = new Sprite(2);
        sprites.push(sprite);

        ctrl = new GamepadController(e.gamepad);
        ctrl.setSprite(sprite);
        controllersList.push(ctrl);
    })
}

function getDieingSprite(x, y) {
    const rv = new AnimatedImage([
        { id: "PAUSE", rect: new Rect(1 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(2 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(3 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(4 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(5 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(6 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(7 * 16, 80, 16, 16) }
    ], 30 / 6);
    rv.x = x;
    rv.y = y;
    rv.time = 30;
    return rv;
}

function begin(timestamp, delta) {
    for (let c of controllersList) {
        c.update();
    }
}

function update(deltaTime) {
    for (let sprite of sprites) {
        sprite.update(1);
    }
    for (let bomb of bombs) {
        if (bomb.ditonate >= 0) {
            bomb.ditonate--;
        } else if (bomb.time < 0) {
            ditonateBomb(bomb, 3);
        } else {
            bomb.time--;
        }
        bombSprite.tick();
    }
    for (let i = bombs.length - 1; i >= 0; i--) {
        if (bombs[i].ditonate == 0) {
            bombs.splice(i, 1);
        }
    }
}


function ditonateBomb(bomb, maxBoom) {
    bomb.ditonate = 30 * 1;

    function distroy(x, y) {
        map.set(x, y, TerrainType.Free);
        // let newSprite = getDieingSprite(x * 16 + 8, y * 16);
        // distroingTiles.push(newSprite);
    }

    bomb.right = 0;
    while (bomb.right < maxBoom) {
        const tile = map.get(bomb.x + bomb.right, bomb.y);
        if (tile == TerrainType.PermanentWall) {
            bomb.right--;
            break;
        };
        if (tile == TerrainType.TemporaryWall) {
            distroy(bomb.x + bomb.right, bomb.y);
            break;
        }
        bomb.right++;
    }

    bomb.left = 0;
    while (bomb.left < maxBoom) {
        const tile = map.get(bomb.x - bomb.left, bomb.y);
        if (tile == TerrainType.PermanentWall) {
            bomb.left--;
            break;
        };
        if (tile == TerrainType.TemporaryWall) {
            distroy(bomb.x - bomb.left, bomb.y);
            break;
        }
        bomb.left++;
    }
    bomb.bottom = 0;
    while (bomb.bottom < maxBoom) {
        const tile = map.get(bomb.x, bomb.y + bomb.bottom);
        if (tile == TerrainType.PermanentWall) {
            bomb.bottom--;
            break;
        };
        if (tile == TerrainType.TemporaryWall) {
            distroy(bomb.x, bomb.y + bomb.bottom);
            break;
        }
        bomb.bottom++;
    }

    bomb.top = 0;
    while (bomb.top < maxBoom) {
        const tile = map.get(bomb.x, bomb.y - bomb.top);
        if (tile == TerrainType.PermanentWall) {
            bomb.top--;
            break;
        };
        if (tile == TerrainType.TemporaryWall) {
            distroy(bomb.x, bomb.y - bomb.top);
            break;
        }
        bomb.top++;
    }
}

function drawAll(interpolationPercentage) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);

    for (let y = 0; y < map.height; y++) {
        for (let x = 0; x < map.width; x++) {
            if (map.get(x, y) == TerrainType.TemporaryWall) {
                gridImage.draw(ctx, x * 16 + 8, y * 16);
            }
        }
    }

    banana.draw(ctx, 16 * 4 + 8, 16 * 3)
    var spritesToDraw = sprites;
    spritesToDraw.sort((a, b) => { return a.y - b.y; });

    drawBombs();
    for (let tile of distroingTiles) {
        tile.tick();
        tile.time--;
        tile.draw(ctx, tile.x, tile.y);
    }
    for (let i = distroingTiles.length - 1; i >= 0; i--) {
        if (distroingTiles[i].time <= 0) {
            distroingTiles.splice(i, 1);
        }
    }
    for (let sprite of spritesToDraw) {
        sprite.draw(ctx)
    }

    igloo.draw(ctx, 232, 57);
    tree.draw(ctx, 112, 30);
}


function drawBombs() {
    for (let bomb of bombs) {
        if (bomb.ditonate == -1) {
            bombSprite.draw(ctx, bomb.x * 16 + 8, bomb.y * 16);
        } else {
            const frame = Math.floor((30 - bomb.ditonate) / 30 * 4);
            boomSpriteMid.draw(ctx, bomb.x * 16 + 8, bomb.y * 16, frame);

            for (let i = 1; i <= bomb.right; i++) {
                if (i == bomb.right) {
                    boomSpriteRight.draw(ctx, (bomb.x + i) * 16 + 8, bomb.y * 16, frame);
                } else {
                    boomSpriteHor.draw(ctx, (bomb.x + i) * 16 + 8, bomb.y * 16, frame);
                }
            }
            for (let i = 1; i <= bomb.left; i++) {
                if (i == bomb.left) {
                    boomSpriteLeft.draw(ctx, (bomb.x - i) * 16 + 8, bomb.y * 16, frame);
                } else {
                    boomSpriteHor.draw(ctx, (bomb.x - i) * 16 + 8, bomb.y * 16, frame);
                }
            }
            for (let i = 1; i <= bomb.bottom; i++) {
                if (i == bomb.bottom) {
                    boomSpriteBottom.draw(ctx, bomb.x * 16 + 8, (bomb.y + i) * 16, frame);
                } else {
                    boomSpriteVert.draw(ctx, bomb.x * 16 + 8, (bomb.y + i) * 16, frame);
                }
            }
            for (let i = 1; i <= bomb.top; i++) {
                if (i == bomb.top) {
                    boomSpriteTop.draw(ctx, bomb.x * 16 + 8, (bomb.y - i) * 16, frame);
                } else {
                    boomSpriteVert.draw(ctx, bomb.x * 16 + 8, (bomb.y - i) * 16, frame);
                }
            }

            console.log(frame);
        }
    }
}

function end(fps, panic) {
    document.getElementById("fps-display").innerText = Math.round(fps) + ' FPS';

    if (panic) {
        // This pattern introduces non-deterministic behavior, but in this case
        // it's better than the alternative (the application would look like it
        // was running very quickly until the simulation caught up to real
        // time). See the documentation for `MainLoop.setEnd()` for additional
        // explanation.
        var discardedTime = Math.round(MainLoop.resetFrameDelta());
        console.warn('Main loop panicked, probably because the browser tab was put in the background. Discarding ' + discardedTime + 'ms');
    }
}

function placeBomb(x, y) {
    if (!bombs.find(function (val) {
        return val.x == x && val.y == y;
    })) {
        bombs.push({
            x: x,
            y: y,
            time: 3 * 60,
            ditonate: -1
        })
    }
}

class AnimatedImage {
    images;
    currentImage;
    delay;

    _time;

    constructor(images, delay) {
        this.images = images;

        this.delay = delay;
        this._time = 0;

        this.tick();
    }

    tick() {
        if (this.delay == -1) {
            return this.currentImage = this.images[0];
        } else {
            this._time += 1/this.delay;

            return this.currentImage = this.images[Math.floor(this._time % this.images.length)];
        }
    }

    draw(ctx, x = 0, y = 0, imageIndex = -1) {
        let img;
        if (imageIndex == -1) {
            img = this.currentImage;
        } else {
            img = this.images[imageIndex % this.images.length];
        }
        ctx.drawImage(
            img.img,
            img.rect.x, img.rect.y,
            img.rect.width, img.rect.height,
            Math.round(x), Math.round(y),
            img.rect.width, img.rect.height);
    }
}

class KeyboardController {
    keyUp;
    keyDown;
    keyLeft;
    keyRight;
    keyBomb;
    sprite;

    constructor(keyUp, keyDown, keyLeft, keyRight, keyBomb) {
        this.keyUp = keyUp;
        this.keyDown = keyDown;
        this.keyLeft = keyLeft;
        this.keyRight = keyRight;
        this.keyBomb = keyBomb;
        this.sprite = null;
    }

    setSprite(sprite) {
        this.sprite = sprite;
    }

    update() {
        if (this.sprite) {
            this.sprite.playerKeys[PlayerKeys.Up] = keys[this.keyUp];
            this.sprite.playerKeys[PlayerKeys.Down] = keys[this.keyDown];
            this.sprite.playerKeys[PlayerKeys.Left] = keys[this.keyLeft];
            this.sprite.playerKeys[PlayerKeys.Right] = keys[this.keyRight];
            this.sprite.playerKeys[PlayerKeys.Bomb] = keys[this.keyBomb];
        }
    }
}

class GamepadController {
    gamepad;
    sprite;

    constructor(gamepad) {
        this.gamepad = gamepad;
        this.sprite = null;
    }

    setSprite(sprite) {
        this.sprite = sprite;
    }

    update() {
        if (this.sprite) {
            const currGamePad = navigator.getGamepads()[this.gamepad.index];

            this.sprite.playerKeys[PlayerKeys.Up] = currGamePad.buttons[12].pressed;
            this.sprite.playerKeys[PlayerKeys.Down] = currGamePad.buttons[13].touched;
            this.sprite.playerKeys[PlayerKeys.Left] = currGamePad.buttons[14].touched;
            this.sprite.playerKeys[PlayerKeys.Right] = currGamePad.buttons[15].touched;
            this.sprite.playerKeys[PlayerKeys.Bomb] = currGamePad.buttons[0].pressed;
        }
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;

    playerKeys;

    speed;

    constructor(spriteIndex) {
        this.animations = [];

        this.animateIndex = 0;

        this.playerKeys = {};

        this.speed = 1;

        for (let img of assets.players[spriteIndex]) {
            this.animations.push(new AnimatedImage(img, -1))
        }

        this.x = 1 * 16;
        this.y = 1 * 16;
    }

    xAlign(deltaY) {
        if (map.isWalkable(Int.divFloor(this.x - 1, 16), Int.divRound(this.y, 16) + deltaY)) {
            this.x -= this.speed;

            this.animateIndex = 2;
        } else if (map.isWalkable(Int.divCeil(this.x + 1, 16), Int.divRound(this.y, 16) + deltaY)) {
            this.x += this.speed;

            this.animateIndex = 1;
        }
    }

    yAlign(deltaX) {
        if (map.isWalkable(Int.divRound(this.x, 16) + deltaX, Int.divFloor(this.y - 1, 16))) {
            this.y -= this.speed;

            this.animateIndex = 3;
        } else if (map.isWalkable(Int.divRound(this.x, 16) + deltaX) , Int.divCeil(this.y + 1, 16)) {
            this.y += this.speed;

            this.animateIndex = 0;
        }
    }

    update() {
        let direction;

        if (this.playerKeys[PlayerKeys.Up]) {
            direction = Direction.Up;
        } else if (this.playerKeys[PlayerKeys.Left]) {
            direction = Direction.Left;
        } else if (this.playerKeys[PlayerKeys.Right]) {
            direction = Direction.Right;
        } else if (this.playerKeys[PlayerKeys.Down]) {
            direction = Direction.Down;
        }

        if (direction == Direction.Up) {
            if (Int.mod(this.x, 16) == 0) {
                if (map.isWalkable(Int.divFloor(this.x, 16), Int.divFloor(this.y - this.speed, 16))) {
                    this.y -= this.speed;
                }

                this.animateIndex = 3;
            } else {
                this.xAlign(-1);
            }

            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (direction == Direction.Down) {
            if (Int.mod(this.x, 16) == 0) {
                if (map.isWalkable(Int.divFloor(this.x, 16), Int.divCeil(this.y + this.speed, 16))) {
                    this.y += this.speed;
                }

                this.animateIndex = 0;
            } else {
                this.xAlign(1);
            }

            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (direction == Direction.Left) {
            if (Int.mod(this.y, 16) == 0) {
                if (map.isWalkable(Int.divFloor(this.x - this.speed, 16), Int.divFloor(this.y, 16))) {
                    this.x -= this.speed;
                }

                this.animateIndex = 2;
            }
            else {
                this.yAlign(-1);
            }

            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (direction == Direction.Right) {
            if (Int.mod(this.y, 16) == 0) {
                if (map.isWalkable(Int.divCeil(this.x + this.speed, 16), Int.divFloor(this.y, 16))) {
                    this.x += this.speed;
                }

                this.animateIndex = 1;
            } else {
                this.yAlign(1);
            }

            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else {
            this.animations[this.animateIndex].delay = -1;
        }

        if (this.playerKeys[PlayerKeys.Bomb]) {
            placeBomb(Int.divRound(this.x, 16), Int.divRound(this.y, 16));
        }
    }

    tick() {
        this.animations[this.animateIndex].tick();
    }

    draw(ctx) {
        this.animations[this.animateIndex].draw(ctx, this.x + 5, this.y - 7);
    }
}

class Rect {
    x;
    y;
    width;
    height;

    constructor(x, y, width, height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}