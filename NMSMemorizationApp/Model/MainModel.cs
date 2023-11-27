using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSMemorizationApp.Model
{
	public class CardInfo
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public int FileRow { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int Memorized { get; set; }
	}
}
