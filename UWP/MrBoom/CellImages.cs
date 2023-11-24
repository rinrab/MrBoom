// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class CellImages
    {
        private readonly Assets assets;
        private readonly Assets.Level levelAssets;

        public CellImages(Assets assets, Assets.Level levelAssets)
        {
            this.assets = assets;
            this.levelAssets = levelAssets;
        }

        public AnimatedImage GetCellImage(CellImageType type)
        {
            switch (type)
            {
                case CellImageType.Walls: return levelAssets.Walls;
                case CellImageType.Bomb: return assets.Bomb;
                case CellImageType.PowerUpFire: return assets.Fire;
                case CellImageType.Apocalypse: return levelAssets.PermanentWalls;

                // Booms
                case CellImageType.BoomMid: return assets.BoomMid;
                case CellImageType.BoomHor: return assets.BoomHor;
                case CellImageType.BoomLeftEnd: return assets.BoomLeftEnd;
                case CellImageType.BoomRightEnd: return assets.BoomRightEnd;
                case CellImageType.BoomVert: return assets.BoomVert;
                case CellImageType.BoomTopEnd: return assets.BoomTopEnd;
                case CellImageType.BoomBottomEnd: return assets.BoomBottomEnd;

                // Power-ups
                case CellImageType.Banana: return assets.PowerUps[0];
                case CellImageType.ExtraBomb: return assets.PowerUps[1];
                case CellImageType.ExtraFire: return assets.PowerUps[2];
                case CellImageType.Skull: return assets.PowerUps[3];
                case CellImageType.Shield: return assets.PowerUps[4];
                case CellImageType.Life: return assets.PowerUps[5];
                case CellImageType.RemoteControl: return assets.PowerUps[6];
                case CellImageType.Kick: return assets.PowerUps[7];
                case CellImageType.RollerSkate: return assets.PowerUps[8];
                case CellImageType.Clock: return assets.PowerUps[9];
                case CellImageType.MultiBomb: return assets.PowerUps[10];

                // Not handled and cells without image
                default: return AnimatedImage.Empty;
            }
        }
    }

    public enum CellImageType
    {
        Walls,
        Bomb,
        PowerUpFire,
        Apocalypse,

        // Booms
        BoomMid,
        BoomHor,
        BoomLeftEnd,
        BoomRightEnd,
        BoomVert,
        BoomTopEnd,
        BoomBottomEnd,

        // Non-image
        Free,
        PermanentWall,
        Rubber,

        // Power-ups
        Banana,
        ExtraBomb,
        ExtraFire,
        Skull,
        Shield,
        Life,
        RemoteControl,
        Kick,
        RollerSkate,
        Clock,
        MultiBomb,
    }
}
