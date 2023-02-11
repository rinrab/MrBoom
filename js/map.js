// "#" - wall can't broke
// "-" - wall can broke
// " " - free space

const defaultFin = [
    "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000",
    "000", "001", "002", "003", "004", "005", "006", "007", "008", "009", "010", "011", "012", "013", "014", "015", "016", "017", "000",
    "000", "052", "053", "054", "055", "056", "057", "058", "059", "060", "061", "062", "063", "064", "065", "066", "067", "018", "000",
    "000", "051", "096", "097", "098", "099", "100", "101", "102", "103", "104", "105", "106", "107", "108", "109", "068", "019", "000",
    "000", "050", "095", "132", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "110", "069", "020", "000",
    "000", "049", "094", "131", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "111", "070", "021", "000",
    "000", "048", "093", "130", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "112", "071", "022", "000",
    "000", "047", "092", "129", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "113", "072", "023", "000",
    "000", "046", "091", "128", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "114", "073", "024", "000",
    "000", "045", "090", "127", "126", "125", "124", "123", "122", "121", "120", "119", "118", "117", "116", "115", "074", "025", "000",
    "000", "044", "089", "088", "087", "086", "085", "084", "083", "082", "081", "080", "079", "078", "077", "076", "075", "026", "000",
    "000", "043", "042", "041", "040", "039", "038", "037", "036", "035", "034", "033", "032", "031", "030", "029", "028", "027", "000",
    "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000"
];

const maps = [
    //const mapSnail = [
    //    "###################",
    //    "#             ----#",
    //    "# #-# # # #-# ##-##",
    //    "# ---     ---     #",
    //    "# #-# #-# #-# #-# #",
    //    "#     ---     --- #",
    //    "# #-# #-# #-# #-# #",
    //    "# ---     ---     #",
    //    "# #-# #-# #-# #-# #",
    //    "#     ---     --- #",
    //    "# # # #-# # # #-# #",
    //    "#                 #",
    //    "###################",
    //]
    //
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 1 },
            { type: PowerUpType.RollerSkate, count: 1 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { startX: 7, startY: 9, waitAfterTurn: 30, type: 0 },
            { startX: 1, startY: 11, waitAfterTurn: 30, type: 0 },
            { startX: 5, startY: 3, waitAfterTurn: 30, type: 0 },
            { startX: 13, startY: 3, waitAfterTurn: 30, type: 0 },
        ],
        time: 120,
        fin: defaultFin
    },
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 1 },
            { type: PowerUpType.RollerSkate, count: 1 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { startX: 7, startY: 9, waitAfterTurn: 20, type: 1, livesCount: 3 },
        ],
        time: 120,
        fin: defaultFin
    },
    //{
    //    map: [
    //        "###################",
    //        "#* ------------- *#",
    //        "# #-#-#-#-#-#-#-# #",
    //        "#------*121*------#",
    //        "#-#-#-#4#-#4#-#-#-#",
    //        "#------ --- ------#",
    //        "#-#-#-#3#-#3#-#-#-#",
    //        "#------ --- ------#",
    //        "#-#-#-#4#-#4#-#-#-#",
    //        "#------*121*------#",
    //        "# #-#-#-#-#-#-#-# #",
    //        "#* ------------- *#",
    //        "###################",
    //    ],
    //    powerUps: [
    //        { type: PowerUpType.ExtraBomb, count: 10 },
    //        { type: PowerUpType.ExtraFire, count: 10 },
    //        { type: PowerUpType.Life, count: 1 },
    //        { type: PowerUpType.RemoteControl, count: 1 },
    //        { type: PowerUpType.RollerSkate, count: 1 },
    //        { type: PowerUpType.Shield, count: 1 },
    //        { type: PowerUpType.Kick, count: 2 }
    //    ],
    //    monsters: [
    //        { startX: 7, startY: 9, waitAfterTurn: 60, type: 2, speed: 0.5, startDelay: 90 },
    //    ],
    //    time: 90,
    //    fin: defaultFin
    //},
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 1 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Banana, count: 10 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { waitAfterTurn: 60, type: 3 },
        ],
        time: 90,
        fin: defaultFin
    },
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 1 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Clock, count: 1 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { waitAfterTurn: 30, type: 0 },
            { waitAfterTurn: 30, type: 0 },
            { waitAfterTurn: 30, type: 1, livesCount: 3 },
            { waitAfterTurn: 30, type: 1, livesCount: 3 },
            { waitAfterTurn: 30, type: 2, speed: 0.5 },
        ],
        time: 60,
        fin: [
             "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000",
             "000", "100", "100", "100", "100", "100", "255", "040", "041", "042", "043", "044", "255", "085", "085", "085", "085", "085", "000",
             "000", "100", "100", "100", "100", "100", "255", "047", "255", "046", "255", "045", "255", "085", "085", "085", "085", "085", "000",
             "000", "100", "100", "100", "100", "100", "255", "255", "255", "255", "255", "255", "255", "085", "085", "085", "085", "085", "000",
             "000", "100", "100", "100", "100", "100", "255", "255", "255", "255", "255", "255", "255", "085", "085", "085", "085", "085", "000",
             "000", "100", "100", "100", "100", "100", "001", "255", "255", "255", "255", "255", "020", "085", "085", "085", "085", "085", "000",
             "000", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "255", "000",
             "000", "070", "070", "070", "070", "070", "030", "255", "255", "255", "255", "255", "010", "110", "110", "110", "110", "110", "000",
             "000", "070", "070", "070", "070", "070", "255", "255", "255", "255", "255", "255", "255", "110", "110", "110", "110", "110", "000",
             "000", "070", "070", "070", "070", "070", "255", "255", "255", "255", "255", "255", "255", "110", "110", "110", "110", "110", "000",
             "000", "070", "070", "070", "070", "070", "255", "056", "255", "057", "255", "058", "255", "110", "110", "110", "110", "110", "000",
             "000", "070", "070", "070", "070", "070", "255", "055", "054", "053", "052", "051", "255", "110", "110", "110", "110", "110", "000",
             "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000", "000",
        ]
    },
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 4 },
            { type: PowerUpType.RemoteControl, count: 1 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Clock, count: 1 },
            { type: PowerUpType.Kick, count: 1 }
        ],
        monsters: [
            { waitAfterTurn: 48, type: 4, livesCount: 3, speed: 0.3 },
        ],
        time: 60,
        fin: defaultFin,
    },
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 2 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { waitAfterTurn: 48, type: 1, livesCount: 3 },
            { waitAfterTurn: 48, type: 4, livesCount: 3, speed: 0.3 },
        ],
        initialBonus: [
            PowerUpType.Kick
        ],
        time: 90,
        fin: defaultFin,
    },
    {
        map: [
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
        ],
        powerUps: [
            { type: PowerUpType.ExtraBomb, count: 10 },
            { type: PowerUpType.ExtraFire, count: 10 },
            { type: PowerUpType.Life, count: 1 },
            { type: PowerUpType.RemoteControl, count: 2 },
            { type: PowerUpType.Shield, count: 1 },
            { type: PowerUpType.Kick, count: 2 }
        ],
        monsters: [
            { waitAfterTurn: 48, type: 5, livesCount: 1 },
        ],
        initialBonus: [
            PowerUpType.Kick
        ],
        time: 120,
        fin: [
            0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0,
            0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0,
        ]
    },
];