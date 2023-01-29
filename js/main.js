let canvas;
let ctx;

let images;

let bg;
let tree;
let igloo;

let controllersList = [];
let sprites = [];

let assets;
let soundAssets;

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
    PowerUpFire: 5
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
    Bomb: 4,
    rcDitonate: 5
};

class Terrain {
    data;
    width;
    height;
    time;
    soundCallback;
    powerUpList;
    monsters;

    get width() {
        return this.width;
    }

    get height() {
        return this.height;
    }

    constructor(initial, powerUpList, monsters) {
        this.time = 0;
        this.powerUpList = powerUpList;
        this.width = initial[0].length;
        this.height = initial.length;
        this.monsters = [];
        for (let monster of monsters) {
            this.monsters.push({
                homeX: monster.startX * 16,
                x: monster.startX * 16,
                homeY: monster.startY * 16,
                y: monster.startY * 16,
                step: 0,
                waitAfterTurn: monster.waitAfterTurn,
                frameIndex: 0
            });
        }

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
            case TerrainType.PowerUpFire:
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
                    if (!cell.rcAllowed || !cell.owner.rcAllowed || cell.owner.isDie) {
                        cell.bombTime--;
                    }

                    if (cell.bombTime == 0 || (cell.owner.rcDitonate && cell.rcAllowed)) {
                        this.ditonateBomb(x, y, cell.maxBoom);
                    }
                }
            }
        }

        for (let monster of this.monsters) {
            const delta = [{ x: 0, y: 1 }, { x: 1, y: 0 }, { x: -1, y: 0 }, { x: 0, y: -1 }];
            const isWalkable = (monster, delta) => {
                return (this.isWalkable(Int.divRound(monster.x + delta.x * 8 + delta.x, 16),
                                        Int.divRound(monster.y + delta.y * 8 + delta.y, 16)));
            }
            if (monster.wait > 0) {
                monster.wait--;
            } else if (monster.isDie) {
                if (monster.frameIndex < 8) monster.frameIndex += 1 / 5;
            } else {
                if ((isWalkable(monster, delta[monster.step]))) {
                    monster.x += delta[monster.step].x;
                    monster.y += delta[monster.step].y;
                    monster.frameIndex += 1 / 10;
                } else {
                    monster.step = undefined;
                    let rnd = [];
                    for (let i = 0; i < 4; i++) {
                        rnd.push({ i: i, rnd: Math.random() });
                    }
                    rnd.sort((a, b) => a.rnd - b.rnd);

                    for (let i = 0; i < rnd.length && monster.step == undefined; i++) {
                        if (isWalkable(monster, delta[rnd[i].i])) {
                            monster.step = rnd[i].i;
                        }
                    }
                    monster.frameIndex = 0;
                    monster.wait = monster.waitAfterTurn;
                }
                monster.frameIndex %= 4;
            }
            if (this.getCell(Int.divRound(monster.x, 16),
                             Int.divRound(monster.y, 16)).type == TerrainType.Fire && !monster.isDie) {
                monster.isDie = true;
                monster.step = 4;
                console.log("a");
                this.playSound("ai");
            }
        }
        for (let i = this.monsters.length - 1; i >= 0; i--) {
            if (this.monsters[i].frameIndex >= 8) {
                this.setCell(Int.divRound(this.monsters[i].x, 16), Int.divRound(this.monsters[i].y, 16), this.generateGiven());
                this.monsters.splice(i, 1);
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

    ditonateBomb(bombX, bombY) {
        const bombCell = this.getCell(bombX, bombY);;
        const maxBoom = bombCell.maxBoom;
        bombCell.owner.bombsPlaced--;

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
                        type: TerrainType.PowerUpFire,
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
                    this.ditonateBomb(x, y);
                    break;
                } else if (cell.type == TerrainType.Fire || cell.type == TerrainType.PowerUpFire) {
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

function newMap(initial) {
    const rv = new Terrain(initial.map, initial.powerUps, initial.monsters);

    let soundManager = new SoundManager(soundAssets);

    rv.soundCallback = function (sound) {
        soundManager.playSound(sound);
    };
    return rv;
}

async function init() {
    soundAssets = await loadSoundAssets();
    assets = loadAssets();

    map = newMap(mapNeigeInitial);

    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage(assets.niegeBg, 1000 / FPS * 8);

    igloo = new AnimatedImage(assets.niegeIgloo, -1);

    tree = new AnimatedImage(assets.niegeTree, 1000 / FPS * 15);

    sprite = new Sprite(2);
    sprite.x = 17 * 16;
    sprite.y = 1 * 16;
    sprites.push(sprite);
    ctrl = new DemoController("lbrdwbulllwbrrddwbuulllw");
    ctrl.setSprite(sprite);
    controllersList.push(ctrl);
    sprite = new Sprite(0);
    sprite.x = 11 * 16;
    sprite.y = 7 * 16;
    sprites.push(sprite);
    ctrl = new DemoController("lbrdwwbulllwwbrrddwwbuulllww");
    ctrl.setSprite(sprite);
    controllersList.push(ctrl);
    sprite = new Sprite(1);
    sprite.x = 1 * 16;
    sprite.y = 1 * 16;
    sprites.push(sprite);
    ctrl = new DemoController("rbldwwburrrwwbllddwwbuurrrww");
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

    document.getElementById("play-btn").addEventListener("click", () => {
        document.body.setAttribute("state", "game");
        sprites = [];
        map = newMap(mapNeigeInitial);

        let sprite = new Sprite(0);
        sprites.push(sprite);
        let ctrl = new KeyboardController("KeyW", "KeyS", "KeyA", "KeyD", "ControlLeft", "AltLeft");
        ctrl.setSprite(sprite);
        controllersList.push(ctrl);

        sprite = new Sprite(1);
        sprite.x = 15 * 16;
        sprite.y = 11 * 16;
        sprites.push(sprite);
        ctrl = new KeyboardController("ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "ControlRight", "AltRight");
        ctrl.setSprite(sprite);
        controllersList.push(ctrl);

        addEventListener("gamepadconnected", function (e) {
            let sprite = new Sprite(2);
            sprites.push(sprite);

            ctrl = new GamepadController(e.gamepad);
            ctrl.setSprite(sprite);
            controllersList.push(ctrl);
        })
        if (document.body.requestFullscreen) {
            document.body.requestFullscreen();
        } else if (document.body.webkitRequestFullscreen) {
            document.body.webkitRequestFullscreen();
        } else if (document.body.msRequestFullscreen) {
            document.body.msRequestFullscreen();
        }
    });
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

    for (let monster of map.monsters) {
        assets.neigeMonster[monster.step][Math.floor(monster.frameIndex)].draw(ctx, monster.x + 8, monster.y - 2);
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
        if (imageIndex == null) {
            return;
        }
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
    keyRcDitonate;
    sprite;

    constructor(keyUp, keyDown, keyLeft, keyRight, keyBomb, keyRcDitonate) {
        this.keyUp = keyUp;
        this.keyDown = keyDown;
        this.keyLeft = keyLeft;
        this.keyRight = keyRight;
        this.keyBomb = keyBomb;
        this.keyRcDitonate = keyRcDitonate;
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
            this.sprite.playerKeys[PlayerKeys.rcDitonate] = keys[this.keyRcDitonate];
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
            this.sprite.playerKeys[PlayerKeys.Bomb] = currGamePad.buttons[1].pressed;
            this.sprite.playerKeys[PlayerKeys.rcDitonate] = currGamePad.buttons[0].pressed;
        }
    }
}

class DemoController {
    moves;
    currentMove;
    step;

    constructor(moves) {
        this.currentMove = 0;
        this.step = 0;
        this.moves = moves;
    }

    setSprite(sprite) {
        this.sprite = sprite;
    }

    update() {
        this.sprite.playerKeys = {};

        const move = (direction) => {
            if (this.step < ((this.sprite.isHaveRollers) ? 8 : 16)) {
                this.sprite.playerKeys[direction] = true;
                this.step++;
            } else {
                this.currentMove++;
                this.step = 0;
            }
        }

        if (this.moves[this.currentMove] == "l") {
            move(PlayerKeys.Left);
        } else if (this.moves[this.currentMove] == "r") {
            move(PlayerKeys.Right);
        } else if (this.moves[this.currentMove] == "u") {
            move(PlayerKeys.Up);
        } else if (this.moves[this.currentMove] == "d") {
            move(PlayerKeys.Down);
        } else if (this.moves[this.currentMove] == "b") {
            this.sprite.playerKeys[PlayerKeys.Bomb] = true;
            this.currentMove++;
        } else if (this.moves[this.currentMove] == "w") {
            this.sprite.playerKeys[PlayerKeys.rcDitonate] = true;

            if (this.step > 16) {
                this.currentMove++;
                this.step = 0;
            }

            if (this.step > 0) {
                this.step++;
            } else {
                let myBombs = 0;
                for (let y = 0; y < map.height; y++) {
                    for (let x = 0; x < map.width; x++) {
                        if (map.getCell(x, y).owner == this.sprite) {
                            myBombs++;
                        }
                    }
                }
                if (myBombs == 0) {
                    this.step = 1;
                }
            }
        } else {
            this.currentMove++;
        }
        if (this.currentMove >= this.moves.length) {
            this.currentMove = 0;
        }
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;
    frameIndex;

    playerKeys;

    speed;

    maxBoom;
    maxBombsCount;
    bombsPlaced;

    rcAllowed;
    rcDitonate;

    isHaveRollers;

    isDie;

    blinking;
    blinkingSpeed;
    unplugin;

    lifeCount = 0;

    constructor(spriteIndex) {
        this.animations = [];

        this.animateIndex = 0;
        this.frameIndex = 0;

        this.playerKeys = {};

        this.speed = 1;

        this.maxBoom = 1;
        this.maxBombsCount = 1;
        this.bombsPlaced = 0;

        this.rcAllowed = false;

        this.blinking = undefined;
        this.blinkingSpeed = 15;

        for (let img of assets.players[spriteIndex]) {
            this.animations.push(new AnimatedImage(img, -1))
        }

        this.x = 1 * 16;
        this.y = 1 * 16;
    }

    xAlign(deltaY) {
        if (map.isWalkable(Int.divFloor(this.x - 1, 16), Int.divRound(this.y, 16) + deltaY)) {
            this.x -= 1;

            this.animateIndex = 2;
        } else if (map.isWalkable(Int.divCeil(this.x + 1, 16), Int.divRound(this.y, 16) + deltaY)) {
            this.x += 1;

            this.animateIndex = 1;
        }
    }

    yAlign(deltaX) {
        if (map.isWalkable(Int.divRound(this.x, 16) + deltaX, Int.divFloor(this.y - 1, 16))) {
            this.y -= 1;

            this.animateIndex = 3;
        } else if (map.isWalkable(Int.divRound(this.x, 16) + deltaX), Int.divCeil(this.y + 1, 16)) {
            this.y += 1;

            this.animateIndex = 0;
        }
    }

    update() {
        if (this.unplugin >= 0) {
            this.blinking++;
            this.unplugin--;
        } else {
            this.blinking = undefined;
            this.unplugin = undefined;
        }
        if (this.isDie) {
            this.animateIndex = 4;
            if (this.frameIndex < 7 && this.frameIndex != null) {
                this.frameIndex += 1 / 6;
            } else {
                this.frameIndex = null;
            }
            return;
        } else {
            this.frameIndex == 0;
        }

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

        if (this.rcAllowed && this.playerKeys[PlayerKeys.rcDitonate]) {
            this.rcDitonate = true;
        } else {
            this.rcDitonate = false;
        }

        const moveY = (delta) => {
            if (Int.mod(this.x, 16) == 0) {
                const y = (delta < 0) ? Int.divFloor(this.y + delta, 16) : Int.divCeil(this.y + delta, 16);
                if (map.isWalkable(Int.divFloor(this.x, 16), y)) {
                    this.y += delta;
                }
            } else {
                this.xAlign(delta);
            }

            this.frameIndex += 1 / 18;
        }
        const moveX = (delta) => {
            if (Int.mod(this.y, 16) == 0) {
                const x = (delta < 0) ? Int.divFloor(this.x + delta, 16) : Int.divCeil(this.x + delta, 16);
                if (map.isWalkable(x, Int.divFloor(this.y, 16))) {
                    this.x += delta;
                }
            } else {
                this.yAlign(delta);
            }

            this.frameIndex += 1 / 18;
        }
        for (let i = 0; i < this.speed; i++) {
            if (direction == Direction.Up) {
                this.animateIndex = 3;
                moveY(-1);
            } else if (direction == Direction.Down) {
                this.animateIndex = 0;
                moveY(1);
            } else if (direction == Direction.Left) {
                moveX(-1);
                this.animateIndex = 2;
            } else if (direction == Direction.Right) {
                moveX(1);
                this.animateIndex = 1;
            } else {
                this.frameIndex = 0;
            }
        }

        const tileX = Int.divRound(this.x, 16);
        const tileY = Int.divRound(this.y, 16);
        const tile = map.getCell(tileX, tileY);

        let isTouchMonster;
        for (let monster of map.monsters) {
            if (Int.divRound(monster.x, 16) == tileX && Int.divRound(monster.y, 16) == tileY) {
                isTouchMonster = true;
            }
        }
        if (tile.type == TerrainType.Fire || isTouchMonster) {
            if (!this.unplugin) {
                if (this.lifeCount > 0) {
                    this.blinkingSpeed = 30;
                    this.blinking = 0;
                    this.unplugin = 165;
                    this.lifeCount--;
                    this.rcAllowed = false;
                    this.isHaveRollers = false;
                    this.speed = 1;
                    this.maxBombsCount = Math.min(this.maxBombsCount, 3);
                    map.playSound("oioi");
                } else {
                    this.isDie = true;
                    this.frameIndex = 0;
                    map.playSound("player_die");
                }
            }
        }

        if (this.playerKeys[PlayerKeys.Bomb]) {
            if (tile.type == TerrainType.Free && this.bombsPlaced < this.maxBombsCount) {
                map.setCell(Int.divRound(this.x, 16), Int.divRound(this.y, 16), {
                    type: TerrainType.Bomb,
                    image: assets.bomb,
                    imageIdx: 0,
                    animateDelay: 12,
                    bombTime: 210,
                    maxBoom: this.maxBoom,
                    rcAllowed: this.rcAllowed,
                    owner: this
                });
                map.playSound("posebomb");
                this.bombsPlaced++;
            }
        }

        if (tile.type == TerrainType.PowerUp) {
            const powerUpType = tile.powerUpType;
            let doFire = false;

            if (powerUpType == PowerUpType.ExtraFire) {
                this.maxBoom++;
            } else if (powerUpType == PowerUpType.ExtraBomb) {
                this.maxBombsCount++;
            } else if (powerUpType == PowerUpType.RemoteControl) {
                if (!this.rcAllowed) {
                    this.rcAllowed = true;
                } else {
                    doFire = true;
                }
            } else if (powerUpType == PowerUpType.RollerSkate) {
                if (!this.isHaveRollers) {
                    this.speed = 2;
                    this.isHaveRollers = true;
                } else {
                    doFire = true;
                }
            } else if (powerUpType == PowerUpType.Life) {
                this.lifeCount++;
            } else if (powerUpType == PowerUpType.Shield) {
                this.unplugin = 600;
                this.blinkingSpeed = 30;
                this.blinking = 0;
            }

            if (doFire) {
                map.setCell(tileX, tileY, {
                    type: TerrainType.PowerUpFire,
                    image: assets.fire,
                    imageIdx: 0,
                    animateDelay: 6,
                    next: {
                        type: TerrainType.Free
                    }
                });
                map.playSound("sac");
            } else {
                map.setCell(tileX, tileY, {
                    type: TerrainType.Free
                });
                map.playSound("pick");
            }
        }
    }

    tick() {
        this.animations[this.animateIndex].tick();
    }

    draw(ctx) {
        if (this.blinking % this.blinkingSpeed * 2 < this.blinkingSpeed) {
            ctx.filter = "brightness(0) invert(1)";
        }
        let frameIndex = (this.frameIndex == null) ? null : Math.floor(this.frameIndex);
        this.animations[this.animateIndex].draw(ctx, this.x + 5, this.y - 7, frameIndex);
        ctx.filter = "none";
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