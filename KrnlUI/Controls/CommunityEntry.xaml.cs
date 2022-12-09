using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KrnlUI.Controls
{
	// Token: 0x0200000C RID: 12
	public partial class CommunityEntry : UserControl
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00007A7B File Offset: 0x00005C7B
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00007A83 File Offset: 0x00005C83
		public string Script { get; set; }

		// Token: 0x060000CB RID: 203 RVA: 0x00007A8C File Offset: 0x00005C8C
		public CommunityEntry()
		{
			this.InitializeComponent();
		}

		// Token: 0x04000111 RID: 273
		public List<string> Tags = new List<string>();
	}
}
