using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSMemorizationApp.Model
{
	public class CardInfo
	{
		public int FileRow { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int Memorized { get; set; }
		public CardInfo(int FileRow = 0, string Question = "", string Anwer = "", int Memorized = 0) 
		{
			this.FileRow = FileRow;
			this.Question = Question;
			this.Answer = Anwer;
			this.Memorized = Memorized;
		}
	}
}
