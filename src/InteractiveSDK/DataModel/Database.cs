using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveSDK.DataModel
{
	/// <summary>
	/// Class that maps to the Sqlite sample database
	/// </summary>
	public class SampleData
	{
		public int Id { get; set; }
		public string Group { get; set; }
		public string Name { get; set; }
		public string CsCode { get; set; }
		public string VbCode { get; set; }
		public string Xaml { get; set; }
		public string Description { get; set; }
	}
}
