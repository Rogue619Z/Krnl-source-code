using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000E RID: 14
	public partial class FolderDisplay : UserControl
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000080EB File Offset: 0x000062EB
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000080F3 File Offset: 0x000062F3
		public bool isFile
		{
			get
			{
				return this.isFileRaw;
			}
			set
			{
				this.isFileRaw = value;
				if (value)
				{
					this.svg240.Visibility = Visibility.Collapsed;
					return;
				}
				this.svg240.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008118 File Offset: 0x00006318
		public FolderDisplay()
		{
			this.InitializeComponent();
		}

		// Token: 0x0400013C RID: 316
		public string Path = "";

		// Token: 0x0400013D RID: 317
		private bool isFileRaw;
	}
}
