using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel.DataAnnotations;

namespace MrBoom
{
    public class Sprite : MovingSprite
    {
        public IController Controller;
        public int BombsPlaced;
        public bool rcDitonate = false;

        public bool rcAllowed { get; private set; }
        public bool isDie { get; private set; } = false;

        private Assets.AssetImage[][] animations;
        private int maxBoom;
        private int maxBombsCount;
        private Assets.AssetImage[] bombAssets;

        public Sprite(Terrain map, Assets.AssetImage[][] animations, Assets.AssetImage[] bombAssets) : base(map)
        {
            //this.isPlayer = true;
            this.animations = animations;
            this.bombAssets = bombAssets;

            this.animateIndex = 0;
            this.frameIndex = 0;

            this.speed = 1;

            this.maxBoom = 1;
            this.maxBombsCount = 1;
            this.BombsPlaced = 0;

            this.rcAllowed = false;

            //this.blinking = undefined;
            //this.blinkingSpeed = 15;

            //if (cheats.god)
            //{
            //    this.unplugin = 999999;
            //    this.blinking = 0;
            //    this.blinkingSpeed = 30;
            //}

            this.x = 1 * 16;
            this.y = 1 * 16;

            //const initialBonus = map.initialBonus;
            //if (initialBonus)
            //{
            //    if (initialBonus.includes(PowerUpType.Kick))
            //    {
            //        this.movingSprite.isHaveKick = true;
            //    }
            //}
            //if (mapIndex == 7)
            //{
            //    this.maxBoom = 8;
            //    this.maxBombsCount = 8;
            //}
        }

        public override void Update()
        {
            if (this.isDie)
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

            Controller.Update();

            this.Direction = MovingSprite.Directions.None;
            if (this.Controller.Keys[PlayerKeys.Up])
            {
                this.Direction = Directions.Up;
            }
            else if (this.Controller.Keys[PlayerKeys.Left])
            {
                this.Direction = Directions.Left;
            }
            else if (this.Controller.Keys[PlayerKeys.Right])
            {
                this.Direction = Directions.Right;
            }
            else if (this.Controller.Keys[PlayerKeys.Down])
            {
                this.Direction = Directions.Down;
            }

            this.rcDitonate = this.rcAllowed && this.Controller.Keys[PlayerKeys.RcDitonate];

            base.Update();

            int cellX = (this.x + 8) / 16;
            int cellY = (this.y + 8) / 16;
            var cell = terrain.GetCell(cellX, cellY);

            if (this.Controller.Keys[PlayerKeys.Bomb])
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
                        rcAllowed = this.rcAllowed,
                        owner = this
                    });
                    this.BombsPlaced++;
                    Game.game.sound.PoseBomb.Play();
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
                    if (!this.rcAllowed)
                    {
                        this.rcAllowed = true;
                    }
                    else
                    {
                        doFire = true;
                    }
                }
                else if (powerUpType == PowerUpType.RollerSkate)
                {
                    //if (!this.isHaveRollers)
                    //{
                    //    this.speed = 2;
                    //    this.isHaveRollers = true;
                    //}
                    //else
                    //{
                    //    doFire = true;
                    //}
                }
                else if (powerUpType == PowerUpType.Kick)
                {
                    //if (this.movingSprite.isHaveKick)
                    //{
                    //    doFire = true;
                    //}
                    //else
                    //{
                    //    this.movingSprite.isHaveKick = true;
                    //}
                }
                else if (powerUpType == PowerUpType.Life)
                {
                    //this.lifeCount++;
                }
                else if (powerUpType == PowerUpType.Shield)
                {
                    //this.unplugin = 600;
                    //this.blinkingSpeed = 30;
                    //this.blinking = 0;
                }
                else if (powerUpType == PowerUpType.Banana)
                {
                    //for (let y = 0; y < map.height; y++)
                    //{
                    //    for (let x = 0; x < map.width; x++)
                    //    {
                    //        if (map.getCell(x, y).type == TerrainType.Bomb)
                    //        {
                    //            map.ditonateBomb(x, y);
                    //        }
                    //    }
                    //}
                }
                else if (powerUpType == PowerUpType.Clock)
                {
                    //map.timeLeft += 60;
                    //soundManager.playSound("clock");
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
                    Game.game.sound.Sac.Play();
                }
                else
                {
                    terrain.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                    Game.game.sound.Pick.Play();
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

            if (cell.Type == TerrainType.Fire || isTouchingMonster)
            {
                this.isDie = true;
                this.frameIndex = 0;
                Game.game.sound.PlayerDie.Play();
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (frameIndex != -1)
            {
                Assets.AssetImage[] animation = this.animations[this.animateIndex];
                Assets.AssetImage img = animation[frameIndex / 20 % animation.Length];
                //if (this.blinking % this.blinkingSpeed * 2 < this.blinkingSpeed)
                //{
                //    img = assets.boyGhost[this.animateIndex * 3 + frames[frameIndex % 4]];
                //}

                int x = this.x + 8 + 8 - img.Width / 2;
                int y = this.y + 16 - img.Height;

                img.Draw(ctx, x, y);
            }
        }
    }
}
