using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSMemorizationApp.Model
{
	public class CardInfo
	{
		public string FilePath { get; set; }
		public string FileName { get; set; }
		public int FileRow { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
		public int Memorized { get; set; }
		public CardInfo(string FilePath = "", string FileName = "", int FileRow = 0, string Question = "", string Answer = "", int Memorized = 0) 
		{
			this.FilePath = FilePath;
			this.FileName = FileName;
			this.FileRow = FileRow;
			this.Question = Question;
			this.Answer = Answer;
			this.Memorized = Memorized;
		}
	}
}
