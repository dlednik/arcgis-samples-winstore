using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveSDK.DataModel
{
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
		}
		private static MainPageVM mainpageVm;
		public MainPageVM MainPage
		{
			get
			{
				if (mainpageVm == null)
					mainpageVm = new MainPageVM();
				return mainpageVm;
			}
		}

		private static GroupDetailPageVM groupdetailpageVm;
		public GroupDetailPageVM GroupDetailPage
		{
			get
			{
				if (groupdetailpageVm == null)
					groupdetailpageVm = new GroupDetailPageVM();
				return groupdetailpageVm;
			}
		}
	}
}