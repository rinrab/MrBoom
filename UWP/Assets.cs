using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls;

namespace MrBoom
{
    public class Assets
    {
        public class Level
        {
            public AssetImage[] Backgrounds;
            public AssetImage[] Walls;
        }

        public Level[] levels { get; private set; }
        public AssetImage[] Bomb { get; private set; }
        public AssetImage[] BoomMid { get; private set; }
        public AssetImage[] BoomHor { get; private set; }
        public AssetImage[] BoomLeftEnd { get; private set; }
        public AssetImage[] BoomRightEnd { get; private set; }
        public AssetImage[] BoomVert { get; private set; }
        public AssetImage[] BoomTopEnd { get; private set; }
        public AssetImage[] BoomBottomEnd { get; private set; }
        public AssetImage[] Fire { get; private set; }
        public AssetImage[][][] Players { get; private set; }
        public AssetImage[] Pause { get; private set; }
        public AssetImage Start { get; private set; }
        public AssetImage[] InsertCoin { get; private set; }
        public AssetImage[] BigDigits { get; private set; }
        public AssetImage[] Draw { get; private set; }
        public AssetImage Med { get; private set; }
        public AssetImage[] Coin { get; private set; }
        public AssetImage[] BoyGhost { get; private set; }
        public AssetImage[] GirlGhost { get; private set; }
        public AssetImage[] Vic { get; private set; }
        public AssetImage Sky { get; private set; }
        public AssetImage Splash { get; private set; }
        public AssetImage[][] PowerUps { get; private set; }

        public static int scale = 2;

        public class AssetImage
        {
            private Texture2D texture;
            private Rectangle rect;

            public int Width { get { return rect.Width; } }
            public int Height { get { return rect.Height; } }

            public AssetImage(Texture2D texture, int x, int y, int width, int height)
            {
                this.texture = texture;
                rect = new Rectangle(x * scale, y * scale, width * scale, height * scale);
            }

            public void Draw(SpriteBatch ctx, int x, int y)
            {
                ctx.Draw(texture, new Vector2(x * scale, y * scale), rect, Color.White);
            }
        }

        public static Assets Load(ContentManager content)
        {
            AssetImage loadImage(Texture2D texture, int x, int y, int width, int height)
            {
                return new AssetImage(texture, x, y, width, height);
            }

            AssetImage[] loadImageStripe(Texture2D texture, int x, int y, int width, int height, int count = 1, int gap = 0)
            {
                AssetImage[] result = new AssetImage[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = loadImage(texture, x + i * (width + gap), y, width, height);
                }
                return result;
            }


            AssetImage[][][] loadPlayers(Texture2D imgSpriteBoys, Texture2D imgSpriteGirl)
            {
                var result = new List<AssetImage[][]>();

                int framesCount = 20;
                var framesIndex = new int[][] {
                    new int [] {0, 1, 0, 2 },
                    new int [] {0, 1, 0, 2 },
                    new int [] {0, 1, 0, 2 },
                    new int [] {0, 1, 0, 2 },
                    new int [] {0, 1, 2, 3, 4, 5, 6, 7 },
                };

                int spriteWidth = 24;
                int spriteHeight = 24;

                int[] spriteIndexes = new int[] { 0, 2, 3, 1 };

                foreach (int spriteIndex in spriteIndexes)
                {
                    var player = new List<AssetImage[]>();

                    for (int x = 0; x < 5; x++)
                    {
                        List<AssetImage> newImages = new List<AssetImage>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            newImages.Add(loadImage(imgSpriteBoys, (frameX % 13) * spriteWidth, frameX / 13 * spriteHeight, 23, 23));
                        }

                        player.Add(newImages.ToArray());
                    }

                    result.Add(player.ToArray());

                    player = new List<AssetImage[]>();

                    for (int x = 0; x < 5; x++)
                    {
                        List<AssetImage> newImages = new List<AssetImage>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            newImages.Add(loadImage(imgSpriteGirl, (frameX % 13) * spriteWidth, frameX / 13 * (spriteHeight + 2), 23, 25));
                        }

                        player.Add(newImages.ToArray());
                    }

                    result.Add(player.ToArray());
                }

                return result.ToArray();
            }

            AssetImage[] loadBonus(AssetImage img)
            {
                // TODO:
                // const frameColors = [
                //     "#FC00FC",
                //     "#FC6CFC",
                //     "#FC90FC",
                //     "#FCB4FC",
                //     "#FCD8FC",
                //     "#FCFCFC",
                //     "#FCD8FC",
                //     "#FCB4FC",
                //     "#FC90FC",
                //     "#FC6CFC"
                // ];

                var width = img.Width;
                var height = img.Height;

                // TODO:
                //canvas.width = width * frameColors.length * scale;
                //canvas.height = height * scale;

                //const ctx = canvas.getContext("2d");

                //for (let i = 0; i < frameColors.length; i++)
                //{
                //    const x = width * i;
                //    const y = 0;

                //    ctx.fillStyle = frameColors[i];
                //    ctx.fillRect(x * scale, y * scale, width * scale, height * scale);

                //    ctx.fillStyle = "#6C90FC";
                //    ctx.fillRect((x + 1) * scale, 1 * scale, (width - 2) * scale, (height - 2) * scale);

                //    // image.draw() handles scaling internally..
                //    img.draw(ctx, x, y);
                //}

                //const bitmap = await createImageBitmap(canvas);

                return new AssetImage[] { img};
            }

