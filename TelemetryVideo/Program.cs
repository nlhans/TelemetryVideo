using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using SimTelemetry.Controls;
using SimTelemetry.Data.Logger;
using SimTelemetry.Data.Track;
using SimTelemetry.Objects;

    public class TrackThumbnail
    {
        protected double pos_x_max = 1000000000.0;
        protected double pos_x_min = -1000000000.0;
        protected double pos_y_max = 1000000000.0;
        protected double pos_y_min = -1000000000.0;
        protected double map_width = 0;
        protected double map_height = 0;

        protected double scale = 0;
        protected double offset_x = 0;
        protected double offset_y = 0;

        #region Settings
        protected bool AutoPosition = true;

        float track_width = 5f;
        float pitlane_width = 0f;

        Pen brush_start = new Pen(Color.FromArgb(200, 50, 30), 6f); // 6f=track_width
        Brush brush_sector1 = new SolidBrush(Color.FromArgb(185, 185, 200));
        Brush brush_sector2 = new SolidBrush(Color.FromArgb(47, 79, 79));
        Brush brush_sector3 = new SolidBrush(Color.FromArgb(85, 107, 47));
        Brush brush_pitlane = new SolidBrush(Color.FromArgb(100, Color.Orange));

        Font tf24 = new Font("calibri", 24f);
        Font tf16 = new Font("calibri", 16f);
        Font tf12 = new Font("calibri", 12f);
        Font tf10 = new Font("calibri", 10f);
        Font tf18 = new Font("calibri", 18f);
        Font font_version = new Font("calibri", 24f, FontStyle.Bold | FontStyle.Italic);
        #endregion

        public float GetX(double x)
        {

            return Convert.ToSingle(6 + ((x - pos_x_min) / scale * map_width) + offset_x);
        }
        public float GetY(double y)
        {
            return Convert.ToSingle(6 + (1 - (y - pos_y_min) / scale) * map_height + offset_y);
        }

        public void Create(string file, string name, string version, RouteCollection route, int width, int height)
        {
            Pen pen_track = new Pen(brush_sector1, track_width);
            try
            {
                Image track_img = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(track_img);
                g.Clear(Color.Transparent);
                //g.FillRectangle(Brushes.Black, 0, 0, width, height);

                if (route == null || route.Racetrack == null) return;

                g.SmoothingMode = SmoothingMode.AntiAlias;

                pos_x_max = -100000;
                pos_x_min = 100000;
                pos_y_max = -100000;
                pos_y_min = 1000000;

                foreach (TrackWaypoint wp in route.Racetrack)
                {
                    if (wp.Route == TrackRoute.MAIN)
                    {
                        pos_x_max = Math.Max(wp.X, pos_x_max);
                        pos_x_min = Math.Min(wp.X, pos_x_min);
                        pos_y_max = Math.Max(wp.Z, pos_y_max);
                        pos_y_min = Math.Min(wp.Z, pos_y_min);
                    }
                }


                scale = Math.Max(pos_x_max - pos_x_min, pos_y_max - pos_y_min);

                map_width = width - 12;
                map_height = height - 12;

                offset_x = map_width / 2 - (pos_x_max - pos_x_min) / scale * map_width / 2;
                offset_y = 0 - (scale - pos_y_max + pos_y_min) / scale * map_height / 2;

                List<PointF> track = new List<PointF>();

                int i = 0;
                foreach (TrackWaypoint wp in route.Racetrack)
                {
                    if (wp.Route == TrackRoute.MAIN)
                    {
                        float x1 = Convert.ToSingle(6 + ((wp.X - pos_x_min) / scale * map_width) + offset_x);
                        float y1 = Convert.ToSingle(6 + (1 - (wp.Z - pos_y_min) / scale) * map_height + offset_y);

                        x1 = Limits.Clamp(x1, -1000, 1000);
                        y1 = Limits.Clamp(y1, -1000, 1000);

                            track.Add(new PointF(x1, y1));
                    }
                }

                // Draw polygons!
                if (track.Count > 0) g.DrawPolygon(pen_track, track.ToArray());


                track_img.Save(file);
            }
            catch (Exception ex)
            {

            }
        }
    }

namespace TelemetryVideo
{

