using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Runtime.Symbology;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace InteractiveSDK.Samples.Misc
{
    public sealed partial class SymbolSwatches : Page
    {
        public SymbolSwatches()
        {
            this.InitializeComponent();

        }
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            float dpi = DisplayProperties.LogicalDpi;
            List<ImageSource> swatches = new List<ImageSource>();
            foreach (var symbol in Resources.Values.OfType<ISymbol>())
            {
                if (symbol is IMarkerSymbol)
                    //For markersymbols we don't need to specify a size but can let the symbol decide based on its properties
                    swatches.Add(new SymbolSwatchImageSource(symbol as IMarkerSymbol, dpi));
                else //Create a 50x50px swatch
                    swatches.Add(new SymbolSwatchImageSource(50, 50, symbol, dpi));
            }
            swatchesList.ItemsSource = swatches;
        }
    }
}
