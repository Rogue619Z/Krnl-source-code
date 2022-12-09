using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000D RID: 13
	public partial class ExplorerEntry : UserControl
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00007B8F File Offset: 0x00005D8F
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00007B97 File Offset: 0x00005D97
		private MainWindow window { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00007BA0 File Offset: 0x00005DA0
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00007BA8 File Offset: 0x00005DA8
		public string Script { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00007BB1 File Offset: 0x00005DB1
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00007BB9 File Offset: 0x00005DB9
		public string Path { get; set; }

		// Token: 0x060000D4 RID: 212 RVA: 0x00007BC2 File Offset: 0x00005DC2
		public ExplorerEntry(MainWindow window)
		{
			this.InitializeComponent();
			this.window = window;
			this.Script = "";
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00007BEC File Offset: 0x00005DEC
		public void SetLuaTheme()
		{
			this.FolderIcon.Visibility = Visibility.Hidden;
			this.FolderIcon2.Visibility = Visibility.Hidden;
			this.LuaIcon.Visibility = Visibility.Visible;
			this.LuaIcon2.Visibility = Visibility.Visible;
			this.BorderBack.Background = new SolidColorBrush(Color.FromRgb(24, 160, 251));
			this.isFile = true;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007C54 File Offset: 0x00005E54
		public void SetFolderTheme()
		{
			this.FolderIcon.Visibility = Visibility.Visible;
			this.FolderIcon2.Visibility = Visibility.Visible;
			this.LuaIcon.Visibility = Visibility.Hidden;
			this.LuaIcon2.Visibility = Visibility.Hidden;
			this.BorderBack.Background = new SolidColorBrush(Color.FromRgb(44, 44, 44));
			this.isFile = false;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007CB3 File Offset: 0x00005EB3
		private void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Hand;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007CC0 File Offset: 0x00005EC0
		private void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			base.Cursor = Cursors.Arrow;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007CCD File Offset: 0x00005ECD
		private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.window.FileContextMenu.Visibility = Visibility.Hidden;
			this.Select();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007CE8 File Offset: 0x00005EE8
		public void Select()
		{
			if (this.window.SelectedExpEntry != null)
			{
				this.window.SelectedExpEntry.CardBorderDown.To = new Color?(Color.FromArgb(17, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.window.SelectedExpEntry.CardBorderUp.To = new Color?(Color.FromArgb(17, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				this.window.SelectedExpEntry.CardBorderDownSB.Begin();
			}
			this.CardBorderDown.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CardBorderUp.To = new Color?(Color.FromRgb(24, 160, 251));
			this.CardBorderDownSB.Begin();
			this.window.SelectedExpEntry = this;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00007DD0 File Offset: 0x00005FD0
		private void EntryEdit_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.EntryEdit.Text == "")
			{
				this.EntryName.Visibility = Visibility.Visible;
				this.EntryEdit.Visibility = Visibility.Hidden;
				return;
			}
			this.EntryName.Content = this.EntryEdit.Text;
			this.EntryName.Visibility = Visibility.Visible;
			this.EntryEdit.Visibility = Visibility.Hidden;
			string text = System.IO.Path.GetDirectoryName(this.Path) + string.Format("\\{0}", this.EntryName.Content);
			if (this.isFile)
			{
				if (!File.Exists(text))
				{
					File.Move(this.Path, text);
				}
			}
			else if (!Directory.Exists(text))
			{
				Directory.Move(this.Path, text);
			}
			this.Path = text;
		}

		// Token: 0x0400011F RID: 287
		public bool isFile = true;
	}
}
