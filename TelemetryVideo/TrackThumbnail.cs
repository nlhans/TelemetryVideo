﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using SimTelemetry.Objects;

namespace TelemetryVideo
{
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
}