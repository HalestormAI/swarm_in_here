using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public enum PacketType
    {
        RequestSpace,
        AllStop,
        HeadColour
    }

    public class MockPacket
    {
        public PacketType type { get; private set; }
        public Guid guid { get; private set; }
        public string LogColor { get; set; }
        public int lifetimeSeconds { get; set; }

        private DateTime creationTime;

        public MockPacket(PacketType type)
        {
            this.type = type;
            guid = Guid.NewGuid();
            creationTime = System.DateTime.UtcNow;
        }

        public bool HasExpired()
        {
            return lifetimeSeconds > 0 && DateTime.UtcNow.Subtract(creationTime).Seconds > lifetimeSeconds;
        }
    }

    public class RequestSpacePacket : MockPacket
    {
        public float AmountOfSpace;

        public RequestSpacePacket(float amt) 
            : base(PacketType.RequestSpace)
        {
            LogColor = "yellow";
            AmountOfSpace = amt;
        }
    }

    public class AllStopPacket : MockPacket
    {
        public AllStopPacket()
            : base(PacketType.AllStop)
        {
            LogColor = "red";
        }
    }

    public class HeadColourPacket : MockPacket
    {
        public Color HeadColor;

        public HeadColourPacket(Color color)
            : base(PacketType.HeadColour)
        {
            LogColor = "green";
            HeadColor = color;
        }
    }
}