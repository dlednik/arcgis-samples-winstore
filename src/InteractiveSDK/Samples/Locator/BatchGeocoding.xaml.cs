using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Locator
{
    public sealed partial class BatchGeocoding : Page
    {
        ESRI.ArcGIS.Runtime.Tasks.Locator _locatorTask;
        ObservableCollection<IDictionary<string, string>> batchaddresses = new ObservableCollection<IDictionary<string, string>>();
        GraphicsLayer geocodedResults;
        Graphic MapTipGraphic = null;

        public BatchGeocoding()
        {
            this.InitializeComponent();

            MyMap.InitialExtent = new Envelope(-15000000, 2000000, -7000000, 8000000) { SpatialReference = new SpatialReference(102100) };

            _locatorTask = new ESRI.ArcGIS.Runtime.Tasks.Locator
                (new Uri("http://serverapps101.esri.com/arcgis/rest/services/USA_Geocode/GeocodeServer",UriKind.Absolute));
            
            geocodedResults = MyMap.Layers["LocationGraphicsLayer"] as GraphicsLayer;

            //List of addresses to geocode
            batchaddresses.Add(new Dictionary<string, string> { { "Street", "4409 Redwood Dr" }, { "Zip", "92501" } });
            batchaddresses.Add(new Dictionary<string, string> { { "Street", "3758 Cedar St" }, { "Zip", "92501" } });
            batchaddresses.Add(new Dictionary<string, string> { { "Street", "3486 Orange St" }, { "Zip", "92501" } });
            batchaddresses.Add(new Dictionary<string, string> { { "Street", "2999 4th St" }, { "Zip", "92507" } });
            batchaddresses.Add(new Dictionary<string, string> { { "Street", "3685 10th St" }, { "Zip", "92501" } });

            AddressListbox.ItemsSource = batchaddresses;

        }

        private async void BatchGeocodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (batchaddresses.Count > 0)
            {
                AddressesToLocationsParameter parameter = new AddressesToLocationsParameter();
                parameter.Addresses = batchaddresses.ToList();
                parameter.OutSpatialReference = MyMap.SpatialReference;                
                var result = await _locatorTask.AddressesToLocationsAsync(parameter, System.Threading.CancellationToken.None);

                if (result != null && result.Locations != null && result.Locations.Count > 0)
                {
                    geocodedResults.Graphics.Clear();
                    foreach (AddressCandidate location in result.Locations)
                    {
                        Graphic graphic = new Graphic() { Geometry = location.Location };
                        graphic.Attributes.Add("X", location.Attributes["X"]);
                        graphic.Attributes.Add("Y", location.Attributes["Y"]);
                        graphic.Attributes.Add("Match_addr", location.Attributes["Match_addr"]);
                        graphic.Attributes.Add("Score", location.Attributes["Score"]);
                        geocodedResults.Graphics.Add(graphic);
                    }
                }
            }
        }             

        private void addtolist_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StreetTextBox.Text) || string.IsNullOrEmpty(ZipTextBox.Text))
            {
                //MessageBox.Show("Value is required for all inputs");
                return;
            }

            int number;
            bool result = Int32.TryParse(ZipTextBox.Text, out number);

            if (!result)
            {
                //MessageBox.Show("Incorrect Zip format");
                return;
            }
            batchaddresses.Add(new Dictionary<string, string> { { "Street", StreetTextBox.Text }, { "Zip", ZipTextBox.Text } });
        }

        private void ResetList_Click(object sender, RoutedEventArgs e)
        {
            batchaddresses.Clear();
            geocodedResults.Graphics.Clear();
        }

        private void GraphicsLayer_PointerEntered_1(object sender, GraphicPointerRoutedEventArgs e)
        {
            MapTipGraphic = e.Graphic;
            RenderMapTip();
        }

        private void RenderMapTip()
        {
            MapPoint anchor = MapTipGraphic.Geometry as MapPoint;
            if (MyMap.SpatialReference != null)
            {
                if (MapTipGraphic != null)
                {
                    maptip.DataContext = MapTipGraphic.Attributes;
                }
                //Convert anchor point to the spatial reference of the map
                var mp = GeometryEngine.Project(anchor, MyMap.SpatialReference) as MapPoint;
                //Convert anchor point to screen coordinate
                var screen = MyMap.MapToScreen(mp);

                if (screen.X >= 0 && screen.Y >= 0 &&
                    screen.X < MyMap.ActualWidth && screen.Y < MyMap.ActualHeight)
                {
                    //Update location of map
                    MapTipTranslate.X = screen.X;
                    MapTipTranslate.Y = screen.Y - maptip.ActualHeight;
                    maptip.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else //Anchor is outside the display so close maptip
                {
                    maptip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
        }

        private void GraphicsLayer_PointerExited_1(object sender, GraphicPointerRoutedEventArgs e)
        {
            maptip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            maptip.DataContext = null;
            MapTipGraphic = null;
        }

        private void maptip_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            RenderMapTip();
        }
    }
}
