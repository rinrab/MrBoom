# Network game specification

## Start game
type: 00

TODO:

## Send player data

```cs
// Binary protocol in C# interface

interface GameData
{
    byte type = 1;
    // byte PlayersCount;
    PlayerData[] Players;
}

interface PlayerData
{
    byte XPart1;
    byte XPart2;
    byte YPart1;
    byte YPart2;
    byte Direction; // 4 = null
    byte BombsCount;
    Bomb[] Bombs;
}

interface BombData
{
    byte X;
    byte Y;
    byte EstimateTime;
    byte MaxFire;
}
```

## Ping Request

```cs
interface PingRequest
{
    byte Type = 2;
}
```

## Ping Response
```cs
interface PingResponse
{
    byte Type = 2;
}
```

## Start dialogs

```mermaid
stateDiagram-v2
    [*] --> DemoScreen
    DemoScreen --> OnlineStartScreen : Action Play Online
    DemoScreen --> StartScreen : Action Play Local
    DemoScreen --> [*] : Action Quit

    state OnlineStartScreen {
        [*] --> AddRemotePlayer : Bomb button
        AddRemotePlayer --> [*]
        [*] --> StartMatchmaking : Again Bomb Button
        StartMatchmaking --> [*]
    }
    
    state StartScreen {
        [*] --> AddLocalPlayer : Bomb button
        AddLocalPlayer --> [*]
        [*] -->  StartGame: Again Bomb Button
        StartGame --> [*]
        [*] --> AddBot: 'B' button
        AddBot --> [*]
    }

    OnlineStartScreen --> SearchingForPlayers
    StartScreen --> GameScreen
    StartScreen --> [*]: Action Quit
    GameScreen --> ResultScreen
    GameScreen --> DrawScreen
    GameScreen --> DemoScreen: Action Start Menu
    GameScreen --> [*]: Action Quit
    ResultScreen --> VictoryScreen
    ResultScreen --> GameScreen
    VictoryScreen --> DemoScreen
    DrawScreen --> GameScreen

    state SearchingForPlayers {
        [*] --> Matchmaking
        Matchmaking --> [*]
    }

    SearchingForPlayers --> OnlinePlayerList
    OnlinePlayerList --> NetworkGameScreen
    OnlinePlayerList --> OnlineStartScreen: Escape
    OnlineStartScreen --> DemoScreen: Escape
    NetworkGameScreen --> TODO
```