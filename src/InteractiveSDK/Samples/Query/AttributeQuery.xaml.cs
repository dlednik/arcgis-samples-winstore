using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Query
{
    public sealed partial class AttributeQuery : Page
    {
        public AttributeQuery()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-15000000, 2000000, -7000000, 8000000);
            InitializeComboBox().ContinueWith((_) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task InitializeComboBox()
        {
            QueryTask queryTask = new QueryTask(new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer/5"));


            ESRI.ArcGIS.Runtime.Tasks.Query query = new ESRI.ArcGIS.Runtime.Tasks.Query()
            {
                ReturnGeometry = false,
                Where = "1=1"
            };
            query.OutFields.Add("STATE_NAME");

            try
            {
                var result = await queryTask.ExecuteAsync(query);
                QueryComboBox.ItemsSource = result.FeatureSet.Features;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async void QueryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await GetAttributes();
        }

        private async Task GetAttributes()
        {
            QueryTask queryTask = new QueryTask(new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer/5"));

            ESRI.ArcGIS.Runtime.Tasks.Query query = new ESRI.ArcGIS.Runtime.Tasks.Query()
            {
                OutFields = OutFields.All,
                ReturnGeometry = true,
                Text = (string)(QueryComboBox.SelectedItem as Graphic).Attributes["STATE_NAME"],
                OutSpatialReference = MyMap.SpatialReference
            };
            try
            {
                ResultsGrid.ItemsSource = null;
                progress.IsActive = true;
                var result = await queryTask.ExecuteAsync(query);
                var featureSet = result.FeatureSet;
                // If an item has been selected            
                GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
                graphicsLayer.Graphics.Clear();

                if (featureSet != null && featureSet.Features.Count > 0)
                {
                    var symbol = LayoutRoot.Resources["DefaultFillSymbol"] as ESRI.ArcGIS.Runtime.Symbology.ISymbol;
                    var g = featureSet.Features[0];
                    graphicsLayer.Graphics.Add(g);
                    var selectedFeatureExtent = g.Geometry.Extent;
                    ESRI.ArcGIS.Runtime.Envelope displayExtent = selectedFeatureExtent.Expand(1.3);
                    MyMap.ZoomTo(displayExtent);
                    ResultsGrid.ItemsSource = g.Attributes;
                }
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                progress.IsActive = false;
            }
        }

        private void KeyLoaded(object sender, object e)
        {
            TextBlock textBlock = (TextBlock)sender;
            dynamic dyn = textBlock.DataContext;
            textBlock.Text = dyn.Key;
        }

        private void ValueLoaded(object sender, object e)
        {
            TextBlock textBlock = (TextBlock)sender;
            dynamic dyn = textBlock.DataContext;
            textBlock.Text = Convert.ToString(dyn.Value, CultureInfo.InvariantCulture);
        }
    }
}