    class TelemetryInfo
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
    class Program
    {
        static void Main(string[] args)
        {
            /******* SPA ********/
            /*TelemetryInfo telemetryData = new TelemetryInfo
                                              {
                                                  FinalTime1 = "01:44.368",
                                                  Circuit = @"C:\Program Files (x86)\rFactor\GameData\Locations\F108_Spa\F108_Spa.gdb",
                                                  DataFile1 = @"..\..\..\Telemetry\Spa 1.44.368.gz",
                                                  PicturesInput = @"H:\Input",
                                                  PicturesOutput = @"H:\Output",
            

                TrackMapSize = new Size(525, 525),

                NewTrackFormat = false,

                                                  FramesFade = 10,
                                                  FramesOffset = 92,
                                                  FramesTotal = 3261,

                                                  SplitScreen = true,
                                                  PointGauge1 = new Point(150, 310),
                                                  PointTrack = new Point(1450, 300)
                                              };*/

            /******* F1 Monaco 1991 vs 2010 ******/
            TelemetryInfo telemetryData = new TelemetryInfo
            {
                Title = "F1 1991 vs F1 2010",

                FinalTime1 = "01:44.368",
                FinalTime2 = "01:44.368",
                Circuit = @"C:\Program Files (x86)\rFactor\GameData\Locations\F1_1988_C4\03_MonteCarlo\MonteCarlo_1988_C4.gdb",
                Circuit2 = @"C:\Program Files (x86)\rFactor\GameData\Locations\F1WCP\2009\Monaco\MonteCarlo2009\MonteCarlo2009.gdb",
                DataFile1 = @"..\..\..\Telemetry\F1 1991 Monaco.gz",
                DataFile2 = @"..\..\..\Telemetry\F1 2010 Monaco.gz",
                PicturesInput = @"H:\Input",
                PicturesOutput = @"H:\Output",

                FramesFade = 10,
                FramesOffset = 94,
                FramesTotal = 2657,

                TimeOffsetA = 3.2,
                TimeOffsetB=0,

                LapTimeA = 84.481*1000,
                LapTimeB = 75.1*1000,

                RepeatDriverA=true,
                RepeatDriverB=true,

                NewTrackFormat = true,

                TrackMapSize = new Size(400, 400),

                SplitScreen = true,
                PointGauge1 = new Point(50, 700),
                PointGauge2 = new Point(1010, 700),

                PointAnnotation1 = new Point(50, 1010),
                PointAnnotation2 = new Point(1010, 1010),

                PointTrack = new Point(760, 500),
                PointTitle = new Point(510, 10),

                AnnotationLeft = "Williams F1 1991 -- Nigel Mansell",
                AnnotationRight = "Williams F1 2010 -- Rubens Barichello",

                RPM_Max1 = 14000,
                RPM_Max2 = 17500,

                RPM_Redline1 = 12000,
                RPM_Redline2 = 16000
            };

            var TrackThumbnail2 = CreateTrackThumbnail(telemetryData.Circuit2, telemetryData, "circuit2.png");
            var TrackThumbnail = CreateTrackThumbnail(telemetryData.Circuit, telemetryData, "circuit.png");
            var imgTrack = Image.FromFile("circuit.png");

            var read1 = new TelemetryLogReader(telemetryData.DataFile1);
            read1.ReadPolling();
            var read2 = read1;
            if (telemetryData.SplitScreen)
            {
                read2 = new TelemetryLogReader(telemetryData.DataFile2);
                read2.ReadPolling();
            }

            var files = Directory.GetFiles(telemetryData.PicturesInput, "*.jpg");

            var fadeStart = telemetryData.FramesOffset;
            var fadeLength = telemetryData.FramesFade;
            var fadeEnd = telemetryData.FramesTotal - telemetryData.FramesFade;

            for (int frameNumber = 95; frameNumber < files.Length; frameNumber+=100)
            {
                var frame = files[frameNumber];
                var file = telemetryData.PicturesOutput + "\\img-" + frameNumber.ToString("0000") + ".png";
                var time = (frameNumber - telemetryData.FramesOffset)*1000.0/30.0;

                double alpha = 1.0*(frameNumber - telemetryData.FramesOffset)/telemetryData.FramesFade;
                if (frameNumber >= fadeEnd) alpha = 1 - 1.0*(frameNumber - fadeEnd)/telemetryData.FramesFade;
                if (alpha > 1) alpha = 1;

                if (frameNumber >= fadeEnd + fadeLength || frameNumber < fadeStart)
                {
                    using (var imIn = Image.FromFile(frame))
                    using (var imOut = new Bitmap(1920, 1080))
                    {
                        Graphics g = Graphics.FromImage(imOut);
                        g.FillRectangle(Brushes.Black, 0, 0, 1920, 1080);
                        g.DrawImage(imIn, (1920 - imIn.Width) / 2, (1080 - imIn.Height) / 2);
                        if (telemetryData.SplitScreen)
                        {
                            if (frameNumber >= fadeEnd + fadeLength)
                            g.DrawString(telemetryData.FinalTime2, new Font("Tahoma", 48), Brushes.White,
                                     telemetryData.PointGauge2.X + 50, telemetryData.PointGauge2.Y + 450);

                            g.DrawString(telemetryData.AnnotationLeft, new Font("Tahoma", 30), Brushes.White, telemetryData.PointAnnotation1);
                            g.DrawString(telemetryData.AnnotationRight, new Font("Tahoma", 30), Brushes.White, telemetryData.PointAnnotation2);
                        }

                        if (frameNumber >= fadeEnd + fadeLength)
                        g.DrawString(telemetryData.FinalTime1, new Font("Tahoma", 48), Brushes.White,
                                     telemetryData.PointGauge1.X + 50, telemetryData.PointGauge1.Y + 450);

                        g.DrawString(telemetryData.Title, new Font("Tahoma", 72), Brushes.White, telemetryData.PointTitle);

                        imOut.Save(file);
                    }
                    continue;
                }
                var timeA = time - telemetryData.TimeOffsetA*1000;
                var timeB = time - telemetryData.TimeOffsetB*1000;
                var sample1 = 1.0;
                var sample2 = 1.0;

                if (telemetryData.RepeatDriverA)
                {
                    sample1 = read1.Samples
                        .Where(x =>
                                   {
                                       if (timeA<=0)
                                           return x.Value.Time <=
                                                  (timeA + telemetryData.LapTimeA);
                                       else
                                           return x.Value.Time <= timeA;
                                   })
                        .OrderBy(x => -1*x.Value.Time)
                        .FirstOrDefault().Key;
                }else
                {
                    sample1 = read1.Samples
                        .Where(x => x.Value.Time <= timeA)
                        .OrderBy(x => -1 * x.Value.Time)
                        .FirstOrDefault().Key;
                    
                }

                sample2 = sample1;

                if(telemetryData.SplitScreen)
                {


                    if (telemetryData.RepeatDriverB)
                    {
                        sample2 = read2.Samples
                            .Where(x =>
                            {
                                if (timeA <= 0)
                                    return x.Value.Time <=
                                           (timeB + telemetryData.LapTimeB);
                                else
                                    return x.Value.Time <= timeB;
                            })
                            .OrderBy(x => -1 * x.Value.Time)
                            .FirstOrDefault().Key;
                    }
                    else
                    {
                        sample2 = read2.Samples
                            .Where(x => x.Value.Time <= timeB)
                            .OrderBy(x => -1 * x.Value.Time)
                            .FirstOrDefault().Key;

                    }
                }

                using (var imIn = Image.FromFile(frame))
                using(var imOut = new Bitmap(1920, 1080))
                {
                    Graphics g = Graphics.FromImage(imOut);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    g.FillRectangle(Brushes.Black, 0, 0, 1920, 1080);
                    g.DrawImage(imIn, (1920 - imIn.Width)/2, (1080 - imIn.Height)/2);

                    /******* TRACK ******/
                    g.DrawImage(imgTrack, telemetryData.PointTrack.X, telemetryData.PointTrack.Y);

                    double xA = 0, yA = 0;
                    double xB = 0, yB = 0;
                    if (sample1 != 0)
                    {

                        var DriverBrushA = GetBrush(alpha, 255, 100, 255, 0);
                        xA = read1.GetDouble(sample1, "Driver.CoordinateX");
                        if (telemetryData.NewTrackFormat)
                            yA = read1.GetDouble(sample1, "Driver.CoordinateY");
                        else
                        yA = read1.GetDouble(sample1, "Driver.CoordinateZ");

                        var pxA = telemetryData.PointTrack.X + TrackThumbnail.GetX(xA);
                        var pyA = telemetryData.PointTrack.Y + TrackThumbnail.GetY(yA);

                        g.FillEllipse(DriverBrushA, pxA - 6, pyA - 6, 13, 13);
                    }

                    if (sample2 != 0 && telemetryData.SplitScreen)
                    {

                        var DriverBrushB = GetBrush(alpha, 255, 255, 100, 0);

                        xB = read2.GetDouble(sample2, "Driver.CoordinateX");
                        if (telemetryData.NewTrackFormat)
                            yB = read2.GetDouble(sample2, "Driver.CoordinateZ");
                        else
                            yB = read2.GetDouble(sample2, "Driver.CoordinateZ");

                        var pxB = telemetryData.PointTrack.X + TrackThumbnail2.GetX(xB);
                        var pyB = telemetryData.PointTrack.Y + TrackThumbnail2.GetY(yB);

                        g.FillEllipse(DriverBrushB, pxB - 6, pyB - 6, 13, 13);
                    }

                    if (telemetryData.SplitScreen)
                    {
                        var BrushBackground = GetBrush(alpha, 150, 0, 0, 0);

                        g.FillRectangle(BrushBackground, 0, 834, 1920,90);

                        DrawSlimGauges(read1, alpha, frameNumber, telemetryData.PointGauge1, sample1, time, g, telemetryData.RPM_Max1, telemetryData.RPM_Redline1);
                        DrawSlimGauges(read2, alpha, frameNumber, telemetryData.PointGauge2, sample2, time, g, telemetryData.RPM_Max2, telemetryData.RPM_Redline2);
                        g.DrawString(telemetryData.AnnotationLeft, new Font("Tahoma", 30), Brushes.White, telemetryData.PointAnnotation1);
                        g.DrawString(telemetryData.AnnotationRight, new Font("Tahoma", 30), Brushes.White, telemetryData.PointAnnotation2);
                    }
                    else
                    {
                        DrawGauges(read1, alpha, frameNumber, telemetryData.PointGauge1, sample1, g, imgTrack,
                                   TrackThumbnail);
                    }
                    g.DrawString(telemetryData.Title, new Font("Tahoma", 72), Brushes.White, telemetryData.PointTitle);
                    imOut.Save(file);
                }
                Console.WriteLine(("frame " + frameNumber));
            }


            //Console.ReadLine();

        }

