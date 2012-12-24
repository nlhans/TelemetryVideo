using System.Drawing;

namespace TelemetryVideo
{
    public class TelemetryInfo
    {
        public double RPM_Max1 { get; set; }
        public double RPM_Max2 { get; set; }
        public double RPM_Redline1 { get; set; }
        public double RPM_Redline2 { get; set; }
        public string Circuit2 { get; set; }
        public string FinalTime1 { get; set; }
        public string FinalTime2 { get; set; }
        public int FramesOffset { get; set; }
        public int FramesTotal { get; set; }
        public int FramesFade { get; set; }

        public string Circuit { get; set; }
        public string DataFile1 { get; set; }
        public string DataFile2 { get; set; }

        public string PicturesInput { get; set; }
        public string PicturesOutput { get; set; }

        public bool SplitScreen { get; set; }
        public bool RepeatDriverA { get; set; }
        public bool RepeatDriverB { get; set; }

        public double TimeOffsetA { get; set; }
        public double TimeOffsetB { get; set; }

        public Point PointGauge1 { get; set; }
        public Point PointGauge2 { get; set; }
        public Point PointTrack { get; set; }
        public Point PointAnnotation1 { get; set; }
        public Point PointAnnotation2 { get; set; }
        public string AnnotationLeft { get; set; }
        public string AnnotationRight { get; set; }

        public string Title { get; set; }
        public Point PointTitle { get; set; }

        public Size TrackMapSize { get; set; }

        public bool NewTrackFormat { get; set; }

        public double LapTimeA { get; set; }
        public double LapTimeB { get; set; }
    }
}