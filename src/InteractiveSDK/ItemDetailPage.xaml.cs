using InteractiveSDK.DataModel;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace InteractiveSDK
{
	public sealed partial class ItemDetailPage : InteractiveSDK.Common.LayoutAwarePage
	{
		public ItemDetailPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			base.OnNavigatingFrom(e);
			Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
			settings.Values.Remove("LastSampleRan");
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var item = SampleDataSource.GetItem((string)e.Parameter);
			if (item == null)
			{
				await new Windows.UI.Popups.MessageDialog("Sample not found").ShowAsync();
				if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
			}
			else
			{
				SampleFrame.Navigate(item.SamplePage);
				DataContext = item; 
				Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;
				settings.Values["LastSampleRan"] = item.UniqueId;
				DescriptionView.Text = item.Description;
				Task loadcode = LoadSampleCode(item.SamplePage.FullName);
			}
		}

		private async Task LoadSampleCode(string fullname)
		{
			var search = await Task.Run(() =>
			{
				var nameparts = fullname.Split(new char[] { '.' });
				var group = nameparts[2];
				var name = nameparts[3];
				string dbPath = "Data\\samples.db";
				using (var db = new SQLite.SQLiteConnection(dbPath))
				{
					return db.Table<SampleData>().Where(s => s.Name == name && s.Group == group).FirstOrDefault();
				}
			});
			if (search != null)
			{
				XamlView.Text = search.Xaml;
				CodeView.Text = search.CsCode;
				if(!string.IsNullOrEmpty(search.Description))
					DescriptionView.Text = search.Description;
			}
		}
	}
}
