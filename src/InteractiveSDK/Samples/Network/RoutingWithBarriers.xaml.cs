using System;
using System.Linq;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Network
{
    public sealed partial class RoutingWithBarriers : Page
    {
        public RoutingWithBarriers()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-117.22, 34.04, -117.17, 34.07);
        }

        private async void MyMap_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var mp = MyMap.ScreenToMap(e.GetPosition(MyMap));
            Graphic g = new Graphic() { Geometry = mp };
            var stopsLayer = MyMap.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            var barriersLayer = MyMap.Layers["MyBarriersGraphicsLayer"] as GraphicsLayer;
            if (StopsRadioButton.IsChecked.Value)
            {
                stopsLayer.Graphics.Add(g);
            }
            else if (BarriersRadioButton.IsChecked.Value)
            {
                barriersLayer.Graphics.Add(g);
            }
            if (stopsLayer.Graphics.Count > 1)
            {
                try
                {
                    var routeTask = new RouteTask(new Uri("http://tasks.arcgisonline.com/ArcGIS/rest/services/NetworkAnalysis/ESRI_Route_NA/NAServer/Route"));
                    var result = await routeTask.SolveAsync(new RouteParameter()
                    {
                        Stops = new FeatureStops(stopsLayer.Graphics),
                        Barriers = new FeatureBarriers(barriersLayer.Graphics),
                        UseTimeWindows = false,
                        OutSpatialReference = MyMap.SpatialReference
                    });
                    if (result != null)
                    {
                        if (result.Directions != null && result.Directions.Count > 0)
                        {
                            var direction = result.Directions.FirstOrDefault();
                            if (direction != null && direction.RouteSummary != null)
                                await new MessageDialog(string.Format("{0} minutes", direction.RouteSummary.TotalDriveTime.ToString("#0.000"))).ShowAsync();
                        }
                        GraphicsLayer routeLayer = MyMap.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
                        routeLayer.Graphics.Clear();
                        routeLayer.Graphics.AddRange(result.Routes);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var layer in MyMap.Layers)
                if (layer is GraphicsLayer)
                    (layer as GraphicsLayer).Graphics.Clear();
        }
    }
}
