using System.Collections.Generic;
using System.Linq;

namespace VoidBattery
{
    public class PacketHandler
    {
        private Queue<int> Packets { get; }

        public int CurrentLevel
        {
            get
            {
                if (Packets.Count == 0)
                    return 0;
                int sum = Packets.Sum();
                return sum / Packets.Count;
            }
        }

        public PacketHandler()
        {
            Packets = new Queue<int>();
        }

        public void RegisterPacket(int entry)
        {
            if (Packets.Count == 5)
                Packets.Dequeue();
            Packets.Enqueue(entry);
        }
    }
}