        private static void DrawSlimGauges(TelemetryLogReader read, double alpha, int frameNumber, Point positionGauge, double sample, double time, Graphics g, double rpm_max, double rpm_redline)
        {
            var Brush_PedalsBackground = GetBrush(alpha, 255, 100, 100, 100);
            var Brush_PedalsBrake = GetBrush(alpha, 255, 200, 0, 0);
            var Brush_PedalsThrottle = GetBrush(alpha, 255, 0, 100, 0);
            var BrushWhite = GetBrush(alpha, 255, 255, 255, 255);
            var BrushRed = GetBrush(alpha, 255, 255, 0, 0);
            var BrushTime = GetBrush(1, 255, 255, 255, 255);
            var BrushGray = GetBrush(alpha, 255, 200, 200, 200);
            var BrushBackground = GetBrush(alpha, 150, 0, 0, 0);

            var GaugeWhite3 = new Pen(BrushWhite, 3.0f);
            var GaugeWhite2 = new Pen(BrushWhite, 2.0f);
            var GaugeWhite1 = new Pen(BrushWhite, 1.0f);

            double throttle = 0, brake = 0, rpm = 0, speed = 0, rpm_min = 0, redline = 0;
            int gear = 0;

            try
            {
                throttle = read.GetDouble(sample, "Driver.Throttle");
                throttle = Math.Max(throttle, read.GetDouble(sample, "Player.Pedals_Throttle"));
                brake = read.GetDouble(sample, "Player.Pedals_Brake");
                rpm = Rotations.Rads_RPM(read.GetDouble(sample, "Driver.RPM"));
                gear = (int)read.Get(sample, "Driver.Gear");
                gear = Math.Max(gear,(int)read.Get(sample, "Player.Gear"));
                speed = read.GetDouble(sample, "Driver.Speed") * 3.6;

                rpm_min = 4000;

                rpm_max = 1000 * Math.Ceiling(1.05 * rpm_max / 1000.0);
                redline = (rpm - 17000) / 1000.0;
                if (redline < 0) redline = 0;
                if (redline > 1) redline = 1;
            }
            catch (Exception ex)
            {

            }
            var BrushGear = GetBrush(alpha, 255, 255, 255 - Convert.ToInt32(redline * 200), 255 - Convert.ToInt32(redline * 200));

            /******* gear etc. ******/
            g.DrawString(gear.ToString(), new Font("Tahoma", 36, FontStyle.Bold), BrushGear, positionGauge.X, positionGauge.Y + 150);
            g.DrawString(Math.Floor(speed).ToString("000"), new Font("Tahoma", 30, FontStyle.Bold), BrushWhite, positionGauge.X+50, positionGauge.Y + 155);
            g.DrawString("kmh", new Font("Tahoma", 16), BrushWhite, positionGauge.X + 140, positionGauge.Y + 175);

            /******** LAPTIME *******/
            var tijd = time / 1000.0;
            var minutes = (tijd - (tijd % 60)) / 60;
            var seconds = Math.Floor(tijd - minutes * 60);
            var mills = (tijd % 1.0) * 1000;
            var txt = String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, mills);
            g.DrawString(txt, new Font("Tahoma", 40), BrushTime, positionGauge.X, positionGauge.Y + 235);
            
