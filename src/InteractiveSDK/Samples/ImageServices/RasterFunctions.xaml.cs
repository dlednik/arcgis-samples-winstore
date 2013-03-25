using System.Threading.Tasks;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.ArcGISServices;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.ImageServices
{
    public sealed partial class RasterFunctions : Page
    {
        ArcGISImageServiceLayer image_layer = null;

        public RasterFunctions()
        {
            this.InitializeComponent();
            MyMap.InitialExtent = new Envelope(1445440, 540657, 1452348, 544407, new SpatialReference(2264));
        }      

        private void RasterFunctionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArcGISImageServiceLayer imageLayer = MyMap.Layers["MyImageLayer"] as ArcGISImageServiceLayer;
            var rasterFunction = (sender as ComboBox).SelectedItem as RasterFunctionInfo;
            if (rasterFunction != null)
            {
                RenderingRule renderingRule = new RenderingRule() { RasterFunctionName = rasterFunction.FunctionName };
                imageLayer.RenderingRule = renderingRule;
            }
        }

        private void ArcGISImageServiceLayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInitialized")
            {
                ArcGISImageServiceLayer imageLayer = MyMap.Layers["MyImageLayer"] as ArcGISImageServiceLayer;
                if (imageLayer.IsInitialized)
                {
                    RasterFunctionsComboBox.ItemsSource = imageLayer.ServiceInfo.RasterFunctionInfos;
                    RasterFunctionsComboBox.SelectedIndex = 0;
                }
            }
        }
    }
}
