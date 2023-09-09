﻿// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public abstract class AbstractPlayer : Sprite
    {
        public int BombsPlaced;
        public bool rcDitonate = false;
        public int MaxBoom { get => maxBoom; }

        public int BombsRemaining
        {
            get
            {
                return maxBombsCount - BombsPlaced;
            }
        }
        protected bool rcDitonateButton;
        protected bool dropBombButton;
        public int Team;
        private int maxBoom;
        private int maxBombsCount;
        private int lifeCount;

        public AbstractPlayer(Terrain map, Assets.MovingSpriteAssets animations, int x, int y, int maxBoom, int maxBombs, int team) :
            base(map, animations, x, y, 3)
        {
            Features = map.StartFeatures;
            this.maxBoom = maxBoom;
            this.maxBombsCount = maxBombs;
            Team = team;
        }

        public override void Update()
        {
            if (IsDie)
            {
                base.Update();
                return;
            }

            if (Skull == SkullType.Reverse)
            {
                if (Direction == Directions.Up) Direction = Directions.Down;
                else if (Direction == Directions.Down) Direction = Directions.Up;
                if (Direction == Directions.Left) Direction = Directions.Right;
                else if (Direction == Directions.Right) Direction = Directions.Left;
            }

            this.rcDitonate = Features.HasFlag(Feature.RemoteControl) &&
                rcDitonateButton;

            base.Update();

            int cellX = (this.X + 8) / 16;
            int cellY = (this.Y + 8) / 16;
            var cell = terrain.GetCell(cellX, cellY);

            if ((dropBombButton || Skull == SkullType.AutoBomb) && Skull != SkullType.BombsDisable)
            {
                if (cell.Type == TerrainType.Free && this.BombsPlaced < this.maxBombsCount)
                {
                    terrain.PutBomb(cellX, cellY, this.maxBoom, Features.HasFlag(Feature.RemoteControl), this);

                    this.BombsPlaced++;
                    terrain.PlaySound(Sound.PoseBomb);
                }
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
                    if (!Features.HasFlag(Feature.RemoteControl))
                    {
                        Features |= Feature.RemoteControl;
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.RollerSkate)
                {
                    if (!Features.HasFlag(Feature.RollerSkates))
                    {
                        Features |= Feature.RollerSkates;
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.Kick)
                {
                    if (Features.HasFlag(Feature.Kick))
                    {
                        doFire = true;
                    }
                    else
                    {
                        Features |= Feature.Kick;
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
                    if (terrain.TimeLeft > 31 * 60 + terrain.MaxApocalypse * terrain.ApocalypseSpeed)
                    {
                        terrain.TimeLeft += 60 * 60;
                        terrain.PlaySound(Sound.Clock);
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.Skull)
                {
                    Array values = Enum.GetValues(typeof(SkullType));
                    SetSkull((SkullType)values.GetValue(Terrain.Random.Next(values.Length)));
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

            bool isTouchingMonster = terrain.IsTouchingMonster((X + 8) / 16, (Y + 8) / 16);

            if ((cell.Type == TerrainType.Fire || isTouchingMonster) && unplugin == 0)
            {
                if (lifeCount > 0)
                {
                    lifeCount--;
                    Features = 0;
                    terrain.PlaySound(Sound.Oioi);
                    this.unplugin = 165;
                }
                else
                {
                    Kill();
                    this.frameIndex = 0;
                    terrain.PlaySound(Sound.PlayerDie);
                }
            }
            if (cell.Type == TerrainType.Apocalypse)
            {
                unplugin = 0;
                Kill();
                this.frameIndex = 0;
                terrain.PlaySound(Sound.PlayerDie);
            }
        }

        public void GiveAll()
        {
            Features |= Feature.RemoteControl | Feature.Kick;
            SetSkull(SkullType.Fast);
        }

        public abstract string GetDebugInfo();
    }
}
