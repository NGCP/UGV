using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UGV.Core.Navigation;
using Microsoft.Maps.MapControl.WPF;

namespace UGVBehaviorMap
{
    /// <summary>
    /// 
    /// </summary>
    public class MapController
    {
        public double ZoomLevel
        {
            get 
            {
                if (BingMap == null)
                    return 1;
                else
                    return BingMap.ZoomLevel;
            }
            set
            {
                BingMap.ZoomLevel = value;
            }
        }

        /// <summary>
        /// Bing Map to modify
        /// </summary>
        Map BingMap;

        /// <summary>
        /// Boundary Lines
        /// </summary>
        MapPolyline BoundaryLine;

        /// <summary>
        /// Boundary Lines
        /// </summary>
        MapPolyline SafeZoneLine;

        /// <summary>
        /// Lines
        /// </summary>
        MapPolyline VisionLine;

        /// <summary>
        /// Lines
        /// </summary>
        MapPolyline SearchLine;

        /// <summary>
        /// Lines
        /// </summary>
        MapPolyline BearingLine;
        MapPolyline AvoidanceLine;
        MapPolyline WaypointLine;
        MapPolyline SumLine;

        /// <summary>
        /// Track Point on map
        /// </summary>
        Pushpin TrackPoint;

        /// <summary>
        /// Target Point on map
        /// </summary>
        Pushpin TargetPoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BingMap"></param>
        public MapController(Map BingMap)
        {
            this.BingMap = BingMap;
        }

        /// <summary>
        /// Define the boundary
        /// </summary>
        /// <param name="Boundary"></param>
        public void DefineBoundary(List<WayPoint> Boundary)
        {
            if (Boundary.Count == 0)
                return;
            //remove previous boundary
            BingMap.Children.Remove(BoundaryLine);
            //draw new boundary
            BoundaryLine = new MapPolyline();
            BoundaryLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            BoundaryLine.StrokeThickness = 5;
            BoundaryLine.Opacity = 0.7;
            BoundaryLine.Locations = new LocationCollection();
            double LatCenter = 0;
            double LongCenter = 0;
            double LatMin = Double.MaxValue;
            double LatMax = Double.MinValue;
            double LongMin = Double.MaxValue;
            double LongMax = Double.MinValue;
            foreach (var p in Boundary)
            {
                Location loc = new Location(p.Lat, p.Long);
                BoundaryLine.Locations.Add(loc);
                LatCenter += p.Lat;
                LongCenter += p.Long;
                LatMin = Math.Min(LatMin, p.Lat);
                LatMax = Math.Max(LatMax, p.Lat);
                LongMin = Math.Min(LongMin, p.Long);
                LongMax = Math.Max(LongMax, p.Long);
            }
            //finish
            BoundaryLine.Locations.Add(new Location(Boundary[0].Lat, Boundary[0].Long));
            LatCenter /= (double)Boundary.Count;
            LongCenter /= (double)Boundary.Count;
            BingMap.Children.Add(BoundaryLine);
            BingMap.Center = new Location(LatCenter, LongCenter);
            BingMap.Mode = new AerialMode(true);
            double LatScale = Math.Abs(LatMax - LatMin);
            double LongScale = Math.Abs(LongMax - LongMin);
            double Scale = Math.Max(LatScale, LongScale);
            BingMap.ZoomLevel = 0.0205 / Scale;
        }

        public void CenterAt(double Lat, double Long)
        {
            BingMap.Center = new Location(Lat, Long);
        }

