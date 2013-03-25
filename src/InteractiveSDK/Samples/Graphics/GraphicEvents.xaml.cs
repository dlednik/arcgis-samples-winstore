using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Graphics
{
    public sealed partial class GraphicEvents : Page
    {
        public GraphicEvents()
        {
            this.InitializeComponent();
        }

        private void FeatureLayer_PointerMoved(object sender, ESRI.ArcGIS.Runtime.Xaml.GraphicPointerRoutedEventArgs e)
        {
            FeatureLayer layer = sender as FeatureLayer;
            eventTb.Text = "Moved";
            layerTb.Text = layer.DisplayName;
            graphicIdTb.Text = string.Format("{0}", e.Graphic.Attributes[layer.ServiceInfo.ObjectIdField]);
        }

        private void FeatureLayer_PointerEntered(object sender, GraphicPointerRoutedEventArgs e)
        {
            FeatureLayer layer = sender as FeatureLayer;
            eventTb.Text = "Entered";
            layerTb.Text = layer.DisplayName;
            graphicIdTb.Text = string.Format("{0}", e.Graphic.Attributes[layer.ServiceInfo.ObjectIdField]);
        }

        private void FeatureLayer_PointerExited(object sender, GraphicPointerRoutedEventArgs e)
        {
            FeatureLayer layer = sender as FeatureLayer;
            eventTb.Text = "Exited";
            layerTb.Text = layer.DisplayName;
            graphicIdTb.Text = string.Format("{0}", e.Graphic.Attributes[layer.ServiceInfo.ObjectIdField]);
        }

        private void FeatureLayer_Tapped(object sender, GraphicTappedRoutedEventArgs e)
        {
            FeatureLayer layer = sender as FeatureLayer;
            eventTb.Text = "Tapped";
            layerTb.Text = layer.DisplayName;
            graphicIdTb.Text = string.Format("{0}", e.Graphic.Attributes[layer.ServiceInfo.ObjectIdField]);
        }
    }
}
