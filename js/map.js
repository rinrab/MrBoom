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
        PowerUpType.ExtraBomb,
        PowerUpType.ExtraFire,
        PowerUpType.Skull,
        PowerUpType.Life,
        PowerUpType.RemoteControl,
        PowerUpType.RollerSkate
    ]
}


const mapNeigeInitial = {
    map: [
        "###################",
        "#  -------------  #",
        "# #-# #-#-#-#-#-# #",
        "#---  ------  ----#",
        "#-#-#-###-#-# # #-#",
        "#-----###-----   -#",
        "#-#-#-###-#-#-# #-#",
        "#---------  ------#",
        "#-#-#-# #-# #-#-#-#",
        "#-----  ----------#",
        "# #-#-#-#-#-#-# ###",
        "#  -----------  ###",
        "###################"
    ],
    powerUps: [
        PowerUpType.ExtraBomb,
        PowerUpType.ExtraFire,
        PowerUpType.Skull,
        PowerUpType.Life,
        PowerUpType.RemoteControl,
        PowerUpType.RollerSkate,
        PowerUpType.Shield
    ],
    monsters: [
        { startX: 7, startY: 9, waitAfterTurn: 30 },
        { startX: 1, startY: 11, waitAfterTurn: 30 },
        { startX: 5, startY: 3, waitAfterTurn: 30 },
        { startX: 13, startY: 3, waitAfterTurn: 30 },
    ]
}
