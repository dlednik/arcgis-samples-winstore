using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Tasks;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteractiveSDK.Samples.Query
{
    public sealed partial class QueryRelatedRecords : Page
    {
        public QueryRelatedRecords()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-10854000, 4502000, -10829000, 4524000, SpatialReference.WebMercator);
        }

        private async Task RunQuery(IGeometry geometry)
        {
            var l = MyMap.Layers["GraphicsWellsLayer"] as GraphicsLayer;
            l.Graphics.Clear();
            QueryTask queryTask =
                new QueryTask(new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Petroleum/KSPetro/MapServer/0"));

            ESRI.ArcGIS.Runtime.Tasks.Query query = new ESRI.ArcGIS.Runtime.Tasks.Query()
             {
                Geometry = geometry,
                ReturnGeometry = true,
                OutSpatialReference = MyMap.SpatialReference,
                OutFields = OutFields.All
             };
            try
            {
                var result = await queryTask.ExecuteAsync(query);
                if (result.FeatureSet.Features != null && result.FeatureSet.Features.Count > 0)
                {
                    ResultsGrid.ItemsSource = result.FeatureSet.Features;
                        l.Graphics.AddRange(from g in result.FeatureSet.Features select g);
                }
            }
            catch (Exception ex)            
            {
                return;
            }
        }

        private async void MyMap_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            //Convert point to mappoint
            var mp = MyMap.ScreenToMap(e.GetPosition(MyMap));
            await RunQuery(Expand(MyMap.Extent, mp, 0.01));
        }

        private Envelope Expand(Envelope mapExtent, MapPoint point, double pct)
        {
            return new Envelope(
                point.X - mapExtent.Width * (pct / 2), point.Y - mapExtent.Height * (pct / 2),
                point.X + mapExtent.Width * (pct / 2), point.Y + mapExtent.Height * (pct / 2))
            {
                SpatialReference = mapExtent.SpatialReference
            };
        }

        private async void ResultsGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                await RunRelationshipQuery(from item in e.AddedItems select Convert.ToInt32((item as Graphic).Attributes["OBJECTID"], CultureInfo.InvariantCulture));
            }
        }

        private async Task RunRelationshipQuery(IEnumerable<int> objectIds)
        {
            QueryTask queryTask =
               new QueryTask(new Uri("http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Petroleum/KSPetro/MapServer/0"));

            //Relationship query
            RelationshipParameter parameters = new RelationshipParameter()
            {
                ObjectIDs = new List<int>(objectIds),
                RelationshipID = 3,
                OutSpatialReference = MyMap.SpatialReference
            };
            parameters.OutFields.AddRange(new string[] { "OBJECTID, API_NUMBER, ELEVATION, FORMATION, TOP" });
            var result = await queryTask.ExecuteRelationshipQueryAsync(parameters);        
                RelationshipResultsGrid.ItemsSource = result.RelatedRecordGroups.FirstOrDefault().Value;
        }
    }
}