            /****** tacho meter******/
            var rpm_size = 300.0f;

            var rpmColor1a = GetBrush(alpha, 220, 100, 100, 100).Color;
            var rpmColor2a = GetBrush(alpha, 220, 240, 240, 240).Color;
            var rpmColor1b = GetBrush(alpha, 220, 100, 0, 0).Color;
            var rpmColor2b = GetBrush(alpha, 220, 255, 0, 0).Color;
            var rpm_brushA = new LinearGradientBrush(new Point(0, 0), new Point(0, 40), rpmColor1a, rpmColor2a);
            var rpm_brushB = new LinearGradientBrush(new Point(0, 0), new Point(0, 40), rpmColor1b, rpmColor2b);

            var rpm_redline_x = rpm_size * Convert.ToSingle((rpm_redline - rpm_min) / (rpm_max - rpm_min));
            for (int r = (int)(rpm_min / 1000); r <= (rpm_max) / 1000; r++)
            {
                var draw = true;

                if (rpm_max > 10000)
                {
                    if (r%2 != 0)
                        draw = false;
                }
                var br = (r >= rpm_redline/1000) ? BrushRed : BrushWhite;
                float x = Convert.ToSingle((r - rpm_min / 1000) * rpm_size / (rpm_max / 1000 - rpm_min / 1000));
                x += positionGauge.X + 200 + 140 + 30;
                if(draw){
                g.DrawString(r.ToString(), new Font("Tahoma", 14.0f), br,
                    x - 7- ((r.ToString().Length==2)?7:0), positionGauge.Y*1.0f + 160.0f);
                }
                g.DrawLine(new Pen(br, 1.0f), x, positionGauge.Y * 1.0f + 180.0f, x, positionGauge.Y * 1.0f + 190.0f);

            }
            g.FillRectangle(rpm_brushA, positionGauge.X + 200 + 140 + 30, positionGauge.Y * 1.0f + 190.0f,
                 Convert.ToSingle(Math.Min(rpm_redline_x, rpm_size * (rpm - rpm_min) / (rpm_max - rpm_min))), 20);
            if(rpm > rpm_redline)
            {

                g.FillRectangle(rpm_brushB, positionGauge.X + 200 + 140 + 30+rpm_redline_x, positionGauge.Y * 1.0f + 190.0f,
                     Convert.ToSingle(rpm_size * (rpm - rpm_min) / (rpm_max - rpm_min)) - rpm_redline_x, 20);
            }

