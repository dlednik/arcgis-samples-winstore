using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Symbology;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Query
{
    public sealed partial class QueryWithBuffer : Page
    {
        public QueryWithBuffer()
        {
            this.InitializeComponent();

            MyMap.InitialExtent = new Envelope(-9270434.248, 5246977.326, -9269261.417, 5247569.712);
            InitializePMS().ContinueWith((_) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task InitializePMS()
        {
            try
            {
                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/i_pushpin.png"));
                var imageSource = await imageFile.OpenReadAsync();
                var pms = LayoutRoot.Resources["DefaultMarkerSymbol"] as PictureMarkerSymbol;
                await pms.SetSourceAsync(imageSource);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async void MyMap_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var mp = MyMap.ScreenToMap(e.GetPosition(MyMap));
            Graphic g = new Graphic() { Geometry = mp };
            var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.Graphics.Add(g);

            var bufferResult = GeometryEngine.Buffer(mp, 100);
            var bufferLayer = MyMap.Layers["BufferLayer"] as GraphicsLayer;
            bufferLayer.Graphics.Add(new Graphic() { Geometry = bufferResult });


            var queryTask = new QueryTask(new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/BloomfieldHillsMichigan/Parcels/MapServer/2"));
            var query = new ESRI.ArcGIS.Runtime.Tasks.Query()
            {
                ReturnGeometry = true,
                OutSpatialReference = MyMap.SpatialReference,
                Geometry = bufferResult
            };
            query.OutFields.Add("OWNERNME1");

            try
            {
                var queryResult = await queryTask.ExecuteAsync(query);
                if (queryResult != null && queryResult.FeatureSet != null)
                {
                    var resultLayer = MyMap.Layers["MyResultsGraphicsLayer"] as GraphicsLayer;
                    resultLayer.Graphics.AddRange(queryResult.FeatureSet.Features);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}