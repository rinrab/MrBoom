namespace MrBoom
{
    public class Map
    {
        public class PowerUpData
        {
            public PowerUpType Type;
            public int Count;

            public PowerUpData(PowerUpType type, int count)
            {
                this.Type = type;
                this.Count = count;
            }
        }

        public string[] Data;
        public int Time;
        public PowerUpData[] PowerUps;

        public static Map[] Maps = new Map[]
        {
            new Map()
            {
                Data = new string[]
                {
                     "###################",
                    "#* ------------- *#",
                    "# #-#-#-#-#-#-#-# #",
                    "#----- *---* -----#",
                    "#-#-#-# #-# #-#-#-#",
                    "#-----------------#",
                    "#-#-#-#-#-#-#-#-#-#",
                    "#-----------------#",
                    "#-#-#-# #-# #-#-#-#",
                    "#----- *---* -----#",
                    "# #-#-#-#-#-#-#-# #",
                    "#* ------------- *#",
                    "###################",
                },
                Time = 120,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 1),
                    new PowerUpData(PowerUpType.RemoteControl, 1),
                    new PowerUpData(PowerUpType.RollerSkate, 1),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Kick, 2),
                }
            }
        };
    }
}
