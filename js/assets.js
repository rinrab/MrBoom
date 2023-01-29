function loadAssets() {
    function loadImage(imgElement, x, y, width, height) {
        return {
            img: imgElement,
            rect: new Rect(x, y, width, height),
            draw: function (ctx, x, y) {
                ctx.drawImage(
                    this.img, this.rect.x, this.rect.y, this.rect.width, this.rect.height,
                    x, y, this.rect.width, this.rect.height);
            }
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
        const framesIndex = [
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 2, 3, 4, 5, 6, 7],
        ];

        const spriteWidth = 24;
        const spriteHeight = 24;

        for (let spriteIndex = 0; spriteIndex < 4; spriteIndex++) {
            let player = [];
            for (let x = 0; x < 5; x++) {
                let newImages = [];

                for (let index of framesIndex[x]) {
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

    const loadNeigeMonster = (imgSprite) => {
        const scan = [];
        for (let i = 0; i < 20; i++) {
            const x = i % 11;
            const y = Int.divFloor(i, 11) + 8;
            scan.push({
                img: imgSprite,
                rect: new Rect(x * 24, y * 18 + (y - 8), 23, 18),
                draw: function (ctx, x, y) {
                    ctx.drawImage(
                        this.img, this.rect.x, this.rect.y, this.rect.width, this.rect.height,
                        x, y, this.rect.width, this.rect.height);
                }
            })
        }
        var indexList = [
            [0, 1, 0, 2],
            [3, 4, 3, 5],
            [6, 7, 6, 8],
            [9, 10, 9, 11],
            [12, 13, 14, 15, 16, 17, 18, 19]
        ]
        const rv = [];
        for (let frames of indexList) {
            const newAnimation = [];
            rv.push(newAnimation);
            for (let frame of frames) {
                newAnimation.push(scan[frame]);
            }
        }
        return rv;
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
        powerups:
            [
                loadImageStripe(imgSprite2, 0 * 16, 0 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 0 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 1 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 2 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 3 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 4 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 5 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 6 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 7 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 8 * 16, 16, 16, 10),
                loadImageStripe(imgSprite2, 10 * 16, 9 * 16, 16, 16, 10),
            ],
        boomMid: loadImageStripe(imgSprite2, 0 * 16, 46 + 0 * 16, 16, 16, 4),
        boomHor: loadImageStripe(imgSprite2, 0 * 16, 46 + 1 * 16, 16, 16, 4),
        boomLeftEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 2 * 16, 16, 16, 4),
        boomRightEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 3 * 16, 16, 16, 4),
        boomVert: loadImageStripe(imgSprite2, 0 * 16, 46 + 4 * 16, 16, 16, 4),
        boomTopEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 5 * 16, 16, 16, 4),
        boomBottomEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 6 * 16, 16, 16, 4),
        fire: loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6),
        players: loadPlayers(imgSprite),
        neigeMonster: loadNeigeMonster(imgSprite)

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
