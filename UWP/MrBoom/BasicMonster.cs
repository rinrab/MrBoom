// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;

namespace MrBoom
{
    public class BasicMonster : AbstractMonster
    {

        public BasicMonster(Terrain map, Map.BasicMonsterData monsterData, Assets.MovingSpriteAssets animations,
                            int x, int y) : base(map, monsterData, animations, x, y)
        {
            tree = new BtSequence()
            {
                new DelayNode(monsterData.IsSlowStart ? 120 : 0),
                new BtRepeater(new BtSequence()
                    {
                        new ActionNode(ChooseDirection, nameof(ChooseDirection)),
                        new ActionNode(Walk, nameof(Walk)),
                        new DelayNode(monsterData.WaitAfterTurn, "Think")
                    })
            };
        }


        private BtStatus ChooseDirection()
        {
            for (int i = 0; ; i++)
            {
                Directions dir = terrain.Random.NextEnum<Directions>();

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
            if (X % 16 == 0 && Y % 16 == 0 && terrain.Random.Next(16) == 0)
            {
                Direction = null;
                return BtStatus.Success;
            }
            else if (!IsWalkable(Direction.DeltaX(), Direction.DeltaY()))
            {
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
    }
}
