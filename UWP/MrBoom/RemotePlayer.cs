// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.NetworkProtocol;

namespace MrBoom
{
    public class RemotePlayer : AbstractPlayer
    {
        public RemotePlayer(Terrain map, Assets.MovingSpriteAssets animations,
                            int team) : base(map, animations, team)
        {
        }

        public void Recieved(ClientGameStateMessage.PlayerData data)
        {
            if (Math.Abs(data.X - X) + Math.Abs(data.Y - Y) >= 4)
            {
                X = data.X;
                Y = data.Y;
            }

            Direction = data.Direction == 255 ? null : (Directions?)data.Direction;

            foreach (ClientGameStateMessage.BombData bomb in data.Bombs)
            {
                Cell bombCell = terrain.GetCell(bomb.X, bomb.Y);
                if (bombCell.Type != TerrainType.Bomb || bombCell.owner != this)
                {
                    bombCell = terrain.PutBomb(bomb.X, bomb.Y, bomb.MaxFire, false, this);
                }
                bombCell.bombCountdown = bomb.EstimateTime;
            }
        }
    }
}
