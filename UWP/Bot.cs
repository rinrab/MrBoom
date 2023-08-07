namespace MrBoom
{
    public class Bot : AbstarctPlayer
    {
        private int timer = 0;

        public Bot(Terrain map, Assets.MovingSpriteAssets animations,
            int maxBoom, int maxBombs) : base(map, animations, maxBoom, maxBombs)
        {
        }

        public override void Update()
        {
            timer++;
            Direction = (Directions)(timer / 16 % 5);

            base.Update();
        }
    }
}
