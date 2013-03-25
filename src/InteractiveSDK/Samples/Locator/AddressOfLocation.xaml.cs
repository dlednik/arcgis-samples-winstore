using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace InteractiveSDK.Samples.Locator
{
    public sealed partial class AddressOfLocation : Page
    {        
        GraphicsLayer _locationGraphicsLayer;
        SpatialReference wgs84 = new SpatialReference(4326);
        SpatialReference mercator = new SpatialReference(102100);
        Graphic MapTipGraphic = null;

        public AddressOfLocation()
        {
            this.InitializeComponent();

            Envelope intial_extent = new Envelope(-117.387,33.97,-117.355,33.988, wgs84);

            MyMap.InitialExtent = GeometryEngine.Project(intial_extent, mercator) as Envelope; ;

            _locationGraphicsLayer = MyMap.Layers["LocationGraphicsLayer"] as GraphicsLayer;

        }

        private async void Map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var mp = MyMap.ScreenToMap(e.GetPosition(MyMap));            

            Graphic g = new Graphic() { Geometry = mp };

            var layer = MyMap.Layers.OfType<GraphicsLayer>().First();
            layer.Graphics.Add(g);

            ESRI.ArcGIS.Runtime.Tasks.Locator locatorTask = 
                new ESRI.ArcGIS.Runtime.Tasks.Locator(
                    new Uri("http://tasks.arcgisonline.com/ArcGIS/rest/services/Locators/TA_Streets_US_10/GeocodeServer", UriKind.Absolute));                
            
            LocationToAddressParameter parameter = new LocationToAddressParameter();

            // Tolerance (distance) specified in meters
            parameter.Distance = 30;
            parameter.Location = mp;
            parameter.OutSpatialReference = MyMap.SpatialReference;

            try
            {
                LocationToAddressResult result = await locatorTask.LocationToAddressAsync(parameter, CancellationToken.None);

                Address address = result.Address;
                IDictionary<string, object> attributes = address.Attributes;

                Graphic graphic = new Graphic() { Geometry = mp};

                string latlon = String.Format("{0}, {1}", address.Location.X, address.Location.Y);
                string address1 = attributes["Street"].ToString();
                string address2 = String.Format("{0}, {1} {2}", attributes["City"], attributes["State"], attributes["ZIP"]);

                graphic.Attributes.Add("LatLon", latlon);
                graphic.Attributes.Add("Address1", address1);
                graphic.Attributes.Add("Address2", address2);

                _locationGraphicsLayer.Graphics.Add(graphic);

            }
            catch(Exception ex)
            {
                return;
            }
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
