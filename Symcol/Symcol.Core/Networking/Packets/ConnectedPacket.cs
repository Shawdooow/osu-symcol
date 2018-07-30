﻿using System;

namespace Symcol.Core.Networking.Packets
{
    [Serializable]
    public class ConnectedPacket : Packet
    {
        public override int PacketSize => 128;
    }
}
