using ESRI.ArcGIS.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InteractiveSDK.Samples.Query
{
    public sealed partial class Statistics : Page
    {
        public Statistics()
        {
            this.InitializeComponent();

            RunQuery().ContinueWith((_) => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task RunQuery()
        {
            QueryTask queryTask =
                new QueryTask(new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/2"));

            ESRI.ArcGIS.Runtime.Tasks.Query query = new ESRI.ArcGIS.Runtime.Tasks.Query()
             {
                 GroupByFieldsForStatistics = new List<string> { "sub_region" },
                 OutStatistics = new List<OutStatistic> { 
                    new OutStatistic(){
                        OnStatisticField = "pop2000",
                        OutStatisticFieldName = "subregionpopulation",
                        StatisticType = StatisticType.Sum
                    },
                    new OutStatistic(){
                        OnStatisticField = "sub_region",
                        OutStatisticFieldName = "numberofstates",
                        StatisticType = StatisticType.Count
                    }
                 }
             };
            try
            {
                var result = await queryTask.ExecuteAsync(query);
                if (result.FeatureSet.Features != null && result.FeatureSet.Features.Count > 0)
                {
                    ResultGrid.ItemsSource = result.FeatureSet.Features;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}