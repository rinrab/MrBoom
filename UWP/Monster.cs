// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;
using MrBoom.Bot;

namespace MrBoom
{
    public class Monster : Sprite
    {
        private readonly BtNode tree;

        private int livesCount;

        public Monster(Terrain map, Map.MonsterData monsterData,
            Assets.MovingSpriteAssets animations, int x, int y) :
            base(map, animations, x, y, monsterData.Speed)
        {
            tree = new BtSequence()
            {
                new DelayNode(monsterData.IsSlowStart ? 120 : 0),
                new BtRepeater(new BtSequence()
                    {
                        new ActionNode(ChooseDirection, "ChooseDirection"),
                        new ActionNode(Walk, "Walk"),
                        new DelayNode(monsterData.WaitAfterTurn, "Wait")
                    })
            };

            livesCount = monsterData.LivesCount - 1;

            if (monsterData.IsSlowStart)
            {
                unplugin = 120;
            }
        }

        private BtStatus ChooseDirection()
        {
            for (int i = 0; ; i++)
            {
                Directions dir = Terrain.Random.NextEnum<Directions>();

                if (IsWalkable(dir.DeltaX(), dir.DeltaY()))
                {
                    Direction = dir;
                    return BtStatus.Success;
                }
                if (i >= 32)
                {
                    Direction = null;
                    return BtStatus.Failure;
                }
            }
        }

        private BtStatus Walk()
        {
            if (X % 16 == 0 && Y % 16 == 0 && Terrain.Random.Next(16) == 0)
            {
                frameIndex = 0;
                Direction = null;
                return BtStatus.Success;
            }
            else if (!IsWalkable(Direction.DeltaX(), Direction.DeltaY()))
            {
                frameIndex = 0;
                Direction = null;
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Running;
            }
        }

        bool IsWalkable(int dx, int dy)
        {
            switch (terrain.GetCell((X + dx * 8 + 8 + dx) / 16, (Y + dy * 8 + 8 + dy) / 16).Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                case TerrainType.PowerUp:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                case TerrainType.Fire:
                case TerrainType.Apocalypse:
                case TerrainType.Rubber:
                    return false;

                default: return true;
            }
        }

        public override void Update()
        {
            if (IsAlive)
            {
                var cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);
                if (cell.Type == TerrainType.Fire && unplugin == 0)
                {
                    terrain.PlaySound(Sound.Ai);
                    if (livesCount > 0)
                    {
                        livesCount--;
                        unplugin = 165;
                    }
                    else
                    {
                        Kill();
                        frameIndex = 0;
                        terrain.SetCell((X + 8) / 16, (Y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                if (cell.Type == TerrainType.Apocalypse)
                {
                    Kill();
                    frameIndex = 0;
                    terrain.PlaySound(Sound.Ai);
                }
                else
                {
                    tree.Update();
                }
            }
            base.Update();
        }

        public override string GetDebugInfo()
        {
            return tree.ToString();
        }
    }
}
