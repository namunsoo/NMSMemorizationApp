using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMSMemorizationApp.Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Memorization
    {
        public Memorization()
        {
            this.InitializeComponent();
			GetMemoryCardData();
		}

		private void GetMemoryCardData()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo("C:\\Users\\namunsoo\\Downloads\\Test");
			string fileName = string.Empty;
			string filePath = string.Empty;
			try
			{
				if (directoryInfo.Exists)
				{
					//foreach (FileInfo file in directoryInfo.GetFiles())
					//{
					//	if (file.Extension.ToLower().CompareTo(".csv") == 0)
					//	{
					//		fileName = file.Name.Substring(0, file.Name.Length - 5);
					//		filePath = file.FullName;
					//		SetCard(fileName, filePath);
					//	}
					//}
					this.txtAnswer.Text = "폴더 있음";
					this.txtQuestion.Text = "폴더 있음";
				}
				else
				{
					this.txtAnswer.Text = "폴더가 존재하지 않습니다.";
					this.txtQuestion.Text = "폴더가 존재하지 않습니다.";
				}
			}
			catch (Exception ex)
			{
				this.txtAnswer.Text = "데이터 가져오다 예상하지 못한 오류 발생";
				this.txtQuestion.Text = "데이터 가져오다 예상하지 못한 오류 발생";
			}
		}

		private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
		{
			object tag = ((Button)sender).Tag.ToString();
			if (tag.Equals("open"))
			{
				((Button)sender).Tag = "close";
				AniOpenSetting.Begin();
			}
            else
            {
				((Button)sender).Tag = "open";
				AniCloseSetting.Begin();
			}
        }
	}

}
