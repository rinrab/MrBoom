// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public abstract class AbstractPlayer : Sprite
    {
        public int BombsPlaced;
        public bool rcDitonate = false;
        public int MaxBoom;
        public int MaxBombsCount;

        public int BombsRemaining
        {
            get
            {
                return MaxBombsCount - BombsPlaced;
            }
        }

        protected bool rcDitonateButton;
        protected bool dropBombButton;

        public int Team;
        public int TeamMask { get => 1 << Team; }
        private int lifeCount;

        public AbstractPlayer(Terrain map,
                              Assets.MovingSpriteAssets animations,
                              int team) : base(map, animations, 0, 0, 3)
        {
            Features = map.StartFeatures;
            MaxBoom = map.StartMaxFire;
            MaxBombsCount = map.SstartMaxBombsCount;
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
                Direction = Direction.Reverse();
            }

            rcDitonate = Features.HasFlag(Feature.RemoteControl) &&
                rcDitonateButton;

            base.Update();

            int cellX = (X + 8) / 16;
            int cellY = (Y + 8) / 16;
            var cell = terrain.GetCell(cellX, cellY);

            if ((dropBombButton || Skull == SkullType.AutoBomb) && Skull != SkullType.BombsDisable)
            {
                if (cell.Type == TerrainType.Free && BombsPlaced < MaxBombsCount)
                {
                    terrain.PutBomb(cellX, cellY, MaxBoom, Features.HasFlag(Feature.RemoteControl), this);

                    BombsPlaced++;
                    terrain.PlaySound(Sound.PoseBomb);
                }
            }

            if (cell.Type == TerrainType.PowerUp)
            {
                var powerUpType = cell.PowerUpType;
                var doFire = false;

                if (powerUpType == PowerUpType.ExtraFire)
                {
                    MaxBoom++;
                }
                else if (powerUpType == PowerUpType.ExtraBomb)
                {
                    MaxBombsCount++;
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
                    lifeCount++;
                }
                else if (powerUpType == PowerUpType.Shield)
                {
                    unplugin = 600;
                }
                else if (powerUpType == PowerUpType.Banana)
                {
                    for (int y = 0; y < terrain.Height; y++)
                    {
                        for (int x = 0; x < terrain.Width; x++)
                        {
                            if (terrain.GetCell(x, y).Type == TerrainType.Bomb)
                            {
                                terrain.DitonateBomb(x, y);
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
                    SetSkull(Terrain.Random.NextEnum<SkullType>());
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
                    unplugin = 165;
                }
                else
                {
                    Kill();
                    terrain.PlaySound(Sound.PlayerDie);
                }
            }
            if (cell.Type == TerrainType.Apocalypse)
            {
                Kill();
                terrain.PlaySound(Sound.PlayerDie);
            }
        }

        public void GiveAll()
        {
            Features |= Feature.RemoteControl | Feature.Kick;
            SetSkull(SkullType.Fast);
        }
    }
}
