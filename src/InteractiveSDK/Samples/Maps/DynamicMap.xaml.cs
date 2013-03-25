using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Maps
{
	public sealed partial class DynamicMap : Page
	{
		public DynamicMap()
		{
			this.InitializeComponent();

            MyMap.InitialExtent =
                new ESRI.ArcGIS.Runtime.Envelope(
                    -12387666.9930794, 3775019.32005654,
                    -12309395.4761154, 3818219.62318802, 
                    new ESRI.ArcGIS.Runtime.SpatialReference(102100));
		}
	}
}
