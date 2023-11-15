// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MrBoom.NetworkProtocol
{
    public class AddPlayerMessage
    {
        public string Name;

        public static AddPlayerMessage Decode(BinaryReader reader)
        {
            var name = reader.ReadString();

            return new AddPlayerMessage
            {
                Name = name
            };
        }

        public void Encode(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
}
