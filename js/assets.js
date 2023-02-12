async function loadAssets(scale = 2) {
    function loadImage(imgElement, x, y, width, height) {
        return {
            img: imgElement,
            rect: new Rect(x, y, width, height),
            draw: function (ctx, x, y) {
                ctx.drawImage(
                    this.img, this.rect.x * scale, this.rect.y * scale, this.rect.width * scale, this.rect.height * scale,
                    x * scale, y * scale, this.rect.width * scale, this.rect.height * scale);
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

    function loadPlayers(imgSpriteBoys, imgSpriteGirl) {
        let result = [];

        const framesCount = 20;
        const framesIndex = [
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 0, 2], [0, 1, 0, 2],
                [0, 1, 2, 3, 4, 5, 6, 7],
        ];

        const spriteWidth = 24;
        const spriteHeight = 24;

        const spriteIndexes = [0, 2, 3, 1];

        for (let spriteIndex of spriteIndexes) {
            let player = [];
            for (let x = 0; x < 5; x++) {
                let newImages = [];

                for (let index of framesIndex[x]) {
                    let frameX = index + x * 3 + spriteIndex * framesCount;

                    newImages.push(loadImage(imgSpriteBoys, (frameX % 13) * spriteWidth,
                        Math.floor(frameX / 13) * spriteHeight, 23, 23));
                }

                player.push(newImages);
            }

            result.push(player);
            player = [];
            for (let x = 0; x < 5; x++) {
                let newImages = [];

                for (let index of framesIndex[x]) {
                    let frameX = index + x * 3 + spriteIndex * framesCount;

                    newImages.push(loadImage(imgSpriteGirl, (frameX % 13) * spriteWidth,
                        Math.floor(frameX / 13) * (spriteHeight + 2), 23, 25));
                }

                player.push(newImages);
            }

            result.push(player);
        }

        return result;
    }

    const loadMonster = (up, left, right, down, die) => {
        return [
            [up[0], up[1], up[0], up[2]],
            [left[0], left[1], left[0], left[2]],
            [right[0], right[1], right[0], right[2]],
            [down[0], down[1], down[0], down[2]],
            die
        ];
    }

    async function loadBonus(img) {
        const canvas = document.createElement("canvas");
        const frameColors = [
            "#FC00FC",
            "#FC6CFC",
            "#FC90FC",
            "#FCB4FC",
            "#FCD8FC",
            "#FCFCFC",
            "#FCD8FC",
            "#FCB4FC",
            "#FC90FC",
            "#FC6CFC"
        ];
        const width = img.rect.width;
        const height = img.rect.height;

        canvas.width = width * frameColors.length * scale;
        canvas.height = height * scale;

        const ctx = canvas.getContext("2d");

        for (let i = 0; i < frameColors.length; i++) {
            const x = width * i;
            const y = 0;

            ctx.fillStyle = frameColors[i];
            ctx.fillRect(x * scale, y * scale, width * scale, height * scale);

            ctx.fillStyle = "#6C90FC";
            ctx.fillRect((x + 1) * scale, 1 * scale, (width - 2) * scale, (height - 2) * scale);

            // image.draw() handles scaling internally..
            img.draw(ctx, x, y);
        }

        const bitmap = await createImageBitmap(canvas);

        return loadImageStripe(bitmap, 0, 0, width, height, frameColors.length);
    }

    async function loadPermanentWall(fire, wall) {
        const width = Math.max(fire[0].rect.width, wall.rect.width);
        const height = Math.max(fire[0].rect.height, wall.rect.height);

        const canvas = document.createElement("canvas");

        canvas.width = width * (fire.length + 1) * scale;
        canvas.height = height * scale;

        const ctx = canvas.getContext("2d");

        for (let i = 0; i < fire.length + 1; i++) {
            const x = width * i;
            const y = 0;

            wall.draw(ctx, x + Int.divRound(width - wall.rect.width, 2), height - wall.rect.height);
            if (i > 0) {
                fire[i - 1].draw(ctx, x, y);
            }
        }

        const bitmap = await createImageBitmap(canvas);

        return loadImageStripe(bitmap, 0, 0, width, height, fire.length + 1);
    }

    const imgNeige1 = document.getElementById("NEIGE1");
    const imgNeige2 = document.getElementById("NEIGE2");
    const imgNeige3 = document.getElementById("NEIGE3");
    const imgSprite = document.getElementById("SPRITE");
    const imgSprite2 = document.getElementById("SPRITE2");
    const imgSprite3 = document.getElementById("SPRITE3");
    const imgMed3 = document.getElementById("MED3");
    const imgPause = document.getElementById("PAUSE");
    const imgFeuille = document.getElementById("FEUILLE");
    const imgAlpha = document.getElementById("ALPHA");
    const imgGhosts = document.getElementById("GHOST");
    const imgCrayon2 = document.getElementById("CRAYON2");
    const imgSoucoupe = document.getElementById("SOUCOUPE");
    const monster2walk = loadImageStripe(imgFeuille, 79, 128, 16, 19, 3, 0);
    const monster2ghost = loadImageStripe(imgGhosts, 195, 93, 16, 19, 3, 0);
    const monster3walk = loadImageStripe(imgFeuille, 42, 148, 16, 18, 5, 1);
    const snail = loadImageStripe(imgFeuille, 41, 17, 38, 32, 6, 1).concat(
        loadImageStripe(imgFeuille, 41, 50, 38, 32, 7, 1));
    const snailGhost = loadImageStripe(imgGhosts, 1, 114, 38, 32, 6, 1).concat(
        loadImageStripe(imgGhosts, 1, 147, 38, 32, 7, 1));

    const fire = loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6);

    return {
        bomb: loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
        powerups:
            [
                await loadBonus(loadImage(imgSprite2, 8 * 16, 2 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 8 * 16, 3 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 1 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 2 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 3 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 4 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 5 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 6 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 7 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 8 * 16, 16, 16)),
                await loadBonus(loadImage(imgSprite2, 9 * 16, 9 * 16, 16, 16)),
            ],
        boomMid: loadImageStripe(imgSprite2, 0 * 16, 46 + 0 * 16, 16, 16, 4),
        boomHor: loadImageStripe(imgSprite2, 0 * 16, 46 + 1 * 16, 16, 16, 4),
        boomLeftEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 2 * 16, 16, 16, 4),
        boomRightEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 3 * 16, 16, 16, 4),
        boomVert: loadImageStripe(imgSprite2, 0 * 16, 46 + 4 * 16, 16, 16, 4),
        boomTopEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 5 * 16, 16, 16, 4),
        boomBottomEnd: loadImageStripe(imgSprite2, 0 * 16, 46 + 6 * 16, 16, 16, 4),
        fire: fire,
        players: loadPlayers(imgSprite, imgSprite3),
        monsters: [
            loadMonster(
                loadImageStripe(imgSprite, 0, 144, 17, 18, 3, 7),
                loadImageStripe(imgSprite, 72, 144, 17, 18, 3, 7),
                loadImageStripe(imgSprite, 144, 144, 17, 18, 3, 7),
                loadImageStripe(imgSprite, 216, 144, 17, 18, 2, 7).
                    concat(loadImageStripe(imgSprite, 0, 163, 17, 18, 1, 7)),
                loadImageStripe(imgSprite, 24, 163, 17, 18, 8, 7)),
            loadMonster(
                loadImageStripe(imgMed3, 89, 56, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 188, 56, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 188, 89, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 89, 89, 32, 32, 3, 1),
                loadImageStripe(imgMed3, 89, 122, 32, 32, 4, 1).
                    concat(loadImageStripe(imgMed3, 89, 155, 32, 32, 3, 1))),
            loadMonster(monster2walk, monster2walk, monster2walk, monster2walk,
                loadImageStripe(imgFeuille, 127, 128, 16, 19, 6, 0)),
            loadMonster(monster3walk, monster3walk, monster3walk, monster3walk,
                loadImageStripe(imgFeuille, 127, 148, 16, 19, 6, 1)),
            [
                [snail[0], snail[1], snail[0], snail[1]],
                [snail[4], snail[5], snail[4], snail[5]],
                [snail[2], snail[3], snail[2], snail[3]],
                [snail[6], snail[7], snail[6], snail[7]],
                [snail[8], snail[9], snail[10], snail[11], snail[12]]
            ],
            loadMonster(
                loadImageStripe(imgPause, 0 * 24 * 3, 158, 23, 21, 3, 1),
                loadImageStripe(imgPause, 1 * 24 * 3, 158, 23, 21, 3, 1),
                loadImageStripe(imgPause, 2 * 24 * 3, 158, 23, 21, 3, 1),
                loadImageStripe(imgPause, 3 * 24 * 3, 158, 23, 21, 2, 1).concat(
                    loadImageStripe(imgPause, 0, 179, 23, 21, 1, 1)),
                loadImageStripe(imgPause, 24, 179, 23, 21, 8, 1))
        ],
        monsterGhosts: [
            null,
            loadMonster(
                loadImageStripe(imgGhosts, 0, 47, 32, 32, 3, 1),
                loadImageStripe(imgGhosts, 99, 47, 32, 32, 3, 1),
                loadImageStripe(imgGhosts, 99, 47 + 33, 32, 32, 3, 1),
                loadImageStripe(imgGhosts, 0, 47 + 33, 32, 32, 3, 1),
                null),
            loadMonster(monster2ghost, monster2ghost,
                monster2ghost, monster2ghost, null),
            null,
            [
                [snailGhost[0], snailGhost[1], snailGhost[0], snailGhost[1]],
                [snailGhost[4], snailGhost[5], snailGhost[4], snailGhost[5]],
                [snailGhost[2], snailGhost[3], snailGhost[2], snailGhost[3]],
                [snailGhost[6], snailGhost[7], snailGhost[6], snailGhost[7]],
                null
            ],
            loadMonster(
                loadImageStripe(imgGhosts, 0 * 24 * 3, 180, 23, 21, 3, 1),
                loadImageStripe(imgGhosts, 1 * 24 * 3, 180, 23, 21, 3, 1),
                loadImageStripe(imgGhosts, 2 * 24 * 3, 180, 23, 21, 3, 1),
                loadImageStripe(imgGhosts, 3 * 24 * 3, 180, 23, 21, 3, 1),
                null),
        ],
        insertCoin: loadImageStripe(imgCrayon2, 74, 27, 58, 62, 3, 0),
        start: loadImage(document.getElementById("MENU"), 0, 0, 320, 200),
        alpha: {
            original: loadImageStripe(imgAlpha, 0, 0, 8, 6, 44),
            white: loadImageStripe(imgAlpha, 0, 8, 8, 6, 44),
            magenta: loadImageStripe(imgAlpha, 0, 16, 8, 6, 44),
            red: loadImageStripe(imgAlpha, 0, 24, 8, 6, 44),
            blue: loadImageStripe(imgAlpha, 0, 32, 8, 6, 44),
            green: loadImageStripe(imgAlpha, 0, 40, 8, 6, 44),
        },
        bigDigits: loadImageStripe(imgFeuille, 80, 83, 15, 16, 11, 1),
        draw: [
            loadImage(document.getElementById("DRAW1"), 0, 0, 320, 200),
            loadImage(document.getElementById("DRAW2"), 0, 0, 320, 200)
        ],
        med: loadImage(document.getElementById("MED"), 0, 0, 320, 200),
        coin: loadImageStripe(imgMed3, 0, 0, 22, 22, 13, 1)
            .concat(loadImageStripe(imgMed3, 0, 23, 22, 22, 3, 1)),
        boyGhost: loadImageStripe(imgGhosts, 0, 0, 23, 23, 12, 1),
        girlGhost: loadImageStripe(imgGhosts, 0, 24, 23, 25, 12, 1),
        vic: [
            loadImage(document.getElementById("VIC1"), 0, 0, 320, 200),
            loadImage(document.getElementById("VIC2"), 0, 0, 320, 200),
            loadImage(document.getElementById("VIC3"), 0, 0, 320, 200),
            loadImage(document.getElementById("VIC4"), 0, 0, 320, 200),
        ],
        sky: loadImage(imgSprite2, 64, 16, 48, 44),

        levels: [
            {
                background: [
                    loadImage(imgNeige1, 0, 0, 320, 200),
                    loadImage(imgNeige2, 0, 0, 320, 200),
                    loadImage(imgNeige3, 0, 0, 320, 200),
                ],
                overlays: [
                    {
                        x: 232, y: 57, idx: 0, animateDelay: 0, images: [
                            loadImage(imgMed3, 0, 77, 6 * 8, 44),
                        ]
                    },
                    {
                        x: 112, y: 30, idx: 0, animateDelay: 1 / 20,
                        images: loadImageStripe(imgMed3, 0, 17 * 8, 32, 49, 2, 1),
                    },
                ],
                walls: loadImageStripe(imgPause, 0 * 16, 80, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 1, 16 * 1, 16, 16)),
            },
            {
                background: [
                    loadImage(document.getElementById("GAME1"), 0, 0, 320, 200),
                    loadImage(document.getElementById("GAME2"), 0, 0, 320, 200),
                    loadImage(document.getElementById("GAME3"), 0, 0, 320, 200),
                ],
                overlays: [],
                walls: loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 0, 16 * 1, 16, 16)),
            },
            // {},
            {
                background: [loadImage(document.getElementById("FOOT"), 0, 0, 320, 200)],
                overlays: [
                    {
                        x: 232, y: 0, idx: 0, animateDelay: 1 / 15,
                        images: loadImageStripe(imgSoucoupe, 0, 133, 88, 49 - 13, 2, 144)
                    }
                ],
                walls: loadImageStripe(imgPause, 160, 128, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 2, 16 * 0, 16, 16)),
            },
            {
                background: [
                    loadImage(document.getElementById("NUAGE1"), 0, 0, 320, 200),
                    loadImage(document.getElementById("NUAGE2"), 0, 0, 320, 200),
                ],
                overlays: [],
                walls: loadImageStripe(imgPause, 0 * 16, 96, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 0, 16 * 0, 16, 16)),
            },
            {
                background: [loadImage(document.getElementById("FORET"), 0, 0, 320, 200)],
                overlays: [],
                walls: loadImageStripe(imgPause, 0 * 16, 64, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 2, 16 * 0, 16, 16)),
            },
            {
                background: [loadImage(document.getElementById("SOCCER"), 0, 0, 320, 200)],
                overlays: [],
                walls: loadImageStripe(imgPause, 160, 112, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 3, 16 * 1, 16, 16)),
            },
            {
                background: [loadImage(document.getElementById("CRAYON"), 0, 0, 320, 200)],
                overlays: [],
                walls: loadImageStripe(imgPause, 0, 112, 16, 16, 8),
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
            },
            {
                background: [loadImage(document.getElementById("MICRO"), 0, 0, 320, 200)],
                overlays: [],
                walls: null,
                permanentWalls: await loadPermanentWall(fire,
                    loadImage(imgPause, 256 + 16 * 2, 16 * 1, 16, 16)),
            },
        ]

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
