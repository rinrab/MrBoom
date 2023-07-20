using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Windows.UI.Xaml.Controls;

namespace MrBoom
{
    public class Assets
    {
        public class Level
        {
            public class Overlay
            {
                public AssetImage[] Images;
                public int AnimationDelay;
                public int x;
                public int y;
            }

            public AssetImage[] Backgrounds;
            public AssetImage[] Walls;
            public AssetImage[] PermanentWalls;
            public Overlay[] Overlays;
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
        public AssetImage[][][] Monsters { get; private set; }
        public AssetImage[] PlayerBoyGhosts { get; private set; }

        public AssetImage[][] Alpha;

        public static int scale = 2;

        public class AssetImage
        {
            private Texture2D texture;
            private Rectangle rect;

            public int Width { get { return rect.Width / scale; } }
            public int Height { get { return rect.Height / scale; } }

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

        public static Assets Load(ContentManager content, GraphicsDevice graphics)
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

            AssetImage[][] loadMonster(AssetImage[] up, AssetImage[] left, AssetImage[] right, AssetImage[] down, AssetImage[] die)
            {
                return new AssetImage[][]
                {
                    new AssetImage[] { up[0], up[1], up[0], up[2] },
                    new AssetImage[] { left[0], left[1], left[0], left[2] },
                    new AssetImage[] { right[0], right[1], right[0], right[2] },
                    new AssetImage[] { down[0], down[1], down[0], down[2] },
                    die
                };
            }

            AssetImage[] loadBonus(AssetImage img, AssetImage background)
            {
                RenderTarget2D result = new RenderTarget2D(
                    graphics, background.Width * scale, background.Height * scale,
                    false, SurfaceFormat.Color, DepthFormat.None, 
                    graphics.PresentationParameters.MultiSampleCount, 
                    RenderTargetUsage.PreserveContents);

                int count = 0;
                graphics.SetRenderTarget(result);
                using (SpriteBatch batch = new SpriteBatch(graphics))
                {
                    batch.Begin(SpriteSortMode.Immediate);
                    background.Draw(batch, 0, 0);
                    for (int x = 0; x < background.Width; x+= img.Width)
                    {
                        img.Draw(batch, x, 0);
                        count++;
                    }
                    batch.End();
                }
                graphics.SetRenderTarget(null);

                return loadImageStripe(result, 0, 0, img.Width, img.Height, count);
            }

            AssetImage[] loadPermanentWall(AssetImage[] fireImages, AssetImage wall)
            {
                int width = Math.Max(fireImages[0].Width, wall.Width);
                int height = Math.Max(fireImages[0].Height, wall.Height);

                RenderTarget2D result = new RenderTarget2D(
                    graphics,
                    width * (fireImages.Length + 1) * scale,
                    height * height * scale,
                    false, SurfaceFormat.Color, DepthFormat.None,
                    graphics.PresentationParameters.MultiSampleCount,
                    RenderTargetUsage.PreserveContents);

                graphics.SetRenderTarget(result);
                using (SpriteBatch batch = new SpriteBatch(graphics))
                {
                    batch.Begin(SpriteSortMode.Immediate);
                    for (int i = 0; i < fireImages.Length + 1; i++)
                    {
                        int x = width * i;
                        int y = 0;

                        wall.Draw(batch, x + (width - wall.Width) / 2, height - wall.Height);

                        if (i > 0)
                        {
                            fireImages[i - 1].Draw(batch, x, y);
                        }
                    }
                    batch.End();
                }
                graphics.SetRenderTarget(null);

                return loadImageStripe(result, 0, 0, width, height, fireImages.Length + 1);
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
            var imgBonus = content.Load<Texture2D>("BONUS");

            var monster2walk = loadImageStripe(imgFeuille, 79, 128, 16, 19, 3, 0);
            var monster2ghost = loadImageStripe(imgGhosts, 195, 93, 16, 19, 3, 0);
            var monster3walk = loadImageStripe(imgFeuille, 42, 148, 16, 18, 5, 1);

            var snail = loadImageStripe(imgFeuille, 41, 17, 38, 32, 6, 1)
                .Concat(loadImageStripe(imgFeuille, 41, 50, 38, 32, 6, 1)).ToArray();
            var snailGhost = loadImageStripe(imgGhosts, 1, 114, 38, 32, 6, 1)
                     .Concat(loadImageStripe(imgGhosts, 1, 147, 38, 32, 7, 1)).ToArray();

            var fire = loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6);
            var bonusBackground = loadImage(imgBonus, 0, 0, 160, 16);

