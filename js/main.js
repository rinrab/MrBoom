let canvas;
let ctx;

let images;

let bg;
let sprite;
let banana;
let tree;
let penguin;
let igloo;

const spriteWidth = 24;
const spriteHeight = 24;

class Terrain {
    data;

    get width() {
        return this.data[0].length;
    }

    get height() {
        return this.data.length;
    }

    constructor(initial) {
        this.data = initial;
    }

    get(x, y) {
        return this.data[y][x];
    }
}

let map;

let gridImage;

const FPS = 60;

let keys = {};

let bombSprite;
let bombs = [];

addEventListener("load", function () {
    init();
});

function init() {
    map = new Terrain([
        "###################",
        "#..-------------..#",
        "#.#-#.#-#-#-#-#-#.#",
        "#---..------..----#",
        "#-#-#-###-#-#.#.#-#",
        "#-----###-----...-#",
        "#-#-#-###-#-#-#.#-#",
        "#---------..------#",
        "#-#-#-#.#-#.#-#-#-#",
        "#-----..----------#",
        "#.#-#-#-#-#-#-#.###",
        "#..-----------..###",
        "###################"
    ]);

    bombSprite = new AnimatedImage([
        { id: "SPRITE2", rect: new Rect(0 * 16, 1 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(1 * 16, 1 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(2 * 16, 1 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(3 * 16, 1 * 16, 16, 16) },
    ], 1000 / FPS * 10)

    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage([
        { id: "NEIGE1", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE2", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE3", rect: new Rect(0, 0, 320, 200) },
    ], 1000 / FPS * 8);

    banana = new AnimatedImage([
        { id: "SPRITE2", rect: new Rect(0 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(1 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(2 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(3 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(4 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(5 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(6 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(7 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(8 * 16, 0 * 16, 16, 16) },
        { id: "SPRITE2", rect: new Rect(9 * 16, 0 * 16, 16, 16) },
    ], 1000 / FPS * 5);

    igloo = new AnimatedImage([
        { id: "MED3", rect: new Rect(0, 77, 6 * 8, 44) },
    ], -1)

    penguin = new Penguin();

    tree = new AnimatedImage([
        { id: "MED3", rect: new Rect(0, 17 * 8, 32, 49) },
        { id: "MED3", rect: new Rect(33, 17 * 8, 32, 49) },
    ], 1000 / FPS * 15)

    gridImage = new AnimatedImage([
        { id: "PAUSE", rect: new Rect(0 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(1 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(2 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(3 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(4 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(5 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(6 * 16, 80, 16, 16) },
        { id: "PAUSE", rect: new Rect(7 * 16, 80, 16, 16) },
    ], -1);

    sprite = new Sprite(1);

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
}

function update(deltaTime) {
    sprite.move(1);
}

function drawAll(interpolationPercentage) {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);

    penguin.draw(ctx, 17 * 1 - 8, 0, true);
    penguin.draw(ctx, 17 * 2 - 8, 0);
    penguin.draw(ctx, 17 * 3 - 8, 0);
    penguin.draw(ctx, 17 * 7 - 8, 0);
    penguin.draw(ctx, 17 * 10 - 8, 0);
    penguin.draw(ctx, 17 * 12 - 8, 0);
    penguin.draw(ctx, 17 * 15 - 8, 0);


    for (let y = 0; y < map.height; y++) {
        for (let x = 0; x < map.width; x++) {
            if (map.get(x, y) == "-") {
                gridImage.draw(ctx, x * 16 + 8, y * 16);
            }
        }
    }

    bombSprite.tick();
    for (let bomb of bombs) {
        bombSprite.draw(ctx, bomb.x * 16 + 8, bomb.y * 16, false);
    }

    banana.draw(ctx, 16 * 4 + 8, 16 * 3)
    sprite.draw(ctx)

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

function placeBomb(x, y) {
    if (!bombs.find(function (val) {
        return val.x == x && val.y == y;
    })) {
        bombs.push({
            x: x,
            y: y
        })
    }
}

class AnimatedImage {
    images;
    currentImage;
    delay;

    _time;

    constructor(images, delay) {
        this.images = [];
        for (let img of images) {
            this.images.push({
                img: document.getElementById(img.id),
                rect: img.rect,
            });
        }

        this.delay = delay;
        this._time = 0;

        this.tick();
    }

    tick() {
        if (this.delay == -1) {
            return this.currentImage = this.images[0];
        } else {
            this._time += 1 / FPS * (1000 / this.delay);

            return this.currentImage = this.images[Math.floor(this._time) % this.images.length];
        }
    }

    draw(ctx, x = 0, y = 0, doTick = true) {
        let img = this.currentImage;
        //if (doTick) {
        //    let img = this.tick();
        //}
        ctx.drawImage(
            img.img,
            img.rect.x, img.rect.y,
            img.rect.width, img.rect.height,
            Math.round(x), Math.round(y),
            img.rect.width, img.rect.height);
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;

    key;

    speed;

    constructor(spriteIndex) {
        this.animations = [];

        this.animateIndex = 0;

        this.key = -1;

        this.speed = 1;

        let y = 1;

        const framesCount = 20;
        const framesIndex = [0, 1, 0, 2];
        for (let x = 0; x < 4; x++) {
            let newImages = [];
            for (let index of framesIndex) {
                let frameX = index + x * 3 + spriteIndex * framesCount;
                newImages.push({
                    id: "SPRITE",
                    rect: new Rect(
                        (frameX % 13) * spriteWidth, Math.floor(frameX / 13) * spriteHeight,
                        spriteWidth - 1, spriteHeight - 1)
                });
            }
            this.animations.push(new AnimatedImage(newImages, -1))
        }

        this.x = 64;
        this.y = 48;
    }

    move() {
        if (keys["KeyW"]) {
            if (map.get(Math.floor(this.x / 16), Math.floor((this.y - 1) / 16)) != "#" &&
                map.get(Math.floor((this.x + 15) / 16), Math.floor((this.y - 1) / 16)) != "#") {
                this.y -= this.speed;
            } else {
                const newPos = Math.floor((this.x + 8) / 16) * 16;
                if (newPos > this.x)
                    this.x += this.speed;
                else if (newPos < this.x)
                    this.x -= this.speed;
            }

            this.animateIndex = 3;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (keys["KeyS"]) {
            if (map.get(Math.floor(this.x / 16), Math.floor((this.y + 16) / 16)) != "#" &&
                map.get(Math.floor((this.x + 15) / 16), Math.floor((this.y + 16) / 16)) != "#") {
                this.y += this.speed;
            } else {
                const newPos = Math.floor((this.x + 8) / 16) * 16;
                if (newPos > this.x)
                    this.x += this.speed;
                else if (newPos < this.x)
                    this.x -= this.speed;
            }

            this.animateIndex = 0;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (keys["KeyA"]) {
            if (map.get(Math.floor((this.x - 1) / 16), Math.floor(this.y / 16)) != "#" &&
                map.get(Math.floor((this.x - 1) / 16), Math.floor((this.y + 15) / 16)) != "#") {
                this.x -= this.speed;
            } else {
                const newPos = Math.floor((this.y + 8) / 16) * 16;
                if (newPos > this.y)
                    this.y += this.speed;
                else if (newPos < this.y)
                    this.y -= this.speed;
            }
            this.animateIndex = 2;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else if (keys["KeyD"]) {
            if (map.get(Math.floor((this.x + 16) / 16), Math.floor(this.y / 16)) != "#" &&
                map.get(Math.floor((this.x + 16) / 16), Math.floor((this.y + 15) / 16)) != "#") {
                this.x += this.speed;
            } else {
                const newPos = Math.floor((this.y + 8) / 16) * 16;
                if (newPos > this.y)
                    this.y += this.speed;
                else if (newPos < this.y)
                    this.y -= this.speed;
            }

            this.animateIndex = 1;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
        } else {
            this.animations[this.animateIndex].delay = -1;
        }

        if (keys["Space"]) {
            placeBomb(Math.floor(this.x / 16), Math.floor(this.y / 16));
        }
    }

    tick() {
        this.animations[this.animateIndex].tick();
    }

    draw(ctx) {
        this.animations[this.animateIndex].draw(ctx, this.x + 5, this.y - 7);
    }
}

class Penguin {
    animation;

    constructor() {
        let newData = [];
        for (let i = 0; i < 5; i++) {
            newData.push(
                { id: "MED3", rect: new Rect(9 * 8 - 2, 3 * 8, 15, 15) },
                { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) });
        }
        newData.push(
            { id: "MED3", rect: new Rect(13 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(15 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(17 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(19 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(21 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(23 * 8 - 2, 3 * 8, 15, 15) },
            { id: "MED3", rect: new Rect(25 * 8 - 2, 3 * 8, 15, 15) })
        this.animation = new AnimatedImage(newData, 1000 / FPS * 7);
    }

    draw(ctx, x, y, doTick = false) {
        this.animation.draw(ctx, x, y, doTick);
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