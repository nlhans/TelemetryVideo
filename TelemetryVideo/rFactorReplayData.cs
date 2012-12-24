namespace TelemetryVideo
{
    public class rFactorReplayData
    {
        public float Time { get; set; }
        public int RPS { get; set; }
        public bool InPit { get; set; }
        public bool Brake { get; set; }
        public double Throttle { get; set; }
        public bool Horn { get; set; }
        public int TractionControl { get; set; }
        public int SpeedLimiter { get; set; }
        public double Steering { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float AngX { get; set; }
        public float AngY { get; set; }
        public float AngZ { get; set; }
    }
}