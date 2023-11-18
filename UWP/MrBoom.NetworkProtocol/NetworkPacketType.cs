// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.NetworkProtocol
{
    public static class NetworkPacketType
    {
        public const byte ConnectReq = 2;
        public const byte ConnectChallengeRequest = 3;
        public const byte ConnectChallengeResponse = 4;
        public const byte ConnectChallengeAck = 5;
        public const byte UnreliableData = 6;
        public const byte EchoRequest = 7;
        public const byte EchoResponse = 8;
    }
}
