// "#" - wall can't broke
// "-" - wall can broke
// " " - free space

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
            { type: PowerUpType.Shield, count: 1 }
        ],
        monsters: [
            { startX: 7, startY: 9, waitAfterTurn: 30, type: 0 },
            { startX: 1, startY: 11, waitAfterTurn: 30, type: 0 },
            { startX: 5, startY: 3, waitAfterTurn: 30, type: 0 },
            { startX: 13, startY: 3, waitAfterTurn: 30, type: 0 },
        ]
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
            { type: PowerUpType.Shield, count: 1 }
        ],
        monsters: [
            { startX: 7, startY: 9, waitAfterTurn: 20, type: 1, livesCount: 3 },
        ]
    },
    {
        map: [
            "###################",
            "#* ------------- *#",
            "# #-#-#-#-#-#-#-# #",
            "#------*121*------#",
            "#-#-#-#4#-#4#-#-#-#",
            "#------ --- ------#",
            "#-#-#-#3#-#3#-#-#-#",
            "#------ --- ------#",
            "#-#-#-#4#-#4#-#-#-#",
            "#------*121*------#",
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
            { type: PowerUpType.Shield, count: 1 }
        ],
        monsters: [
            { startX: 7, startY: 9, waitAfterTurn: 60, type: 2, speed: 0.5 },
        ]
    },
];