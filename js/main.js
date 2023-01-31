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

let soundManager;
let music;

let cheats = {
    noClip: false
};

let startMenu;

let elemFpsDisplay;

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

const States = {
    start: 0,
    game: 1,
    results: 2,
    draw: 3
}
let state = States.game;
let isDemo = true;

class Terrain {
    data;
    width;
    height;
    time;
    soundCallback;
    powerUpList;
    monsters;
    spawns;
    timeLeft;

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
        this.monsters = [];
        this.spawns = [];
        this.timeLeft = 121;
        
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
                } else if (src == '*') {
                    this.spawns.push({ x: x, y: y });
                    this.data[y * this.width + x] = {
                        type: TerrainType.Free
                    };
                } else {
                    this.data[y * this.width + x] = {
                        type: TerrainType.Free
                    };
                }
            }
        }
    }

    spawnMonsters(monsters) {
        for (let i = 0; i < 8 - sprites.length; i++) {
            const monster = monsters[Int.random(monsters.length)];
            const spawn = this.spawns[this.generateSpawn()];
            this.monsters.push({
                homeX: monster.startX * 16,
                x: spawn.x * 16,
                homeY: monster.startY * 16,
                y: spawn.y * 16,
                step: 0,
                waitAfterTurn: monster.waitAfterTurn,
                frameIndex: 0
            });
        }
    }

    generateSpawn(spawnIndex = -1) {
        if (spawnIndex == -1) {
            let indexList = []
            for (let i = 0; i < this.spawns.length; i++) {
                if (!this.spawns[i].busy) {
                    indexList.push(i);
                }
            }
            spawnIndex = indexList[Int.random(indexList.length)];
        }
        this.spawns[spawnIndex].busy = true;
        return spawnIndex;
    }

    locateSprite(sprite, index = -1) {
        const spawn = this.spawns[this.generateSpawn(index)];
        sprite.x = spawn.x * 16;
        sprite.y = spawn.y * 16;
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
        this.timeLeft -= 1 / 60;

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
            const delta = [{ x: 0, y: 1 }, { x: 1, y: 0 }, { x: -1, y: 0 }, { x: 0, y: -1 }, {}, { x: 0, y: 0 }];
            const isWalkable = (monster, delta) => {
                switch (this.getCell(Int.divRound(monster.x + delta.x * 8 + delta.x, 16),
                    Int.divRound(monster.y + delta.y * 8 + delta.y, 16)).type) {
                    case TerrainType.Free: case TerrainType.PowerUpFire:
                        return true;

                    case TerrainType.PermanentWall: case TerrainType.TemporaryWall:
                    case TerrainType.Bomb: case TerrainType.PowerUp: case TerrainType.PowerUpFire:
                    case TerrainType.Fire:
                        return false;

                    default: return true;
                }
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
                    if (monster.step == undefined) {
                        monster.step = 5;
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
                this.playSound("ai");
            }
        }
        for (let i = this.monsters.length - 1; i >= 0; i--) {
            if (this.monsters[i].frameIndex >= 8) {
                this.setCell(Int.divRound(this.monsters[i].x, 16), Int.divRound(this.monsters[i].y, 16), {
                    type: TerrainType.PowerUp,
                    image: assets.powerups[PowerUpType.Life],
                    imageIdx: 0,
                    animateDelay: 8,
                    powerUpType: PowerUpType.Life
                });
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

        let playersCount = 0;
        for (let sprite of sprites) {
            if (!sprite.isDie) {
                playersCount++;
            }
        }
        if (this.timeLeft < 0 || playersCount == 0) {
            state = States.draw;
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

    static random(max) {
        return Math.floor(Math.random() * max);
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

    rv.soundCallback = function (sound) {
        soundManager.playSound(sound);
    };

    bg = { images: assets.niegeBg, time: 0 };

    return rv;
}

function createTextImage(text, filter) {
    let width = 0;
    let height = 1;

    for (let ch of text) {
        if (ch == "\n") {
            width = 0;
            height++;
        } else {
            width++;
        }
    }

    const canvas = document.createElement("canvas");
    canvas.width = width * 8;
    canvas.height = height * 10;

    const ctx = canvas.getContext("2d");
    ctx.imageSmoothingEnabled = false;
    if (filter) {
        ctx.filter = filter;
    }

    let x = 0;
    let y = 0;
    for (let ch of text) {
        if (ch == "\n") {
            x = 0;
            y++;
        } else {
            drawString(ctx, x * 8, y * 10, ch);
            x++;
        }
    }

    return canvas;
}

async function init() {
    elemFpsDisplay = document.getElementById("fps-display");

    soundAssets = await loadSoundAssets();
    assets = loadAssets();

    soundManager = new SoundManager(soundAssets);

    music = new MusicManager([
        "music/anar11.mp3",
        "music/chipmunk.mp3",
        "music/chiptune.mp3",
        "music/deadfeel.mp3",
        "music/drop.mp3",
        "music/external.mp3",
        "music/matkamie.mp3",
        "music/unreeeal.mp3",
    ]);

    startMenu = new StartMenu()
    map = newMap(mapNeigeInitial);

    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d", { alpha: false });


    igloo = new AnimatedImage(assets.niegeIgloo, -1);

    tree = { images: assets.niegeTree, time: 0 };

    startGame([]);

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

    controllersList.push(new KeyboardController("KeyW", "KeyS", "KeyA", "KeyD", "ControlLeft", "AltLeft"));
    controllersList.push(new KeyboardController("ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "ControlRight", "AltRight"));
    addEventListener("gamepadconnected", function (e) {
        ctrl = new GamepadController(e.gamepad);
        controllersList.push(ctrl);
    });

    assets.subtitles = createTextImage(helpText, "brightness(0) invert(1)");
    assets.joinUs = createTextImage("join\n us \n !! ");
    assets.pushFire = createTextImage("push\nfire\n !! ");

    document.getElementById("play-btn").addEventListener("click", () => {
        document.body.setAttribute("state", "game");
        sprites = [];
        map = newMap(mapNeigeInitial);

        if (document.body.requestFullscreen) {
            document.body.requestFullscreen();
        } else if (document.body.webkitRequestFullscreen) {
            document.body.webkitRequestFullscreen();
        } else if (document.body.msRequestFullscreen) {
            document.body.msRequestFullscreen();
        }
        state = States.start;

        music.start(3);
    });
}

const helpText = "welcome to mr.boom v0.1!   right keyboard controller:   " +
    "use arrows to move   use ctr to drop bomb   use alt to triger it by radio control   " +
    "gamepad controller:   use d-pad arrows to move   use   use b button to drop bomb   use a button to triger it by radio control";

function begin(timestamp, delta) {
    for (let c of controllersList) {
        c.update();
    }
}

function update(deltaTime) {
    if (state == States.game) {
        map.update();
    } else if (state == States.start) {
        startMenu.update();
    }
}

let menustep = 0;

const alpha = "abcdefghijklmnopqrstuvwxyz0123456789!.-:/()?";
function drawString(ctx, x, y, str) {
    for (let i = 0; i < str.length; i++) {
        const index = alpha.indexOf(str[i]);
        if (index != -1) {
            assets.alpha[index].draw(ctx, x + i * 8, y);
        }
    }
}

class StartMenu {
    constructor() {
        this.names = [
            "gin", "jai", "jay", "lad", "dre", "ash", "zev", "buz", "nox", "oak",
            "coy", "eza", "fil", "kip", "aya", "jem", "roy", "rex", "ryu", "gus"
        ];
    }

    update() {
        this.subtitlesMove++;
        if (this.subtitlesMove > assets.subtitles.width + 320) {
            this.subtitlesMove = 0;
        }

        for (let controller of controllersList) {
            controller.update();
            if (controller.playerKeys[PlayerKeys.Bomb] && !controller.id && !controller.isDemo) {
                const id = Int.random(1000000);
                const name = this.names.splice(Int.random(this.names.length), 1)[0];
                this.playerList.push({ id: id, name: name, controller: controller });
                controller.id = id;
                soundManager.playSound("addplayer");
            }
        }

        if (keys["Enter"]) {
            if (this.playerList.length >= 1) {
                isDemo = false;
                music.next();
                startGame(this.playerList);
            }
        }
    }

    playerList = [];
    subtitlesMove = 0;
}

function drawAll(interpolationPercentage) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (state == States.game) {
        bg.images[Math.floor(bg.time) % bg.images.length].draw(ctx, 0, 0);
        bg.time += 1 / 30;

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
            if (monster.step == 5) {
                assets.neigeMonster[0][0].draw(ctx, monster.x + 8, monster.y - 2);
            } else {
                assets.neigeMonster[monster.step][Math.floor(monster.frameIndex)].draw(ctx, monster.x + 8, monster.y - 2);
            }
        }

        igloo.draw(ctx, 232, 57);
        tree.images[Math.floor(tree.time) % 2].draw(ctx, 112, 30);
        tree.time += 1 / 30;

        if (map.timeLeft > 0) {
            let min = Math.floor(map.timeLeft / 60);
            let sec = Math.floor(map.timeLeft % 60);
            if (sec < 10) {
                sec = "0" + sec;
            }
            let time = min + ":" + sec;
            let x = 270;
            for (let char of time) {
                const alpha = "0123456789:";
                const index = alpha.indexOf(char);
                assets.bigDigits[index].draw(ctx, x, 182);
                if (index == 10) {
                    x += 9;
                } else {
                    x += 14;
                }
            }
        }
    } else if (state == States.start) {
        assets.start.draw(ctx, 0, 0);
        const colors = [
            "brightness(.5) hue-rotate(-80deg)",
            "brightness(.4) hue-rotate(-60deg)",
            "brightness(.5) hue-rotate(150deg)",
            "brightness(.5) hue-rotate(40deg)",
        ];
        for (let x = 0; x < 4; x++) {
            for (let y = 0; y < 2; y++) {
                const player = startMenu.playerList[y * 4 + x];
                ctx.filter = colors[Int.divFloor(y * 4 + x, 2)];
                if (player) {
                    drawString(ctx, 13 + x * 80, 78 + y * 70, "name ?");
                    drawString(ctx, 21 + x * 80, 88 + y * 70, player.name);
                } else if (Int.mod(menustep, 4) == 0) {
                    ctx.drawImage(assets.joinUs, x * 80 + 20, y * 70 + 78);
                } else if (Int.mod(menustep, 4) == 2) {
                    ctx.drawImage(assets.pushFire, x * 80 + 20, y * 70 + 78);
                }
            }

            menustep += 1 / 100;
        }
        ctx.filter = "none";

        ctx.drawImage(assets.subtitles, 320 - startMenu.subtitlesMove, 192);
    } else if (state == States.draw) {
        assets.draw.draw(ctx);
        let keyCount = 0;
        for (let ctr of controllersList) {
            ctr.update();
            for (let key of ctr.playerKeys) {
                if (key) {
                    keyCount++;
                }
            }
        }
        if (keyCount > 0) {
            startGame(startMenu.playerList);
        } else if (isDemo == true) {
            isDemo = 2;
        } else if (isDemo > 120) {
            startGame([]);
        } else if (isDemo != false) {
            isDemo++;
        }
    }
    ctx.filter = "none";
}

function startGame(playerList) {
    state = States.game;
    map = newMap(mapNeigeInitial);
    sprites = [];

    if (isDemo) {
        sprite = new Sprite(2);
        sprite.controller = new DemoController("lbrdwbulllwbrrddwbuulllw", sprite);
        map.locateSprite(sprite, 1);
        sprites.push(sprite);

        sprite = new Sprite(0);
        map.locateSprite(sprite, 4);
        sprites.push(sprite);
        sprite.controller = new DemoController("lbrdwwbulllwwbrrddwwbuulllww", sprite);

        sprite = new Sprite(1);
        map.locateSprite(sprite, 0);
        sprites.push(sprite);
        sprite.controller = new DemoController("rbldwwburrrwwbllddwwbuurrrww", sprite);
    } else {
        for (let i = 0; i < playerList.length; i++) {
            sprite = new Sprite(i);
            map.locateSprite(sprite);
            sprite.controller = playerList[i].controller;
            sprites.push(sprite);
        }
    }

    map.spawnMonsters(mapNeigeInitial.monsters);

    music.next();
}

function end(fps, panic) {
    elemFpsDisplay.innerText = Math.round(fps) + ' FPS';

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
    playerKeys;
    keyUp;
    keyDown;
    keyLeft;
    keyRight;
    keyBomb;
    keyRcDitonate;

    constructor(keyUp, keyDown, keyLeft, keyRight, keyBomb, keyRcDitonate) {
        this.keyUp = keyUp;
        this.keyDown = keyDown;
        this.keyLeft = keyLeft;
        this.keyRight = keyRight;
        this.keyBomb = keyBomb;
        this.keyRcDitonate = keyRcDitonate;
        this.playerKeys = [];
    }

    update() {
        this.playerKeys[PlayerKeys.Up] = keys[this.keyUp];
        this.playerKeys[PlayerKeys.Down] = keys[this.keyDown];
        this.playerKeys[PlayerKeys.Left] = keys[this.keyLeft];
        this.playerKeys[PlayerKeys.Right] = keys[this.keyRight];
        this.playerKeys[PlayerKeys.Bomb] = keys[this.keyBomb];
        this.playerKeys[PlayerKeys.rcDitonate] = keys[this.keyRcDitonate];
    }
}

class GamepadController {
    playerKeys;
    gamepad;

    constructor(gamepad) {
        this.gamepad = gamepad;
        this.playerKeys = [];
        this.sprite = null;
    }

    update() {
        const currGamePad = navigator.getGamepads()[this.gamepad.index];

        this.playerKeys[PlayerKeys.Up] = currGamePad.buttons[12].pressed;
        this.playerKeys[PlayerKeys.Down] = currGamePad.buttons[13].touched;
        this.playerKeys[PlayerKeys.Left] = currGamePad.buttons[14].touched;
        this.playerKeys[PlayerKeys.Right] = currGamePad.buttons[15].touched;
        this.playerKeys[PlayerKeys.Bomb] = currGamePad.buttons[1].pressed;
        this.playerKeys[PlayerKeys.rcDitonate] = currGamePad.buttons[0].pressed;
    }
}

class DemoController {
    playerKeys;
    moves;
    currentMove;
    step;
    isDemo = true;
    sprite;

    constructor(moves, sprite) {
        this.playerKeys = [];
        this.currentMove = 0;
        this.step = 0;
        this.moves = moves;
        this.sprite = sprite;
    }

    update() {
        this.playerKeys = {};

        const move = (direction) => {
            if (this.step < ((this.sprite.isHaveRollers) ? 8 : 16)) {
                this.playerKeys[direction] = true;
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
            this.playerKeys[PlayerKeys.Bomb] = true;
            this.currentMove++;
        } else if (this.moves[this.currentMove] == "w") {
            this.playerKeys[PlayerKeys.rcDitonate] = true;

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

    controller;

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

        if (this.controller) {
            this.controller.update();
        }

        if (this.controller.playerKeys[PlayerKeys.Up]) {
            direction = Direction.Up;
        } else if (this.controller.playerKeys[PlayerKeys.Left]) {
            direction = Direction.Left;
        } else if (this.controller.playerKeys[PlayerKeys.Right]) {
            direction = Direction.Right;
        } else if (this.controller.playerKeys[PlayerKeys.Down]) {
            direction = Direction.Down;
        }

        if (this.rcAllowed && this.controller.playerKeys[PlayerKeys.rcDitonate]) {
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
            if (Int.divRound(monster.x, 16) == tileX &&
                Int.divRound(monster.y, 16) == tileY && !monster.isDie) {
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

        if (this.controller.playerKeys[PlayerKeys.Bomb]) {
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