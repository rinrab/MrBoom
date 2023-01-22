function loadAssets() {
    function loadImageStripe(imgElement, imgRects) {
        let result = [];

        for (let rect of imgRects) {
            result.push({
                img: imgElement,
                rect: rect,
            });
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
        bomb: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 1 * 16, 16, 16),
                new Rect(1 * 16, 1 * 16, 16, 16),
                new Rect(2 * 16, 1 * 16, 16, 16),
                new Rect(3 * 16, 1 * 16, 16, 16)
            ]),

        niegeBg: loadImageStripe(
            imgNeige1,
            [
                new Rect(0, 0, 320, 200),
                new Rect(0, 0, 320, 200),
                new Rect(0, 0, 320, 200),
            ]),
        niegeIgloo: loadImageStripe(
            imgMed3,
            [
                new Rect(0, 77, 6 * 8, 44),
            ]),
        niegeTree: loadImageStripe(
            imgMed3,
            [
                new Rect(0, 17 * 8, 32, 49),
                new Rect(33, 17 * 8, 32, 49),
            ]),
        niegeWall: loadImageStripe(
            imgPause,
            [
                new Rect(0 * 16, 80, 16, 16),
                new Rect(1 * 16, 80, 16, 16),
                new Rect(2 * 16, 80, 16, 16),
                new Rect(3 * 16, 80, 16, 16),
                new Rect(4 * 16, 80, 16, 16),
                new Rect(5 * 16, 80, 16, 16),
                new Rect(6 * 16, 80, 16, 16),
                new Rect(7 * 16, 80, 16, 16),
            ]),
        banana: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 0 * 16, 16, 16),
                new Rect(1 * 16, 0 * 16, 16, 16),
                new Rect(2 * 16, 0 * 16, 16, 16),
                new Rect(3 * 16, 0 * 16, 16, 16),
                new Rect(4 * 16, 0 * 16, 16, 16),
                new Rect(5 * 16, 0 * 16, 16, 16),
                new Rect(6 * 16, 0 * 16, 16, 16),
                new Rect(7 * 16, 0 * 16, 16, 16),
                new Rect(8 * 16, 0 * 16, 16, 16),
                new Rect(9 * 16, 0 * 16, 16, 16)
            ]),
        boomMid: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 46 + 0 * 16, 16, 16),
                new Rect(1 * 16, 46 + 0 * 16, 16, 16),
                new Rect(2 * 16, 46 + 0 * 16, 16, 16),
                new Rect(3 * 16, 46 + 0 * 16, 16, 16),
            ]),
        boomHor: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 46 + 1 * 16, 16, 16),
                new Rect(1 * 16, 46 + 1 * 16, 16, 16),
                new Rect(2 * 16, 46 + 1 * 16, 16, 16),
                new Rect(3 * 16, 46 + 1 * 16, 16, 16),
            ]),
        boomLeftEnd: loadImageStripe(
            imgSprite2,
            [
            new Rect(0 * 16, 46 + 2 * 16, 16, 16),
            new Rect(1 * 16, 46 + 2 * 16, 16, 16),
            new Rect(2 * 16, 46 + 2 * 16, 16, 16),
            new Rect(3 * 16, 46 + 2 * 16, 16, 16),
            ]),
        boomRightEnd: loadImageStripe(
            imgSprite2,
            [
            new Rect(0 * 16, 46 + 3 * 16, 16, 16),
            new Rect(1 * 16, 46 + 3 * 16, 16, 16),
            new Rect(2 * 16, 46 + 3 * 16, 16, 16),
            new Rect(3 * 16, 46 + 3 * 16, 16, 16),
            ]),
        boomVert: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 46 + 4 * 16, 16, 16),
                new Rect(1 * 16, 46 + 4 * 16, 16, 16),
                new Rect(2 * 16, 46 + 4 * 16, 16, 16),
                new Rect(3 * 16, 46 + 4 * 16, 16, 16),
            ]),
        boomTopEnd: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 46 + 5 * 16, 16, 16),
                new Rect(1 * 16, 46 + 5 * 16, 16, 16),
                new Rect(2 * 16, 46 + 5 * 16, 16, 16),
                new Rect(3 * 16, 46 + 5 * 16, 16, 16),
                new Rect(3 * 16, 46 + 5 * 16, 16, 16),
            ]),
        boomBottomEnd: loadImageStripe(
            imgSprite2,
            [
                new Rect(0 * 16, 46 + 6 * 16, 16, 16),
                new Rect(1 * 16, 46 + 6 * 16, 16, 16),
                new Rect(2 * 16, 46 + 6 * 16, 16, 16),
                new Rect(3 * 16, 46 + 6 * 16, 16, 16),
            ]),
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