            return new Assets()
            {
                Bomb = loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
                PowerUps = new AssetImage[][] {
                    loadBonus(loadImage(imgSprite2, 8 * 16, 2 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 8 * 16, 3 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 1 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 2 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 3 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 4 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 5 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 6 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 7 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 8 * 16, 16, 16), bonusBackground),
                    loadBonus(loadImage(imgSprite2, 9 * 16, 9 * 16, 16, 16), bonusBackground),
                },
                levels = new Level[]
                {
                    new Level()
                    {
                        Backgrounds = new AssetImage[] {
                            loadImage(imgNeige1, 0, 0, 320, 200),
                            loadImage(imgNeige2, 0, 0, 320, 200),
                            loadImage(imgNeige3, 0, 0, 320, 200),
                        },
                        Overlays = new Level.Overlay[] {
                            new Level.Overlay() {
                                x = 232,
                                y = 57,
                                AnimationDelay = 1,
                                Images = new AssetImage[] { loadImage(imgMed3, 0, 77, 6 * 8, 44) }
                            },
                            new Level.Overlay() {
                                x = 112,
                                y = 30,
                                AnimationDelay = 20,
                                Images = loadImageStripe(imgMed3, 0, 17 * 8, 32, 49, 2, 1),
                            },
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 80, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 1, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("GAME1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME2"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME3"), 0, 0, 320, 200),
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("FOOT"), 0, 0, 320, 200),
                        },
                        Overlays = new Level.Overlay[]
                        {
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(imgSoucoupe, 0, 133, 88, 49 - 13, 2, 144),
                                AnimationDelay = 30,
                                x = 320 - 88,
                                y = 0
                            }
                        },
                        Walls = loadImageStripe(imgPause, 160, 128, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 2, 16 * 0, 16, 16))
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("NUAGE1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("NUAGE2"), 0, 0, 320, 200),
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 96, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("FORET"), 0, 0, 320, 200),
                        },
                        Overlays = new Level.Overlay[]
                        {
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(content.Load<Texture2D>("FEUILLE_OVERLAY"), 0, 0, 320, 200),
                                x = 0,
                                y = 0,
                                AnimationDelay = 1
                            }
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 64, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("SOCCER"), 0, 0, 320, 200),
                        },
                        Walls = loadImageStripe(imgPause, 160, 112, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("CRAYON"), 0, 0, 320, 200),
                        },
                        Walls = loadImageStripe(imgPause, 0, 112, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AssetImage[]
                        {
                            loadImage(content.Load<Texture2D>("MICRO"), 0, 0, 320, 200),
                        },
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 2, 16 * 1, 16, 16)),
                    },
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
                Monsters = new AssetImage[][][]
                {
                    loadMonster(loadImageStripe(imgSprite, 0, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 72, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 144, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 216, 144, 17, 18, 2, 7)
                                    .Concat(loadImageStripe(imgSprite, 0, 163, 17, 18, 1, 7)).ToArray(),
                                loadImageStripe(imgSprite, 24, 163, 17, 18, 8, 7)),

                    loadMonster(loadImageStripe(imgMed3, 89, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 89, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 89, 89, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 89, 122, 32, 32, 4, 1)
                                .Concat(loadImageStripe(imgMed3, 89, 155, 32, 32, 3, 1)).ToArray()),

                    loadMonster(monster2walk, monster2walk, monster2walk, monster2walk,
                                loadImageStripe(imgFeuille, 127, 128, 16, 19, 6, 0)),

                    loadMonster(new AssetImage[] { snail[0], snail[1], snail[0], snail[1] },
                                new AssetImage[] { snail[4], snail[5], snail[4], snail[5] },
                                new AssetImage[] { snail[2], snail[3], snail[2], snail[3] },
                                new AssetImage[] { snail[6], snail[7], snail[6], snail[7] },
                                new AssetImage[] { snail[8], snail[9], snail[10], snail[11] }),

                    loadMonster(monster3walk, monster3walk, monster3walk, monster3walk,
                                loadImageStripe(imgFeuille, 127, 148, 16, 19, 6, 1)),

                    loadMonster(loadImageStripe(imgPause, 0 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 1 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 2 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 3 * 24 * 3, 158, 23, 21, 2, 1)
                                    .Concat(loadImageStripe(imgPause, 0, 179, 23, 21, 1, 1)).ToArray(),
                                loadImageStripe(imgPause, 24, 179, 23, 21, 8, 1))
                },
                PlayerBoyGhosts = loadImageStripe(imgGhosts, 0, 0, 23, 23, 12, 1),
                //MonsterGhosts = 
                InsertCoin = loadImageStripe(imgCrayon2, 74, 27, 58, 62, 3, 0),
                Start = loadImage(content.Load<Texture2D>("MENU"), 0, 0, 320, 200),
                Alpha = new AssetImage[][] {
                    loadImageStripe(imgAlpha, 0, 0, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 8, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 16, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 24, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 32, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 40, 8, 6, 44),
                },
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