        /// <summary>
        /// Define the safezone
        /// </summary>
        /// <param name="SafeZone"></param>
        public void DefineSafeZone(List<WayPoint> SafeZone)
        {
            if (SafeZone.Count == 0)
                return;
            //remove previous boundary
            BingMap.Children.Remove(SafeZoneLine);
            //draw new boundary
            SafeZoneLine = new MapPolyline();
            SafeZoneLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightBlue);
            SafeZoneLine.StrokeThickness = 5;
            SafeZoneLine.Opacity = 0.7;
            SafeZoneLine.Locations = new LocationCollection();
            double LatCenter = 0;
            double LongCenter = 0;
            double LatMin = Double.MaxValue;
            double LatMax = Double.MinValue;
            double LongMin = Double.MaxValue;
            double LongMax = Double.MinValue;
            foreach (var p in SafeZone)
            {
                Location loc = new Location(p.Lat, p.Long);
                SafeZoneLine.Locations.Add(loc);
                LatCenter += p.Lat;
                LongCenter += p.Long;
                LatMin = Math.Min(LatMin, p.Lat);
                LatMax = Math.Max(LatMax, p.Lat);
                LongMin = Math.Min(LongMin, p.Long);
                LongMax = Math.Max(LongMax, p.Long);
            }
            //finish
            SafeZoneLine.Locations.Add(new Location(SafeZone[0].Lat, SafeZone[0].Long));
            LatCenter /= (double)SafeZone.Count;
            LongCenter /= (double)SafeZone.Count;
            BingMap.Children.Add(SafeZoneLine);
        }


        /// <summary>
        /// Define vision path
        /// </summary>
        /// <param name="VisionPath"></param>
        public void DefineVisionPath(WayPoint CurrectLocation, List<NGCP.UGV.VisionWayPoint> VisionPath)
        {
            //remove previous boundary
            BingMap.Children.Remove(VisionLine);
            //draw new boundary
            VisionLine = new MapPolyline();
            VisionLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
            VisionLine.StrokeThickness = 3;
            VisionLine.Opacity = 0.7;
            VisionLine.Locations = new LocationCollection();
            VisionLine.Locations.Add(new Location(CurrectLocation.Lat, CurrectLocation.Long));
            foreach (var p in VisionPath)
            {
                Location loc = new Location(p.X, p.Y);
                VisionLine.Locations.Add(loc);
            }
            BingMap.Children.Add(VisionLine);
        }

