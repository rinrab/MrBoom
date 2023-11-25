// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public class CellImages
    {
        private readonly AnimatedImage[] images;

        public CellImages(Assets assets, Assets.Level levelAssets)
        {
            images = new AnimatedImage[]
            {
                levelAssets.Walls,
                assets.Bomb,
                assets.Fire,
                levelAssets.PermanentWalls,

                // Booms
                assets.BoomMid,
                assets.BoomHor,
                assets.BoomLeftEnd,
                assets.BoomRightEnd,
                assets.BoomVert,
                assets.BoomTopEnd,
                assets.BoomBottomEnd,

                // Non-image
                AnimatedImage.Empty,
                AnimatedImage.Empty,
                AnimatedImage.Empty,

                // Power-ups
                assets.PowerUps[0],
                assets.PowerUps[1],
                assets.PowerUps[2],
                assets.PowerUps[3],
                assets.PowerUps[4],
                assets.PowerUps[5],
                assets.PowerUps[6],
                assets.PowerUps[7],
                assets.PowerUps[8],
                assets.PowerUps[9],
                assets.PowerUps[10],
            };
        }

        public AnimatedImage GetCellImage(CellImageType type)
        {
            return images[(int)type];
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
