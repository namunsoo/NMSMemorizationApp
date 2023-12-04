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
using System.Text;

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

		#region [| 이전 카드 버튼|]
		private async void BtnBeforeCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardNum <= 0)
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "이전 카드가 없습니다.",
					Content = "지금 카드가 1번 카드입니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			} else
			{
				cardNum--;
				this.cardBar.Value = cardNum + 1;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				CardInfo card = cardList[cardNum];
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
			}
		}
		#endregion

		#region [| 다음 카드 버튼|]
		private async void BtnNextCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardNum >= cardList.Count - 1)
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "다음 카드가 없습니다.",
					Content = "마지막 카드 입니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
			else
			{
				cardNum++;
				this.cardBar.Value = cardNum + 1;
				this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
				CardInfo card = cardList[cardNum];
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
			}
		}
		#endregion

		#region [| 새로운 카드 추가 버튼|]
		private async void BtnNewCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (isNewCard)
			{
				if (string.IsNullOrEmpty(this.txtQuestion.Text))
				{
					ContentDialog noDataDialog = new ContentDialog
					{
						Title = "내용이 없습니다.",
						Content = "문제가 비어있습니다.",
						CloseButtonText = "확인"
					};

					// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
					noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

					ContentDialogResult result = await noDataDialog.ShowAsync();
				}
				else if (string.IsNullOrEmpty(this.txtAnswer.Text))
				{
					ContentDialog noDataDialog = new ContentDialog
					{
						Title = "내용이 없습니다.",
						Content = "정답이 비어있습니다.",
						CloseButtonText = "확인"
					};

					// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
					noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

					ContentDialogResult result = await noDataDialog.ShowAsync();
				}
				else
				{
					CardInfo card = new CardInfo();
					card.Question = this.txtQuestion.Text.Replace(",", "&#44;").Replace("\r", "&#92;r");
					card.Answer = this.txtAnswer.Text.Replace(",", "&#44;").Replace("\r", "&#92;r");
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
		#endregion

		#region [| 새로운 카드 추가 취소 버튼|]
		private async void BtnCloseNewCard_OnClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txtQuestion.Text) || !string.IsNullOrEmpty(this.txtAnswer.Text))
			{
				ContentDialog deleteDialog = new ContentDialog
				{
					Title = "기존에 작성중인 카드가 있습니다.",
					Content = "작성중인 카드는 사라집니다. 그래도 취소하시겠습니까?",
					PrimaryButtonText = "작성 취소",
					CloseButtonText = "이어서 작성하기"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
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
		#endregion

		#region [| 카드 저장 버튼|]
		private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardList.Count != 0)
			{
				var savePicker = new Windows.Storage.Pickers.FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
				// 파일 저장 방식 설정
				savePicker.FileTypeChoices.Add("csv파일", new List<string>() { ".csv" }); // .csv 파일로 저장
				// 기본으로 설정시킬 파일 이름
				savePicker.SuggestedFileName = "NewCard";
				nint windowHandle = WindowNative.GetWindowHandle(App.Window);
				InitializeWithWindow.Initialize(savePicker, windowHandle);
				Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
				if (file != null)
				{
					List<string> cardCsv = new List<string>();
					string line = "question,answer,isMemoized";
					cardCsv.Add(line);
					for (int i = 0; i < cardList.Count; i++)
					{
						line = cardList[i].Question + "," + cardList[i].Answer + "," + "0";
						cardCsv.Add(line);
					}
					// Prevent updates to the remote version of the file until
					// we finish making changes and call CompleteUpdatesAsync.
					Windows.Storage.CachedFileManager.DeferUpdates(file);

					// 파일 내용 작성
					await Windows.Storage.FileIO.WriteLinesAsync(file, cardCsv);

					// Let Windows know that we're finished changing the file so
					// the other app can update the remote version of the file.
					// Completing updates may require Windows to ask for user input.
					Windows.Storage.Provider.FileUpdateStatus status =
						await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
					if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
					{
						ContentDialog noDataDialog = new ContentDialog
						{
							Title = "카드 저장 완료",
							Content = "카드가 정상적으로 저장되었습니다.",
							CloseButtonText = "확인"
						};

						// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
						noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

						ContentDialogResult result = await noDataDialog.ShowAsync();
					}
					else
					{
						ContentDialog noDataDialog = new ContentDialog
						{
							Title = "카드 저장 실패",
							Content = "카드 저장에 실패했습니다.",
							CloseButtonText = "확인"
						};

						// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
						noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

						ContentDialogResult result = await noDataDialog.ShowAsync();
					}
				} 
			}
			else {
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "카드 저장 실패",
					Content = "저장할 카드가 없습니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| 카드 수정 버튼|]
		private async void BtnRepair_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardList.Count != 0 && !string.IsNullOrEmpty(this.txtQuestion.Text) && !string.IsNullOrEmpty(this.txtQuestion.Text))
			{
				ContentDialog repairDialog = new ContentDialog
				{
					Title = "수정 확인",
					Content = "수정 하시겠습니까?",
					PrimaryButtonText = "수정",
					CloseButtonText = "취소"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				repairDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await repairDialog.ShowAsync();

				if (result == ContentDialogResult.Primary)
				{
					cardList[cardNum].Question = this.txtQuestion.Text.Replace(",", "&#44;").Replace("\r", "&#92;r");
					cardList[cardNum].Answer = this.txtAnswer.Text.Replace(",", "&#44;").Replace("\r", "&#92;r");
				}
			} else 
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "수정할 수 없습니다.",
					Content = cardList.Count != 0 ? "문제나 정답은 비어있을 수 없습니다." : "수정할 카드가 없습니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| 카드 삭제 버튼|]
		private async void BtnDelete_OnClick(object sender, RoutedEventArgs e)
		{
			if (cardList.Count != 0)
			{
				ContentDialog deleteDialog = new ContentDialog
				{
					Title = "삭제 확인",
					Content = "삭제 하시겠습니까?",
					PrimaryButtonText = "삭제",
					CloseButtonText = "취소"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
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
					this.txtQuestion.Text = card.Question.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
					this.txtAnswer.Text = card.Answer.Replace("&#44;", ",").Replace("&#92;r", "\r"); ;
					this.cardBar.Value = (cardNum + 1);
					this.cardBar.Maximum = cardCount;
					this.txtCardBar.Text = (cardNum + 1) + " / " + cardCount;
				}
			} 
			else
			{
				ContentDialog noDataDialog = new ContentDialog
				{
					Title = "수정할 수 없습니다.",
					Content = "카드가 없습니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

		#region [| 카드(CSV 파일) 불러오기 버튼|]
		private async void BtnLoadFile_OnClick(object sender, RoutedEventArgs e)
		{
			bool checkOpen = true;
			if (cardList.Count > 0)
			{
				ContentDialog deleteDialog = new ContentDialog
				{
					Title = "기존에 작성중인 카드가 있습니다.",
					Content = "카드를 불러오면 기존에 작성중인 카드는 사라집니다.",
					PrimaryButtonText = "불러오기",
					CloseButtonText = "취소"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				deleteDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await deleteDialog.ShowAsync();

				if (result != ContentDialogResult.Primary)
				{
					checkOpen = false;
				}
			}

			try {
				if (checkOpen)
				{
					FileOpenPicker fileOpenPicker = new FileOpenPicker();
					fileOpenPicker.FileTypeFilter.Add(".csv");
					nint windowHandle = WindowNative.GetWindowHandle(App.Window);
					InitializeWithWindow.Initialize(fileOpenPicker, windowHandle);

					StorageFile file = await fileOpenPicker.PickSingleFileAsync();
					if (file != null)
					{
						using (StreamReader sr = new StreamReader(file.Path))
						{
							string line = string.Empty;
							string[] property = null;
							cardList = new List<CardInfo>();
							CardInfo card = null;
							int i = 0;
							while (!sr.EndOfStream)
							{
								line = sr.ReadLine();
								if (i != 0)
								{
									property = line.Split(',');
									card = new CardInfo(file.Path, file.Name.Substring(0, file.Name.Length - 4), i, property[0], property[1], Convert.ToInt32(property[2]));
									cardList.Add(card);
								}
								i++;
							}

							cardNum = 0;
							card = cardList[0];
							this.cardBar.Value = cardNum + 1;
							this.cardBar.Maximum = cardList.Count;
							this.txtCardBar.Text = (cardNum + 1) + " / " + cardList.Count;
							this.txtQuestion.Text = card.Question.Replace("&#44;", ",").Replace("&#92;r", "\r");
							this.txtAnswer.Text = card.Answer.Replace("&#44;", ",").Replace("&#92;r", "\r");
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
					Title = "오류",
					Content = "파일을 불러오다 오류가 발생했습니다.",
					CloseButtonText = "확인"
				};

				// 따로 오픈할 경로를 지정해주지 않으면 프로퍼티 오류 발생
				noDataDialog.XamlRoot = this.MyPanel.XamlRoot;

				ContentDialogResult result = await noDataDialog.ShowAsync();
			}
		}
		#endregion

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
				this.txtQuestion.Text = card.Question.Replace("&#44;", ",").Replace("&#92;r", "\r");
				this.txtAnswer.Text = card.Answer.Replace("&#44;", ",").Replace("&#92;r", "\r");
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