        /// <summary>
        /// Define search path
        /// </summary>
        /// <param name="SearchPath"></param>
        public void DefineSearchPath(WayPoint CurrectLocation, List<WayPoint> SearchPath)
        {
            //remove previous boundary
            BingMap.Children.Remove(SearchLine);
            //draw new boundary
            SearchLine = new MapPolyline();
            SearchLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            SearchLine.StrokeThickness = 3;
            SearchLine.Opacity = 0.7;
            SearchLine.Locations = new LocationCollection();
            SearchLine.Locations.Add(new Location(CurrectLocation.Lat, CurrectLocation.Long));
            foreach (var p in SearchPath)
            {
                Location loc = new Location(p.Lat, p.Long);
                SearchLine.Locations.Add(loc);
            }
            BingMap.Children.Add(SearchLine);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CurrentLocation"></param>
        /// <param name="Bearing"></param>
        public void DefineHeading(WayPoint CurrentLocation, double Bearing, double size)
        {
            // Front Waypoint
            var frontWP = WayPoint.Projection(CurrentLocation, Bearing, 3.0 * size);
            //var rightWP = WayPoint.Projection(CurrentLocation, Bearing + Math.PI / 2.0, size);
            //var backWP = WayPoint.Projection(CurrentLocation, Bearing + Math.PI, size);
            //var leftWP = WayPoint.Projection(CurrentLocation, Bearing - Math.PI / 2.0, size);
            //remove previous bearing line
            BingMap.Children.Remove(BearingLine);
            //draw new bearing line
            BearingLine = new MapPolyline();
            BearingLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            BearingLine.StrokeThickness = 2;
            BearingLine.Opacity = 0.7;
            BearingLine.Locations = new LocationCollection();
            BearingLine.Locations.Add(new Location(CurrentLocation.Lat, CurrentLocation.Long));
            BearingLine.Locations.Add(new Location(frontWP.Lat, frontWP.Long));
            //BearingLine.Locations.Add(new Location(rightWP.Lat, rightWP.Long));
            //BearingLine.Locations.Add(new Location(backWP.Lat, backWP.Long));
            //BearingLine.Locations.Add(new Location(leftWP.Lat, leftWP.Long));
            //BearingLine.Locations.Add(new Location(frontWP.Lat, frontWP.Long));
            BingMap.Children.Add(BearingLine);

            if (ConsoleDisplay.ugv.Settings.UseVision)
            {
                // Avoidance Vector
                BingMap.Children.Remove(AvoidanceLine);
                var avoidanceWP = WayPoint.Projection(CurrentLocation, ConsoleDisplay.ugv.AvoidanceVector.angle, ConsoleDisplay.ugv.AvoidanceVector.magnitude);
                AvoidanceLine = new MapPolyline();
                AvoidanceLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                AvoidanceLine.StrokeThickness = 2;
                AvoidanceLine.Opacity = 0.7;
                AvoidanceLine.Locations = new LocationCollection();
                AvoidanceLine.Locations.Add(new Location(CurrentLocation.Lat, CurrentLocation.Long));
                AvoidanceLine.Locations.Add(new Location(avoidanceWP.Lat, avoidanceWP.Long));
                BingMap.Children.Add(AvoidanceLine);

                // Waypoint Vector
                BingMap.Children.Remove(WaypointLine);
                var waypointWP = WayPoint.Projection(CurrentLocation, ConsoleDisplay.ugv.WaypointVector.angle, ConsoleDisplay.ugv.WaypointVector.magnitude);
                WaypointLine = new MapPolyline();
                WaypointLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Purple);
                WaypointLine.StrokeThickness = 2;
                WaypointLine.Opacity = 0.7;
                WaypointLine.Locations = new LocationCollection();
                WaypointLine.Locations.Add(new Location(CurrentLocation.Lat, CurrentLocation.Long));
                WaypointLine.Locations.Add(new Location(waypointWP.Lat, waypointWP.Long));
                BingMap.Children.Add(WaypointLine);

                // Sum Vector
                BingMap.Children.Remove(SumLine);
                var sumWP = WayPoint.Projection(CurrentLocation, ConsoleDisplay.ugv.SumVector.angle, ConsoleDisplay.ugv.SumVector.magnitude);
                SumLine = new MapPolyline();
                SumLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
                SumLine.StrokeThickness = 2;
                SumLine.Opacity = 0.7;
                SumLine.Locations = new LocationCollection();
                SumLine.Locations.Add(new Location(CurrentLocation.Lat, CurrentLocation.Long));
                SumLine.Locations.Add(new Location(sumWP.Lat, sumWP.Long));
                BingMap.Children.Add(SumLine);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoint"></param>
        public void DefineTrackPoint(WayPoint waypoint)
        {
            BingMap.Children.Remove(TrackPoint);
            TrackPoint = new Pushpin();
            TrackPoint.Location = new Location(waypoint.Lat, waypoint.Long);
            BingMap.Children.Add(TrackPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoint"></param>
        public void DefineTargetPoint(WayPoint waypoint)
        {
            BingMap.Children.Remove(TargetPoint);
            TargetPoint = new Pushpin();
            TargetPoint.Location = new Location(waypoint.Lat, waypoint.Long);
            TargetPoint.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            BingMap.Children.Add(TargetPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveTargetPoint()
        {
            BingMap.Children.Remove(TargetPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoint"></param>
        public void DefineTargetPoint(double lat, double lon)
        {
            WayPoint wp = new WayPoint(lat, lon, 0);
            DefineTargetPoint(wp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoint"></param>
        public void AddPushpin(WayPoint waypoint)
        {
            Pushpin pin = new Pushpin();
            pin.Location = new Location(waypoint.Lat, waypoint.Long);
            BingMap.Children.Add(pin);
        }
    }
}
