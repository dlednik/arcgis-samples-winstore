using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Maps
{
	public sealed partial class LargeFeatureLayers : Page
	{
		public LargeFeatureLayers()
		{
			this.InitializeComponent();
            
            MyMap.InitialExtent =
                new ESRI.ArcGIS.Runtime.Envelope(
                    -16671654.3509, 3323634.5475,
                    -8652337.0746, 8764783.7309,
                    new ESRI.ArcGIS.Runtime.SpatialReference(102100));
		}
	}
}
