using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using KrnlUI.Controls;
using Microsoft.Win32;

namespace KrnlUI
{
	// Token: 0x02000006 RID: 6
	public partial class MainWindow : Window
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002194 File Offset: 0x00000394
		// (set) Token: 0x06000010 RID: 16 RVA: 0x0000219C File Offset: 0x0000039C
		public ChromiumWebBrowser browser { get; set; }

		// Token: 0x06000011 RID: 17 RVA: 0x000021A8 File Offset: 0x000003A8
		public void WriteScript(string script, bool tabPrompt)
		{
			this.TabChanging = tabPrompt;
			if (this.browser.IsLoaded)
			{
				WebBrowserExtensions.EvaluateScriptAsync(this.browser, "SetText(`" + script.Replace("`", "\\`").Replace("\\", "\\\\") + "`)", null, false);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002210 File Offset: 0x00000410
		public string ReadScript()
		{
			if (!this.browser.IsLoaded)
			{
				return "";
			}
			string text = WebBrowserExtensions.EvaluateScriptAsync(this.browser, "(function() { return GetText() })();", null, false).GetAwaiter().GetResult().Result.ToString();
			if (text != null)
			{
				return text;
			}
			return "";
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000226C File Offset: 0x0000046C
		public MainWindow()
		{
			this.InitializeComponent();
			this.MainMenu.Visibility = Visibility.Hidden;
			this.InitBrowser();
			this.Initialize();
			this.InitHotkeys();
			this.LoadCommunity();
			this.MainDirDisplay.isFile = true;
			this.MainDirDisplay.Path = "Scripts";
			this.MainMenu.Margin = new Thickness(0.0, 37.0, 0.0, 0.0);
			this.InitRecents();
			this.ActivateRecent();
			this.AutoAttach();
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
			if (registryKey.OpenSubKey("Krnl") == null)
			{
				RegistryKey registryKey2 = registryKey.CreateSubKey("Krnl", true);
				registryKey2.SetValue("AutoAttach", false);
				registryKey2.SetValue("AutoLaunch", false);
				registryKey2.SetValue("Topmost", false);
				registryKey2.SetValue("UnlockFPS", false);
				registryKey2.SetValue("EditorMinimap", false);
			}
			RegistryKey krnlSubkey = this.getKrnlSubkey();
			object value = krnlSubkey.GetValue("Topmost");
			object value2 = krnlSubkey.GetValue("AutoAttach");
			object value3 = krnlSubkey.GetValue("UnlockFPS");
			object value4 = krnlSubkey.GetValue("EditorMinimap");
			if (value != null && value.ToString() == "true")
			{
				this.TopmostOpt_MouseLeftButtonUp(null, null);
			}
			if (value2 != null && value2.ToString() == "true")
			{
				this.AutoAttachOpt_MouseLeftButtonUp(null, null);
			}
			if (value3 != null && value3.ToString() == "true")
			{
				this.UnlockFPSOpt_MouseLeftButtonUp(null, null);
			}
			if (value4 != null && value4.ToString() == "true")
			{
				this.MinimapOpt_MouseLeftButtonUp(null, null);
			}
			foreach (Process process in Process.GetProcessesByName("KrnlUI"))
			{
				if (Process.GetCurrentProcess().Id != process.Id)
				{
					process.Kill();
				}
			}
			this.BringDownMenu();
			this.MenuDown = true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002510 File Offset: 0x00000710
		private void InitTabs()
		{
			if (!Directory.Exists("Data"))
			{
				Directory.CreateDirectory("Data");
				return;
			}
			if (!Directory.Exists("Data\\SavedTabs"))
			{
				Directory.CreateDirectory("Data\\SavedTabs");
				return;
			}
			if (File.Exists("Data\\SavedTabs\\tabs.config"))
			{
				foreach (string str in File.ReadAllLines("Data\\SavedTabs\\tabs.config"))
				{
					string text = "Data\\SavedTabs\\" + str;
					if (Directory.Exists(text))
					{
						this.CreateTab(System.IO.Path.GetFileName(text), File.ReadAllText(text + "\\script.lua"), File.ReadAllText(text + "\\tab.config"));
						Directory.Delete(text, true);
					}
				}
				File.Delete("Data\\SavedTabs\\tabs.config");
			}
			if (this.TabFlow.Children.Count == 1)
			{
				this.CreateTab("Untitled", "", "");
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000025F4 File Offset: 0x000007F4
		private void InitHotkeys()
		{
			RoutedCommand routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
			RoutedCommand routedCommand2 = new RoutedCommand();
			routedCommand2.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
			RoutedCommand routedCommand3 = new RoutedCommand();
			routedCommand3.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
			RoutedCommand routedCommand4 = new RoutedCommand();
			routedCommand4.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
			base.CommandBindings.Add(new CommandBinding(routedCommand, new ExecutedRoutedEventHandler(this.CreateTabHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand2, new ExecutedRoutedEventHandler(this.OpenHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand3, new ExecutedRoutedEventHandler(this.SaveHotkey)));
			base.CommandBindings.Add(new CommandBinding(routedCommand4, new ExecutedRoutedEventHandler(this.SaveAsHotkey)));
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000026E1 File Offset: 0x000008E1
		private void CreateTabHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.CreateTab("", "", "").Select();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026FD File Offset: 0x000008FD
		private void OpenHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptOpenFile();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002705 File Offset: 0x00000905
		private void SaveHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptSaveFile();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000270D File Offset: 0x0000090D
		private void SaveAsHotkey(object sender, ExecutedRoutedEventArgs e)
		{
			this.PromptSaveAsFile();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002718 File Offset: 0x00000918
		private void AutoAttach()
		{
			MainWindow.<AutoAttach>d__21 <AutoAttach>d__;
			<AutoAttach>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AutoAttach>d__.<>4__this = this;
			<AutoAttach>d__.<>1__state = -1;
			<AutoAttach>d__.<>t__builder.Start<MainWindow.<AutoAttach>d__21>(ref <AutoAttach>d__);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002750 File Offset: 0x00000950
		private void LoadCommunity()
		{
			if (Directory.Exists("Community"))
			{
				foreach (string text in Directory.GetDirectories("Community"))
				{
					string str = AppDomain.CurrentDomain.BaseDirectory + "\\" + text;
					CommunityEntry communityEntry = new CommunityEntry();
					communityEntry.EntryPreview.Source = new BitmapImage(new Uri(str + "\\preview.png"));
					communityEntry.EntryCreatorIcon.ImageSource = new BitmapImage(new Uri(str + "\\profile.png"));
					communityEntry.EntryName.Content = text.Split(new char[]
					{
						'\\'
					})[1];
					communityEntry.EntryCreator.Content = File.ReadAllText(str + "\\card.config");
					communityEntry.Script = File.ReadAllText(str + "\\script.lua");
					if (File.Exists(str + "\\tags.config"))
					{
						communityEntry.Tags = File.ReadAllText(str + "\\tags.config").Split(new char[]
						{
							','
						}).ToList<string>();
					}
					communityEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
					communityEntry.RunBorder.MouseLeftButtonUp += this.CommunityCard_MouseLeftButtonUp;
					communityEntry.Width = 242.0;
					communityEntry.Height = 185.0;
					this.CommunityCards.Add(communityEntry);
				}
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000028D8 File Offset: 0x00000AD8
		private void Initialize()
		{
			if (!Directory.Exists("Scripts"))
			{
				Directory.CreateDirectory("Scripts");
			}
			if (!Directory.Exists("Recent"))
			{
				Directory.CreateDirectory("Recent");
			}
			else
			{
				new DirectoryInfo("Recent").Delete(true);
				Directory.CreateDirectory("Recent");
			}
			this.Introduct.Content = "Welcome " + Environment.UserName + "!";
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002950 File Offset: 0x00000B50
		private void InitBrowser()
		{
			CefSettings cefSettings = new CefSettings();
			cefSettings.SetOffScreenRenderingBestPerformanceArgs();
			cefSettings.MultiThreadedMessageLoop = true;
			cefSettings.DisableGpuAcceleration();
			Cef.Initialize(cefSettings);
			string currentDirectory = Directory.GetCurrentDirectory();
			this.browser = new ChromiumWebBrowser(currentDirectory + "\\Monaco\\Monaco.html");
			this.browser.BrowserSettings.WindowlessFrameRate = 144;
			this.browser.IsBrowserInitializedChanged += this.Browser_IsBrowserInitializedChanged;
			this.browser.JavascriptMessageReceived += this.Browser_JavascriptMessageReceived;
			this.browser.AllowDrop = false;
			this.browser.BrowserSettings.JavascriptDomPaste = 1;
			this.browser.BrowserSettings.JavascriptAccessClipboard = 1;
			this.Editor.Children.Add(this.browser);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002A20 File Offset: 0x00000C20
		private void Browser_JavascriptMessageReceived(object sender, JavascriptMessageReceivedEventArgs e)
		{
			if (e.Message is string)
			{
				string a = e.Message as string;
				if (a == "save_tabs")
				{
					base.Dispatcher.Invoke(new Action(this.SaveTabs));
				}
				if (a == "init")
				{
					base.Dispatcher.Invoke(new Action(this.InitTabs));
				}
				if (a == "keydown")
				{
					base.Dispatcher.Invoke(delegate()
					{
						if (Common.SelectedTab != null)
						{
							Common.SelectedTab.IsSaved = false;
						}
					});
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002AC6 File Offset: 0x00000CC6
		private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002AC8 File Offset: 0x00000CC8
		private void Button_MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002ACA File Offset: 0x00000CCA
		private void ExitButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Hide();
			this.SaveTabs();
			this.disable_auto_launch();
			Cef.Shutdown();
			Environment.Exit(1);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002AE9 File Offset: 0x00000CE9
		private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002AF4 File Offset: 0x00000CF4
		private void MaximizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (base.WindowState == WindowState.Maximized)
			{
				base.WindowState = WindowState.Normal;
				this.svg356.Visibility = Visibility.Visible;
				this.svg223.Visibility = Visibility.Hidden;
				base.BorderThickness = new Thickness(0.0);
				return;
			}
			base.WindowState = WindowState.Maximized;
			this.svg356.Visibility = Visibility.Hidden;
			this.svg223.Visibility = Visibility.Visible;
			base.BorderThickness = new Thickness(7.0);
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00002B71 File Offset: 0x00000D71
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002B79 File Offset: 0x00000D79
		private Grid selectedTab { get; set; }

		// Token: 0x06000026 RID: 38 RVA: 0x00002B82 File Offset: 0x00000D82
		private void NewTabButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.CreateTab("", "", "").Select();
			this.MyScrollViewer.ScrollToRightEnd();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002BAC File Offset: 0x00000DAC
		private string DynamicTabName()
		{
			string text = "Untitled ";
			List<int> list = new List<int>();
			foreach (object obj in this.TabFlow.Children)
			{
				Tab tab = ((UIElement)obj) as Tab;
				if (tab != null && tab.TabName.StartsWith(text))
				{
					list.Add(tab.DefaultedNameTabNr);
				}
			}
			list.Sort();
			return text + Enumerable.Range(1, list.Count + 1).ToList<int>().Except(list).ToList<int>()[0].ToString();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002C70 File Offset: 0x00000E70
		private Tab CreateTab(string Name, string Content, string path = "")
		{
			this.DragAvailable = false;
			Tab tab = new Tab(this.TabFlow, this);
			tab.TabName = ((Name == "") ? this.DynamicTabName() : Name);
			tab.Script = Content;
			tab.Path = path;
			this.TabFlow.Children.Insert(this.TabFlow.Children.Count - 1, tab);
			this.MyScrollViewer.ScrollToRightEnd();
			this.DragAvailable = true;
			return tab;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002CF4 File Offset: 0x00000EF4
		private void Tab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Grid grid = (Grid)sender;
			if (this.selectedTab != grid)
			{
				grid.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
				((Label)grid.Children[0]).Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.selectedTab.Background = new SolidColorBrush(Color.FromRgb(34, 34, 34));
				((Label)this.selectedTab.Children[0]).Foreground = new SolidColorBrush(Color.FromRgb(122, 122, 122));
				this.selectedTab = grid;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002DA8 File Offset: 0x00000FA8
		private void KrnlWindow_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Point position = Mouse.GetPosition(this.ManagedWrapper);
			if (this.FileContextMenu.Margin.Left >= position.X || this.FileContextMenu.Margin.Left + this.FileContextMenu.Width <= position.X || this.FileContextMenu.Margin.Top >= position.Y || this.FileContextMenu.Margin.Top + this.FileContextMenu.Height <= position.Y)
			{
				this.FileContextMenu.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002E55 File Offset: 0x00001055
		private void appIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002E57 File Offset: 0x00001057
		private void appIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.MenuDown)
			{
				this.BringUpMenu(false);
				this.MenuDown = false;
				return;
			}
			this.BringDownMenu();
			this.MenuDown = true;
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002E7D File Offset: 0x0000107D
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002E85 File Offset: 0x00001085
		public Tab LastTab { get; set; }

		// Token: 0x0600002F RID: 47 RVA: 0x00002E90 File Offset: 0x00001090
		public void BringDownMenu()
		{
			this.LastTab = Common.SelectedTab;
			if (Common.SelectedTab != null)
			{
				Common.SelectedTab.Script = this.ReadScript();
				Common.SelectedTab.Deselect();
			}
			this.MainMenu.Visibility = Visibility.Visible;
			this.appIconAnimDown.To = new Color?(Color.FromRgb(48, 48, 48));
			this.appIconAnimUp.To = new Color?(Color.FromRgb(44, 44, 44));
			this.appIconAnimEnterBack.To = new Color?(Color.FromRgb(44, 44, 44));
			this.appIconAnimLeaveBack.To = new Color?(Color.FromRgb(44, 44, 44));
			base.BeginStoryboard(this.appIconAnimDownSB);
			base.BeginStoryboard(this.appIconAnimUpSB);
			base.BeginStoryboard(this.appIconAnimEnterBackSB);
			base.BeginStoryboard(this.appIconAnimLeaveBackSB);
			if (this.MainDirDisplay.EntryName.Content != "Community")
			{
				if (this.MainDirDisplay.Name == "MainDirDisplay")
				{
					this.CurrentDraftPath = "Scripts";
					this.LayDirPath("");
					this.MainDirDisplay.isFile = true;
					if (this.MainDirDisplay.EntryName.Content == "Scripts")
					{
						this.InitDrafts(this.MainDirDisplay.Path);
					}
					else if (this.MainDirDisplay.EntryName.Content == "Recent")
					{
						this.InitRecents();
					}
				}
				else
				{
					this.CurrentDraftPath = this.MainDirDisplay.Path;
					this.LayDirPath(this.MainDirDisplay.Path);
					this.InitDrafts(this.MainDirDisplay.Path);
				}
			}
			this.MenuDown = true;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003048 File Offset: 0x00001248
		public void SaveTabs()
		{
			if (this.tabs_saving)
			{
				return;
			}
			this.tabs_saving = true;
			if (!Directory.Exists("Data"))
			{
				Directory.CreateDirectory("Data");
			}
			if (Directory.Exists("Data\\SavedTabs"))
			{
				try
				{
					Directory.Delete("Data\\SavedTabs", true);
				}
				catch
				{
					FileAPI.MoveToRecycleBin("Data\\SavedTabs");
				}
			}
			Directory.CreateDirectory("Data\\SavedTabs");
			List<string> list = new List<string>();
			for (int i = 0; i < this.TabFlow.Children.Count - 1; i++)
			{
				Tab tab = (Tab)this.TabFlow.Children[i];
				Directory.CreateDirectory("Data\\SavedTabs\\" + tab.TabName);
				File.WriteAllText("Data\\SavedTabs\\" + tab.TabName + "\\tab.config", tab.Path);
				File.WriteAllText("Data\\SavedTabs\\" + tab.TabName + "\\script.lua", tab.Script);
				list.Add(tab.TabName);
			}
			File.WriteAllText("Data\\SavedTabs\\tabs.config", string.Join("\n", list));
			if (Common.SelectedTab != null)
			{
				Directory.CreateDirectory("Data\\SavedTabs\\" + Common.SelectedTab.TabName);
				File.WriteAllText("Data\\SavedTabs\\" + Common.SelectedTab.TabName + "\\tab.config", Common.SelectedTab.Path);
				File.WriteAllText("Data\\SavedTabs\\" + Common.SelectedTab.TabName + "\\script.lua", this.ReadScript());
			}
			this.tabs_saving = false;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000031E8 File Offset: 0x000013E8
		public void BringUpMenu(bool isTabCaller)
		{
			if (this.TabFlow.Children.Count > 1)
			{
				if (!isTabCaller)
				{
					if (Common.PreviousTab != null)
					{
						Common.PreviousTab.Select();
					}
					else
					{
						((Tab)this.TabFlow.Children[0]).Select();
					}
				}
				this.MainMenu.Visibility = Visibility.Hidden;
				this.appIconAnimDown.To = new Color?(Color.FromRgb(48, 48, 48));
				this.appIconAnimUp.To = new Color?(Color.FromRgb(39, 39, 39));
				this.appIconAnimEnterBack.To = new Color?(Color.FromRgb(39, 39, 39));
				this.appIconAnimLeaveBack.To = new Color?(Color.FromRgb(34, 34, 34));
				base.BeginStoryboard(this.appIconAnimDownSB);
				base.BeginStoryboard(this.appIconAnimUpSB);
				base.BeginStoryboard(this.appIconAnimEnterBackSB);
				base.BeginStoryboard(this.appIconAnimLeaveBackSB);
				this.MenuDown = false;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000032EB File Offset: 0x000014EB
		private void appIcon_MouseEnter(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000032ED File Offset: 0x000014ED
		private void appIcon_MouseLeave(object sender, MouseEventArgs e)
		{
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000032EF File Offset: 0x000014EF
		private void MovableForm_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.DragAvailable)
			{
				base.DragMove();
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000032FF File Offset: 0x000014FF
		private void injectOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (Pipes.PipeActive())
			{
				this.DisplayNotification("Already injected");
				return;
			}
			if (Process.GetProcessesByName("RobloxPlayerBeta").Length != 0)
			{
				this.Inject();
				return;
			}
			this.DisplayNotification("No Roblox process found");
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003334 File Offset: 0x00001534
		private void Inject()
		{
			if (File.Exists(this.DllPath) && File.Exists(Directory.GetCurrentDirectory() + string.Format("\\\\{0}", "injector.dll")))
			{
				this.DisplayNotification("Injecting");
				Task.Run(delegate()
				{
					Injector.inject_status status = Injector.inject(this.DllPath);
					base.Dispatcher.Invoke(delegate()
					{
						switch (status)
						{
						case Injector.inject_status.failure:
							this.DisplayNotification("Failed to inject :(");
							return;
						case Injector.inject_status.success:
							this.DisplayNotification("Injected");
							return;
						case Injector.inject_status.loadimage_fail:
							MessageBox.Show("Failed to access dll file.\n\nKrnl is most likely already injected, or your anti-virus is on!", "krnl", MessageBoxButton.OK, MessageBoxImage.Exclamation);
							return;
						case Injector.inject_status.no_rbx_proc:
							this.DisplayNotification("No Roblox process found");
							return;
						default:
							return;
						}
					});
				});
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x0000338B File Offset: 0x0000158B
		private void AnimateInjecting()
		{
			this.LoadBar.Visibility = Visibility.Visible;
			this.LoaderAnim.To = new Color?(Color.FromArgb(byte.MaxValue, 77, 146, byte.MaxValue));
			this.LoaderAnimStoryboard.Begin();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000033CC File Offset: 0x000015CC
		private void AnimateInjected()
		{
			MainWindow.<AnimateInjected>d__58 <AnimateInjected>d__;
			<AnimateInjected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimateInjected>d__.<>4__this = this;
			<AnimateInjected>d__.<>1__state = -1;
			<AnimateInjected>d__.<>t__builder.Start<MainWindow.<AnimateInjected>d__58>(ref <AnimateInjected>d__);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003403 File Offset: 0x00001603
		private void executeOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.TryExecuteScript(this.ReadScript());
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003412 File Offset: 0x00001612
		private bool TryExecuteScript(string script)
		{
			if (Pipes.PipeActive())
			{
				return Pipes.PassString(script);
			}
			this.DisplayNotification("Krnl is not injected");
			return false;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000342E File Offset: 0x0000162E
		private void menuOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.HBOpts.Visibility == Visibility.Hidden)
			{
				this.HBOpts.Visibility = Visibility.Visible;
				return;
			}
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003457 File Offset: 0x00001657
		private void FileHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.FileOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewOpts.Visibility = Visibility.Hidden;
			this.FileHBOptOpen = 0;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003490 File Offset: 0x00001690
		private void FileHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.FileHBOptOpen != 1)
			{
				this.FileOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000034A7 File Offset: 0x000016A7
		private void FileHBOptsGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.FileHBOptOpen == 0)
			{
				this.FileHBOptOpen = 1;
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000034B8 File Offset: 0x000016B8
		private void FileOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.FileHBOptOpen = 2;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000034C1 File Offset: 0x000016C1
		private void FileOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.FileHBOptOpen = 0;
			this.FileOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000034D6 File Offset: 0x000016D6
		private void EditHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.EditOpts.Visibility = Visibility.Visible;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewOpts.Visibility = Visibility.Hidden;
			this.EditHBOptOpen = 0;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000350F File Offset: 0x0000170F
		private void EditHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.EditHBOptOpen != 1)
			{
				this.EditOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003526 File Offset: 0x00001726
		private void EditHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.EditHBOptOpen == 0)
			{
				this.EditHBOptOpen = 1;
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003537 File Offset: 0x00001737
		private void EditOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.EditHBOptOpen = 2;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003540 File Offset: 0x00001740
		private void EditOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.EditHBOptOpen = 0;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003558 File Offset: 0x00001758
		private void CloseHBOpt_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			if (processesByName.Length != 0)
			{
				Process[] array = processesByName;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Kill();
				}
			}
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003598 File Offset: 0x00001798
		private void ActivateRecent()
		{
			this.RecentTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.RecentTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003704 File Offset: 0x00001904
		private void DeactivateRecent()
		{
			this.RecentTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.RecentTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.RecentTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.RecentTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.RecentTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.RecentTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.circle19.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path21.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.RecentTab2SB);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003884 File Offset: 0x00001A84
		private void ActivateDrafts()
		{
			this.DraftsTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.DraftsTab12.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab13.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab14.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab15.To = new Color?(Color.FromRgb(24, 160, 251));
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003A74 File Offset: 0x00001C74
		private void DeactivateDrafts()
		{
			this.DraftsTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.DraftsTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.DraftsTab12.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTab13.To = new Color?(Color.FromRgb(34, 34, 34));
			this.DraftsTab14.To = new Color?(Color.FromRgb(24, 160, 251));
			this.DraftsTab15.To = new Color?(Color.FromRgb(38, 38, 38));
			this.DraftsTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path208.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path56.Stroke = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.DraftsTab2SB);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003C64 File Offset: 0x00001E64
		private void ActivateCommunity()
		{
			this.CommunityTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab6.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab7.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.CommunityTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003D88 File Offset: 0x00001F88
		private void DeactivateCommunity()
		{
			this.CommunityTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.CommunityTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.CommunityTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CommunityTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab6.To = new Color?(Color.FromRgb(38, 38, 38));
			this.CommunityTab7.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.CommunityTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path20.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.CommunityTab2SB);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003EB8 File Offset: 0x000020B8
		private void ActivatePlan()
		{
			this.UpgradeTab1.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab2.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab4.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab5.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab6.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab7.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab8.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab9.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			this.UpgradeTab10.To = new Color?(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004024 File Offset: 0x00002224
		private void DeactivatePlan()
		{
			this.UpgradeTab1.To = new Color?(Color.FromRgb(38, 38, 38));
			this.UpgradeTab2.To = new Color?(Color.FromRgb(34, 34, 34));
			this.UpgradeTab3.To = new Color?(Color.FromRgb(24, 160, 251));
			this.UpgradeTab4.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab5.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab6.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab7.To = new Color?(Color.FromRgb(38, 38, 38));
			this.UpgradeTab8.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab9.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTab10.To = new Color?(Color.FromRgb(125, 125, 125));
			this.UpgradeTabLabel.Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path42.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			this.path44.Fill = new SolidColorBrush(Color.FromRgb(125, 125, 125));
			base.BeginStoryboard(this.UpgradeTab2SB);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000041A2 File Offset: 0x000023A2
		private void RecentTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DisplayRecent();
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000041AC File Offset: 0x000023AC
		private void DisplayRecent()
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitRecents();
			this.ActivateRecent();
			this.DeactivateDrafts();
			this.DeactivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004219 File Offset: 0x00002419
		private void DraftsTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DisplayDrafts();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004224 File Offset: 0x00002424
		private void DisplayDrafts()
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitDrafts("Scripts");
			this.DeactivateRecent();
			this.ActivateDrafts();
			this.DeactivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004296 File Offset: 0x00002496
		public void SaveRecent(string Name, string Content)
		{
			File.WriteAllText("Recent\\" + Name, Content);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000042AC File Offset: 0x000024AC
		private void InitRecents()
		{
			this.CardHolder.Children.Clear();
			this.MainDirDisplay.EntryName.Content = "Recent";
			foreach (string path in Directory.GetFiles("Recent"))
			{
				string script = File.ReadAllText(path);
				string fileName = System.IO.Path.GetFileName(path);
				TimeSpan creationDate = DateTime.Now - File.GetLastWriteTime(path);
				this.CreateExplorerCard(fileName, script, creationDate, path);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000432C File Offset: 0x0000252C
		private void InitDrafts(string Dire)
		{
			this.CardHolder.Children.Clear();
			this.MainDirDisplay.EntryName.Content = "Scripts";
			string[] files = Directory.GetFiles(Dire);
			foreach (string path in Directory.GetDirectories(Dire))
			{
				string fileName = System.IO.Path.GetFileName(path);
				TimeSpan creationDate = DateTime.Now - File.GetLastWriteTime(path);
				this.CreateExplorerCard(fileName, "", creationDate, path).SetFolderTheme();
			}
			foreach (string path2 in files)
			{
				string fileName2 = System.IO.Path.GetFileName(path2);
				if (fileName2 != "temp.bin")
				{
					string script = File.ReadAllText(path2);
					TimeSpan creationDate2 = DateTime.Now - File.GetLastWriteTime(path2);
					this.CreateExplorerCard(fileName2, script, creationDate2, path2).SetLuaTheme();
				}
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004408 File Offset: 0x00002608
		private void InitCommunity()
		{
			this.MainDirDisplay.EntryName.Content = "Community";
			this.CardHolder.Children.Clear();
			foreach (CommunityEntry element in this.CommunityCards)
			{
				this.CardHolder.Children.Add(element);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000448C File Offset: 0x0000268C
		private void CommunityTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
			this.SearchInput.Text = "Search";
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			this.MainDirDisplay.isFile = true;
			this.InitCommunity();
			this.DeactivateRecent();
			this.DeactivateDrafts();
			this.ActivateCommunity();
			this.DeactivatePlan();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000044FC File Offset: 0x000026FC
		private CommunityEntry CreateCommunityCard(string Title, string Script, string CreatorName)
		{
			CommunityEntry communityEntry = new CommunityEntry();
			communityEntry.EntryName.Content = Title;
			communityEntry.Script = Script;
			communityEntry.EntryCreator.Content = CreatorName;
			communityEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
			communityEntry.Width = 242.0;
			communityEntry.Height = 185.0;
			this.CardHolder.Children.Add(communityEntry);
			communityEntry.RunBorder.MouseLeftButtonUp += this.CommunityCard_MouseLeftButtonUp;
			return communityEntry;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004588 File Offset: 0x00002788
		private void CommunityCard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			CommunityEntry communityEntry = (CommunityEntry)((Grid)((Grid)((Border)sender).Parent).Parent).Parent;
			if (this.TryExecuteScript(communityEntry.Script))
			{
				this.DisplayNotification("File executed");
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000045D3 File Offset: 0x000027D3
		// (set) Token: 0x0600005B RID: 91 RVA: 0x000045DB File Offset: 0x000027DB
		public ExplorerEntry SelectedExpEntry { get; set; }

		// Token: 0x0600005C RID: 92 RVA: 0x000045E4 File Offset: 0x000027E4
		private ExplorerEntry CreateExplorerCard(string Title, string Script, TimeSpan CreationDate, string Path)
		{
			ExplorerEntry explorerEntry = new ExplorerEntry(this);
			explorerEntry.EntryEdit.PreviewKeyDown += this.Entry_PreviewKeyDown;
			explorerEntry.EntryName.Content = Title;
			explorerEntry.Script = Script;
			explorerEntry.EntryEditstamp.Content = this.TranslateDate(CreationDate);
			explorerEntry.Path = Path;
			explorerEntry.MouseDoubleClick += this.CardHolder_MouseDoubleClick;
			explorerEntry.MouseRightButtonUp += this.CardHolder_MouseRightButtonUp;
			explorerEntry.Width = 242.0;
			explorerEntry.Height = 185.0;
			this.CardHolder.Children.Add(explorerEntry);
			return explorerEntry;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004694 File Offset: 0x00002894
		private string TranslateDate(TimeSpan Date)
		{
			if (Date.Days > 0)
			{
				if (Date.Days != 1)
				{
					return string.Format("Edited {0} days ago", Date.Days);
				}
				return string.Format("Edited {0} day ago", Date.Days);
			}
			else if (Date.Hours > 0)
			{
				if (Date.Hours != 1)
				{
					return string.Format("Edited {0} hours ago", Date.Hours);
				}
				return string.Format("Edited {0} hour ago", Date.Hours);
			}
			else if (Date.Minutes > 0)
			{
				if (Date.Minutes != 1)
				{
					return string.Format("Edited {0} minutes ago", Date.Minutes);
				}
				return string.Format("Edited {0} minute ago", Date.Minutes);
			}
			else
			{
				if (Date.Seconds <= 0)
				{
					return "Edited now";
				}
				if (Date.Seconds != 1)
				{
					return string.Format("Edited {0} seconds ago", Date.Seconds);
				}
				return string.Format("Edited {0} second ago", Date.Seconds);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000047AE File Offset: 0x000029AE
		private void AddRecent(string Name, string Content)
		{
			if (!File.Exists("Recent\\" + Name))
			{
				File.WriteAllText("Recent\\" + Name, Content);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000047D4 File Offset: 0x000029D4
		private void CardHolder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				ExplorerEntry explorerEntry = (ExplorerEntry)sender;
				if (explorerEntry.isFile)
				{
					if (File.Exists(explorerEntry.Path))
					{
						explorerEntry.Script = File.ReadAllText(explorerEntry.Path);
					}
					this.AddRecent(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script);
					Tab tab = this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
					tab.Path = explorerEntry.Path;
					this.BringUpMenu(false);
					tab.Select();
					return;
				}
				if (Directory.Exists(explorerEntry.Path))
				{
					this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
					this.MainDirDisplay.isFile = false;
					this.CurrentDraftPath = explorerEntry.Path;
					this.LayDirPath(explorerEntry.Path);
					this.InitDrafts(explorerEntry.Path);
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000048D0 File Offset: 0x00002AD0
		private void LayDirPath(string RelativePath)
		{
			string[] array = RelativePath.Split(new char[]
			{
				'\\'
			});
			string text = "Drafts\\";
			for (int i = 1; i < array.Length - 1; i++)
			{
				FolderDisplay folderDisplay = new FolderDisplay();
				folderDisplay.EntryName.Content = array[i];
				folderDisplay.EntryName.Foreground = new SolidColorBrush(Color.FromRgb(163, 163, 163));
				folderDisplay.isFile = false;
				folderDisplay.MouseLeftButtonUp += this.FolderDisplay_MouseLeftButtonUp;
				folderDisplay.MouseEnter += this.FolderDisplay_MouseEnter;
				folderDisplay.MouseLeave += this.FolderDisplay_MouseLeave;
				text += array[i];
				folderDisplay.Path = text;
				this.FolderDisplayer.Children.Add(folderDisplay);
			}
			FolderDisplay folderDisplay2 = new FolderDisplay();
			folderDisplay2.EntryName.Content = array[array.Length - 1];
			folderDisplay2.EntryName.Foreground = new SolidColorBrush(Color.FromRgb(163, 163, 163));
			folderDisplay2.isFile = true;
			folderDisplay2.Path = RelativePath;
			folderDisplay2.MouseLeftButtonUp += this.FolderDisplay_MouseLeftButtonUp;
			folderDisplay2.MouseEnter += this.FolderDisplay_MouseEnter;
			folderDisplay2.MouseLeave += this.FolderDisplay_MouseLeave;
			this.FolderDisplayer.Children.Add(folderDisplay2);
			this.CardHolder.Children.Clear();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004A50 File Offset: 0x00002C50
		private void FolderDisplay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.FolderDisplayer.Children.RemoveRange(1, this.FolderDisplayer.Children.Count);
			FolderDisplay folderDisplay = (FolderDisplay)sender;
			if (folderDisplay.EntryName.Content != "Community")
			{
				if (folderDisplay.Name == "MainDirDisplay")
				{
					this.CurrentDraftPath = "Scripts";
					this.LayDirPath("");
					this.MainDirDisplay.isFile = true;
					if (folderDisplay.EntryName.Content == "Scripts")
					{
						this.InitDrafts(folderDisplay.Path);
						return;
					}
					if (folderDisplay.EntryName.Content == "Recent")
					{
						this.InitRecents();
						return;
					}
				}
				else
				{
					this.CurrentDraftPath = folderDisplay.Path;
					this.LayDirPath(folderDisplay.Path);
					this.InitDrafts(folderDisplay.Path);
				}
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004B2A File Offset: 0x00002D2A
		private void FolderDisplay_MouseEnter(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Hand;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004B36 File Offset: 0x00002D36
		private void FolderDisplay_MouseLeave(object sender, MouseEventArgs e)
		{
			Mouse.OverrideCursor = Cursors.Arrow;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00004B42 File Offset: 0x00002D42
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00004B4A File Offset: 0x00002D4A
		private UIElement FileContextSelected { get; set; }

		// Token: 0x06000066 RID: 102 RVA: 0x00004B54 File Offset: 0x00002D54
		private void CardHolder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Point position = Mouse.GetPosition(this.ManagedWrapper);
			this.FileContextMenu.Margin = new Thickness(position.X, position.Y, this.FileContextMenu.Margin.Right, this.FileContextMenu.Margin.Bottom);
			if (sender is ExplorerEntry)
			{
				this.FileContextMenu.Height = 157.0;
			}
			else if (sender is CommunityEntry)
			{
				this.FileContextMenu.Height = 41.0;
			}
			this.FileContextMenu.Visibility = Visibility.Visible;
			this.FileContextSelected = (UIElement)sender;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004C03 File Offset: 0x00002E03
		private void UpgradeTab_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DeactivateRecent();
			this.DeactivateDrafts();
			this.DeactivateCommunity();
			this.ActivatePlan();
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004C20 File Offset: 0x00002E20
		private void MyScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			ScrollViewer scrollViewer = sender as ScrollViewer;
			if (e.Delta > 0)
			{
				scrollViewer.LineLeft();
			}
			else
			{
				scrollViewer.LineRight();
			}
			e.Handled = true;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004C52 File Offset: 0x00002E52
		private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004C54 File Offset: 0x00002E54
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004C56 File Offset: 0x00002E56
		private void SearchGlass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
			this.SearchInput.Focus();
			Keyboard.Focus(this.SearchInput);
			this.SearchInput.SelectAll();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004C80 File Offset: 0x00002E80
		private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (base.IsLoaded && this.Searchable)
			{
				if (this.MainDirDisplay.EntryName.Content == "Scripts")
				{
					this.InitDrafts("Scripts");
				}
				else if (this.MainDirDisplay.EntryName.Content == "Recent")
				{
					this.InitRecents();
				}
				else if (this.MainDirDisplay.EntryName.Content == "Community")
				{
					this.InitCommunity();
				}
				if (this.SearchInput.Text != "")
				{
					Console.WriteLine(this.SearchInput.Text);
					List<UIElement> list = new List<UIElement>();
					if (this.MainDirDisplay.EntryName.Content == "Recent" || this.MainDirDisplay.EntryName.Content == "Scripts")
					{
						using (IEnumerator enumerator = this.CardHolder.Children.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								ExplorerEntry explorerEntry = (ExplorerEntry)obj;
								if (CultureInfo.CurrentUICulture.CompareInfo.IndexOf(explorerEntry.EntryName.Content.ToString(), this.SearchInput.Text, CompareOptions.IgnoreCase) >= 0)
								{
									list.Add(explorerEntry);
								}
							}
							goto IL_23C;
						}
					}
					if (this.MainDirDisplay.EntryName.Content == "Community")
					{
						foreach (object obj2 in this.CardHolder.Children)
						{
							CommunityEntry communityEntry = (CommunityEntry)obj2;
							if (communityEntry.EntryName.Content.ToString().ToLower().IndexOf(this.SearchInput.Text.ToLower()) >= 0)
							{
								list.Add(communityEntry);
							}
							else if (communityEntry.Tags.Count > 0)
							{
								for (int i = 0; i < communityEntry.Tags.Count; i++)
								{
									if (communityEntry.Tags[i].ToString().ToLower().StartsWith(this.SearchInput.Text.ToLower()))
									{
										list.Add(communityEntry);
										break;
									}
								}
							}
						}
					}
					IL_23C:
					this.CardHolder.Children.Clear();
					foreach (UIElement element in list)
					{
						this.CardHolder.Children.Add(element);
					}
				}
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004F40 File Offset: 0x00003140
		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004F42 File Offset: 0x00003142
		private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
		{
			this.SearchInput.Clear();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004F4F File Offset: 0x0000314F
		private void Window_DpiChanged(object sender, DpiChangedEventArgs e)
		{
			base.Activate();
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004F58 File Offset: 0x00003158
		private void OpenFileOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptOpenFile();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004F60 File Offset: 0x00003160
		private void PromptOpenFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = "lua";
			bool? flag = openFileDialog.ShowDialog();
			bool flag2 = true;
			if (flag.GetValueOrDefault() == flag2 & flag != null)
			{
				string text = File.ReadAllText(openFileDialog.FileName);
				Tab tab = this.CreateTab(openFileDialog.SafeFileName, text, "");
				tab.Path = openFileDialog.FileName;
				tab.fileWatcher.filePath = tab.Path;
				tab.fileWatcher.Start();
				tab.Select();
				this.WriteScript(text, false);
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000500E File Offset: 0x0000320E
		private void SaveFileOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptSaveAsFile();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005018 File Offset: 0x00003218
		private void PromptSaveAsFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			if (Common.SelectedTab != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\Scripts";
				saveFileDialog.FileName = Common.SelectedTab.TabName;
				saveFileDialog.DefaultExt = "lua";
				saveFileDialog.Filter = "Lua files (*.lua)|*.lua";
				bool? flag = saveFileDialog.ShowDialog();
				bool flag2 = true;
				if (flag.GetValueOrDefault() == flag2 & flag != null)
				{
					File.WriteAllText(saveFileDialog.FileName, this.ReadScript());
					Common.SelectedTab.IsSaved = true;
					Common.SelectedTab.Path = saveFileDialog.FileName;
					Common.SelectedTab.fileWatcher.filePath = saveFileDialog.FileName;
					Common.SelectedTab.fileWatcher.Start();
				}
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000050F8 File Offset: 0x000032F8
		private void DisplayNotification(string Text)
		{
			MainWindow.<DisplayNotification>d__127 <DisplayNotification>d__;
			<DisplayNotification>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DisplayNotification>d__.<>4__this = this;
			<DisplayNotification>d__.Text = Text;
			<DisplayNotification>d__.<>1__state = -1;
			<DisplayNotification>d__.<>t__builder.Start<MainWindow.<DisplayNotification>d__127>(ref <DisplayNotification>d__);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005137 File Offset: 0x00003337
		private void FileContextMenu_LostFocus(object sender, RoutedEventArgs e)
		{
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005148 File Offset: 0x00003348
		private void OpenHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005192 File Offset: 0x00003392
		private void RenameHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000051A0 File Offset: 0x000033A0
		private void DeleteHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				File.GetAttributes(explorerEntry.Path);
				if (explorerEntry.isFile)
				{
					if (File.Exists(explorerEntry.Path))
					{
						File.Delete(explorerEntry.Path);
					}
				}
				else if (Directory.Exists(explorerEntry.Path))
				{
					new DirectoryInfo(explorerEntry.Path).Delete(true);
				}
				this.CardHolder.Children.Remove(explorerEntry);
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00005228 File Offset: 0x00003428
		private void ExplorerHBOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				Process.Start("explorer.exe", "/select, \"" + explorerEntry.Path + "\"");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005270 File Offset: 0x00003470
		private void OpenHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null && explorerEntry.isFile && File.Exists(explorerEntry.Path))
			{
				Tab tab = this.CreateTab(explorerEntry.EntryName.Content.ToString(), explorerEntry.Script, "");
				tab.Path = explorerEntry.Path;
				tab.Select();
				this.BringUpMenu(true);
			}
			CommunityEntry communityEntry = this.FileContextSelected as CommunityEntry;
			if (communityEntry != null)
			{
				this.CreateTab(communityEntry.EntryName.Content.ToString(), communityEntry.Script, "").Select();
				this.BringUpMenu(true);
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005324 File Offset: 0x00003524
		private void ExecuteHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			string script = "";
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				script = explorerEntry.Script;
			}
			else
			{
				CommunityEntry communityEntry = this.FileContextSelected as CommunityEntry;
				if (communityEntry != null)
				{
					script = communityEntry.Script;
				}
			}
			if (this.TryExecuteScript(script))
			{
				this.DisplayNotification("File executed");
			}
			this.FileContextMenu.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005388 File Offset: 0x00003588
		private void RenameHBOpt_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
		{
			ExplorerEntry explorerEntry = this.FileContextSelected as ExplorerEntry;
			if (explorerEntry != null)
			{
				explorerEntry.EntryName.Visibility = Visibility.Hidden;
				explorerEntry.EntryEdit.Text = explorerEntry.EntryName.Content.ToString();
				explorerEntry.EntryEdit.Visibility = Visibility.Visible;
				explorerEntry.Select();
				explorerEntry.Focus();
				explorerEntry.EntryEdit.Focus();
				Keyboard.Focus(explorerEntry.EntryEdit);
				explorerEntry.EntryEdit.SelectAll();
				this.FileContextMenu.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005414 File Offset: 0x00003614
		private void Entry_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				ExplorerEntry explorerEntry = (ExplorerEntry)((Grid)((Border)((Grid)((Grid)((TextBox)sender).Parent).Parent).Parent).Parent).Parent;
				if (explorerEntry.EntryEdit.Text == "")
				{
					explorerEntry.EntryName.Visibility = Visibility.Visible;
					explorerEntry.EntryEdit.Visibility = Visibility.Hidden;
					return;
				}
				explorerEntry.EntryName.Content = explorerEntry.EntryEdit.Text;
				explorerEntry.EntryName.Visibility = Visibility.Visible;
				explorerEntry.EntryEdit.Visibility = Visibility.Hidden;
				string text = System.IO.Path.GetDirectoryName(explorerEntry.Path) + string.Format("\\{0}", explorerEntry.EntryName.Content);
				if (explorerEntry.isFile)
				{
					if (!File.Exists(text))
					{
						File.Move(explorerEntry.Path, text);
					}
				}
				else if (!Directory.Exists(text))
				{
					Directory.Move(explorerEntry.Path, text);
				}
				explorerEntry.Path = text;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005523 File Offset: 0x00003723
		private void SaveOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.PromptSaveFile();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000552C File Offset: 0x0000372C
		private void PromptSaveFile()
		{
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			if (Common.SelectedTab != null)
			{
				if (Common.SelectedTab.Path == "" || !File.Exists(Common.SelectedTab.Path))
				{
					SaveFileDialog saveFileDialog = new SaveFileDialog();
					saveFileDialog.DefaultExt = "lua";
					saveFileDialog.Filter = "Lua files (*.lua)|*.lua";
					bool? flag = saveFileDialog.ShowDialog();
					bool flag2 = true;
					if (flag.GetValueOrDefault() == flag2 & flag != null)
					{
						Common.SelectedTab.TabName = saveFileDialog.SafeFileName;
						Common.SelectedTab.Path = saveFileDialog.FileName;
						File.WriteAllText(saveFileDialog.FileName, this.ReadScript());
						Common.SelectedTab.isSaved = false;
						Common.SelectedTab.IsSaved = true;
						return;
					}
				}
				else
				{
					Common.SelectedTab.isSaved = false;
					Common.SelectedTab.IsSaved = true;
					Common.SelectedTab.TabName = Common.SelectedTab.TabName;
					Common.SelectedTab.watchSave = true;
					File.WriteAllText(Common.SelectedTab.Path, this.ReadScript());
				}
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005653 File Offset: 0x00003853
		private void NewDraft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.MainDirDisplay.EntryName.Content == "Scripts")
			{
				this.CreateDraft();
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005674 File Offset: 0x00003874
		private void CreateDraft()
		{
			ExplorerEntry explorerEntry = this.CreateExplorerCard("Untitled", "", new TimeSpan(0, 0, 0), this.CurrentDraftPath + "\\temp.bin");
			File.WriteAllText(this.CurrentDraftPath + "\\temp.bin", "");
			explorerEntry.EntryName.Visibility = Visibility.Hidden;
			explorerEntry.EntryEdit.Text = explorerEntry.EntryName.Content.ToString();
			explorerEntry.EntryEdit.Visibility = Visibility.Visible;
			explorerEntry.EntryEdit.PreviewKeyDown += this.Entry_PreviewKeyDown;
			Keyboard.ClearFocus();
			explorerEntry.EntryEdit.SelectAll();
			explorerEntry.EntryEdit.Focus();
			UIElement relativeTo = VisualTreeHelper.GetParent(explorerEntry) as UIElement;
			explorerEntry.TranslatePoint(new Point(0.0, 0.0), relativeTo);
			this.CardHolderScroller.ScrollToBottom();
			explorerEntry.Select();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00005768 File Offset: 0x00003968
		private void UndoOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Undo();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000057B8 File Offset: 0x000039B8
		private void RedoOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Redo();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005808 File Offset: 0x00003A08
		private void MinimapOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryKey krnlSubkey = this.getKrnlSubkey();
			object value = krnlSubkey.GetValue("EditorMinimap", false);
			bool flag = value == null || !(value.ToString() == "true");
			if (sender != null)
			{
				krnlSubkey.SetValue("EditorMinimap", flag ? "true" : "false", RegistryValueKind.String);
			}
			else
			{
				flag = !flag;
			}
			this.svg2422.Visibility = (flag ? Visibility.Visible : Visibility.Hidden);
			WebBrowserExtensions.ExecuteScriptAsyncWhenPageLoaded(this.browser, "SwitchMinimap(" + flag.ToString().ToLower() + ");", true);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000058A6 File Offset: 0x00003AA6
		private void PrefHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.PrefOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefHBOptOpen = 0;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000058D3 File Offset: 0x00003AD3
		private void PrefHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.PrefHBOptOpen != 1)
			{
				this.PrefOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000058EA File Offset: 0x00003AEA
		private void PrefHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.PrefHBOptOpen == 0)
			{
				this.PrefHBOptOpen = 1;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000058FB File Offset: 0x00003AFB
		private void PrefOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.PrefHBOptOpen = 2;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005904 File Offset: 0x00003B04
		private void PrefOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.PrefHBOptOpen = 0;
			this.PrefOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000591C File Offset: 0x00003B1C
		private void TopmostOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			base.Topmost = !base.Topmost;
			this.getKrnlSubkey().SetValue("Topmost", base.Topmost ? "true" : "false");
			this.svg242_Copy.Visibility = (base.Topmost ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005974 File Offset: 0x00003B74
		private void UnlockFPSOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			RegistryKey krnlSubkey = this.getKrnlSubkey();
			object value = krnlSubkey.GetValue("UnlockFPS");
			bool flag = value == null || !(value.ToString() == "true");
			if (sender != null)
			{
				krnlSubkey.SetValue("UnlockFPS", flag ? "true" : "false", RegistryValueKind.String);
			}
			else
			{
				flag = !flag;
			}
			this.TryExecuteScript("setfpscap(144)");
			this.svg242_Copy1.Visibility = (flag ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000059F4 File Offset: 0x00003BF4
		private RegistryKey getKrnlSubkey()
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE");
			RegistryKey result = null;
			try
			{
				result = registryKey.OpenSubKey("Krnl", true);
			}
			catch
			{
				result = registryKey.CreateSubKey("Krnl", true);
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005A44 File Offset: 0x00003C44
		private void ViewHBOpt_MouseEnter(object sender, MouseEventArgs e)
		{
			this.ViewOpts.Visibility = Visibility.Visible;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
			this.PrefOpts.Visibility = Visibility.Hidden;
			this.ViewHBOptOpen = 0;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00005A7D File Offset: 0x00003C7D
		private void ViewHBOpt_MouseLeave(object sender, MouseEventArgs e)
		{
			if (this.ViewHBOptOpen != 1)
			{
				this.ViewOpts.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005A94 File Offset: 0x00003C94
		private void ViewHBOptGate_MouseEnter(object sender, MouseEventArgs e)
		{
			if (this.ViewHBOptOpen == 0)
			{
				this.ViewHBOptOpen = 1;
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005AA5 File Offset: 0x00003CA5
		private void ViewOpts_MouseEnter(object sender, MouseEventArgs e)
		{
			this.ViewHBOptOpen = 2;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005AAE File Offset: 0x00003CAE
		private void ViewOpts_MouseLeave(object sender, MouseEventArgs e)
		{
			this.ViewHBOptOpen = 0;
			this.ViewOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005AC4 File Offset: 0x00003CC4
		private void AutoAttachOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.AutoAttachEnabled = !this.AutoAttachEnabled;
			this.getKrnlSubkey().SetValue("AutoAttach", this.AutoAttachEnabled ? "true" : "false");
			this.svg242.Visibility = (this.AutoAttachEnabled ? Visibility.Visible : Visibility.Hidden);
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000093 RID: 147 RVA: 0x00005B1B File Offset: 0x00003D1B
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00005B23 File Offset: 0x00003D23
		private string roblox_path { get; set; } = "";

		// Token: 0x06000095 RID: 149 RVA: 0x00005B2C File Offset: 0x00003D2C
		private void update_roblox_path()
		{
			try
			{
				RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("ROBLOX Corporation");
				if (registryKey == null)
				{
					throw new Exception();
				}
				RegistryKey registryKey2 = registryKey.OpenSubKey("Environments");
				if (registryKey2 == null)
				{
					throw new Exception();
				}
				RegistryKey registryKey3 = registryKey2.OpenSubKey("roblox-player");
				if (registryKey3 == null)
				{
					throw new Exception();
				}
				string path = registryKey3.GetValue("").ToString();
				this.roblox_path = Directory.GetParent(path).FullName;
			}
			catch (Exception)
			{
				this.roblox_path = "";
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005BC4 File Offset: 0x00003DC4
		private void AutoLaunchOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.update_roblox_path();
			if (this.roblox_path == "")
			{
				this.DisplayNotification("ROBLOX Player is not installed.");
				return;
			}
			this.AutoLaunchEnabled = !this.AutoLaunchEnabled;
			if (this.AutoLaunchEnabled)
			{
				this.enable_auto_launch();
				return;
			}
			this.disable_auto_launch();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005C1C File Offset: 0x00003E1C
		private void enable_auto_launch()
		{
			string path = System.IO.Path.Combine(this.roblox_path, "XInput1_4.dll");
			if (!File.Exists(path))
			{
				this.svg3.Visibility = Visibility.Visible;
				if (File.Exists(this.DllPath))
				{
					File.WriteAllBytes(path, File.ReadAllBytes(this.DllPath));
				}
				else
				{
					this.AutoLaunchEnabled = false;
					this.svg3.Visibility = Visibility.Hidden;
					this.DisplayNotification("DLL not found, please close the UI and relaunch the bootstrapper");
				}
			}
			this.auto_launch_mutex = new Mutex(false, "RJ_AL_MTX0001");
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005CA0 File Offset: 0x00003EA0
		private void disable_auto_launch()
		{
			if (this.auto_launch_mutex != null)
			{
				this.auto_launch_mutex.Dispose();
			}
			bool flag;
			Mutex mutex = new Mutex(true, "RJ_AL_MTX0001", ref flag);
			if (flag)
			{
				mutex.ReleaseMutex();
			}
			mutex.Dispose();
			string path = System.IO.Path.Combine(this.roblox_path, "XInput1_4.dll");
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
					this.svg3.Visibility = Visibility.Hidden;
				}
				catch (Exception)
				{
					this.svg3.Visibility = Visibility.Visible;
					MessageBox.Show("Please close Roblox before trying to disable auto launch.", "Krnl", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005D3C File Offset: 0x00003F3C
		private void SearchInput_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.SearchInput.Text == "")
			{
				this.Searchable = false;
				this.SearchInput.Text = "Search";
				this.Searchable = true;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005D74 File Offset: 0x00003F74
		private void CutOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Cut();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.browser.Focus();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005DD0 File Offset: 0x00003FD0
		private void CopyOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Copy();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.browser.Focus();
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005E2C File Offset: 0x0000402C
		private void PasteOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Paste();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.browser.Focus();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005E88 File Offset: 0x00004088
		private void DeleteOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "Delete();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.browser.Focus();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005EE4 File Offset: 0x000040E4
		private void SelectAllOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			WebBrowserExtensions.EvaluateScriptAsync(this.browser, "SelectAll();", null, false).GetAwaiter().GetResult();
			this.HBOpts.Visibility = Visibility.Hidden;
			this.EditOpts.Visibility = Visibility.Hidden;
			this.browser.Focus();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005F3D File Offset: 0x0000413D
		private void MainMenu_MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005F3F File Offset: 0x0000413F
		private void Workspace_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.Workspace.Focus();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005F4D File Offset: 0x0000414D
		private void HBOpts_LostFocus(object sender, RoutedEventArgs e)
		{
			this.HBOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005F5B File Offset: 0x0000415B
		private void OpenKrnlOpt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Process.Start(Directory.GetCurrentDirectory());
			this.HBOpts.Visibility = Visibility.Hidden;
			this.FileOpts.Visibility = Visibility.Hidden;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005F80 File Offset: 0x00004180
		private void SearchBorder_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.IBeam;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005F8D File Offset: 0x0000418D
		private void SearchBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00005F9C File Offset: 0x0000419C
		private void Editor_Drop(object sender, DragEventArgs e)
		{
			DataObject dataObject = (DataObject)e.Data;
			if (dataObject.ContainsFileDropList())
			{
				StringCollection fileDropList = dataObject.GetFileDropList();
				Tab tab = null;
				foreach (string path in fileDropList)
				{
					try
					{
						tab = this.CreateTab(System.IO.Path.GetFileName(path), File.ReadAllText(path), "");
					}
					catch (Exception ex)
					{
						tab = this.CreateTab("<ERROR> " + System.IO.Path.GetFileName(path), "Couldn't access the file\nComputer produced error; " + ex.Message, "");
					}
				}
				if (tab == null)
				{
					MessageBox.Show("FATAL CODE FAULT . Some code in the application produced unexpected output . 1941");
					return;
				}
				tab.Select();
			}
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006074 File Offset: 0x00004274
		private void TabContainer_Drop(object sender, DragEventArgs e)
		{
			DataObject dataObject = (DataObject)e.Data;
			if (dataObject.ContainsFileDropList())
			{
				StringCollection fileDropList = dataObject.GetFileDropList();
				Tab tab = null;
				foreach (string path in fileDropList)
				{
					tab = this.CreateTab(System.IO.Path.GetFileName(path), File.ReadAllText(path), path);
				}
				if (tab != null)
				{
					tab.Select();
				}
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000060F8 File Offset: 0x000042F8
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			this.SaveTabs();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006100 File Offset: 0x00004300
		private void Window_Closed(object sender, EventArgs e)
		{
		}

		// Token: 0x04000002 RID: 2
		private string SiteUrl = "https://cdn.krnl.place/";

		// Token: 0x04000004 RID: 4
		private List<CommunityEntry> CommunityCards = new List<CommunityEntry>();

		// Token: 0x04000005 RID: 5
		private bool TabChanging;

		// Token: 0x04000006 RID: 6
		private string DllPath = Directory.GetCurrentDirectory() + string.Format("\\\\{0}", "krnl.dll");

		// Token: 0x04000007 RID: 7
		private bool DragAvailable = true;

		// Token: 0x04000008 RID: 8
		private bool AutoAttachEnabled;

		// Token: 0x04000009 RID: 9
		private bool isAutoAttached;

		// Token: 0x0400000A RID: 10
		private bool AutoLaunchEnabled = true;

		// Token: 0x0400000C RID: 12
		private int TabCount = 2;

		// Token: 0x0400000D RID: 13
		public bool MenuDown;

		// Token: 0x0400000F RID: 15
		private bool tabs_saving;

		// Token: 0x04000010 RID: 16
		private int FileHBOptOpen;

		// Token: 0x04000011 RID: 17
		private int EditHBOptOpen;

		// Token: 0x04000012 RID: 18
		private List<ValueTuple<string, string, string>> Scripts = new List<ValueTuple<string, string, string>>
		{
			new ValueTuple<string, string, string>("OpenGui", "loadstring(game:HttpGet('https://pastebin.com/raw/UXmbai5q', true))()", "stickmasterluke")
		};

		// Token: 0x04000014 RID: 20
		private string CurrentDraftPath = "Scripts";

		// Token: 0x04000016 RID: 22
		private bool isAnimatingNotf;

		// Token: 0x04000017 RID: 23
		private int PrefHBOptOpen;

		// Token: 0x04000018 RID: 24
		private int ViewHBOptOpen;

		// Token: 0x0400001A RID: 26
		private Mutex auto_launch_mutex;

		// Token: 0x0400001B RID: 27
		private bool Searchable = true;
	}
}
