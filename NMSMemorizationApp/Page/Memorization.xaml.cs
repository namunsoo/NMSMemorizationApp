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
using NMSMemorizationApp.Model;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

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
		}

		private List<CardInfo> cardList = new List<CardInfo>();
		private List<CardInfo> cardTemp = new List<CardInfo>();
		private string cardFolder = string.Empty;
		private int cardNum = 0;
		private bool isNewCard = false;


		#region [| ���� ī�� ��ư|]
		private async void BtnBeforeCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardNum <= 0)
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "���� ī�尡 �����ϴ�.",
					Content = "���� ī�尡 1�� ī���Դϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
			else
			{
				cardNum--;
				this.cardBar.Value = cardNum + 1;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				CardInfo card = cardList[cardNum];
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
			}
		}
		#endregion

		#region [| ���� ī�� ��ư|]
		private async void BtnNextCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardNum >= cardList.Count - 1)
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "���� ī�尡 �����ϴ�.",
					Content = "������ ī�� �Դϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
			else
			{
				cardNum++;
				this.cardBar.Value = cardNum + 1;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				CardInfo card = cardList[cardNum];
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
			}
		}
		#endregion

		#region [| ī�� ���� ��ư|]
		private async void BtnRepair_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardList.Count != 0 && !string.IsNullOrEmpty(this.txtQuestion.Text) && !string.IsNullOrEmpty(this.txtQuestion.Text)  && this.sbIconRepair.Symbol == Symbol.Repair)
			{
				this.sbIconRepair.Symbol = Symbol.Accept;
				this.txtQuestion.IsReadOnly = false;
				this.txtAnswer.IsReadOnly = false;
				this.btnCloseRepair.Visibility = Visibility.Visible;
				this.btnBeforeCard.IsEnabled = false;
				this.btnNextCard.IsEnabled = false;
				this.btnMemorizationCompleted.IsEnabled = false;
				this.btnMemorizationFailure.IsEnabled = false;
				this.btnSetting.IsEnabled = false;
				this.btnLoadFile.IsEnabled = false;
			}
			else if(cardList.Count != 0 && !string.IsNullOrEmpty(this.txtQuestion.Text) && !string.IsNullOrEmpty(this.txtQuestion.Text) && this.sbIconRepair.Symbol == Symbol.Accept) 
			{
				this.sbIconRepair.Symbol = Symbol.Repair;
				this.txtQuestion.IsReadOnly = true;
				this.txtAnswer.IsReadOnly = true;
				this.btnCloseRepair.Visibility = Visibility.Collapsed;
				this.btnBeforeCard.IsEnabled = true;
				this.btnNextCard.IsEnabled = true;
				this.btnMemorizationCompleted.IsEnabled = true;
				this.btnMemorizationFailure.IsEnabled = true;
				this.btnSetting.IsEnabled = true;
				this.btnLoadFile.IsEnabled = true;
				UpdateCard(cardList[cardNum].FilePath, cardList[cardNum].FileRow, this.txtQuestion.Text.Replace(",", "&#44;"), this.txtAnswer.Text.Replace(",", "&#44;"), cardList[cardNum].Memorized);

				cardList[cardNum].Question = this.txtQuestion.Text.Replace(",", "&#44;");
				cardList[cardNum].Answer = this.txtAnswer.Text.Replace(",", "&#44;");
			}
			else
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "������ �� �����ϴ�.",
					Content = cardList.Count != 0 ? "������ ������ ������� �� �����ϴ�." : "������ ī�尡 �����ϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| ī�� ���� ��� ��ư|]
		private void BtnCloseRepair_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.btnCloseRepair.Visibility == Visibility.Visible)
			{
				this.sbIconRepair.Symbol = Symbol.Repair;
				this.txtQuestion.IsReadOnly = true;
				this.txtAnswer.IsReadOnly = true;
				this.btnCloseRepair.Visibility = Visibility.Collapsed;
				this.btnBeforeCard.IsEnabled = true;
				this.btnNextCard.IsEnabled = true;
				this.btnMemorizationCompleted.IsEnabled = true;
				this.btnMemorizationFailure.IsEnabled = true;
				this.btnSetting.IsEnabled = true;
				this.btnLoadFile.IsEnabled = true;

				CardInfo card = cardList[cardNum];
				this.cardBar.Value = cardNum + 1;
				this.cardBar.Maximum = cardList.Count;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
			}
		}
		#endregion

		#region [| ���� Ȯ�� ��ư|]
		private void BtnShowAnswer_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.txtAnswer.Visibility == Visibility.Visible)
			{
				this.txtAnswer.Visibility = Visibility.Collapsed;
			} else
			{
				this.txtAnswer.Visibility = Visibility.Visible;
			}
		}
		#endregion

		#region [| ī��(CSV ����) �ҷ����� ��ư|]
		private async void BtnLoadFile_OnClick(object sender, RoutedEventArgs e)
		{
			bool checkOpen = true;
			if (cardList.Count > 0)
			{
				ContentDialog deleteDialog = new ContentDialog
				{
					Title = "������ �ۼ����� ī�尡 �ֽ��ϴ�.",
					Content = "ī�带 �ҷ����� ������ �ۼ����� ī��� ������ϴ�.",
					PrimaryButtonText = "�ҷ�����",
					CloseButtonText = "���"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				deleteDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await deleteDialog.ShowAsync();

				if (result != ContentDialogResult.Primary)
				{
					checkOpen = false;
				}
			}

			try
			{
				if (checkOpen)
				{
					FolderPicker folderPicker = new FolderPicker();
					folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
					folderPicker.FileTypeFilter.Add("*");
					nint windowHandle = WindowNative.GetWindowHandle(App.Window);
					InitializeWithWindow.Initialize(folderPicker, windowHandle);

					StorageFolder folder = await folderPicker.PickSingleFolderAsync();
					if (folder != null)
					{
						DirectoryInfo di = new DirectoryInfo(folder.Path);

						string line = string.Empty;
						string[] property = null;
						cardList = new List<CardInfo>();
						CardInfo card = null;
						int i = 0;
						foreach (FileInfo file in di.GetFiles())
						{
							if (file.Extension.ToLower().CompareTo(".csv") == 0)
							{
								line = string.Empty;
								property = null;
								i = 0;
								using (StreamReader sr = new StreamReader(file.FullName))
								{
									while (!sr.EndOfStream)
									{
										line = sr.ReadLine();
										if (i != 0)
										{
											property = line.Split(',');
											card = new CardInfo(file.FullName, file.Name.Substring(0, file.Name.Length - 4), i, property[0], property[1], Convert.ToInt32(property[2]));
											cardList.Add(card);
										}
										i++;
									}

									cardNum = 0;
									card = cardList[0];
									this.cardBar.Value = cardNum + 1;
									this.cardBar.Maximum = cardList.Count;
									this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
									this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
									this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
								}
							}
						}
					}
				}
			} catch
			{
				cardList = new List<CardInfo>();
				cardNum = 0;
				isNewCard = false;

				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "����",
					Content = "������ �ҷ����� ������ �߻��߽��ϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| �ϱ� �Ϸ� ��ư|]
		private void BtnMemorizationCompleted_OnClick(object sender, RoutedEventArgs e)
		{
			UpdateCard(cardList[cardNum].FilePath, cardList[cardNum].FileRow, this.txtQuestion.Text.Replace(",", "&#44;"), this.txtAnswer.Text.Replace(",", "&#44;"), 1);
			BtnNextCard_OnClick(sender, e);
		}
		#endregion

		#region [| �ϱ� ���� ��ư|]
		private void BtnMemorizationFailure_OnClick(object sender, RoutedEventArgs e)
		{
			UpdateCard(cardList[cardNum].FilePath, cardList[cardNum].FileRow, this.txtQuestion.Text.Replace(",", "&#44;"), this.txtAnswer.Text.Replace(",", "&#44;"), 0);
			BtnNextCard_OnClick(sender, e);
		}
		#endregion

		#region [| ���� ���� ��ư|]
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
		#endregion

		#region [| ���� ���� ��ư|]
		private void MemorizationOption_OnChange(object sender, RoutedEventArgs e)
		{
			int option = ((ComboBox)sender).SelectedIndex;
			switch (option)
			{
				case 0:
					break;
				case 1:
					break;
				case 2:
					break;
				default:
					break;
			}
		}
		#endregion

		private void UpdateCard(string filePath, int fileRow, string question, string answer, int memorized)
		{
			string[] arrLine = File.ReadAllLines(filePath);
			arrLine[fileRow] = question + "," + answer + "," + memorized;
			File.WriteAllLines(filePath, arrLine);
		}
	}

}
