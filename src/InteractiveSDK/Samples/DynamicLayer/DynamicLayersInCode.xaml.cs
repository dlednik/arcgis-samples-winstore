using System.Collections.Generic;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Symbology;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.DynamicLayer
{
    public sealed partial class DynamicLayersInCode : Page
    {
        public DynamicLayersInCode()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-3548912, -1847469, 2472012, 1742990, new SpatialReference(102009));
        }

        private void ApplyRangeValueClick(object sender, RoutedEventArgs e)
        {
            ClassBreaksRenderer newClassBreaksRenderer = new ClassBreaksRenderer();
            newClassBreaksRenderer.Field = "POP00_SQMI";
            var Infos = new List<ClassBreakInfo>();
            Infos.Add(new ClassBreakInfo()
            {

                Minimum = 0,
                Maximum = 12,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 0)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 31.3,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 100, 255, 100)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 59.7,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 200)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 146.2,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 255)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 57173,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 0, 255)
                }
            });
            newClassBreaksRenderer.Infos = Infos;
            var layer = MyMap.Layers["USA"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Add(new LayerDrawingOptions()
            {
                LayerID = 3,
                Renderer = newClassBreaksRenderer
            });
            layer.VisibleLayers = new int[] { 3 };
        }

        private void ApplyUniqueValueClick(object sender, RoutedEventArgs e)
        {

            UniqueValueRenderer newUniqueValueRenderer = new UniqueValueRenderer()
            {
                DefaultSymbol = new SimpleFillSymbol()
                {
                    Color = Colors.Gray
                },
                Fields = new string[] { "SUB_REGION" }
            };

            var Infos = new List<UniqueValueInfo>();
            Infos.Add(new UniqueValueInfo()
             {
                 Values = new object[] { "Pacific" },
                 Symbol = new SimpleFillSymbol()
                 {
                     Color = Colors.Purple,
                     Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                 }
             });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "W N Cen" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Green,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "W S Cen" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Cyan,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "E N Cen" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Yellow,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "Mtn" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Blue,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "N Eng" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Red,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "E S Cen" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Brown,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "Mid Atl" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Magenta,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });

            Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "S Atl" },
                Symbol = new SimpleFillSymbol()
                {
                    Color = Colors.Orange,
                    Outline = new SimpleLineSymbol() { Color = Colors.Transparent }
                }
            });
            newUniqueValueRenderer.Infos = Infos;
            var layer = MyMap.Layers["USA"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Add(new LayerDrawingOptions()
            {
                LayerID = 2,
                Renderer = newUniqueValueRenderer
            });
            layer.VisibleLayers = new int[] { 2 };
        }

        private void ChangeLayerOrderClick(object sender, RoutedEventArgs e)
        {
            var layer = MyMap.Layers["USA"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Clear();
            layer.DynamicLayerInfos = layer.CreateDynamicLayerInfosFromLayerInfos();
            var aDynamicLayerInfo = layer.DynamicLayerInfos[0];
            layer.DynamicLayerInfos.RemoveAt(0);
            layer.DynamicLayerInfos.Add(aDynamicLayerInfo);
            layer.VisibleLayers = null;
        }

        private void AddLayerClick(object sender, RoutedEventArgs e)
        {
            var layer = MyMap.Layers["USA"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Clear();
            layer.DynamicLayerInfos = layer.CreateDynamicLayerInfosFromLayerInfos();
            layer.DynamicLayerInfos.Insert(0, new DynamicLayerInfo()
            {
                ID = 4,
                Source = new LayerDataSource()
                {
                    DataSource = new TableDataSource()
                    {
                        WorkspaceID = "MyDatabaseWorkspaceIDSSR2",
                        DataSourceName = "ss6.gdb.Lakes"
                    }
                }
            });
            layer.LayerDrawingOptions.Add(new LayerDrawingOptions()
            {
                LayerID = 4,
                Renderer = new SimpleRenderer()
            {
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb((int)255, (int)0, (int)0, (int)255)
                }
            }
            });

            layer.VisibleLayers = new int[] { 3, 4 };
        }
    }
}