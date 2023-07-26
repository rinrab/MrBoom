using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Assets
    {
        public class Level
        {
            public class Overlay
            {
                public ImageStripe Images;
                public int AnimationDelay;
                public int x;
                public int y;
            }

            public ImageStripe Backgrounds;
            public ImageStripe Walls;
            public ImageStripe PermanentWalls;
            public Overlay[] Overlays;
        }

        public class PlayerAssets
        {
            public ImageStripe[] Normal;
            public ImageStripe Ghost;
        }

        public class ImageStripe
        {
            private readonly Image[] images;

            public ImageStripe(params Image[] images)
            {
                this.images = images;
            }

            public ImageStripe(params ImageStripe[] stripes)
            {
                int len = 0;

                foreach(ImageStripe stripe in stripes)
                {
                    len += stripe.Length;
                }

                List<Image> images = new List<Image>(len);

                foreach (ImageStripe stripe in stripes)
                {
                    images.AddRange(stripe.images);
                }

                this.images = images.ToArray();
            }

            public ImageStripe(List<Image> images)
            {
                this.images = images.ToArray();
            }

            public Image this[int animateIndex]
            {
                get
                {
                    return images[animateIndex % images.Length];
                }
            }

            public int Length { get => images.Length; }
        }

        public SoundAssets Sounds { get; private set; }
        public Level[] levels { get; private set; }
        public ImageStripe Bomb { get; private set; }
        public ImageStripe BoomMid { get; private set; }
        public ImageStripe BoomHor { get; private set; }
        public ImageStripe BoomLeftEnd { get; private set; }
        public ImageStripe BoomRightEnd { get; private set; }
        public ImageStripe BoomVert { get; private set; }
        public ImageStripe BoomTopEnd { get; private set; }
        public ImageStripe BoomBottomEnd { get; private set; }
        public ImageStripe Fire { get; private set; }
        public PlayerAssets[] Players { get; private set; }
        public ImageStripe Pause { get; private set; }
        public Image Start { get; private set; }
        public ImageStripe InsertCoin { get; private set; }
        public ImageStripe BigDigits { get; private set; }
        public ImageStripe Draw { get; private set; }
        public Image Med { get; private set; }
        public ImageStripe Coin { get; private set; }
        public ImageStripe Vic { get; private set; }
        public Image Sky { get; private set; }
        public Image Splash { get; private set; }
        public ImageStripe[] PowerUps { get; private set; }
        public ImageStripe[][] Monsters { get; private set; }
        public ImageStripe[] MonsterGhosts { get; private set; }
        public Image DrawGameIn;
        public ImageStripe DrawGameInNumbers;

        public ImageStripe[] Alpha;

        public static int scale = 2;

        public class Image
        {
            private Texture2D texture;
            private Rectangle rect;

            public int Width { get => rect.Width / scale; }
            public int Height { get => rect.Height / scale; }
            public int X { get => rect.X / scale; }
            public int Y { get => rect.Y / scale; }

            public Texture2D Texture { get => texture; }

            public Image(Texture2D texture, int x, int y, int width, int height)
            {
                this.texture = texture;
                rect = new Rectangle(x * scale, y * scale, width * scale, height * scale);
            }

            public void Draw(SpriteBatch ctx, int x, int y, Color? color = null)
            {
                ctx.Draw(texture, new Vector2(x * scale, y * scale), rect, (color == null) ? Color.White : (Color)color);
            }
        }

        public static Assets Load(ContentManager content, GraphicsDevice graphics)
        {
            Image loadImage(Texture2D texture, int x, int y, int width, int height)
            {
                return new Image(texture, x, y, width, height);
            }

            ImageStripe loadImageStripe(Texture2D texture, int x, int y, int width, int height, int count = 1, int gap = 0)
            {
                Image[] result = new Image[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = loadImage(texture, x + i * (width + gap), y, width, height);
                }
                return new ImageStripe(result);
            }

            Texture2D changeColor(Texture2D src, Color color)
            {
                Color[] data = new Color[src.Width * src.Height];
                src.GetData(data);

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].A != 0)
                    {
                        data[i] = color;
                    }
                }

                var texture = new Texture2D(graphics, src.Width, src.Height);
                texture.SetData(data, 0, src.Width * src.Height);

                return texture;
            }

            PlayerAssets[] loadPlayers(Texture2D imgSpriteBoys, ImageStripe boyGhost,
                                       Texture2D imgSpriteGirl, ImageStripe girlGhost)
            {
                var result = new List<PlayerAssets>();

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
                    var player = new List<ImageStripe>();

                    for (int x = 0; x < 5; x++)
                    {
                        List<Image> newImages = new List<Image>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            newImages.Add(loadImage(imgSpriteBoys, (frameX % 13) * spriteWidth, frameX / 13 * spriteHeight, 23, 23));
                        }

                        player.Add(new ImageStripe(newImages));
                    }

                    result.Add(new PlayerAssets()
                    {
                        Normal = player.ToArray(),
                        Ghost = boyGhost
                    });

                    player = new List<ImageStripe>();

                    for (int x = 0; x < 5; x++)
                    {
                        List<Image> newImages = new List<Image>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            newImages.Add(loadImage(imgSpriteGirl, (frameX % 13) * spriteWidth, frameX / 13 * (spriteHeight + 2), 23, 25));
                        }

                        player.Add(new ImageStripe(newImages.ToArray()));
                    }

                    result.Add(new PlayerAssets()
                    {
                        Normal = player.ToArray(),
                        Ghost = girlGhost
                    });
                }

                return result.ToArray();
            }

            ImageStripe[] loadMonster(ImageStripe up, ImageStripe left, ImageStripe right, ImageStripe down, ImageStripe die)
            {
                return new ImageStripe[]
                {
                    new ImageStripe(new Image[] { up[0], up[1], up[0], up[2] }),
                    new ImageStripe(new Image[] { left[0], left[1], left[0], left[2] }),
                    new ImageStripe(new Image[] { right[0], right[1], right[0], right[2] }),
                    new ImageStripe(new Image[] { down[0], down[1], down[0], down[2] }),
                    die
                };
            }

            ImageStripe loadBonus(Image img, Image background)
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
                    for (int x = 0; x < background.Width; x += img.Width)
                    {
                        img.Draw(batch, x, 0);
                        count++;
                    }
                    batch.End();
                }
                graphics.SetRenderTarget(null);

                return loadImageStripe(result, 0, 0, img.Width, img.Height, count);
            }

            ImageStripe loadPermanentWall(ImageStripe fireImages, Image wall)
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

            ImageStripe[] monsterToGhost(ImageStripe[][] src)
            {
                var rv = new ImageStripe[src.Length];
                for (int i = 0; i < src.Length; i++)
                {
                    Image[] monster = new Image[src[i].Length * src[i][0].Length];
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < src[i][j].Length; k++)
                        {
                            var item = src[i][j][k];
                            var texture = changeColor(item.Texture, Color.White);
                            monster[j * src[i][j].Length + k] = loadImage(texture, item.X, item.Y, item.Width, item.Height);
                        }
                    }
                    rv[i] = new ImageStripe(monster);
                }

                return rv;
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

            var monster1ghost = 
                new ImageStripe(
                    loadImageStripe(imgGhosts, 0, 47, 32, 32, 6, 1),
                    loadImageStripe(imgGhosts, 0, 47 + 33, 32, 32, 6, 1));
            var monster2walk = loadImageStripe(imgFeuille, 79, 128, 16, 19, 3, 0);
            var monster2ghost = loadImageStripe(imgGhosts, 195, 93, 16, 19, 3, 0);
            var monster3walk = loadImageStripe(imgFeuille, 42, 148, 16, 18, 5, 1);

            var snail =
                new ImageStripe(
                    loadImageStripe(imgFeuille, 41, 17, 38, 32, 6, 1),
                    loadImageStripe(imgFeuille, 41, 50, 38, 32, 6, 1));
            var snailGhost = new ImageStripe(loadImageStripe(imgGhosts, 1, 114, 38, 32, 6, 1),
                     loadImageStripe(imgGhosts, 1, 147, 38, 32, 7, 1));

            var fire = loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6);
            var bonusBackground = loadImage(imgBonus, 0, 0, 160, 16);

            var imgSpriteWhite = changeColor(imgSprite, Color.White);
            var imgSprite3White = changeColor(imgSprite3, Color.White);

            var monsters = new ImageStripe[][]
                {
                    loadMonster(loadImageStripe(imgSprite, 0, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 72, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 144, 144, 17, 18, 3, 7),
                                new ImageStripe(loadImageStripe(imgSprite, 216, 144, 17, 18, 2, 7),
                                    loadImageStripe(imgSprite, 0, 163, 17, 18, 1, 7)),
                                loadImageStripe(imgSprite, 24, 163, 17, 18, 8, 7)),

                    loadMonster(loadImageStripe(imgMed3, 89, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 89, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 89, 89, 32, 32, 3, 1),
                                new ImageStripe(loadImageStripe(imgMed3, 89, 122, 32, 32, 4, 1),
                                loadImageStripe(imgMed3, 89, 155, 32, 32, 3, 1))),

                    loadMonster(monster2walk, monster2walk, monster2walk, monster2walk,
                                loadImageStripe(imgFeuille, 127, 128, 16, 19, 6, 0)),

                    loadMonster(new ImageStripe(snail[0], snail[1], snail[0], snail[1]),
                                new ImageStripe(snail[4], snail[5], snail[4], snail[5]),
                                new ImageStripe(snail[2], snail[3], snail[2], snail[3]),
                                new ImageStripe(snail[6], snail[7], snail[6], snail[7]),
                                new ImageStripe(snail[8], snail[9], snail[10], snail[11])),

                    loadMonster(monster3walk, monster3walk, monster3walk, monster3walk,
                                loadImageStripe(imgFeuille, 127, 148, 16, 19, 6, 1)),

                    loadMonster(loadImageStripe(imgPause, 0 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 1 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 2 * 24 * 3, 158, 23, 21, 3, 1),
                                new ImageStripe(loadImageStripe(imgPause, 3 * 24 * 3, 158, 23, 21, 2, 1),
                                    loadImageStripe(imgPause, 0, 179, 23, 21, 1, 1)),
                                loadImageStripe(imgPause, 24, 179, 23, 21, 8, 1))
                };

            return new Assets()
            {
                Sounds = SoundAssets.Load(content),
                Bomb = loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
                PowerUps = new ImageStripe[] {
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
                        Backgrounds = new ImageStripe(
                            loadImage(imgNeige1, 0, 0, 320, 200),
                            loadImage(imgNeige2, 0, 0, 320, 200),
                            loadImage(imgNeige3, 0, 0, 320, 200)
                        ),
                        Overlays = new Level.Overlay[] {
                            new Level.Overlay() {
                                x = 232,
                                y = 57,
                                AnimationDelay = 1,
                                Images = new ImageStripe(loadImage(imgMed3, 0, 77, 6 * 8, 44))
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
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("GAME1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME2"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME3"), 0, 0, 320, 200)
                        ),
                        Walls = loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("FOOT"), 0, 0, 320, 200)
                        ),
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
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("NUAGE1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("NUAGE2"), 0, 0, 320, 200)
                        ),
                        Walls = loadImageStripe(imgPause, 0 * 16, 96, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("FORET"), 0, 0, 320, 200)
                        ),
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
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("SOCCER"), 0, 0, 320, 200)
                        ),
                        Walls = loadImageStripe(imgPause, 160, 112, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("CRAYON"), 0, 0, 320, 200)
                        ),
                        Overlays = new Level.Overlay[]
                        {
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(imgCrayon2, 0, 0, 50, 200),
                                x = 0,
                                y = 0,
                                AnimationDelay = 1
                            },
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(imgCrayon2, 320 - 50, 0, 50, 200),
                                x = 320 - 50,
                                y = 0,
                                AnimationDelay = 1
                            },
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(imgCrayon2, 0, 0, 320, 10),
                                x = 0,
                                y = 0,
                                AnimationDelay = 1
                            }
                        },
                        Walls = loadImageStripe(imgPause, 0, 112, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new ImageStripe(
                            loadImage(content.Load<Texture2D>("MICRO"), 0, 0, 320, 200)
                        ),
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
                Players = loadPlayers(imgSprite, loadImageStripe(imgSpriteWhite, 0, 0, 23, 23, 12, 1),
                                      imgSprite3, loadImageStripe(imgSprite3White, 0, 0, 23, 25, 12, 1)),
                Pause = loadImageStripe(imgPause, 0, 0, 48, 64, 4, 0),
                Monsters = monsters,
                MonsterGhosts = monsterToGhost(monsters),
                InsertCoin = loadImageStripe(imgCrayon2, 74, 27, 58, 62, 3, 0),
                Start = loadImage(content.Load<Texture2D>("MENU"), 0, 0, 320, 200),
                Alpha = new ImageStripe[] {
                    loadImageStripe(imgAlpha, 0, 0, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 8, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 16, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 24, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 32, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 40, 8, 6, 44),
                },
                BigDigits = loadImageStripe(imgFeuille, 80, 83, 15, 16, 11, 1),
                Draw = new ImageStripe(
                    loadImage(content.Load<Texture2D>("DRAW1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("DRAW2"), 0, 0, 320, 200)),
                Med = loadImage(content.Load<Texture2D>("MED"), 0, 0, 320, 200),
                Coin = new ImageStripe(loadImageStripe(imgMed3, 0, 0, 22, 22, 13, 1),
                    loadImageStripe(imgMed3, 0, 23, 22, 22, 3, 1)),
                Vic = new ImageStripe(
                    loadImage(content.Load<Texture2D>("VIC1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC2"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC3"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC4"), 0, 0, 320, 200)),
                Sky = loadImage(imgSprite2, 64, 16, 48, 44),
                Splash = loadImage(content.Load<Texture2D>("PIC"), 0, 0, 320, 200),
                DrawGameIn = loadImage(imgSoucoupe, 96, 48, 78, 36),
                DrawGameInNumbers = loadImageStripe(imgSoucoupe, 173, 32, 8, 7, 10)
            };
        }
    }
}
