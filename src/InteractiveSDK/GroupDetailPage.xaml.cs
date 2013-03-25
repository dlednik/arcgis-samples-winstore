using InteractiveSDK.DataModel;

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace InteractiveSDK
{
    public sealed partial class GroupDetailPage : InteractiveSDK.Common.LayoutAwarePage
    {
        public GroupDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
			var group = navigationParameter as Group;
			((LayoutRoot.DataContext) as DataModel.GroupDetailPageVM).Title = group.Title;
			((LayoutRoot.DataContext) as DataModel.GroupDetailPageVM).UniqueId = group.UniqueId;
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((Sample)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }
    }
}
