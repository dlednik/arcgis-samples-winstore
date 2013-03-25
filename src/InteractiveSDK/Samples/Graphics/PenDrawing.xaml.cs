using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Xaml;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteractiveSDK.Samples.Graphics
{
    public sealed partial class PenDrawing : Page
    {
        public PenDrawing()
        {
            this.InitializeComponent();
        }

        private void Map_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            // Convert screen point to mappoint
            var mp = MyMap.ScreenToMap(e.GetPosition(MyMap));

            // Create graphic
            Graphic g = new Graphic() { Geometry = mp };

            // Get layer and add point 
            var layer = MyMap.Layers.OfType<GraphicsLayer>().First();
            layer.Graphics.Add(g);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var layer = MyMap.Layers.OfType<GraphicsLayer>().First();
            layer.Graphics.Clear();
        }

        private uint? currentPointerId;
        Polyline currentPolyline;
        Graphic currentGraphic;
        ManipulationModes manipulationModeCache;

        private void MyMap_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (MyMap.SpatialReference == null || MyMap.Extent == null)
                return;
            if (currentPointerId.HasValue && currentPointerId.Value != e.Pointer.PointerId)
                return;
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                manipulationModeCache = MyMap.ManipulationMode;
                MyMap.ManipulationMode = ManipulationModes.None;
                e.Handled = true;
                currentPointerId = e.Pointer.PointerId;
                currentGraphic = new Graphic();
                currentPolyline = new Polyline() { SpatialReference = MyMap.SpatialReference };
                var p = e.GetCurrentPoint(MyMap).Position;
                currentPolyline.AddPart(new MapPoint[] { MyMap.ScreenToMap(p), MyMap.ScreenToMap(p) });
                currentGraphic.Geometry = currentPolyline;
                MyMap.Layers.OfType<GraphicsLayer>().First().Graphics.Add(currentGraphic);
            }
        }

        private void MyMap_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (currentPointerId.HasValue && currentPointerId.Value != e.Pointer.PointerId || currentPolyline == null)
                return;
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                e.Handled = true;
                var p = e.GetCurrentPoint(MyMap).Position;
                currentPolyline.InsertPoint(0, currentPolyline.GetPointCount(0), MyMap.ScreenToMap(p));
                MyMap.CapturePointer(e.Pointer);
            }
        }
        private void MyMap_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (currentPointerId.HasValue && currentPointerId.Value != e.Pointer.PointerId)
                return;
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                e.Handled = true;
                currentPointerId = null;
                currentGraphic = null;
                currentPolyline = null;
                MyMap.ManipulationMode = manipulationModeCache;
            }
        }
    }
}
