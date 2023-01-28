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
    ]
}