            var imgNeige1 = content.Load<Texture2D>("NEIGE1");
            var imgNeige2 = content.Load<Texture2D>("NEIGE2");
            var imgNeige3 = content.Load<Texture2D>("NEIGE3");
            var imgSprite = content.Load<Texture2D>("SPRITE");
            var imgSprite2 = content.Load<Texture2D>("SPRITE2");
            var imgSprite3 = content.Load<Texture2D>("SPRITE3");
            var imgMed3 = content.Load<Texture2D>("MED3");
            var imgPause = content.Load<Texture2D>("PAUSE");
            var imgFeuille = content.Load<Texture2D>("FEUILLE");
            var imgAlpha = content.Load<Texture2D>("ALPHA");
            var imgGhosts = content.Load<Texture2D>("GHOST");
            var imgCrayon2 = content.Load<Texture2D>("CRAYON2");
            var imgSoucoupe = content.Load<Texture2D>("SOUCOUPE");

            var monster2walk = loadImageStripe(imgFeuille, 79, 128, 16, 19, 3, 0);
            var monster2ghost = loadImageStripe(imgGhosts, 195, 93, 16, 19, 3, 0);
            var monster3walk = loadImageStripe(imgFeuille, 42, 148, 16, 18, 5, 1);
            var snail = loadImageStripe(imgFeuille, 41, 17, 38, 32, 6, 1)
                .Concat(loadImageStripe(imgFeuille, 41, 50, 38, 32, 7, 1)).ToArray();
            var snailGhost = loadImageStripe(imgGhosts, 1, 114, 38, 32, 6, 1)
                .Concat(loadImageStripe(imgGhosts, 1, 147, 38, 32, 7, 1)).ToArray();
            var fire = loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6);

            return new Assets()
            {
                Bomb = loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
                PowerUps = new AssetImage[][] {
                    loadBonus(loadImage(imgSprite2, 8 * 16, 2 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 8 * 16, 3 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 1 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 2 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 3 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 4 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 5 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 6 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 7 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 8 * 16, 16, 16)),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 9 * 16, 16, 16)),
                },
                levels = new Level[]
                {
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("GAME1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME2"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME3"), 0, 0, 320, 200),
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
                    }
                },
                BoomMid = loadImageStripe(imgSprite2, 0 * 16, 46 + 0 * 16, 16, 16, 4),
                BoomHor = loadImageStripe(imgSprite2, 0 * 16, 46 + 1 * 16, 16, 16, 4),
                BoomLeftEnd = loadImageStripe(imgSprite2, 0 * 16, 46 + 2 * 16, 16, 16, 4),
                BoomRightEnd = loadImageStripe(imgSprite2, 0 * 16, 46 + 3 * 16, 16, 16, 4),
                BoomVert = loadImageStripe(imgSprite2, 0 * 16, 46 + 4 * 16, 16, 16, 4),
                BoomTopEnd = loadImageStripe(imgSprite2, 0 * 16, 46 + 5 * 16, 16, 16, 4),
                BoomBottomEnd = loadImageStripe(imgSprite2, 0 * 16, 46 + 6 * 16, 16, 16, 4),
                Fire = fire,
                Players = loadPlayers(imgSprite, imgSprite3),
                Pause = loadImageStripe(imgPause, 0, 0, 48, 64, 4, 0),
                //Monsters = 
                //MonsterGhosts = 
                InsertCoin = loadImageStripe(imgCrayon2, 74, 27, 58, 62, 3, 0),
                Start = loadImage(content.Load<Texture2D>("MENU"), 0, 0, 320, 200),
                BigDigits = loadImageStripe(imgFeuille, 80, 83, 15, 16, 11, 1),
                Draw = new AssetImage[] {
                    loadImage(content.Load<Texture2D>("DRAW1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("DRAW2"), 0, 0, 320, 200)
                },
                Med = loadImage(content.Load<Texture2D>("MED"), 0, 0, 320, 200),
                Coin = loadImageStripe(imgMed3, 0, 0, 22, 22, 13, 1)
                    .Concat(loadImageStripe(imgMed3, 0, 23, 22, 22, 3, 1)).ToArray(),
                BoyGhost = loadImageStripe(imgGhosts, 0, 0, 23, 23, 12, 1),
                GirlGhost = loadImageStripe(imgGhosts, 0, 24, 23, 25, 12, 1),
                Vic = new AssetImage[] {
                    loadImage(content.Load<Texture2D>("VIC1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC2"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC3"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC4"), 0, 0, 320, 200),
                },
                Sky = loadImage(imgSprite2, 64, 16, 48, 44),
                Splash = loadImage(content.Load<Texture2D>("MRFOND"), 0, 0, 320, 200)
            };
        }
    }
}
