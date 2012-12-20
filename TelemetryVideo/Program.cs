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
        public TrackThumbnail()
        {


        }

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
    class Program
    {
        private static string telem = @"..\..\..\Telemetry\Spa 1.44.368.gz";
        private static string picInput = @"H:\Input";
        private static string picOutput = @"H:\Output";

        static void Main(string[] args)
        {
            TrackThumbnail t = new TrackThumbnail();
            var track =
                new SimTelemetry.Game.Rfactor.Garage.rFactorTrack(
                    @"C:\Program Files (x86)\rFactor\GameData\Locations\F108_Spa\F108_Spa.gdb");
            track.Scan();
            track.ScanRoute();
            t.Create("circuit.png", "Spa Franchorchamps", "", track.Route, 525,525);

            var imgTrack = Image.FromFile("circuit.png");

            TelemetryLogReader read = new TelemetryLogReader(telem);
            read.ReadPolling();


            var files = Directory.GetFiles(picInput, "*.jpg");

            Console.WriteLine(read.Samples.Count);

            int fade_start = 93;
            int fade_length = 10;
            int fade_end = 3223 - fade_length;
            var positionGauge = new Point(150, 310);

            int f = 93;
            for (int i = 0; i < files.Length; i++)
            {
                var frame = files[i];
                var file = picOutput + "\\img-" + i.ToString("0000") + ".jpg";
                var time = (i - 92) * 1000.0 / 30.0;

                if (i < fade_start )
                {
                    try
                    {
                        File.Copy(frame, file);
                    }catch(Exception ex)
                    {
                        
                    }
                    continue;
                }
                if(i >= fade_end+fade_length)
                {
                    var tijd = "01:44.368";
                    using(Image im = Image.FromFile(frame))
                    {
                        Graphics g = Graphics.FromImage(im);
                        g.DrawString(tijd, new Font("Tahoma", 48), Brushes.White, positionGauge.X + 50, positionGauge.Y + 450);
                        im.Save(file);
                    }
                    continue;
                }

                var sample = read.Samples.Where(x => x.Value.Time <= time).OrderBy(x => -1 * x.Value.Time).FirstOrDefault().Key;
                if (sample == null) continue;


                double alpha = 1.0*(i - fade_start) / fade_length;
                if(i>=fade_end) alpha = 1-1.0*(i - fade_end)/fade_length;
                if (alpha > 1) alpha = 1;

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

                var DriverBrush = GetBrush(alpha, 255, 100, 255, 0);

                using (var imIn = Image.FromFile(frame))
                {
                    Graphics g = Graphics.FromImage(imIn);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;


                    double throttle = 0, brake = 0, rpm = 0, speed = 0, rpm_max = 0, rpm_min = 0, redline = 0,x =0, y=0;
                    object gear = 0;

                    try
                    {
                        throttle = read.GetDouble(sample, "Driver.Throttle");
                        brake = read.GetDouble(sample, "Player.Pedals_Brake");
                        rpm = Rotations.Rads_RPM(read.GetDouble(sample, "Driver.RPM"));
                        gear = read.Get(sample, "Driver.Gear");
                        x = read.GetDouble(sample, "Driver.CoordinateX");
                        y = read.GetDouble(sample, "Driver.CoordinateZ");
                        if (i < 168) gear = 0;
                        if (i < 167) gear = 4;
                        if (i < 163) gear = 0;
                        if (i < 162) gear = 5;
                        if (i < 158) gear = 0;
                        if (i < 157) gear = 6;
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

                    /******* TRACK ******/
                    g.DrawImage(imgTrack, 1450, 300);

                    var px = 1450 + t.GetX(x);
                    var py = 300+ t.GetY(y);
                    
                    g.FillEllipse(DriverBrush, px-6, py-6, 13,13);

                    /******** SPEEDO etc ********/

                    g.DrawString(Math.Round(speed).ToString("000"), new Font("Tahoma", 36, FontStyle.Bold), BrushWhite, positionGauge.X + 200 - 50, positionGauge.Y + 200 + 35);
                    g.DrawString("km/h", new Font("Tahoma", 14), BrushWhite, positionGauge.X + 200 - 20, positionGauge.Y + 200 + 90);

                    g.DrawString(gear.ToString(), new Font("Tahoma", 48), BrushGear, positionGauge.X + 200 + 30, positionGauge.Y + 200 + 110);

                    /******** LAPTIME *******/
                    var tijd = time/1000.0;
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
                    imIn.Save(file);
                }
                Console.WriteLine(("frame " + i));
            }

            
            //Console.ReadLine();

        }

        private static SolidBrush GetBrush(double fade, int alpha, int r, int g, int b)
        {
            return new SolidBrush(Color.FromArgb(Convert.ToInt32(fade * alpha), r, g, b));
        }
    }
}