            /******** PEDALS ********/
            g.FillRectangle(Brush_PedalsBackground, positionGauge.X + 200, positionGauge.Y + 170, 140, 20);
            g.FillRectangle(Brush_PedalsThrottle, positionGauge.X + 200, positionGauge.Y + 170, Convert.ToInt32(throttle * 140), 20);
            g.DrawString("Throttle", new Font("Tahoma", 11.0f, FontStyle.Bold), BrushWhite, positionGauge.X + 237, positionGauge.Y + 170);

            g.FillRectangle(Brush_PedalsBackground, positionGauge.X + 200, positionGauge.Y + 190, 140, 20);
            g.FillRectangle(Brush_PedalsBrake, positionGauge.X + 200, positionGauge.Y + 190, Convert.ToInt32(brake * 140), 20);
            g.DrawString("Brake", new Font("Tahoma", 11.0f, FontStyle.Bold), BrushWhite, positionGauge.X + 247, positionGauge.Y + 190);
        }


        private static void DrawGauges(TelemetryLogReader read, double alpha,int frameNumber,  Point positionGauge, double sample, Graphics g, Image imgTrack, TrackThumbnail TrackThumbnail)
        {

            var Brush_PedalsBackground = GetBrush(alpha, 255, 100, 100, 100);
            var Brush_PedalsBrake = GetBrush(alpha, 255, 200, 0, 0);
            var Brush_PedalsThrottle = GetBrush(alpha, 255, 0, 100, 0);
            var BrushWhite = GetBrush(alpha, 255, 255, 255, 255);
            var BrushTime = GetBrush(1, 255, 255, 255, 255);
            var BrushGray = GetBrush(alpha, 255, 200,200,200);
            var BrushBackground = GetBrush(alpha, 100, 0,0,0);

            var GaugeWhite3 = new Pen(BrushWhite, 3.0f);
            var GaugeWhite2 = new Pen(BrushWhite, 2.0f);
            var GaugeWhite1 = new Pen(BrushWhite, 1.0f);

            double throttle = 0, brake = 0, rpm = 0, speed = 0, rpm_max = 0, rpm_min = 0, redline = 0;
            object gear = 0;

            try
            {
                throttle = read.GetDouble(sample, "Driver.Throttle");
                brake = read.GetDouble(sample, "Player.Pedals_Brake");
                rpm = Rotations.Rads_RPM(read.GetDouble(sample, "Driver.RPM"));
                gear = read.Get(sample, "Driver.Gear");
                speed = read.GetDouble(sample, "Driver.Speed") * 3.6;

                rpm_max = 18000;
                rpm_min = 6000;

                rpm_max = 1000 * Math.Ceiling(1.05 * rpm_max / 1000.0);
                redline = (rpm - 17000) / 1000.0;
                if (redline < 0) redline = 0;
                if (redline > 1) redline = 1;
            }
            catch (Exception ex)
            {

            }
            var BrushGear = GetBrush(alpha, 255, 255, 255 - Convert.ToInt32(redline * 200), 255 - Convert.ToInt32(redline * 200));

            /******** SPEEDO etc ********/

            g.DrawString(Math.Round(speed).ToString("000"), new Font("Tahoma", 36, FontStyle.Bold), BrushWhite, positionGauge.X + 200 - 50, positionGauge.Y + 200 + 35);
            g.DrawString("km/h", new Font("Tahoma", 14), BrushWhite, positionGauge.X + 200 - 20, positionGauge.Y + 200 + 90);

            g.DrawString(gear.ToString(), new Font("Tahoma", 48), BrushGear, positionGauge.X + 200 + 30, positionGauge.Y + 200 + 110);

            /******** LAPTIME *******/
            var tijd = sample/1000.0;
            var minutes = (tijd - (tijd%60))/60;
            var seconds = Math.Floor(tijd - minutes*60);
            var mills = (tijd%1.0)*1000;
            var txt = String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, mills);
            g.DrawString(txt, new Font("Tahoma", 48), BrushTime, positionGauge.X + 50, positionGauge.Y + 450);

