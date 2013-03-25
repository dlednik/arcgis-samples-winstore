using ESRI.ArcGIS.Runtime;
using ESRI.ArcGIS.Runtime.Xaml;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK.Samples.Maps
{
	public sealed partial class OverviewMap : Page
	{
		public OverviewMap()
		{
			this.InitializeComponent();          
  
            MyMap.InitialExtent =
                GeometryEngine.Project(new ESRI.ArcGIS.Runtime.Envelope(
                    -5, 20, 50, 65, SpatialReference.Wgs84), SpatialReference.WebMercator) as Envelope;
		}

		private void MyMap_ExtentChanged(object sender, System.EventArgs e)
		{
			var graphicslayer = overviewMap.Layers.OfType<GraphicsLayer>().First();
			Graphic g = graphicslayer.Graphics.FirstOrDefault();
			if (g == null) //first time
			{
				g = new Graphic();
				graphicslayer.Graphics.Add(g);
			}
			g.Geometry = MyMap.Extent;
		}
	}
}
