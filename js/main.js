let canvas;
let ctx;

let images;

let bg;
let tree;
let igloo;

let controllersList = [];
let sprites = [];

let assets;

let cheats = {
    noClip: false
};

const TerrainType =
{
    Free: 0,
    PermanentWall: 1,
    TemporaryWall: 2,
    Bomb: 3,
    PowerUp: 4,
};

const PowerUpType = {
    Banana: 0,
    ExtraBomb: 1,
    ExtraFire: 2,
    Skull: 3,
    Shield: 4,
    Life: 5,
    RemoteControl: 6,
    Kick: 7,
    RollerSkate: 8,
    Clock: 9,
    MultiBomb: 10
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
    time;
    soundCallback;
    powerUpList;

    get width() {
        return this.width;
    }

    get height() {
        return this.height;
    }

    constructor(initial, powerUpList) {
        this.time = 0;
        this.powerUpList = powerUpList;
        this.width = initial[0].length;
        this.height = initial.length;

        this.data = new Array(this.width * this.height);

        for (let y = 0; y < this.height; y++) {
            for (let x = 0; x < this.width; x++) {
                let src = initial[y][x];

                if (src == '#') {
                    this.data[y * this.width + x] = {
                        type: TerrainType.PermanentWall,
                    };
                } else if (src == '-') {
                    this.data[y * this.width + x] = {
                        type: TerrainType.TemporaryWall,
                        image: assets.niegeWall
                    };
                } else {
                    this.data[y * this.width + x] = {
                        type: TerrainType.Free
                    };
                }
            }
        }
    }

    getCell(x, y) {
        if (x >= 0 && x < this.width && y >= 0 && y < this.height) {
            return this.data[y * this.width + x];
        }
        else {
            return {
                type: TerrainType.PermanentWall
            };
        }
    }

    setCell(x, y, cell) {
        this.data[y * this.width + x] = cell;
    }

    isWalkable(x, y) {
        let cell = this.getCell(x, y);

        switch (cell.type) {
            case TerrainType.Free:
            case TerrainType.Bomb:
                return true;

            case TerrainType.PermanentWall:
                return false;

            case TerrainType.TemporaryWall:
                return cheats.noClip;

            default:
                return true;
        }
    }

    update() {
        this.soundsToPlay = {};

        this.time++;

        for (let y = 0; y < this.height; y++) {
            for (let x = 0; x < this.width; x++) {
                let cell = this.getCell(x, y);

                if (cell.imageIdx !== undefined) {
                    const animateDelay = cell.animateDelay || 6;
                    if (Int.mod(this.time, animateDelay) == 0) {
                        cell.imageIdx++;
                        if (cell.imageIdx >= cell.image.length) {
                            if (cell.next) {
                                this.setCell(x, y, cell.next);
                            } else {
                                cell.imageIdx = 0;
                            }
                        }
                    }
                }

                if (cell.bombTime) {
                    cell.bombTime--;

                    if (cell.bombTime == 0) {
                        this.ditonateBomb(x, y, cell.maxBoom);
                    }
                }
            }
        }

        for (let sprite of sprites) {
            sprite.update(1);
        }

        if (this.soundCallback) {
            for (let sound in this.soundsToPlay) {
                if (this.soundsToPlay[sound]) {
                    this.soundCallback(sound);
                }
            }
        }
    }

    generateGiven() {
        let rnd = Math.random();
        if (rnd < 0.5) {
            const powerUpIndex = Math.floor(Math.random() * this.powerUpList.length);
            const powerUpType = this.powerUpList[powerUpIndex];
            return {
                type: TerrainType.PowerUp,
                image: assets.powerups[powerUpType],
                imageIdx: 0,
                animateDelay: 8,
                powerUpType: powerUpType
            };
        } else {
            return {
                type: TerrainType.Free
            };
        }
    }

    ditonateBomb(bombX, bombY, maxBoom) {
        let burn = (dx, dy, image, imageEnd) => {
            for (let i = 1; i <= maxBoom; i++) {
                const x = bombX + i * dx;
                const y = bombY + i * dy;
                const cell = map.getCell(x, y);

                if (cell.type == TerrainType.PermanentWall) {
                    break;
                };

                if (cell.type == TerrainType.TemporaryWall) {
                    let next = this.generateGiven();
                    map.setCell(x, y, {
                        type: TerrainType.PermanentWall,
                        image: assets.niegeWall,
                        imageIdx: 0,
                        animateDelay: 4,
                        next: next
                    });
                    break;
                } else if (cell.type == TerrainType.PowerUp) {
                    map.setCell(x, y, {
                        type: TerrainType.Free,
                        image: assets.fire,
                        imageIdx: 0,
                        animateDelay: 6,
                        next: {
                            type: TerrainType.Free
                        }
                    });
                    this.playSound("sac");
                    break;
                } else if (cell.type == TerrainType.Bomb) {
                    this.ditonateBomb(x, y, cell.maxBoom);
                    break;
                } else if (cell.type == TerrainType.Fire) {
                } else {
                    map.setCell(x, y, {
                        type: TerrainType.Fire,
                        image: i == maxBoom ? imageEnd : image,
                        imageIdx: 0,
                        next: {
                            type: TerrainType.Free
                        }
                    });
                }
            }
        }

        this.playSound("bang");

        map.setCell(bombX, bombY, {
            type: TerrainType.Fire,
            image: assets.boomMid,
            imageIdx: 0,
            next: {
                type: TerrainType.Free
            }
        });

        burn(1, 0, assets.boomHor, assets.boomRightEnd);
        burn(-1, 0, assets.boomHor, assets.boomLeftEnd);
        burn(0, 1, assets.boomVert, assets.boomBottomEnd);
        burn(0, -1, assets.boomVert, assets.boomTopEnd);
    }

    playSound(sound) {
        this.soundsToPlay[sound] = true;
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

const FPS = 60;

let keys = {};

addEventListener("load", function () {
    init();
});

async function init() {
    assets = loadAssets();
    soundAssets = await loadSoundAssets();

    map = new Terrain(
        [
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
        ],
        [
            PowerUpType.ExtraBomb,
            PowerUpType.ExtraFire
        ]);

    let soundManager = new SoundManager(soundAssets);

    map.soundCallback = function (sound) {
        soundManager.playSound(sound);
    };

    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage(assets.niegeBg, 1000 / FPS * 8);

    igloo = new AnimatedImage(assets.niegeIgloo, -1);

    tree = new AnimatedImage(assets.niegeTree, 1000 / FPS * 15);

    let sprite = new Sprite(0);
    sprites.push(sprite);
    let ctrl = new KeyboardController("KeyW", "KeyS", "KeyA", "KeyD", "ControlLeft");
    ctrl.setSprite(sprite);
    controllersList.push(ctrl);

    sprite = new Sprite(1);
    sprite.x = 15*16;
    sprite.y = 11*16;
    sprites.push(sprite);
    ctrl = new KeyboardController("ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "ControlRight");
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
    map.update();
}

function drawAll(interpolationPercentage) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);

    for (let y = 0; y < map.height; y++) {
        for (let x = 0; x < map.width; x++) {
            const cell = map.getCell(x, y);

            if (cell.image) {
                const image = cell.image[cell.imageIdx || 0];

                image.draw(ctx,
                    x * 16 + 8 + 8 - Int.divFloor(image.rect.width, 2),
                    y * 16 + 16 - image.rect.height);
            }
        }
    }

    var spritesToDraw = sprites;
    spritesToDraw.sort((a, b) => { return a.y - b.y; });

    for (let sprite of spritesToDraw) {
        sprite.draw(ctx)
    }

    igloo.draw(ctx, 232, 57);
    tree.draw(ctx, 112, 30);
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
            this._time += 1 / this.delay;

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

    maxBoom;

    constructor(spriteIndex) {
        this.animations = [];

        this.animateIndex = 0;

        this.playerKeys = {};

        this.speed = 1;

        this.maxBoom = 1;

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
        } else if (map.isWalkable(Int.divRound(this.x, 16) + deltaX), Int.divCeil(this.y + 1, 16)) {
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

        const tileX = Int.divRound(this.x, 16);
        const tileY = Int.divRound(this.y, 16);
        const tile = map.getCell(tileX, tileY);

        if (this.playerKeys[PlayerKeys.Bomb]) {
            if (tile.type == TerrainType.Free) {
                map.setCell(Int.divRound(this.x, 16), Int.divRound(this.y, 16), {
                    type: TerrainType.Bomb,
                    image: assets.bomb,
                    imageIdx: 0,
                    animateDelay: 12,
                    bombTime: 210,
                    maxBoom: this.maxBoom
                });
                map.playSound("posebomb");
            }
        }

        if (tile.type == TerrainType.PowerUp) {
            const powerUpType = tile.powerUpType;
            if (powerUpType == PowerUpType.ExtraFire) {
                this.maxBoom++;
            }

            map.setCell(tileX, tileY, {
                type: TerrainType.Free
            });
            map.playSound("pick");
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