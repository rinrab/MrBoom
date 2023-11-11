// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.MediaFoundation;

namespace MrBoom
{
    public class Human : AbstractPlayer
    {
        public readonly IController Controller;

        public Human(Terrain map,
                     Assets.MovingSpriteAssets animations,
                     IController controller,
                     int team) : base(map, animations, team)
        {
            Controller = controller;
        }

        public override string GetDebugInfo()
        {
            string position = string.Format("({0,3},{1,3})/({2,2},{3,2})", X, Y, (X + 8) / 16, (Y + 8) / 16);
            if (IsDie)
            {
                position = "DEAD";
            }

            return $"H:{position}";
        }

        public override void Update()
        {
            Direction = null;
            if (Controller.IsKeyDown(PlayerKeys.Up))
            {
                Direction = Directions.Up;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Left))
            {
                Direction = Directions.Left;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Right))
            {
                Direction = Directions.Right;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Down))
            {
                Direction = Directions.Down;
            }
            dropBombButton = Controller.IsKeyDown(PlayerKeys.Bomb);
            rcDitonateButton = Controller.IsKeyDown(PlayerKeys.RcDitonate);

            base.Update();
        }

        public IEnumerable<byte> GetDataToSend()
        {
            yield return (byte)(X / 256);
            yield return (byte)(X % 256);
            yield return (byte)(Y / 256);
            yield return (byte)(Y % 256);
            yield return (Direction == null) ? (byte)4 : (byte)Direction;

            Tuple<CellCoord, Cell>[] bombs = terrain.GetMyBombs(this).ToArray();

            yield return (byte)bombs.Length;

            foreach (Tuple<CellCoord, Cell> cell in bombs)
            {
                yield return (byte)cell.Item1.X;
                yield return (byte)cell.Item1.Y;
                yield return (byte)cell.Item2.bombCountdown;
                yield return (byte)cell.Item2.maxBoom;
            }
        }
    }
}
