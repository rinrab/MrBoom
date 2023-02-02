// "#" - wall can't broke
// "-" - wall can broke
// " " - free space

const mapSnail = [
    "###################",
    "#             ----#",
    "# #-# # # #-# ##-##",
    "# ---     ---     #",
    "# #-# #-# #-# #-# #",
    "#     ---     --- #",
    "# #-# #-# #-# #-# #",
    "# ---     ---     #",
    "# #-# #-# #-# #-# #",
    "#     ---     --- #",
    "# # # #-# # # #-# #",
    "#                 #",
    "###################",
]

const mapPink = {
    map: [
        "###################",
        "#  -------------  #",
        "# #-#-#-#-#-#-#-# #",
        "-#-#-#-#-#-#-#-#-#-",
        "#-#-#-#-#-#-#-#-#-#",
        "-#-#-#-#-#-#-#-#-#-",
        "#-#-#-#-#-#-#-#-#-#",
        "-#-#-#-#-#-#-#-#-#-",
        "#-#-#-#-#-#-#-#-#-#",
        "-#-#-#-#-#-#-#-#-#-",
        "# #-#-#-#-#-#-#-# #",
        "#  -------------  #",
        "###################"
    ],
    powerUps: [
        { type: PowerUpType.ExtraBomb, count: 10 },
        { type: PowerUpType.ExtraFire, count: 10 },
        { type: PowerUpType.Life, count: 1 },
        { type: PowerUpType.RemoteControl, count: 1 },
        { type: PowerUpType.RollerSkate, count: 1 },
    ]
}


const mapNeigeInitial = {
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
        { startX: 7, startY: 9, waitAfterTurn: 30 },
        { startX: 1, startY: 11, waitAfterTurn: 30 },
        { startX: 5, startY: 3, waitAfterTurn: 30 },
        { startX: 13, startY: 3, waitAfterTurn: 30 },
    ]
}
