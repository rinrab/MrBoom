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

        private int maxBoom;
        private int maxBombsCount;
        private bool IsHaveRollers;
        private int lifeCount;

        public Sprite(Terrain map, Assets.MovingSpriteAssets animations) : base(map, animations)
        {
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
                this.frameIndex += 4;
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

            base.Update(true);

            int cellX = (this.x + 8) / 16;
            int cellY = (this.y + 8) / 16;
            var cell = terrain.GetCell(cellX, cellY);

            if ((this.Controller.IsKeyDown(PlayerKeys.Bomb) || autoBombPlacing > 0) &&
                bombsPlacingDisabled == 0)
            {
                if (cell.Type == TerrainType.Free && this.BombsPlaced < this.maxBombsCount)
                {
                    terrain.PutBomb(cellX, cellY, this.maxBoom, this.RcAllowed, this);

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
    }
}
