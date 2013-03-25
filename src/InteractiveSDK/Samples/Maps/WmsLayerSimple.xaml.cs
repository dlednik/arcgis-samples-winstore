using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Maps
{
	public sealed partial class WmsLayerSimple : Page
	{
		public WmsLayerSimple()
		{
			this.InitializeComponent();
			MyMap.InitialExtent = new ESRI.ArcGIS.Runtime.Envelope(-15000000, 2000000, -7000000, 8000000);
		}
	}
}
