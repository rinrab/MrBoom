// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Runtime.InteropServices;

namespace MrBoom
{
    public static class PlayFabPartyApi
    {
        [DllImport("MrBoom.PlayFabParty.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void MrBoom_Test();
    }
}
