using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Symbology;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Graphics
{
    public sealed partial class RendereringInCode : Page
    {       
        public RendereringInCode()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(-15000000, 2000000, -7000000, 8000000);
            MyMap.Loaded += MyMap_Loaded;
            
        }
        void MyMap_Loaded(object sender, RoutedEventArgs e)
        {

            SimpleRenderer simpleRenderer = new SimpleRenderer()
            {
                 Description = "Rivers",
                Label = "Rivers",
                Symbol = new SimpleLineSymbol() { Color = Colors.Blue, Style = SimpleLineStyle.Dash, Width = 2 }
            };
            (MyMap.Layers["MyFeatureLayerSimple"] as FeatureLayer).Renderer = simpleRenderer;
            
            UniqueValueRenderer uvr = new UniqueValueRenderer();
            
            uvr.Fields = new string[] { "STATE_NAME" };
            uvr.Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "New Mexico" },
                Symbol = new SimpleFillSymbol() { Color = Colors.Yellow }
            });

            uvr.Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "Texas" },
                Symbol = new SimpleFillSymbol() { Color = Colors.PaleGreen }
            });

            uvr.Infos.Add(new UniqueValueInfo()
            {
                Values = new object[] { "Arizona" },
                Symbol = new SimpleFillSymbol() { Color = Colors.YellowGreen }
            });
            (MyMap.Layers["MyFeatureLayerUnique"] as FeatureLayer).Renderer = uvr;


            ClassBreaksRenderer CBR = new ClassBreaksRenderer()
            {
                DefaultLabel = "All Other Values",
                DefaultSymbol = new SimpleMarkerSymbol() { Color = Colors.Black, Style = SimpleMarkerStyle.Cross, Size = 10 },
                Field = "POP1990",
                Minimum = 0
            };

            CBR.Infos.Add(new ClassBreakInfo()
            {
                Maximum = 30000,
                Label = "0-30000",
                Description = "Pop between 0 and 30000",
                Symbol = new SimpleMarkerSymbol() { Color = Colors.Yellow, Size = 8, Style = SimpleMarkerStyle.Circle }
            });
            CBR.Infos.Add(new ClassBreakInfo()
            {
                Maximum = 300000,
                Label = "30000-300000",
                Description = "Pop between 30000 and 300000",
                Symbol = new SimpleMarkerSymbol() { Color = Colors.Red, Size = 10, Style = SimpleMarkerStyle.Circle }
            });

            CBR.Infos.Add(new ClassBreakInfo()
            {
                Maximum = 5000000,
                Label = "300000-5000000",
                Description = "Pop between 300000 and 5000000",
                Symbol = new SimpleMarkerSymbol() { Color = Colors.Orange, Size = 12, Style = SimpleMarkerStyle.Circle }
            });
            (MyMap.Layers["MyFeatureLayerClassBreak"] as FeatureLayer).Renderer = CBR;

        }
    }
}
