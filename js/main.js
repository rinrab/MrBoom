let canvas;
let ctx;

let images;

let time = 0;

let bg;
let sprite;

const FPS = 30;

addEventListener("load", function () {
    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    bg = new AnimatedImage([
        "NEIGE1",
        "NEIGE2",
        "NEIGE3",
    ], 200);

    sprite = new AnimatedImage([
        "SPRITE"
    ], 100)

    setInterval(timerTick, 1000 / FPS);
});

function timerTick() {
    time += 1000 / FPS;
    drawAll();
}

function drawAll() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    bg.draw(ctx);
}

class AnimatedImage {
    images;
    currentImage;
    delay;

    _time;

    constructor(imagesId, delay) {
        this.images = [];
        for (var id of imagesId) {
            this.images.push(document.getElementById(id));
        }

        this.delay = delay;
        this._time = 0;
    }

    tick() {
        this._time += 1 / FPS * (1000 / this.delay);

        return this.images[Math.floor(this._time) % this.images.length];
    }

    draw(ctx) {
        ctx.drawImage(this.tick(), 0, 0);
    }
}