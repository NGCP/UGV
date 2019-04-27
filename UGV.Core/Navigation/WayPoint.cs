using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core.Navigation
{
    public class WayPoint
    {
        /// <summary>
        /// Radius of earth
        /// </summary>
        const double R = 6371000; // meter
        /// <summary>
        /// Guid of waypoint
        /// </summary>
        public Guid GUID;
        /// <summary>
        /// Lat of waypoint
        /// </summary>
        public double Lat;
        /// <summary>
        /// Long of waypoint
        /// </summary>
        public double Long;
        /// <summary>
        /// Altitute of waypoint
        /// </summary>
        public double Alt;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Lat"></param>
        /// <param name="Long"></param>
        /// <param name="Alt"></param>
        public WayPoint(double Lat, double Long, double Alt)
        {
            this.Lat = Lat;
            this.Long = Long;
            this.Alt = Alt;
            this.GUID = Guid.NewGuid();
        }
        /// <summary>
        /// String format of a waypoint
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<LAT:" + Lat + ", LONG:" + Long + ", ALT:" + Alt + "> id:";
        }

        /// <summary>
        /// Calculate bearing angle from one waypoint to another
        /// </summary>
        /// <param name="LatStart">Lat of Start Point</param>
        /// <param name="LonStart">Long of Start Point</param>
        /// <param name="LatEnd">Lat of End Point</param>
        /// <param name="LongEnd">Long of End Point</param>
        /// <returns></returns>
        public static double GetBearing(double LatStart, double LonStart, double LatEnd, double LongEnd)
        {
            double dLat = (LatEnd - LatStart) * Math.PI / 180.0;
            double dLon = (LongEnd - LonStart) * Math.PI / 180.0;
            LatStart = LatStart * Math.PI / 180.0;
            LatEnd = LatEnd * Math.PI / 180.0;
            double y = Math.Sin(dLon) * Math.Cos(LatEnd);
            double x = Math.Cos(LatStart) * Math.Sin(LatEnd) -
                        Math.Sin(LatStart) * Math.Cos(LatEnd) * Math.Cos(dLon);
            double bearing = Math.Atan2(y, x);
            return bearing;
        }
        /// <summary>
        /// Calculate distance from one waypoint to another
        /// </summary>
        /// <param name="LatStart">Lat of Start Point</param>
        /// <param name="LonStart">Long of Start Point</param>
        /// <param name="LatEnd">Lat of End Point</param>
        /// <param name="LongEnd">Long of End Point</param>
        /// <returns></returns>
        public static double GetDistance(double LatStart, double LonStart, double LatEnd, double LongEnd)
        {
            double dLat = (LatEnd - LatStart) * Math.PI / 180.0;
            double dLon = (LongEnd - LonStart) * Math.PI / 180.0;
            LatStart = LatStart * Math.PI / 180.0;
            LatEnd = LatEnd * Math.PI / 180.0;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(LatStart) * Math.Cos(LatEnd);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return distance;
        }
        /// <summary>
        /// Intersection of two waypoint
        /// </summary>
        /// <param name="WaypointA"></param>
        /// <param name="HeadingA"></param>
        /// <param name="WaypointB"></param>
        /// <param name="HeadingB"></param>
        /// <returns></returns>
        public static WayPoint GetIntersection(WayPoint WaypointA, double HeadingA, WayPoint WaypointB, double HeadingB)
        {
            //convert lat long to radian
            double LatA = WaypointA.Lat * Math.PI / 180.0;
            double LongA = WaypointA.Long * Math.PI / 180.0;
            double LatB = WaypointB.Lat * Math.PI / 180.0;
            double LongB = WaypointB.Long * Math.PI / 180.0;

            double deltaLat = LatB - LatA;
            double deltaLong = LongB - LongA;
            double roAB = 2 *
                Math.Asin(
                    Math.Sqrt(
                        Math.Sin(deltaLat / 2.0) * Math.Sin(deltaLat / 2.0) +
                        Math.Cos(LatA) * Math.Cos(LatB) *
                        Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2)
                    )
                );
            //no intersection
            if (roAB == 0)
                return null;
            //get theta
            double thetaA =
                Math.Acos(
                    (Math.Sin(LatB) -
                    Math.Sin(LatA) * Math.Cos(roAB))
                    / (Math.Sin(roAB) * Math.Cos(LatA))
                );
            thetaA = thetaA == Double.NaN ? 0 : thetaA;
            double thetaB =
                Math.Acos(
                    (Math.Sin(LatA) -
                    Math.Sin(LatB) * Math.Cos(roAB))
                    / (Math.Sin(roAB) * Math.Cos(LatB))
                );
            //calculate theta of two 
            double thetaAB, thetaBA;
            if (Math.Sin(deltaLong) > 0)
            {
                thetaAB = thetaA;
                thetaBA = 2.0 * Math.PI - thetaB;
            }
            else
            {
                thetaAB = 2.0 * Math.PI - thetaA;
                thetaBA = thetaB;
            }
            //calculate alpha
            // angle 2-1-3
            double AlphaA = (HeadingA - thetaAB + Math.PI) % (2.0 * Math.PI) - Math.PI;
            // angle 1-2-3
            double AlphaB = (thetaBA - HeadingB + Math.PI) % (2.0 * Math.PI) - Math.PI;
            // infinite intersections
            if (Math.Sin(AlphaA) == 0 && Math.Sin(AlphaB) == 0) return null;
            // ambiguous intersection
            if (Math.Sin(AlphaA) * Math.Sin(AlphaB) < 0) return null;
            //apporach intersection
            double AlphaC = Math.Acos(-Math.Cos(AlphaA) * Math.Cos(AlphaB) +
                        Math.Sin(AlphaA) * Math.Sin(AlphaB) * Math.Cos(roAB));
            double roAC = Math.Atan2(Math.Sin(roAB) * Math.Sin(AlphaA) * Math.Sin(AlphaB),
                        Math.Cos(AlphaB) + Math.Cos(AlphaA) * Math.Cos(AlphaC));
            double LatC = Math.Asin(Math.Sin(LatA) * Math.Cos(roAC) +
                        Math.Cos(LatA) * Math.Sin(roAC) * Math.Cos(HeadingA));
            double deltaLongAC = Math.Atan2(
                Math.Sin(HeadingA) * Math.Sin(roAC) * Math.Cos(LatA),
                Math.Cos(roAC) - Math.Sin(LatA) * Math.Sin(LatC));
            double LongC = LongA + deltaLongAC;
            LongC = (LongC + 3.0 * Math.PI) % (2.0 * Math.PI) - Math.PI;
            return new WayPoint(LatC * 180.0 / Math.PI, LongC * 180.0 / Math.PI, 0);
        }

        /// <summary>
        /// Get the center of waypoints
        /// </summary>
        /// <param name="Boundary"></param>
        /// <returns></returns>
        public static WayPoint GetCenter(List<WayPoint> Boundary)
        {
            double LatCenter = 0;
            double LongCenter = 0;
            foreach (var waypoint in Boundary)
            {
                LatCenter += waypoint.Lat;
                LongCenter += waypoint.Long;
            }
            LatCenter /= (double)Boundary.Count;
            LongCenter /= (double)Boundary.Count;
            //get center point
            WayPoint center = new WayPoint(LatCenter, LongCenter, 0);
            return center;
        }

        /// <summary>
        /// Project from one point to a distance
        /// </summary>
        /// <param name="wp"></param>
        /// <param name="bearing">In radians</param>
        /// <param name="distance">Distance in meters</param>
        /// <returns></returns>
        public static WayPoint Projection(WayPoint wp, double bearing, double distance)
        {
            // ======= bad calculation ========
            double tLat;
            double tLong;

            // ==== deleted old code for Projection ====

            double cLat = wp.Lat * Math.PI / 180.0;
            double cLon = wp.Long * Math.PI / 180.0;
            var newLat = Math.Asin(Math.Sin(cLat) * Math.Cos(distance / R) +
                        Math.Cos(cLat) * Math.Sin(distance / R) * Math.Cos(bearing));
            var newLong = cLon + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / R) * Math.Cos(cLat),
                                 Math.Cos(distance / R) - Math.Sin(cLat) * Math.Sin(newLat));
            return new WayPoint(newLat * 180.0 / Math.PI, newLong * 180.0 / Math.PI, 0);

        }

        /// <summary>
        /// Check if the waypoint is inside boundary
        /// </summary>
        /// <param name="Lat"></param>
        /// <param name="Long"></param>
        /// <param name="Boundary"></param>
        /// <returns></returns>
        public static bool IsInsideBoundary(double Lat, double Long, List<WayPoint> Boundary)
        {

            // ==== deleted old code for IsInsideBoundary ====

            if (Boundary.Count < 3)
                return true;
            double LatCenter = 0;
            double LongCenter = 0;
            foreach (var waypoint in Boundary)
            {
                LatCenter += waypoint.Lat;
                LongCenter += waypoint.Long;
            }
            LatCenter /= (double)Boundary.Count;
            LongCenter /= (double)Boundary.Count;
            //get center point
            WayPoint center = new WayPoint(LatCenter, LongCenter, 0);
            //get heading from center to location
            double heading = WayPoint.GetBearing(center.Lat, center.Long, Lat, Long);
            //Count intersect
            int intersectCount = 0;
            for (int i = 0; i < Boundary.Count; i++)
            {
                //get bound
                WayPoint BoundX = Boundary[i];
                WayPoint BoundY = Boundary[(i + 1) % Boundary.Count];
                double boundHeading = GetBearing(BoundX.Lat, BoundX.Long, BoundY.Lat, BoundY.Long);
                WayPoint intersect = WayPoint.GetIntersection(center, heading, BoundX, boundHeading);
                if (intersect == null)
                    continue;
                //validate intersection
                if (
                    //is with target ray
                    ((intersect.Lat >= center.Lat && intersect.Lat <= Lat) || (intersect.Lat <= center.Lat && intersect.Lat >= Lat))
                    &&
                    ((intersect.Long >= center.Long && intersect.Long <= Lat) || (intersect.Long <= center.Long && intersect.Long >= Long))
                    &&
                    //is with boundary ray
                    ((intersect.Lat >= BoundX.Lat && intersect.Lat <= BoundY.Lat) || (intersect.Lat <= BoundX.Lat && intersect.Lat >= BoundY.Lat))
                    &&
                    ((intersect.Long >= BoundX.Long && intersect.Long <= BoundY.Lat) || (intersect.Long <= BoundX.Long && intersect.Long >= BoundY.Long))

                    )
                {
                    intersectCount++;
                }
            }
            return intersectCount % 2 == 0;
        }

        /// <summary>
        /// Generate a random waypoint inside boundary
        /// </summary>
        /// <param name="Boundary"></param>
        /// <returns></returns>
        public static WayPoint GenerateRandomWaypoint(List<WayPoint> Boundary)
        {
            if (Boundary.Count < 3)
                return null;
            //get a fuzzy boundary
            double LatMin = 360;
            double LatMax = -360;
            double LongMin = 360;
            double LongMax = -360;
            foreach (var waypoint in Boundary)
            {
                LatMin = Math.Min(LatMin, waypoint.Lat);
                LatMax = Math.Max(LatMax, waypoint.Lat);
                LongMin = Math.Min(LongMin, waypoint.Long);
                LongMax = Math.Max(LongMax, waypoint.Long);
            }
            //prepare a random set
            Random rand = new Random(DateTime.Now.Millisecond);
            while (true)
            {
                double Lat = (double)rand.Next((int)(LatMin * 1000000.0), (int)(LatMax * 1000000.0)) / 1000000.0;
                double Long = (double)rand.Next((int)(LongMin * 1000000.0), (int)(LongMax * 1000000.0)) / 1000000.0;
                if (WayPoint.IsInsideBoundary(Lat, Long, Boundary))
                    return new WayPoint(Lat, Long, 0);
            }
        }
    }
}
