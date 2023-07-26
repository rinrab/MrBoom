using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class Sprite : MovingSprite
    {
        public IController Controller;
        public int BombsPlaced;
        public bool rcDitonate = false;
        
        public bool RcAllowed { get; private set; }
        public bool IsDie { get; private set; } = false;

        private readonly Assets.PlayerAssets animations;
        private readonly Assets.ImageStripe ghosts;
        private readonly Assets.ImageStripe bombAssets;

        private int maxBoom;
        private int maxBombsCount;
        private bool IsHaveRollers;
        private int lifeCount;
        private int unplugin = 180;
        private int freeze = 180;
        private int reverse;
        private int autoBombPlacing;
        private int bombsPlacingDisabled;

        public Sprite(Terrain map, Assets.PlayerAssets animations, Assets.ImageStripe ghosts, Assets.ImageStripe bombAssets) : base(map)
        {
            this.animations = animations;
            this.ghosts = ghosts;
            this.bombAssets = bombAssets;
            this.animateIndex = 0;
            this.frameIndex = 0;
            this.speed = 1;
            this.BombsPlaced = 0;
            this.RcAllowed = false;
            this.isHaveKick = map.StartKick;
            this.maxBoom = map.StartMaxFire;
            this.maxBombsCount = map.StartMaxBombsCount;
        }

        public override void Update()
        {
            if (this.IsDie)
            {
                this.animateIndex = 4;
                if (this.frameIndex < 7 * 20 && this.frameIndex != -1)
                {
                    this.frameIndex += 4;
                }
                else
                {
                    this.frameIndex = -1;
                }
                return;
            }

            this.Direction = MovingSprite.Directions.Right;

            this.Direction = MovingSprite.Directions.None;
            if (this.Controller.IsKeyDown(PlayerKeys.Up))
            {
                this.Direction = Directions.Up;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Left))
            {
                this.Direction = Directions.Left;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Right))
            {
                this.Direction = Directions.Right;
            }
            else if (this.Controller.IsKeyDown(PlayerKeys.Down))
            {
                this.Direction = Directions.Down;
            }

            if (reverse > 0)
            {
                if (Direction == Directions.Up) Direction = Directions.Down;
                else if (Direction == Directions.Down) Direction = Directions.Up;
                if (Direction == Directions.Left) Direction = Directions.Right;
                else if (Direction == Directions.Right) Direction = Directions.Left;
            }

            this.rcDitonate = this.RcAllowed && this.Controller.IsKeyDown(PlayerKeys.RcDitonate);

            if (freeze > 0)
            {
                base.Update(false);
                freeze--;
            }
            else
            {
                base.Update(true);
            }

            int cellX = (this.x + 8) / 16;
            int cellY = (this.y + 8) / 16;
            var cell = terrain.GetCell(cellX, cellY);

            if ((this.Controller.IsKeyDown(PlayerKeys.Bomb) || autoBombPlacing > 0) && bombsPlacingDisabled == 0 && freeze == 0)
            {
                if (cell.Type == TerrainType.Free && this.BombsPlaced < this.maxBombsCount)
                {
                    terrain.SetCell(cellX, cellY, new Cell(TerrainType.Bomb)
                    {
                        Images = bombAssets,
                        Index = 0,
                        animateDelay = 12,
                        bombTime = 210,
                        maxBoom = this.maxBoom,
                        rcAllowed = this.RcAllowed,
                        owner = this
                    });
                    this.BombsPlaced++;
                    terrain.PlaySound(Sound.PoseBomb);
                }
            }

            if (autoBombPlacing > 0)
            {
                autoBombPlacing--;
            }
            if (bombsPlacingDisabled > 0)
            {
                bombsPlacingDisabled--;
            }
            if (reverse > 0)
            {
                reverse--;
            }

            if (cell.Type == TerrainType.PowerUp)
            {
                var powerUpType = cell.PowerUpType;
                var doFire = false;

                if (powerUpType == PowerUpType.ExtraFire)
                {
                    this.maxBoom++;
                }
                else if (powerUpType == PowerUpType.ExtraBomb)
                {
                    this.maxBombsCount++;
                }
                else if (powerUpType == PowerUpType.RemoteControl)
                {
                    if (!this.RcAllowed)
                    {
                        this.RcAllowed = true;
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.RollerSkate)
                {
                    if (!this.IsHaveRollers)
                    {
                        this.speed = 2;
                        this.IsHaveRollers = true;
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.Kick)
                {
                    if (this.isHaveKick)
                    {
                        doFire = true;
                    }
                    else
                    {
                        this.isHaveKick = true;
                    }
                }
                else if (powerUpType == PowerUpType.Life)
                {
                    this.lifeCount++;
                }
                else if (powerUpType == PowerUpType.Shield)
                {
                    this.unplugin = 600;
                }
                else if (powerUpType == PowerUpType.Banana)
                {
                    for (int y = 0; y < terrain.Height; y++)
                    {
                        for (int x = 0; x < terrain.Width; x++)
                        {
                            if (terrain.GetCell(x, y).Type == TerrainType.Bomb)
                            {
                                terrain.ditonateBomb(x, y);
                            }
                        }
                    }
                }
                else if (powerUpType == PowerUpType.Clock)
                {
                    terrain.TimeLeft += 60 * 60;
                    terrain.PlaySound(Sound.Clock);
                }
                else if (powerUpType == PowerUpType.Skull)
                {
                    reverse = 0;
                    bombsPlacingDisabled = 0;
                    autoBombPlacing = 0;

                    int rnd = Terrain.Random.Next(3);
                    if (rnd == 0)
                    {
                        reverse = 600;
                    }
                    else if (rnd == 1)
                    {
                        bombsPlacingDisabled = 600;
                    }
                    else if (rnd == 2)
                    {
                        autoBombPlacing = 600;
                    }
                }

                if (doFire)
                {
                    terrain.SetCell(cellX, cellY, new Cell(TerrainType.PowerUpFire)
                    {
                        Images = terrain.assets.Fire,
                        Index = 0,
                        animateDelay = 6,
                        Next = new Cell(TerrainType.Free)
                    });
                    terrain.PlaySound(Sound.Sac);
                }
                else
                {
                    terrain.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                    terrain.PlaySound(Sound.Pick);
                }
            }

            bool isTouchingMonster = false;
            foreach (Monster m in terrain.Monsters)
            {
                if (!m.IsDie && (m.x + 8) / 16 == (x + 8) / 16 && (m.y + 8) / 16 == (y + 8) / 16)
                {
                    isTouchingMonster = true;
                }
            }

            if ((cell.Type == TerrainType.Fire || isTouchingMonster) && unplugin == 0)
            {
                if (lifeCount > 0)
                {
                    lifeCount--;
                    this.unplugin = 165;
                }
                else
                {
                    this.IsDie = true;
                    this.frameIndex = 0;
                    terrain.PlaySound(Sound.PlayerDie);
                }
            }
            if (cell.Type == TerrainType.Apocalypse)
            {
                unplugin = 0;
                this.IsDie = true;
                this.frameIndex = 0;
                terrain.PlaySound(Sound.PlayerDie);
            }

            if (unplugin != 0)
            {
                unplugin--;
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (frameIndex != -1)
            {
                Assets.ImageStripe animation = this.animations.Normal[this.animateIndex];
                Assets.AssetImage img = animation[frameIndex / 20];

                int x = this.x + 8 + 8 - img.Width / 2;
                int y = this.y + 16 - img.Height;

                Color color = Color.White;

                if (autoBombPlacing % 30 > 15 || reverse % 30 > 15 || bombsPlacingDisabled % 30 > 15)
                {
                    color = new Color(255, 0, 0);
                }

                int[] index = new int[] { 0, 1, 0, 2 };

                if (unplugin == 0 || unplugin % 30 < 15)
                {
                    img.Draw(ctx, x, y, color);
                }
                else
                {
                    ghosts[animateIndex * 3 + index[frameIndex / 20 % 4]].Draw(ctx, x, y, color);
                }
            }
        }
    }
}
