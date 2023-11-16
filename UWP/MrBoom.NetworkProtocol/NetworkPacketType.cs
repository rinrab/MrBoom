// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.NetworkProtocol
{
    public static class NetworkPacketType
    {
        public const byte ConnectReq = 2;
        public const byte ConnectAck = 3;
        public const byte UnreliableData = 4;
        public const byte EchoRequest = 5;
        public const byte EchoResponse = 6;
    }
}
