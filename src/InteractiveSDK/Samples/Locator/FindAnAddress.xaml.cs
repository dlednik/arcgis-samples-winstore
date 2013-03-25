using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace InteractiveSDK.Samples.Locator
{
    public sealed partial class FindAnAddress : Page
    {
        ESRI.ArcGIS.Runtime.Tasks.Locator _locatorTask;
        GraphicsLayer _candidateGraphicsLayer;
        Graphic MapTipGraphic = null;

        public FindAnAddress()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = GeometryEngine.Project(new Envelope(-122.554, 37.615, -122.245, 37.884, SpatialReference.Wgs84),
                SpatialReference.WebMercator) as Envelope;
            _candidateGraphicsLayer = MyMap.Layers["CandidateGraphicsLayer"] as GraphicsLayer;
        }

        private async void FindAddressButton_Click(object sender, RoutedEventArgs e)
        {
            _locatorTask = new ESRI.ArcGIS.Runtime.Tasks.Locator
                (new Uri("http://tasks.arcgisonline.com/ArcGIS/rest/services/Locators/TA_Streets_US_10/GeocodeServer", UriKind.Absolute));

            AddressToLocationsParameter addressParams = new AddressToLocationsParameter()
            {
                OutSpatialReference = MyMap.SpatialReference
            };

            IDictionary<string, string> address = addressParams.Address;

            if (!string.IsNullOrEmpty(InputAddress.Text))
                address.Add("Street", InputAddress.Text);
            if (!string.IsNullOrEmpty(City.Text))
                address.Add("City", City.Text);
            if (!string.IsNullOrEmpty(State.Text))
                address.Add("State", State.Text);
            if (!string.IsNullOrEmpty(Zip.Text))
                address.Add("ZIP", Zip.Text);

            try
            {
                AddressToLocationsResult result = await _locatorTask.AddressToLocationsAsync(addressParams);

                if (_candidateGraphicsLayer.Graphics != null && _candidateGraphicsLayer.Graphics.Count > 0)
                    _candidateGraphicsLayer.Graphics.Clear();

                if (result != null)
                {
                    foreach (AddressCandidate candidate in result.AddressCandidates)
                    {
                        if (candidate.Score >= 80)
                        {
                            Graphic graphic = new Graphic()
                            {
                                Geometry = candidate.Location
                            };

                            graphic.Attributes.Add("Address", candidate.Address);

                            string latlon = String.Format("{0}, {1}", candidate.Location.X, candidate.Location.Y);
                            graphic.Attributes.Add("LatLon", latlon);

                            if (candidate.Location.SpatialReference == null)
                            {
                                candidate.Location.SpatialReference = SpatialReference.Wgs84;
                            }

                            _candidateGraphicsLayer.Graphics.Add(graphic);
                        }
                    }
                }
            }
            catch (Exception ex)
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
