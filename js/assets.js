function loadAssets() {
    function loadImage(imgElement, x, y, width, height) {
        return {
            img: imgElement,
            rect: new Rect(x, y, width, height),
        };
    }

    function loadImageStripe(imgElement, x, y, width, height, count, gap) {
        let result = [];
        count = count || 1;
        gap = gap || 0;

        for (let i = 0; i < count; i++) {
            result.push(loadImage(imgElement, x + i * (width + gap), y, width, height));
        }

        return result;
    }

    function loadPlayers(imgSprite) {
        let result = [];

        const framesCount = 20;
        const framesIndex = [0, 1, 0, 2];

        for (let spriteIndex = 0; spriteIndex < 4; spriteIndex++) {
            let player = [];
            for (let x = 0; x < 4; x++) {
                let newImages = [];

                for (let index of framesIndex) {
                    let frameX = index + x * 3 + spriteIndex * framesCount;

                    newImages.push({
                        img: imgSprite,
                        rect: new Rect(
                            (frameX % 13) * spriteWidth, Math.floor(frameX / 13) * spriteHeight,
                            23, 23)
                    });
                }

                player.push(newImages);
            }

            result.push(player);
        }

        return result;
    }

    const imgNeige1 = document.getElementById("NEIGE1");
    const imgNeige2 = document.getElementById("NIEGE2");
    const imgNeige3 = document.getElementById("NIEGE3");
    const imgSprite = document.getElementById("SPRITE");
    const imgSprite2 = document.getElementById("SPRITE2");
    const imgMed3 = document.getElementById("MED3");
    const imgPause = document.getElementById("PAUSE");

    return {
        bomb: loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
        niegeBg:
            [
                loadImage(imgNeige1, 0, 0, 320, 200),
                loadImage(imgNeige2, 0, 0, 320, 200),
                loadImage(imgNeige2, 0, 0, 320, 200),
            ],
        niegeIgloo:
            [
                loadImage(imgMed3, 0, 77, 6 * 8, 44),
            ],
        niegeTree: loadImageStripe(imgMed3, 0, 17 * 8, 32, 49, 2, 1),
        niegeWall: loadImageStripe(imgPause, 0 * 16, 80, 16, 16, 8),
        banana: loadImageStripe(imgSprite2, 0 * 16, 0 * 16, 16, 16, 10),
        extraBomb: loadImageStripe(imgSprite2, 10 * 16, 0 * 16, 16, 16, 10),
        extraFire: loadImageStripe(imgSprite2, 10 * 16, 1 * 16, 16, 16, 10),
        skull: loadImageStripe(imgSprite2, 10 * 16, 2 * 16, 16, 16, 10),
        shield: loadImageStripe(imgSprite2, 10 * 16, 3 * 16, 16, 16, 10),
        life: loadImageStripe(imgSprite2, 10 * 16, 4 * 16, 16, 16, 10),
        remoteControl: loadImageStripe(imgSprite2, 10 * 16, 5 * 16, 16, 16, 10),
        kick: loadImageStripe(imgSprite2, 10 * 16, 6 * 16, 16, 16, 10),
        rollerSkate: loadImageStripe(imgSprite2, 10 * 16, 7 * 16, 16, 16, 10),
        clock: loadImageStripe(imgSprite2, 10 * 16, 8 * 16, 16, 16, 10),
        multiBomb: loadImageStripe(imgSprite2, 10 * 16, 9 * 16, 16, 16, 10),
        boomMid: loadImageStripe(imgSprite2, 0 * 16, 46 + 0 * 16, 16, 16, 4),
        boomHor: loadImageStripe(imgSprite2, 0 * 16, 46 + 1 * 16, 16, 16, 4),
        boomLeftEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 2 * 16, 16, 16, 4),
        boomRightEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 3 * 16, 16, 16, 4),
        boomVert: loadImageStripe(imgSprite2, 0 * 16, 46 + 4 * 16, 16, 16, 4),
        boomTopEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 5 * 16, 16, 16, 4),
        boomBottomEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 6 * 16, 16, 16, 4),
        players: loadPlayers(imgSprite)

        // igloo penguin
        //    for(let i = 0; i < 5; i++) {
        //    newData.push(
        //        { id: "MED3", rect: new Rect(9 * 8 - 2, 3 * 8, 15, 15) },
        //        { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) });
        //}
        //newData.push(
        //    { id: "MED3", rect: new Rect(13 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(11 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(15 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(17 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(19 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(21 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(23 * 8 - 2, 3 * 8, 15, 15) },
        //    { id: "MED3", rect: new Rect(25 * 8 - 2, 3 * 8, 15, 15) })
    };
}
