using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace InteractiveSDK.Samples.Maps
{
	public sealed partial class EnableTouchRotation : Page
	{
		public EnableTouchRotation()
		{
			this.InitializeComponent();
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			if(MyMap == null)
				return;
			var isOn = (sender as ToggleSwitch).IsOn;
			if (isOn)
				MyMap.ClearValue(UIElement.ManipulationModeProperty); //Set back to default setting
			else
				MyMap.ManipulationMode = ManipulationModes.All; //Enable all manipulation modes including rotation
		}
	}
}
