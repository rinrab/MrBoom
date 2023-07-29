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

        public class MonsterData
        {
            public int Slow;
            public int WaitAfterTurn { get; }
            public int Type { get; }
            public int LivesCount { get; }

            public MonsterData(int type, int waitAfterTurn, int livesCount)
            {
                WaitAfterTurn = waitAfterTurn;
                Type = type;
                LivesCount = livesCount;

                Slow = 1;
            }
        }

        public string[] Data;
        public int Time;
        public PowerUpData[] PowerUps;
        public MonsterData[] Monsters;
        public byte[] Final;
        public int StartMaxFire = 1;
        public int StartMaxBombsCount = 1;
        public Feature StartFeatures = 0;

        public static byte[] DefaultFinal = new byte[]
        {
            000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000,
            000, 001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016, 017, 000,
            000, 052, 053, 054, 055, 056, 057, 058, 059, 060, 061, 062, 063, 064, 065, 066, 067, 018, 000,
            000, 051, 096, 097, 098, 099, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 068, 019, 000,
            000, 050, 095, 132, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 110, 069, 020, 000,
            000, 049, 094, 131, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 111, 070, 021, 000,
            000, 048, 093, 130, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 112, 071, 022, 000,
            000, 047, 092, 129, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 113, 072, 023, 000,
            000, 046, 091, 128, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 114, 073, 024, 000,
            000, 045, 090, 127, 126, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 074, 025, 000,
            000, 044, 089, 088, 087, 086, 085, 084, 083, 082, 081, 080, 079, 078, 077, 076, 075, 026, 000,
            000, 043, 042, 041, 040, 039, 038, 037, 036, 035, 034, 033, 032, 031, 030, 029, 028, 027, 000,
            000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000
        };

        public static Map[] Maps = new Map[]
        {
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#* ------------- *#",
                    "# #-# #-#-#-#-#-# #",
                    "#--- *------ *----#",
                    "#-#-#-###-#-# # #-#",
                    "#-----###-----   -#",
                    "#-#-#-###-#-#-# #-#",
                    "#--------- *------#",
                    "#-#-#-# #-# #-#-#-#",
                    "#----- *----------#",
                    "# #-#-#-#-#-#-# ###",
                    "#* ----------- *###",
                    "###################"
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
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(0, 30, 1)
                },
                Final = DefaultFinal
            },
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
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(1, 30, 3)
                },
                Final = DefaultFinal
            },
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#-------------    #",
                    "#-# #-#-#-# #-## ##",
                    "#- * ----- * -----#",
                    "#-# #-# #-# #-# #-#",
                    "#----- * ----- * -#",
                    "#-# #-# #-# #-# #-#",
                    "#- * ----- * -----#",
                    "#-# #-# #-# #-# #-#",
                    "#----- * ----- * -#",
                    "#-#-#-# #-#-#-# #-#",
                    "#-----------------#",
                    "###################"
                },
                Time = 90,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 1),
                    new PowerUpData(PowerUpType.RemoteControl, 1),
                    new PowerUpData(PowerUpType.RollerSkate, 1),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Banana, 10),
                    new PowerUpData(PowerUpType.Kick, 2),
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(4, 60, 1)
                },
                Final = DefaultFinal
            },
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#--- *#-734-#* ---#",
                    "#-#-# #-#-#-# #-#-#",
                    "#-----#-----#-----#",
                    "# #-#-#-#-#-#-#-# #",
                    "#* ------------- *#",
                    "#######-#-#-#######",
                    "#* ------------- *#",
                    "# #-#-#-#-#-#-#-# #",
                    "#-----#-----#-----#",
                    "#-#-# #-#-#-# #-#-#",
                    "#--- *#-635-#* ---#",
                    "###################",
                },
                Time = 60,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 1),
                    new PowerUpData(PowerUpType.RemoteControl, 1),
                    new PowerUpData(PowerUpType.RollerSkate, 1),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Kick, 2),
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(0, 30, 1),
                    new MonsterData(0, 30, 1),
                    new MonsterData(1, 30, 3),
                    new MonsterData(1, 30, 3),
                    new MonsterData(2, 60, 1) { Slow = 2 },
                    new MonsterData(3, 90, 3) { Slow = 3 },
                    new MonsterData(3, 90, 3) { Slow = 3 },
                },
                Final = new byte[]
                {
                    000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000,
                    000, 100, 100, 100, 100, 100, 255, 040, 041, 042, 043, 044, 255, 085, 085, 085, 085, 085, 000,
                    000, 100, 100, 100, 100, 100, 255, 047, 255, 046, 255, 045, 255, 085, 085, 085, 085, 085, 000,
                    000, 100, 100, 100, 100, 100, 255, 255, 255, 255, 255, 255, 255, 085, 085, 085, 085, 085, 000,
                    000, 100, 100, 100, 100, 100, 255, 255, 255, 255, 255, 255, 255, 085, 085, 085, 085, 085, 000,
                    000, 100, 100, 100, 100, 100, 001, 255, 255, 255, 255, 255, 020, 085, 085, 085, 085, 085, 000,
                    000, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 000,
                    000, 070, 070, 070, 070, 070, 030, 255, 255, 255, 255, 255, 010, 110, 110, 110, 110, 110, 000,
                    000, 070, 070, 070, 070, 070, 255, 255, 255, 255, 255, 255, 255, 110, 110, 110, 110, 110, 000,
                    000, 070, 070, 070, 070, 070, 255, 255, 255, 255, 255, 255, 255, 110, 110, 110, 110, 110, 000,
                    000, 070, 070, 070, 070, 070, 255, 056, 255, 057, 255, 058, 255, 110, 110, 110, 110, 110, 000,
                    000, 070, 070, 070, 070, 070, 255, 055, 054, 053, 052, 051, 255, 110, 110, 110, 110, 110, 000,
                    000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000,
                }
            },
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#* ------------- *#",
                    "# #-###-#-###-### #",
                    "# ---*  #--*  --- #",
                    "#-### #-### ###-#-#",
                    "#-#-------#-------#",
                    "#-#-#-#-#-#-#-#-#-#",
                    "#---#- *------#---#",
                    "#-#-### #-###-###-#",
                    "# ----# ---*  --# #",
                    "# ###-#-### ###-# #",
                    "#* ------------- *#",
                    "###################",
                },
                Time = 120,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 3),
                    new PowerUpData(PowerUpType.RemoteControl, 1),
                    new PowerUpData(PowerUpType.RollerSkate, 1),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Kick, 2),
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(3, 90, 3) { Slow = 3 },
                },
                Final = DefaultFinal
            },
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#-----------------#",
                    "#-# #-#-#-#-#-# #-#",
                    "#- * --------- * -#",
                    "##-#-# #-#-# #-#-##",
                    "#---- * --- * ----#",
                    "#-#-#-#-#-#-#-#-#-#",
                    "#---- * --- * ----#",
                    "##-#-# #-#-# #-#-##",
                    "#- * --------- * -#",
                    "#-# #-#-#-#-#-# #-#",
                    "#-----------------#",
                    "###################",
                },
                StartFeatures = Feature.Kick,
                Time = 90,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 3),
                    new PowerUpData(PowerUpType.RemoteControl, 1),
                    new PowerUpData(PowerUpType.RollerSkate, 1),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Kick, 2),
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(1, 30, 3),
                    new MonsterData(3, 90, 3) { Slow = 3 },
                },
                Final = DefaultFinal
            },
            new Map()
            {
                Data = new string[]
                {
                    "###################",
                    "#* ------------- *#",
                    "# #######-####### #",
                    "#-----------------#",
                    "# #######-####### #",
                    "#* ------------- *#",
                    "#-#######-#######-#",
                    "#* ------------- *#",
                    "# #######-####### #",
                    "#-----------------#",
                    "# #######-####### #",
                    "#* ------------- *#",
                    "###################",
                },
                StartFeatures = Feature.Kick,
                Time = 120,
                PowerUps = new PowerUpData[]
                {
                    new PowerUpData(PowerUpType.ExtraBomb, 10),
                    new PowerUpData(PowerUpType.ExtraFire, 10),
                    new PowerUpData(PowerUpType.Life, 3),
                    new PowerUpData(PowerUpType.RemoteControl, 2),
                    new PowerUpData(PowerUpType.Shield, 1),
                    new PowerUpData(PowerUpType.Kick, 2),
                    new PowerUpData(PowerUpType.Clock, 1),
                    new PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new MonsterData[]
                {
                    new MonsterData(5, 30, 2),
                },
                Final = new byte[]
                {
                    0, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 0,
                    0, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 0,
                },
            },
            new Map()
            {
                Data = new string[]
                {
                    "#%%%%%%%###%%%%%%%#",
                    "%*               *%",
                    "% # # # # # # # # %",
                    "%      *   *      %",
                    "% # # # # # # # # %",
                    "%                 %",
                    "% # # # # # # # # %",
                    "%                 %",
                    "% # # # # # # # # %",
                    "%      *   *      %",
                    "% # # # # # # # # %",
                    "%*               *%",
                    "#%%%%%%%%%%%%%%%%%#",
                },
                StartFeatures = Feature.Kick,
                StartMaxFire = 8,
                StartMaxBombsCount = 8,
                Time = 30,
                PowerUps = new PowerUpData[] { },
                Monsters = new MonsterData[]
                {
                    new MonsterData(1, 30, 3),
                },
                Final = new byte[]
                {
                    000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000,
                    000, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 001, 000,
                    000, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 015, 000,
                    000, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 030, 000,
                    000, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 045, 000,
                    000, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 060, 000,
                    000, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 075, 000,
                    000, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 090, 000,
                    000, 105, 105, 105, 102, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 000,
                    000, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 000,
                    000, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 000,
                    000, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 000,
                    000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000, 000,
                },
            },
        };
    }
}
