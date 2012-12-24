using System.Collections.Generic;

namespace TelemetryVideo
{
    public class rFactorReplayPlayer
    {
        public int Slot { get; set; }
        public string Name { get; set; }
        public string VehicleName { get; set; }
        public string Skin { get; set; }
        public char[] Upgrades { get; set; }
        public float EntryTime { get; set; }
        public float ExitTime { get; set; }

        public List<rFactorReplayData> Data { get; set; }
    }
}