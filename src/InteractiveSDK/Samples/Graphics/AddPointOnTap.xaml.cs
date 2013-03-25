using System.Linq;
using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteractiveSDK.Samples.Graphics
{
    public sealed partial class AddPointOnTap : Page
    {
        public AddPointOnTap()
        {
            this.InitializeComponent();
        }

        private void Map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            // Convert screen point to map point
            var mapPoint = MyMap.ScreenToMap(e.GetPosition(MyMap));

            // Create graphic
            Graphic g = new Graphic() { Geometry = mapPoint };

            // Get layer and add point to it
            var graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.Graphics.Add(g);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var layer = MyMap.Layers.OfType<GraphicsLayer>().First();
            layer.Graphics.Clear();
        }
    }
}
