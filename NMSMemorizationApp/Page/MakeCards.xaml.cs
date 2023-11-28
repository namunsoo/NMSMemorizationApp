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
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using NMSMemorizationApp.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NMSMemorizationApp.Page
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MakeCards
    {
		private List<CardInfo> cardList = new List<CardInfo>();
		private int cardNum = 0;
		private bool isNewCard = false;

		public MakeCards()
        {
            this.InitializeComponent();
        }

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
			} else
			{
				cardNum--;
				this.cardBar.Value = cardNum + 1;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				CardInfo card = cardList[cardNum];
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
			}
		}

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

		private async void BtnNewCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (isNewCard)
			{
				if (string.IsNullOrEmpty(this.txtQuestion.Text))
				{
					ContentDialog noDataDialog = new ContentDialog
					{
						Title = "������ �����ϴ�.",
						Content = "������ ����ֽ��ϴ�.",
						CloseButtonText = "Ȯ��"
					};

					// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
					noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

					ContentDialogResult result = await noDataDialog.ShowAsync();
				}
				else if (string.IsNullOrEmpty(this.txtAnswer.Text))
				{
					ContentDialog noDataDialog = new ContentDialog
					{
						Title = "������ �����ϴ�.",
						Content = "������ ����ֽ��ϴ�.",
						CloseButtonText = "Ȯ��"
					};

					// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
					noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

					ContentDialogResult result = await noDataDialog.ShowAsync();
				}
				else
				{
					CardInfo card = new CardInfo();
					card.Question = this.txtQuestion.Text.Replace(",", "&#44;");
					card.Answer = this.txtAnswer.Text.Replace(",", "&#44;");
					cardList.Add(card);

					int cardCount = cardList.Count;
					cardNum = cardCount - 1;
					this.cardBar.Value = cardCount;
					this.cardBar.Maximum = cardCount;
					this.txtCardBar.Text = cardCount + " / " + cardCount;

					AddCardSetting(false);
				}
			}
			else
			{
				AddCardSetting(true);
			}
		}

		private async void BtnCloseNewCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txtQuestion.Text) || !string.IsNullOrEmpty(this.txtAnswer.Text))
			{
				ContentDialog deleteDialog = new ContentDialog
				{
					Title = "������ �ۼ����� ī�尡 �ֽ��ϴ�.",
					Content = "�ۼ����� ī��� ������ϴ�. �׷��� ����Ͻðڽ��ϱ�?",
					PrimaryButtonText = "�ۼ� ���",
					CloseButtonText = "�̾ �ۼ��ϱ�"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				deleteDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await deleteDialog.ShowAsync();

				if (result == ContentDialogResult.Primary)
				{
					AddCardSetting(false);
				}
			} else
			{
				AddCardSetting(false);
			}
		}

		private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
		{
			var folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
			folderPicker.FileTypeFilter.Add("*");

			nint windowHandle = WindowNative.GetWindowHandle(App.Window);
			InitializeWithWindow.Initialize(folderPicker, windowHandle);

			StorageFolder folder = await folderPicker.PickSingleFolderAsync();

			if (folder != null)
			{
				// Do something with the file.
			}
		}

		private async void BtnRepair_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardList.Count != 0 && !string.IsNullOrEmpty(this.txtQuestion.Text) && !string.IsNullOrEmpty(this.txtQuestion.Text))
			{
				ContentDialog repairDialog = new ContentDialog
				{
					Title = "���� Ȯ��",
					Content = "���� �Ͻðڽ��ϱ�?",
					PrimaryButtonText = "����",
					CloseButtonText = "���"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				repairDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await repairDialog.ShowAsync();

				if (result == ContentDialogResult.Primary)
				{
					cardList[cardNum].Question = this.txtQuestion.Text.Replace(",", "&#44;");
					cardList[cardNum].Answer = this.txtAnswer.Text.Replace(",", "&#44;");
				}
			} else 
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "������ �� �����ϴ�.",
					Content = cardList.Count != 0 ? "������ ������ ������� �� �����ϴ�." : "���� ī�尡 �����ϴ�.",
					CloseButtonText = "Ȯ��"
				};

				// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}

		private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
		{
			ContentDialog deleteDialog = new ContentDialog
			{
				Title = "���� Ȯ��",
				Content = "���� �Ͻðڽ��ϱ�?",
				PrimaryButtonText = "����",
				CloseButtonText = "���"
			};

			// ���� ������ ��θ� ���������� ������ ������Ƽ ���� �߻�
			deleteDialog.XamlRoot = this.MyPanel.XamlRoot;

			ContentDialogResult result = await deleteDialog.ShowAsync();

			if (result == ContentDialogResult.Primary)
			{
				cardList.RemoveAt(cardNum);
				if (cardNum > 0)
				{
					cardNum--;
				}
				int cardCount = cardList.Count;
				CardInfo card = cardCount != 0 ? cardList[cardNum] : new CardInfo();
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
				this.cardBar.Value = (cardNum + 1);
				this.cardBar.Maximum = cardCount;
				this.txtCardBar.Text = (cardNum +1) + " / " + cardCount;
			}
		}

		private async void BtnLoadFile_OnClick(object sender, RoutedEventArgs e)
		{
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

				if (result == ContentDialogResult.Primary)
				{
					// SCV ���� �ε�
				}
			}
			else
			{
				// SCV ���� �ε�
			}
		}

		public void AddCardSetting(bool isAddCard)
		{
			if (isAddCard)
			{
				isNewCard = isAddCard;
				this.btnCloseNewCard.Visibility = Visibility.Visible;
				this.txtQuestion.Text = string.Empty;
				this.txtAnswer.Text = string.Empty;
				this.btnBeforeCard.IsEnabled = false;
				this.btnNextCard.IsEnabled = false;
				this.btnDelete.IsEnabled = false;
				this.btnRepair.IsEnabled = false;
				this.btnSave.IsEnabled = false;
				this.btnLoadFile.IsEnabled = false;
				this.sbIconNewCard.Symbol = Symbol.Accept;
				this.sbIconNewCard.Foreground = new SolidColorBrush(GetColor("#FF128b44"));
				this.btnNewCardBrush.Color = GetColor("#40128b44");
			} else
			{
				isNewCard = isAddCard;
				this.btnCloseNewCard.Visibility = Visibility.Collapsed;
				CardInfo card = cardList.Count != 0 ? cardList[cardNum] : new CardInfo();
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",");
				this.btnBeforeCard.IsEnabled = true;
				this.btnNextCard.IsEnabled = true;
				this.btnDelete.IsEnabled = true;
				this.btnRepair.IsEnabled = true;
				this.btnSave.IsEnabled = true;
				this.btnLoadFile.IsEnabled = true;
				this.sbIconNewCard.Symbol = Symbol.Add;
				this.sbIconNewCard.Foreground = new SolidColorBrush(GetColor("#FF131210"));
				this.btnNewCardBrush.Color = GetColor("#FFB0A695");
			}
		}

		public Windows.UI.Color GetColor(string hex)
		{
			hex = hex.Replace("#", string.Empty);
			byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
			byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
			byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
			byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
			return Windows.UI.Color.FromArgb(a, r, g, b);
		}
	}
}
