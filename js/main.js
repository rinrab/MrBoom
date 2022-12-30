let canvas;
let ctx;

let time = 0;

let bg;

let ImagesData = [
    { url: "gfx/NEIGE1.gif", w: 320, h: 200, id: "NEIGE1" },
    { url: "gfx/NEIGE2.gif", w: 320, h: 200, id: "NEIGE2" },
    { url: "gfx/NEIGE3.gif", w: 320, h: 200, id: "NEIGE3" },
]

let images = {};

const FPS = 30;

function loadAssets(callAfterLoad) {
    for (let img of ImagesData) {
        let newImage = new Image();
        newImage.addEventListener("load", function () {
            img.isLoad = true;
            images[img.id] = newImage;
            if (checkLoadedImages(callAfterLoad)) {
                callAfterLoad();
            }
        });
        newImage.src = img.url;
    }
}

function checkLoadedImages() {
    let loadedImagesCount = 0;
    for (let img of ImagesData) {
        if (img.isLoad == true) {
            loadedImagesCount++;
        }
    }
    console.log(loadedImagesCount);

    if (loadedImagesCount == ImagesData.length) {
        return true;
    }
}

addEventListener("load", function () {
    canvas = document.getElementById("grafic");
    ctx = canvas.getContext("2d");

    loadAssets(function () {
        bg = new AnimatedImage([
            images.NEIGE1, images.NEIGE2, images.NEIGE3
        ], 200);

        setInterval(timerTick, 1000 / FPS);
    });
});

function timerTick() {
    time += 1000 / FPS;
    drawAll();
}

function drawAll() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    let bgImg = bg.tick();

    ctx.drawImage(bgImg, 0, 0, 320, 200);
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
    }

    tick() {
        this._time += 1 / FPS * (1000 / this.delay);

        return this.images[Math.floor(this._time) % this.images.length];
    }
}