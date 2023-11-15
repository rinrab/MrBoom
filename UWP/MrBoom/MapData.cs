// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public static class MapData
    {
        private static readonly byte[] defaultFinal = new byte[]
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

        public static readonly Map[] Data = new Map[]
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
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 1),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(0, 30, 1)
                },
                Final = defaultFinal
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
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 1),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(1, 30, 3)
                },
                Final = defaultFinal
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
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 1),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Banana, 10),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(4, 60, 1)
                },
                Final = defaultFinal
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
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 1),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(0, 30, 1),
                    new Map.BasicMonsterData(0, 30, 1),
                    new Map.BasicMonsterData(1, 30, 3),
                    new Map.BasicMonsterData(1, 30, 3),
                    new Map.BasicMonsterData(2, 60, 1) { Speed = 2 },
                    new Map.BasicMonsterData(3, 90, 3) { Speed = 1 },
                    new Map.BasicMonsterData(3, 90, 3) { Speed = 1 },
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
                Song = 5,
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 3),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(3, 90, 3) { Speed = 1 },
                },
                Final = defaultFinal
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
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 3),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 1),
                    new Map.PowerUpData(PowerUpType.RollerSkate, 1),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(1, 30, 3),
                    new Map.BasicMonsterData(3, 90, 3) { Speed = 1 },
                },
                Final = defaultFinal
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
                IsBombApocalypse = true,
                PowerUps = new Map.PowerUpData[]
                {
                    new Map.PowerUpData(PowerUpType.ExtraBomb, 10),
                    new Map.PowerUpData(PowerUpType.ExtraFire, 10),
                    new Map.PowerUpData(PowerUpType.Life, 3),
                    new Map.PowerUpData(PowerUpType.RemoteControl, 2),
                    new Map.PowerUpData(PowerUpType.Shield, 1),
                    new Map.PowerUpData(PowerUpType.Kick, 2),
                    new Map.PowerUpData(PowerUpType.Clock, 1),
                    new Map.PowerUpData(PowerUpType.Skull, 2),
                },
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(5, 30, 2),
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
                PowerUps = new Map.PowerUpData[] { },
                Song = 2,
                Monsters = new Map.MonsterData[]
                {
                    new Map.BasicMonsterData(6, 0, 3) { IsSlowStart = true },
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
