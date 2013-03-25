using ESRI.ArcGIS.Runtime;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Misc
{
	public sealed partial class SimpleMapTip : Page
	{
		private MapPoint anchor = new ESRI.ArcGIS.Runtime.MapPoint(-117.19568, 34.056601, new SpatialReference(4326));
		
		public SimpleMapTip()
		{
			this.InitializeComponent();
		}
		
		private void MyMap_ExtentChanged(object sender, System.EventArgs e)
		{
			if(MyMap.SpatialReference != null) {
				//Convert anchor point to the spatial reference of the map
				var mp = GeometryEngine.Project(anchor, MyMap.SpatialReference) as MapPoint;
				//Convert anchor point to screen coordinate
				var screen = MyMap.MapToScreen(mp);
				//Note: If MapTip and MyMap has different parents, this screen coordinate might need to be transformed
				if (screen.X >= 0 && screen.Y >= 0 &&
					screen.X < MyMap.ActualWidth && screen.Y < MyMap.ActualHeight)
				{
					//Update location of map
					MapTipTranslate.X = screen.X;
					MapTipTranslate.Y = screen.Y - maptip.ActualHeight;
					maptip.Visibility = Windows.UI.Xaml.Visibility.Visible;
				}
				else //Anchor is outside the display so close maptip
				{
					maptip.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
				}
			}
		}
	}
}
