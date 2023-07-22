namespace MrBoom
{
    public class Player
    {
        public IController Controller;
        public string Name;
        public int VictoryCount;

        public Player(IController controller)
        {
            Controller = controller;
        }
    }
}
