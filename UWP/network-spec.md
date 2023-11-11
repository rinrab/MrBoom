# Network game specification

## Start game
type: 00

TODO:

## Send player data
type: 01

| Byte number | Meaning              |
|-------------|----------------------|
| 0           | Type (0x01)          |
| 1           | X / 256              |
| 2           | X % 256              |
| 3           | Y / 256              |
| 4           | Y % 256              |
| 5           | Direction (4 = null) |
| 6           | Bombs count          |

Then bombs list:

| Byte number | Meaning       |
|-------------|---------------|
| 0           | Bomb X        |
| 1           | Bomb Y        |
| 2           | Estimate time |
| 3           | Max fire      |
