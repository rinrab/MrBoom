// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;

namespace MrBoom
{
    public abstract class AbstractMonster : Sprite
    {
        protected BtNode tree;

        private int livesCount;

        public AbstractMonster(Terrain map,Map.AbstractMonsterData monsterData, Assets.MovingSpriteAssets animations,
                               int x, int y) : base(map, animations, x, y, monsterData.Speed)
        {
            livesCount = monsterData.LivesCount - 1;

            if (monsterData.IsSlowStart)
            {
                unplugin = 120;
            }
        }

        public override void Update()
        {
            tree.Update();

            base.Update();

            if (IsAlive)
            {
                Cell cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);
                if (cell.Type == TerrainType.Fire && unplugin == 0)
                {
                    PlaySound(Sound.Ai);
                    if (livesCount > 0)
                    {
                        livesCount--;
                        unplugin = 165;
                    }
                    else
                    {
                        Kill();
                        terrain.SetCell((X + 8) / 16, (Y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                if (cell.Type == TerrainType.Apocalypse)
                {
                    Kill();
                    PlaySound(Sound.Ai);
                }
            }
        }

        public override string GetDebugInfo()
        {
            return tree.ToString();
        }
    }
}
