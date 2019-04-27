using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UGVBehaviorMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer timer;

        MapController map;

        public MainWindow()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(
                (obj) =>
                {
                    ConsoleDisplay.StartMain(null);
                }));
            InitializeComponent();
            map = new MapController(BingMap);
            this.Visibility = System.Windows.Visibility.Hidden;
            timer = new System.Timers.Timer(200);
            timer.Elapsed += Timer_Tick;
            timer.Start();
        }

        List<UGV.Core.Navigation.WayPoint> PreviousBoundary;
        List<UGV.Core.Navigation.WayPoint> PreviousSafeZone;

        void Timer_Tick(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        this.Visibility = ConsoleDisplay.DisplayMap ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                    }
                    catch (Exception)
                    { }

                    if (!ConsoleDisplay.DisplayMap)
                        return;

                    if (PreviousBoundary == null || PreviousBoundary != ConsoleDisplay.ugv.Boundary)
                    {
                        PreviousBoundary = ConsoleDisplay.ugv.Boundary;
                        map.DefineBoundary(ConsoleDisplay.ugv.Boundary);
                    }

                    if (PreviousSafeZone == null || PreviousSafeZone != ConsoleDisplay.ugv.SafeZone)
                    {
                        PreviousSafeZone = ConsoleDisplay.ugv.SafeZone;
                        map.DefineSafeZone(ConsoleDisplay.ugv.SafeZone);
                    }

                    UGV.Core.Navigation.WayPoint CurrentLocation = new UGV.Core.Navigation.WayPoint(ConsoleDisplay.ugv.Latitude, ConsoleDisplay.ugv.Longitude, 0);
                    map.DefineVisionPath(CurrentLocation, ConsoleDisplay.ugv.VisionWaypoints);
                    map.DefineSearchPath(CurrentLocation, ConsoleDisplay.ugv.Waypoints.ToList());
                    //map.DefineTrackPoint(CurrentLocation);
                    map.DefineHeading(CurrentLocation, ConsoleDisplay.ugv.Heading, 1000000.0 / Math.Pow(2, map.ZoomLevel));
                    if (ConsoleDisplay.AlwaysCenter)
                        map.CenterAt(CurrentLocation.Lat, CurrentLocation.Long);
                    if (ConsoleDisplay.ugv.TargetLockedLocation != null)
                        map.DefineTargetPoint(ConsoleDisplay.ugv.TargetLockedLocation);
                    else if (ConsoleDisplay.ugv.VisionTargets.Count > 0)
                    {
                        var target = ConsoleDisplay.ugv.VisionTargets[0];
                        map.DefineTargetPoint(target.Lat, target.Long);
                    }
                    else
                        map.RemoveTargetPoint();

                    if (ConsoleDisplay.ZoomIn)
                    {
                        map.ZoomLevel += 0.1;
                        ConsoleDisplay.ZoomIn = false;
                    }

                    if (ConsoleDisplay.ZoomOut)
                    {
                        map.ZoomLevel -= 0.1;
                        ConsoleDisplay.ZoomOut = false;
                    }
                });
            }
            catch (Exception)
            { }
        }
    }
}