            /******** RPM GAUGE ********/
            g.FillEllipse(BrushBackground, positionGauge.X, positionGauge.Y, 400, 400);
            g.DrawArc(GaugeWhite1, positionGauge.X + 45, positionGauge.Y + 45, 310, 310, 90, 270);
            g.DrawArc(GaugeWhite1, positionGauge.X + 50, positionGauge.Y + 50, 300, 300, 90, 270);
            g.DrawArc(GaugeWhite3, positionGauge.X + 55, positionGauge.Y + 55, 290, 290, 90, 270);

            for (int r = (int)(rpm_min / 1000); r <= (rpm_max) / 1000; r++)
            {
                var angle = (r * 1000 - rpm_min) / (rpm_max - rpm_min);
                angle *= 270;
                angle += 90;
                angle *= Math.PI;
                angle /= 180;
                var x1 = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle) * 155.0f;
                var y1 = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle) * 155.0f;

                var x2 = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle) * 165.0f;
                var y2 = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle) * 165.0f;

                var x3 = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle) * 180.0f;
                var y3 = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle) * 180.0f;

                g.DrawLine(GaugeWhite2, x1, y1, x2, y2);

                x3 -= 14;
                y3 -= 12;

                g.DrawString(r.ToString(), new Font("Tahoma", 16.0f, FontStyle.Bold), BrushWhite, x3, y3);
            }

            // NEEDLE
            g.FillEllipse(BrushWhite, positionGauge.X + 400 / 2 - 15, positionGauge.Y + 400 / 2 - 15, 30, 30);

            var angle_needle = 90 + 270 * (rpm - rpm_min) / (rpm_max - rpm_min);

            angle_needle *= Math.PI;
            angle_needle /= 180;

            // Tip of needle
            var nx1 = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle) * 155.0f;
            var ny1 = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle) * 155.0f;
            // Tip of needle
            var nx1a = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle - 0.5 / 180.0 * Math.PI) * 155.0f;
            var ny1a = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle - 0.5 / 180.0 * Math.PI) * 155.0f;

            // Tip of needle
            var nx1b = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle + 0.5 / 180.0 * Math.PI) * 155.0f;
            var ny1b = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle + 0.5 / 180.0 * Math.PI) * 155.0f;

            // Back (white)
            var nx2a = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle + 25.0 / 180 * Math.PI) * -25.0f;
            var ny2a = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle + 25.0 / 180 * Math.PI) * -25.0f;

            // Back (gray)
            var nx2b = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle - 25.0 / 180 * Math.PI) * -25.0f;
            var ny2b = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle - 25.0 / 180 * Math.PI) * -25.0f;

            // Middle (white)
            var nx3 = positionGauge.X + 400.0f / 2 + (float)Math.Cos(angle_needle) * -17.0f;
            var ny3 = positionGauge.Y + 400.0f / 2 + (float)Math.Sin(angle_needle) * -17.0f;

            var arr = new[] { new PointF(nx1, ny1), new PointF(nx1b, ny1b), new PointF(nx2b, ny2b), new PointF(nx3, ny3) };
            g.FillPolygon(BrushGray, arr);
            arr = new[] { new PointF(nx1, ny1), new PointF(nx1a, ny1a), new PointF(nx2a, ny2a), new PointF(nx3, ny3) };
            g.FillPolygon(BrushWhite, arr);

            /******** PEDALS ********/

            g.FillRectangle(Brush_PedalsBackground, positionGauge.X + 260, positionGauge.Y + 220, 140, 30);
            g.FillRectangle(Brush_PedalsThrottle, positionGauge.X + 260, positionGauge.Y + 220, Convert.ToInt32(throttle * 140), 30);
            g.DrawString("Throttle", new Font("Tahoma", 14.0f, FontStyle.Bold), BrushWhite, positionGauge.X + 290, positionGauge.Y + 225);

            g.FillRectangle(Brush_PedalsBackground, positionGauge.X + 260, positionGauge.Y + 260, 140, 30);
            g.FillRectangle(Brush_PedalsBrake, positionGauge.X + 260, positionGauge.Y + 260, Convert.ToInt32(brake * 140), 30);
            g.DrawString("Brake", new Font("Tahoma", 14.0f, FontStyle.Bold), BrushWhite, positionGauge.X + 302, positionGauge.Y + 265);
        }

        private static TrackThumbnail CreateTrackThumbnail(string sTrack, TelemetryInfo data, string file)
        {
            TrackThumbnail t = new TrackThumbnail();
            var track =
                new SimTelemetry.Game.Rfactor.Garage.rFactorTrack(sTrack);
            track.Scan();
            track.ScanRoute();
            t.Create(file, "Spa Franchorchamps", "", track.Route, data.TrackMapSize.Width, data.TrackMapSize.Height);
            return t;
        }

        private static SolidBrush GetBrush(double fade, int alpha, int r, int g, int b)
        {
            return new SolidBrush(Color.FromArgb(Convert.ToInt32(fade * alpha), r, g, b));
        }
    }
}