using InteractiveSDK.DataModel;

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace InteractiveSDK
{
    public sealed partial class MainPage : InteractiveSDK.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (settings.Values.ContainsKey("LastSampleRan"))
                {
                    var itemId = settings.Values["LastSampleRan"] as string;
                    if (!string.IsNullOrWhiteSpace(itemId))
                    {
                        var item = SampleDataSource.GetItem(itemId);
                        if (item != null)
                        {
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            this.Frame.Navigate(typeof(ItemDetailPage), itemId));
                        }
                    }
                }
            }
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }


        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var group = (Group)e.ClickedItem;
            this.Frame.Navigate(typeof(GroupDetailPage), group);
        }
    }
}
