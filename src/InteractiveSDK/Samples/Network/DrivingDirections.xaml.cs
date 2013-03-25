using System;
using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.ArcGISServices;
using ESRI.ArcGIS.Runtime.Symbology;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Network
{
    public sealed partial class DrivingDirections : Page
    {
        public DrivingDirections()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-123, 33, -115, 37);
        }

        private async void GetDirections_Click(object sender, RoutedEventArgs e)
        {
            //Reset
            DirectionsStackPanel.Children.Clear();
            var _stops = new List<Graphic>();
            var _locator = new ESRI.ArcGIS.Runtime.Tasks.Locator(new Uri("http://tasks.arcgisonline.com/ArcGIS/rest/services/Locators/TA_Address_NA/GeocodeServer"));
            var routeLayer = MyMap.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
            routeLayer.Graphics.Clear();
            try
            {
                //Geocode from address
                var fromLocation = await _locator.AddressToLocationsAsync(ParseAddress(FromTextBox.Text));
                if (fromLocation.AddressCandidates != null && fromLocation.AddressCandidates.Count > 0)
                {
                    AddressCandidate address = fromLocation.AddressCandidates.FirstOrDefault();
                    Graphic graphicLocation = new Graphic() { Geometry = address.Location, Symbol = LayoutRoot.Resources["FromSymbol"] as ISymbol };
                    graphicLocation.Attributes["address"] = address.Address;
                    graphicLocation.Attributes["score"] = address.Score;
                    _stops.Add(graphicLocation);
                    routeLayer.Graphics.Add(graphicLocation);
                }
                //Geocode to address
                var toLocation = await _locator.AddressToLocationsAsync(ParseAddress(ToTextBox.Text));
                if (toLocation.AddressCandidates != null && toLocation.AddressCandidates.Count > 0)
                {
                    AddressCandidate address = toLocation.AddressCandidates.FirstOrDefault();
                    Graphic graphicLocation = new Graphic() { Geometry = address.Location, Symbol = LayoutRoot.Resources["ToSymbol"] as ISymbol };
                    graphicLocation.Attributes["address"] = address.Address;
                    graphicLocation.Attributes["score"] = address.Score;
                    _stops.Add(graphicLocation);
                    routeLayer.Graphics.Add(graphicLocation);
                }
                //Get route between from and to
                var _routeParams = new RouteParameter()
                {
                    ReturnRoutes = false,
                    ReturnDirections = true,
                    DirectionsLengthUnits = MapUnit.Miles,
                    Stops = new FeatureStops(_stops),
                    UseTimeWindows = false
                };

                var _routeTask = new RouteTask(new Uri("http://tasks.arcgisonline.com/ArcGIS/rest/services/NetworkAnalysis/ESRI_Route_NA/NAServer/Route"));

                _routeParams.OutSpatialReference = MyMap.SpatialReference;
                var routeTaskResult = await _routeTask.SolveAsync(_routeParams);
                _directionsFeatureSet = routeTaskResult.Directions.FirstOrDefault();

                routeLayer.Graphics.Add(new Graphic() { Geometry = _directionsFeatureSet.MergedGeometry, Symbol = LayoutRoot.Resources["RouteSymbol"] as ISymbol });
                TotalDistanceTextBlock.Text = string.Format("Total Distance: {0}", FormatDistance(_directionsFeatureSet.RouteSummary.TotalLength, "miles"));
                TotalTimeTextBlock.Text = string.Format("Total Time: {0}", FormatTime(_directionsFeatureSet.RouteSummary.TotalTime));
                TitleTextBlock.Text = _directionsFeatureSet.RouteName;

                int i = 1;
                foreach (Graphic graphic in _directionsFeatureSet.Graphics)
                {
                    System.Text.StringBuilder text = new System.Text.StringBuilder();
                    text.AppendFormat("{0}. {1}", i, graphic.Attributes["text"]);
                    if (i > 1 && i < _directionsFeatureSet.Graphics.Count)
                    {
                        string distance = FormatDistance(Convert.ToDouble(graphic.Attributes["length"]), "miles");
                        string time = null;
                        if (graphic.Attributes.ContainsKey("time"))
                        {
                            time = FormatTime(Convert.ToDouble(graphic.Attributes["time"]));
                        }
                        if (!string.IsNullOrEmpty(distance) || !string.IsNullOrEmpty(time))
                            text.Append(" (");
                        text.Append(distance);
                        if (!string.IsNullOrEmpty(distance) && !string.IsNullOrEmpty(time))
                            text.Append(", ");
                        text.Append(time);
                        if (!string.IsNullOrEmpty(distance) || !string.IsNullOrEmpty(time))
                            text.Append(")");
                    }
                    TextBlock textBlock = new TextBlock() { Text = text.ToString(), Tag = graphic, Margin = new Thickness(4) };
                    textBlock.Tapped += TextBlock_Tapped;
                    DirectionsStackPanel.Children.Add(textBlock);
                    i++;
                }
                MyMap.ZoomTo(_directionsFeatureSet.RouteSummary.Extent.Expand(0.6));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        Direction _directionsFeatureSet;
        Graphic _activeSegmentGraphic;
        private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

            TextBlock textBlock = sender as TextBlock;
            Graphic feature = textBlock.Tag as Graphic;
            MyMap.ZoomTo(feature.Geometry.Extent.Expand(0.6));
            if (_activeSegmentGraphic == null)
            {
                _activeSegmentGraphic = new Graphic() { Symbol = LayoutRoot.Resources["SegmentSymbol"] as ISymbol };
                GraphicsLayer graphicsLayer = MyMap.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
                graphicsLayer.Graphics.Add(_activeSegmentGraphic);
            }
            _activeSegmentGraphic.Geometry = feature.Geometry;
        }

        void StackPanel_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (_directionsFeatureSet != null)
            {
                MyMap.ZoomTo(_directionsFeatureSet.RouteSummary.Extent.Expand(0.6));
            }
        }

        private AddressToLocationsParameter ParseAddress(string address)
        {
            string[] fromArray = address.Split(new char[] { ',' });
            AddressToLocationsParameter fromAddress = new AddressToLocationsParameter();
            fromAddress.OutFields.Add("Loc_name");
            fromAddress.Address.Add("Address", fromArray[0]);
            fromAddress.Address.Add("City", fromArray[1]);
            fromAddress.Address.Add("State", fromArray[2]);
            fromAddress.Address.Add("Zip", fromArray[3]);
            fromAddress.Address.Add("Country", "USA");
            return fromAddress;
        }

        private string FormatDistance(double dist, string units)
        {
            string result = "";
            double formatDistance = Math.Round(dist, 2);
            if (formatDistance != 0)
            {
                result = formatDistance + " " + units;
            }
            return result;
        }

        private string FormatTime(double minutes)
        {
            TimeSpan time = TimeSpan.FromMinutes(minutes);
            string result = "";
            int hours = (int)Math.Floor(time.TotalHours);
            if (hours > 1)
                result = string.Format("{0} hours ", hours);
            else if (hours == 1)
                result = string.Format("{0} hour ", hours);
            if (time.Minutes > 1)
                result += string.Format("{0} minutes ", time.Minutes);
            else if (time.Minutes == 1)
                result += string.Format("{0} minute ", time.Minutes);
            return result;
        }
    }
}
