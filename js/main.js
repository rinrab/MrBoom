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
    ], 200);

    sprite = new Sprite(0);

    setInterval(timerTick, 1000 / FPS);
});

function timerTick() {
    time += 1000 / FPS;
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
        this._time += 1 / FPS * (1000 / this.delay);

        return this.images[Math.floor(this._time) % this.images.length];
    }

    draw(ctx) {
        const img = this.tick();
        ctx.drawImage(img.img, img.rect.x, img.rect.y, img.rect.width, img.rect.height, 0, 0, img.rect.width, img.rect.height);
    }
}

class Sprite {
    animations;

    x;
    y;

    animateIndex;

    constructor(index) {
        this.animations = [];

        this.animateIndex = 0;

        let newImages = [];
        let y = 0;
        const framesIndex = [0, 1, 0, 2];
        for (let x = 0; x < 4; x++) {
            for (let index of framesIndex) {
                newImages.push({
                    id: "SPRITE",
                    rect: new Rect((index + x) * spriteWidth, y * spriteHeight, spriteWidth - 1, spriteHeight - 1)
                });
            }
        }

        this.animations.push(new AnimatedImage(newImages, 200))
    }

    tick() {
        for (let img of this.animations) {
            img.tick();
        }

        if (Math.random() < 0.01) {
            this.animateIndex = 2;
        }
    }

    draw(ctx) {
        this.animations[this.animateIndex].draw(ctx);
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