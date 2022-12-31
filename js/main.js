let canvas;
let ctx;

let images;

let time = 0;

let bg;
let sprite;

const spriteWidth = 24;
const spriteHeight = 24;

const FPS = 30;

addEventListener("load", function () {
    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage([
        { id: "NEIGE1", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE2", rect: new Rect(0, 0, 320, 200) },
        { id: "NEIGE3", rect: new Rect(0, 0, 320, 200) },
    ], 1000 / FPS * 8);

    sprite = new Sprite(0);

    setInterval(timerTick, 1000 / FPS);

    addEventListener("resize", resize);
    resize();

    addEventListener("keydown", function (e) {
        if (e.code == "KeyW") {
            sprite.key = 3;
        }
        else if (e.code == "KeyS") {
            sprite.key = 0;
        }
        else if (e.code == "KeyA") {
            sprite.key = 2;
        }
        else if (e.code == "KeyD") {
            sprite.key = 1;
        } else {
            sprite.key = -1;
        }
    })

    addEventListener("keyup", function (e) {
        switch (e.code) {
            case "KeyW": case "KeyS":
            case "KeyA": case "KeyD":
                sprite.key = -1;
        }
    })
});

function resize() {
    if (window.innerHeight / 200 < window.innerWidth / 320) {
        canvas.style.scale = window.innerHeight / 200;
    } else {
        canvas.style.scale = window.innerWidth / 320;
    }
}

function timerTick() {
    time += 1000 / FPS;
    sprite.move();
    drawAll();
}

function drawAll() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);
    sprite.draw(ctx)
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
    }

    tick() {
        if (this.delay == -1) {
            return this.images[0];
        } else {
            this._time += 1 / FPS * (1000 / this.delay);

            return this.images[Math.floor(this._time) % this.images.length];
        }
    }

    draw(ctx, x = 0, y = 0) {
        const img = this.tick();
        ctx.drawImage(img.img, img.rect.x, img.rect.y, img.rect.width, img.rect.height, x, y, img.rect.width, img.rect.height);
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;

    key;

    speed;

    constructor(index) {
        this.animations = [];

        this.animateIndex = 0;

        this.key = -1;

        this.speed = 2;

        let y = 0;
        const framesIndex = [0, 1, 0, 2];
        for (let x = 0; x < 4; x++) {
            let newImages = [];
            for (let index of framesIndex) {
                newImages.push({
                    id: "SPRITE",
                    rect: new Rect(
                        (index + x * 3) * spriteWidth, y * spriteHeight,
                        spriteWidth - 1, spriteHeight - 1)
                });
            }
            this.animations.push(new AnimatedImage(newImages, -1))
        }

        this.x = 50;
        this.y = 50;
    }

    move() {
        const delta = [
            { x: 0, y: 1 },
            { x: 1, y: 0 },
            { x: -1, y: 0 },
            { x: 0, y: -1 },
        ]

        if (this.key == -1) {
            this.animations[this.animateIndex].delay = -1;
        } else {
            this.animateIndex = this.key;
            this.animations[this.animateIndex].delay = 1000 / FPS * 7;
            this.x += delta[this.key].x * this.speed;
            this.y += delta[this.key].y * this.speed;
        }
    }

    tick() {
        this.animations[this.animateIndex].tick();
    }

    draw(ctx) {
        this.animations[this.animateIndex].draw(ctx, this.x, this.y);
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