﻿using System;

namespace Symcol.Core.NetworkingV2
{
    [Serializable]
    public class Packet
    {
        /// <summary>
        /// Just a Signature
        /// </summary>
        public string Address;

        /// <summary>
        /// Specify starting size of a packet (bytes) for efficiency
        /// </summary>
        public virtual int PacketSize => 1024;

        public Packet(string address)
        {
            Address = address;
        }
    }
}
