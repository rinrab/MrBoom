// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
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
                public AnimatedImage Images;
                public int AnimationDelay;
                public int x;
                public int y;

                public Overlay()
                {
                }

                public Overlay(int x, int y, AnimatedImage images, int animationDelay)
                {
                    this.x = x;
                    this.y = y;
                    Images = images;
                    AnimationDelay = animationDelay;
                }
            }

            public AnimatedImage Backgrounds;
            public AnimatedImage Walls;
            public AnimatedImage PermanentWalls;
            public Overlay[] Overlays;
            public Image MovingBackground;
            public Overlay[] BackgroundSprites;
        }

        public class MovingSpriteAssets
        {
            public readonly AnimatedImage[] Normal;
            public readonly AnimatedImage[] Ghost;
            public readonly AnimatedImage[] Red;

            public MovingSpriteAssets(AnimatedImage[] normal, AnimatedImage[] ghost, AnimatedImage[] red)
            {
                Normal = normal;
                Ghost = ghost;
                Red = red;
            }
        }

        public SoundAssets Sounds { get; private set; }
        public Level[] Levels { get; private set; }
        public AnimatedImage Bomb { get; private set; }
        public AnimatedImage BoomMid { get; private set; }
        public AnimatedImage BoomHor { get; private set; }
        public AnimatedImage BoomLeftEnd { get; private set; }
        public AnimatedImage BoomRightEnd { get; private set; }
        public AnimatedImage BoomVert { get; private set; }
        public AnimatedImage BoomTopEnd { get; private set; }
        public AnimatedImage BoomBottomEnd { get; private set; }
        public AnimatedImage Fire { get; private set; }
        public MovingSpriteAssets[] Players { get; private set; }
        public AnimatedImage Pause { get; private set; }
        public Image Start { get; private set; }
        public AnimatedImage InsertCoin { get; private set; }
        public AnimatedImage BigDigits { get; private set; }
        public AnimatedImage Draw { get; private set; }
        public Image Med { get; private set; }
        public Image MedC { get; private set; }
        public Image MedG { get; private set; }
        public AnimatedImage Coin { get; private set; }
        public AnimatedImage Vic { get; private set; }
        public Image Sky { get; private set; }
        public Image Splash { get; private set; }
        public AnimatedImage[] PowerUps { get; private set; }
        public MovingSpriteAssets[] Monsters { get; private set; }
        public Image[] Controls { get; private set; }
        public Image PauseHelp { get; private set; }
        public Image GameHelp { get; private set; }
        public Texture2D BlackPixel { get; private set; }
        public Texture2D StartButton { get; private set; }
        public SpriteFont MenuFontBig { get; private set; }

        public Image DrawGameIn;
        public AnimatedImage DrawGameInNumbers;

        public AnimatedImage[] Alpha;
        public SpriteFont MenuFont;
        public SpriteFont DebugFont;

        public static int scale = 2;

        public static Assets Load(ContentManager content, GraphicsDevice graphics)
        {
            Image loadImage(Texture2D texture, int x, int y, int width, int height)
            {
                return new Image(texture, x, y, width, height, scale);
            }

            AnimatedImage loadImageStripe(Texture2D texture, int x, int y, int width, int height, int count = 1, int gap = 0)
            {
                Image[] result = new Image[count];

                for (int i = 0; i < count; i++)
                {
                    result[i] = loadImage(texture, x + i * (width + gap), y, width, height);
                }
                return new AnimatedImage(result);
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

            MovingSpriteAssets[] loadPlayers(Texture2D imgSpriteBoys, Texture2D imgSpriteGirl)
            {
                var result = new List<MovingSpriteAssets>();

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

                var imgSpriteBoysWhite = changeColor(imgSpriteBoys, Color.White);
                var imgSpriteGirlWhite = changeColor(imgSpriteGirl, Color.White);
                var imgSpriteBoysRed = changeColor(imgSpriteBoys, Color.Red);
                var imgSpriteGirlRed = changeColor(imgSpriteGirl, Color.Red);

                foreach (int spriteIndex in spriteIndexes)
                {
                    var normal = new List<AnimatedImage>();
                    var white = new List<AnimatedImage>();
                    var red = new List<AnimatedImage>();

                    for (int x = 0; x < 5; x++)
                    {
                        List<Image> normalImages = new List<Image>();
                        List<Image> whiteImages = new List<Image>();
                        List<Image> redImages = new List<Image>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            normalImages.Add(loadImage(imgSpriteBoys, (frameX % 13) * spriteWidth, frameX / 13 * spriteHeight, 23, 23));
                            whiteImages.Add(loadImage(imgSpriteBoysWhite, (frameX % 13) * spriteWidth, frameX / 13 * spriteHeight, 23, 23));
                            redImages.Add(loadImage(imgSpriteBoysRed, (frameX % 13) * spriteWidth, frameX / 13 * spriteHeight, 23, 23));
                        }

                        normal.Add(new AnimatedImage(normalImages));
                        white.Add(new AnimatedImage(whiteImages));
                        red.Add(new AnimatedImage(redImages));
                    }

                    result.Add(new MovingSpriteAssets(normal.ToArray(), white.ToArray(), red.ToArray()));

                    normal.Clear();
                    white.Clear();
                    red.Clear();

                    for (int x = 0; x < 5; x++)
                    {
                        List<Image> normalImages = new List<Image>();
                        List<Image> whiteImages = new List<Image>();
                        List<Image> redImages = new List<Image>();

                        foreach (var index in framesIndex[x])
                        {
                            var frameX = index + x * 3 + spriteIndex * framesCount;

                            normalImages.Add(loadImage(imgSpriteGirl, (frameX % 13) * spriteWidth, frameX / 13 * (spriteHeight + 2), 23, 25));
                            whiteImages.Add(loadImage(imgSpriteGirlWhite, (frameX % 13) * spriteWidth, frameX / 13 * (spriteHeight + 2), 23, 25));
                            redImages.Add(loadImage(imgSpriteGirlRed, (frameX % 13) * spriteWidth, frameX / 13 * (spriteHeight + 2), 23, 25));
                        }

                        normal.Add(new AnimatedImage(normalImages));
                        white.Add(new AnimatedImage(whiteImages));
                        red.Add(new AnimatedImage(redImages));
                    }

                    result.Add(new MovingSpriteAssets(normal.ToArray(), white.ToArray(), red.ToArray()));
                }

                return result.ToArray();
            }

            AnimatedImage[] monsterToGhost(AnimatedImage[] normal)
            {
                List<AnimatedImage> white = new List<AnimatedImage>(normal.Length);

                for (int j = 0; j < normal.Length; j++)
                {
                    var stripe = new List<Image>(normal[j].Length);
                    for (int k = 0; k < normal[j].Length; k++)
                    {
                        var item = normal[j][k];
                        var texture = changeColor(item.Texture, Color.White);
                        stripe.Add(loadImage(texture, item.X, item.Y, item.Width, item.Height));
                    }

                    white.Add(new AnimatedImage(stripe));
                }

                return white.ToArray();
            }

            AnimatedImage[] monsterToRed(AnimatedImage[] normal)
            {
                List<AnimatedImage> white = new List<AnimatedImage>(normal.Length);

                for (int j = 0; j < normal.Length; j++)
                {
                    var stripe = new List<Image>(normal[j].Length);
                    for (int k = 0; k < normal[j].Length; k++)
                    {
                        var item = normal[j][k];
                        var texture = changeColor(item.Texture, Color.Red);
                        stripe.Add(loadImage(texture, item.X, item.Y, item.Width, item.Height));
                    }

                    white.Add(new AnimatedImage(stripe));
                }

                return white.ToArray();
            }

            MovingSpriteAssets loadMonster(AnimatedImage up, AnimatedImage left, AnimatedImage right, AnimatedImage down, AnimatedImage die)
            {
                var normal = new AnimatedImage[]
                {
                    new AnimatedImage(new Image[] { up[0], up[1], up[0], up[2] }),
                    new AnimatedImage(new Image[] { left[0], left[1], left[0], left[2] }),
                    new AnimatedImage(new Image[] { right[0], right[1], right[0], right[2] }),
                    new AnimatedImage(new Image[] { down[0], down[1], down[0], down[2] }),
                    die
                };

                return new MovingSpriteAssets(normal, monsterToGhost(normal), monsterToRed(normal));
            }

            AnimatedImage loadBonus(Image img, Image background)
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

            AnimatedImage loadPermanentWall(AnimatedImage fireImages, Image wall)
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
            var imgCrayon2 = content.Load<Texture2D>("CRAYON2");
            var imgSoucoupe = content.Load<Texture2D>("SOUCOUPE");
            var imgBonus = content.Load<Texture2D>("BONUS");
            var imgFootanim = content.Load<Texture2D>("FOOTANIM");
            var imgControls = content.Load<Texture2D>("CONTROLS");

            var monster2walk = loadImageStripe(imgFeuille, 79, 128, 16, 19, 3, 0);
            var monster3walk = loadImageStripe(imgFeuille, 42, 148, 16, 18, 5, 1);
            var monster6walk = loadImageStripe(imgFeuille, 80, 100, 26, 27, 2, 1);

            var snail =
                new AnimatedImage(
                    loadImageStripe(imgFeuille, 41, 17, 38, 32, 6, 1),
                    loadImageStripe(imgFeuille, 41, 50, 38, 32, 6, 1));

            var fire = loadImageStripe(imgSprite2, 0, 172, 26, 27, 7, 6);
            var bonusBackground = loadImage(imgBonus, 0, 0, 160, 16);

            var blackPixel = new Texture2D(graphics, 1, 1);
            blackPixel.SetData(new Color[] { Color.Black });

            var pa = new AnimatedImage(
                loadImageStripe(imgMed3, 69, 23, 16, 16, 9),
                loadImageStripe(imgMed3, 69, 40, 16, 16, 4),
                loadImageStripe(imgMed3, 133, 44, 16, 12, 2)
                );

            var penguinUp = new AnimatedImage(
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[0], pa[1],
                pa[2], pa[0], pa[3],
                pa[4], pa[5], pa[6], pa[7], pa[8]
            );

            var penguinLeft = new AnimatedImage(
                pa[9], pa[10],
                pa[9], pa[10],
                pa[9], pa[10],
                pa[9], pa[10],
                pa[9], pa[10],
                pa[9], pa[10],
                pa[9],
                pa[7], pa[8], pa[4], pa[5], pa[6]
            );

            var penguinDown = new AnimatedImage(
                pa[13], pa[14],
                pa[13], pa[14],
                pa[13], pa[14],
                pa[13], pa[14],
                pa[13], pa[14],
                pa[13],
                pa[6], pa[7], pa[8], pa[4], pa[5]
            );

            var footanim1 = loadImageStripe(imgFootanim, 24, 24, 23, 23, 12, 1);
            var footanim2 = loadImageStripe(imgFootanim, 0, 48, 23, 23, 8, 1);

            var footanimAssets = new AnimatedImage(
                loadImageStripe(imgFootanim, 0, 72, 48, 32, 6, 1),
                loadImageStripe(imgFootanim, 0, 105, 48, 32, 6, 1),
                loadImageStripe(imgFootanim, 0, 138, 48, 32, 6, 1)
            );

            var footanimGirls1 = new AnimatedImage(
                footanimAssets[0], footanimAssets[1], footanimAssets[2], footanimAssets[3], footanimAssets[4],
                footanimAssets[5], footanimAssets[4], footanimAssets[3], footanimAssets[2], footanimAssets[1]
            );

            var footanimGirls2 = new AnimatedImage(footanimAssets[4], footanimAssets[5]);

            var monsters = new MovingSpriteAssets[]
                {
                    loadMonster(loadImageStripe(imgSprite, 0, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 72, 144, 17, 18, 3, 7),
                                loadImageStripe(imgSprite, 144, 144, 17, 18, 3, 7),
                                new AnimatedImage(
                                    loadImageStripe(imgSprite, 216, 144, 17, 18, 2, 7),
                                    loadImageStripe(imgSprite, 0, 163, 17, 18, 1, 7)),
                                loadImageStripe(imgSprite, 24, 163, 17, 18, 8, 7)),

                    loadMonster(loadImageStripe(imgMed3, 89, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 56, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 188, 89, 32, 32, 3, 1),
                                loadImageStripe(imgMed3, 89, 89, 32, 32, 3, 1),
                                new AnimatedImage(loadImageStripe(imgMed3, 89, 122, 32, 32, 4, 1),
                                loadImageStripe(imgMed3, 89, 155, 32, 32, 3, 1))),

                    loadMonster(monster2walk, monster2walk, monster2walk, monster2walk,
                                loadImageStripe(imgFeuille, 127, 128, 16, 19, 6, 0)),

                    loadMonster(new AnimatedImage(snail[0], snail[1], snail[0], snail[1]),
                                new AnimatedImage(snail[4], snail[5], snail[4], snail[5]),
                                new AnimatedImage(snail[2], snail[3], snail[2], snail[3]),
                                new AnimatedImage(snail[6], snail[7], snail[6], snail[7]),
                                new AnimatedImage(snail[8], snail[9], snail[10], snail[11])),

                    loadMonster(monster3walk, monster3walk, monster3walk, monster3walk,
                                loadImageStripe(imgFeuille, 127, 148, 16, 19, 6, 1)),

                    loadMonster(loadImageStripe(imgPause, 0 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 1 * 24 * 3, 158, 23, 21, 3, 1),
                                loadImageStripe(imgPause, 2 * 24 * 3, 158, 23, 21, 3, 1),
                                new AnimatedImage(
                                    loadImageStripe(imgPause, 3 * 24 * 3, 158, 23, 21, 2, 1),
                                    loadImageStripe(imgPause, 0, 179, 23, 21, 1, 1)),
                                loadImageStripe(imgPause, 24, 179, 23, 21, 8, 1)),

                    loadMonster(monster6walk, monster6walk, monster6walk, monster6walk,
                                loadImageStripe(imgFeuille, 134, 100, 26, 27, 4, 1))
                };

            return new Assets()
            {
                Sounds = SoundAssets.Load(content),
                Bomb = loadImageStripe(imgSprite2, 0 * 16, 1 * 16, 16, 16, 4),
                PowerUps = new AnimatedImage[] {
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
                Levels = new Level[]
                {
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
                            loadImage(imgNeige1, 0, 0, 320, 200),
                            loadImage(imgNeige2, 0, 0, 320, 200),
                            loadImage(imgNeige3, 0, 0, 320, 200)
                        ),
                        BackgroundSprites = new Level.Overlay[]
                        {
                            new Level.Overlay(9 + 0 * 17, 1, penguinUp, 16),
                            new Level.Overlay(9 + 1 * 17, 1, penguinUp, 16),
                            new Level.Overlay(9 + 2 * 17, 1, penguinUp, 16),
                            new Level.Overlay(114, 1, penguinUp, 16),
                            new Level.Overlay(162, 1, penguinUp, 16),
                            new Level.Overlay(194, 1, penguinUp, 16),
                            new Level.Overlay(258, 1, penguinUp, 16),
                            new Level.Overlay(1, 33, penguinLeft, 16),
                            new Level.Overlay(1, 50, penguinLeft, 16),
                            new Level.Overlay(1, 104, penguinLeft, 16),
                            new Level.Overlay(1, 151, penguinLeft, 16),
                            new Level.Overlay(1, 168, penguinLeft, 16),
                        },
                        Overlays = new Level.Overlay[] {
                            new Level.Overlay() {
                                x = 232,
                                y = 57,
                                AnimationDelay = 1,
                                Images = new AnimatedImage(loadImage(imgMed3, 0, 77, 6 * 8, 44))
                            },
                            new Level.Overlay() {
                                x = 112,
                                y = 30,
                                AnimationDelay = 20,
                                Images = loadImageStripe(imgMed3, 0, 17 * 8, 32, 49, 2, 1),
                            },
                            new Level.Overlay(50,  200 - 12, penguinDown, 16),
                            new Level.Overlay(67,  200 - 12, penguinDown, 16),
                            new Level.Overlay(117, 200 - 12, penguinDown, 16),
                            new Level.Overlay(202, 200 - 12, penguinDown, 16),
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 80, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 1, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
                            loadImage(content.Load<Texture2D>("GAME1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME2"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("GAME3"), 0, 0, 320, 200)
                        ),
                        Walls = loadImageStripe(imgPause, 0 * 16, 128, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
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
                        Backgrounds = new AnimatedImage(
                            loadImage(content.Load<Texture2D>("NUAGE1"), 0, 0, 320, 200),
                            loadImage(content.Load<Texture2D>("NUAGE2"), 0, 0, 320, 200)
                        ),
                        MovingBackground = loadImage(imgSprite2, 64, 16, 48, 44),
                        Walls = loadImageStripe(imgPause, 0 * 16, 96, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 0, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
                            loadImage(content.Load<Texture2D>("FORET"), 0, 0, 320, 200)
                        ),
                        Overlays = new Level.Overlay[]
                        {
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(content.Load<Texture2D>("FEUILLE"), 0, 0, 320, 15),
                                x = 0, y = 0, AnimationDelay = 1
                            },
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(content.Load<Texture2D>("FEUILLE"), 0, 16, 31, 149),
                                x = 0, y = 16, AnimationDelay = 1
                            },
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(content.Load<Texture2D>("FEUILLE"), 320 - 31, 16, 31, 149),
                                x = 320 - 31, y = 16, AnimationDelay = 1
                            },
                            new Level.Overlay()
                            {
                                Images = loadImageStripe(content.Load<Texture2D>("FEUILLE"), 0, 166, 320, 34),
                                x = 0, y = 166, AnimationDelay = 1
                            },
                        },
                        Walls = loadImageStripe(imgPause, 0 * 16, 64, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 1, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
                            loadImage(content.Load<Texture2D>("SOCCER"), 0, 0, 320, 200)
                        ),
                        BackgroundSprites = new Level.Overlay[]
                        {
                            new Level.Overlay(0, 140, new AnimatedImage(
                                new AnimatedImage(Enumerable.Repeat(loadImage(imgFootanim, 0, 0, 23, 23), 16).ToList()),
                                loadImageStripe(imgFootanim, 0, 0, 23, 23, 5, 1)), 10),
                            new Level.Overlay(299, 24, new AnimatedImage(
                                new AnimatedImage(Enumerable.Repeat(loadImage(imgFootanim, 120, 0, 23, 23), 24).ToList()),
                                loadImageStripe(imgFootanim, 120, 0, 23, 23, 5, 1)), 10),

                            new Level.Overlay(2, 0, new AnimatedImage(footanim1[0], footanim1[1], footanim1[0], footanim1[2]), 16),
                            new Level.Overlay(320 - 2 - 23, 0, new AnimatedImage(footanim1[5], footanim1[6], footanim1[5], footanim1[7]), 16),
                            new Level.Overlay(0, 200 - 23 - 10, new AnimatedImage(footanim1[11], footanim2[1], footanim2[0], footanim2[1]), 16),
                            new Level.Overlay(320 - 2 - 23, 200 - 23 - 40, new AnimatedImage(footanim2[3], footanim2[4], footanim2[3], footanim2[5]), 16),
                        },
                        Overlays = new Level.Overlay[]
                        {
                            new Level.Overlay(20, 200 - 32, footanimGirls1, 12),

                            new Level.Overlay(100, 200 - 32, footanimGirls2, 12),
                            new Level.Overlay(150, 200 - 32, footanimGirls2, 12),

                            new Level.Overlay(320 - 48 - 48, 200 - 32, footanimGirls1, 12),
                        },
                        Walls = loadImageStripe(imgPause, 160, 112, 16, 16, 8),
                        PermanentWalls = loadPermanentWall(fire, loadImage(imgPause, 256 + 16 * 3, 16 * 0, 16, 16)),
                    },
                    new Level()
                    {
                        Backgrounds = new AnimatedImage(
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
                        Backgrounds = new AnimatedImage(
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
                Players = loadPlayers(imgSprite, imgSprite3),
                Pause = loadImageStripe(imgPause, 0, 0, 48, 64, 4, 0),
                Monsters = monsters,
                InsertCoin = loadImageStripe(imgCrayon2, 74, 27, 58, 62, 3, 0),
                Start = loadImage(content.Load<Texture2D>("MENU"), 0, 0, 320, 200),
                Alpha = new AnimatedImage[] {
                    loadImageStripe(imgAlpha, 0, 0, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 8, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 16, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 24, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 32, 8, 6, 44),
                    loadImageStripe(imgAlpha, 0, 40, 8, 6, 44),
                },
                BigDigits = loadImageStripe(imgFeuille, 80, 83, 15, 16, 11, 1),
                Draw = new AnimatedImage(
                    loadImage(content.Load<Texture2D>("DRAW1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("DRAW2"), 0, 0, 320, 200)),
                Med = loadImage(content.Load<Texture2D>("MED"), 0, 0, 320, 200),
                MedC = loadImage(content.Load<Texture2D>("MEDC"), 0, 0, 320, 200),
                MedG = loadImage(content.Load<Texture2D>("MEDG"), 0, 0, 320, 200),
                Coin = new AnimatedImage(loadImageStripe(imgMed3, 0, 0, 22, 22, 13, 1),
                    loadImageStripe(imgMed3, 0, 23, 22, 22, 3, 1)),
                Vic = new AnimatedImage(
                    loadImage(content.Load<Texture2D>("VIC1"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC2"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC3"), 0, 0, 320, 200),
                    loadImage(content.Load<Texture2D>("VIC4"), 0, 0, 320, 200)),
                Splash = loadImage(content.Load<Texture2D>("PIC"), 0, 0, 320, 200),
                DrawGameIn = loadImage(imgSoucoupe, 96, 48, 78, 36),
                DrawGameInNumbers = loadImageStripe(imgSoucoupe, 173, 32, 8, 7, 10),
                Controls = new Image[]
                {
                    loadImage(content.Load<Texture2D>("UI_A"), 0, 0, 32, 32),
                    loadImage(imgControls, 273, 97, 25, 14),
                    loadImage(imgControls, 81, 305, 14, 14),
                    loadImage(imgControls, 513, 161, 30, 30),
                    loadImage(imgControls, 193, 353, 14, 14),
                    loadImage(imgControls, 161, 353, 14, 14),
                },
                MenuFont = content.Load<SpriteFont>(@"font\Menu"),
                MenuFontBig = content.Load<SpriteFont>(@"font\Menu-big"),
                DebugFont = content.Load<SpriteFont>(@"font\Debug"),
                BlackPixel = blackPixel,
                StartButton = content.Load<Texture2D>("START"),
            };
        }
    }
}
