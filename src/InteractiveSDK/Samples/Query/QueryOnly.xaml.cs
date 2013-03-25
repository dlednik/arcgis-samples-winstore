using ESRI.ArcGIS.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Query
{
    public sealed partial class QueryOnly : Page
    {
        public QueryOnly()
        {
            this.InitializeComponent();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Uri myUri = new Uri("http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/3");
            ESRI.ArcGIS.Runtime.Tasks.QueryTask myQueryTask = new ESRI.ArcGIS.Runtime.Tasks.QueryTask(myUri);

            List<string> myListOfString = new List<string>();
            myListOfString.Add("state_name");

            ESRI.ArcGIS.Runtime.Tasks.Query myQuery = new ESRI.ArcGIS.Runtime.Tasks.Query();
            myQuery.GroupByFieldsForStatistics = myListOfString;

            List<ESRI.ArcGIS.Runtime.Tasks.OutStatistic> myListOfOutStatistic =
                new List<ESRI.ArcGIS.Runtime.Tasks.OutStatistic>();

            ESRI.ArcGIS.Runtime.Tasks.OutStatistic myOutStatistic1 = new ESRI.ArcGIS.Runtime.Tasks.OutStatistic();
            myOutStatistic1.OnStatisticField = "pop2000";
            myOutStatistic1.OutStatisticFieldName = "StatePopulation";
            myListOfOutStatistic.Add(myOutStatistic1);

            ESRI.ArcGIS.Runtime.Tasks.OutStatistic myOutStatistic2 = null;
            myOutStatistic2 = new ESRI.ArcGIS.Runtime.Tasks.OutStatistic("state_name", "NumberOfCounties",
                ESRI.ArcGIS.Runtime.Tasks.StatisticType.Count);
            myListOfOutStatistic.Add(myOutStatistic2);

            ESRI.ArcGIS.Runtime.Tasks.OrderByField myOrderByField = new ESRI.ArcGIS.Runtime.Tasks.OrderByField();
            myOrderByField.Field = "state_name";
            myOrderByField.SortOrder = ESRI.ArcGIS.Runtime.Tasks.SortOrder.Descending;
            List<ESRI.ArcGIS.Runtime.Tasks.OrderByField> myOrderByFieldsList =
                new List<ESRI.ArcGIS.Runtime.Tasks.OrderByField>();
            myOrderByFieldsList.Add(myOrderByField);
            myQuery.OrderByFields = myOrderByFieldsList;

            myQuery.OutStatistics = myListOfOutStatistic;

            try
            {
                ESRI.ArcGIS.Runtime.Tasks.QueryResult myQueryResult = await myQueryTask.ExecuteAsync(myQuery);
                if (myQueryResult.FeatureSet.Features != null && myQueryResult.FeatureSet.Features.Count > 0)
                {
                    ESRI.ArcGIS.Runtime.FeatureSet theFeatureSet = myQueryResult.FeatureSet;
                    System.Collections.Generic.IList<ESRI.ArcGIS.Runtime.Graphic> theIReadOnlyListOfgraphic =
                        theFeatureSet.Features;
                    System.Text.StringBuilder theStringBuilder = new System.Text.StringBuilder();
                    foreach (ESRI.ArcGIS.Runtime.Graphic oneGraphic in theIReadOnlyListOfgraphic)
                    {
                        System.Collections.Generic.IDictionary<string, object> theIDictionaryOfString_Object =
                            oneGraphic.Attributes;
                        System.Collections.Generic.ICollection<string> theKeys = theIDictionaryOfString_Object.Keys;
                        foreach (string oneKey in theKeys)
                        {
                            if (theIDictionaryOfString_Object[oneKey] != null)
                            {
                                object theValue = theIDictionaryOfString_Object[oneKey];
                                string theValueAsString = theValue.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private async void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            Button_Click_2(sender, e);
            await RunQuery();
        }

        private async Task RunQuery()
        {
            QueryTask queryTask =
                new QueryTask(new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Demographics/ESRI_Census_USA/MapServer/5"));

            ESRI.ArcGIS.Runtime.Tasks.Query query = new ESRI.ArcGIS.Runtime.Tasks.Query();
            query.Text = StateNameTextBox.Text;

            query.OutFields.Add("*");
            try
            {
                var result = await queryTask.ExecuteAsync(query);
                ResultGrid.ItemsSource = result.FeatureSet.Features;
            }
            catch (TaskCanceledException taskCanceledEx)
            {
                System.Diagnostics.Debug.WriteLine(taskCanceledEx.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}