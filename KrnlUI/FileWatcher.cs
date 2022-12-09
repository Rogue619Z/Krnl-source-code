using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using KrnlUI.Controls;

namespace KrnlUI
{
	// Token: 0x02000008 RID: 8
	public class FileWatcher
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x0000790B File Offset: 0x00005B0B
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00007913 File Offset: 0x00005B13
		private Tab Tab { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x0000791C File Offset: 0x00005B1C
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00007924 File Offset: 0x00005B24
		public string filePath { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x0000792D File Offset: 0x00005B2D
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00007935 File Offset: 0x00005B35
		private bool isWatching { get; set; }

		// Token: 0x060000B8 RID: 184 RVA: 0x0000793E File Offset: 0x00005B3E
		public FileWatcher(Tab tab)
		{
			this.Tab = tab;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000794D File Offset: 0x00005B4D
		public void Start()
		{
			if (!this.isWatching)
			{
				this.isWatching = true;
				new Task(new Action(this.Initialize)).Start();
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007974 File Offset: 0x00005B74
		private void Initialize()
		{
			Application.Current.Dispatcher.Invoke<Task>(delegate()
			{
				FileWatcher.<<Initialize>b__14_0>d <<Initialize>b__14_0>d;
				<<Initialize>b__14_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Initialize>b__14_0>d.<>4__this = this;
				<<Initialize>b__14_0>d.<>1__state = -1;
				<<Initialize>b__14_0>d.<>t__builder.Start<FileWatcher.<<Initialize>b__14_0>d>(ref <<Initialize>b__14_0>d);
				return <<Initialize>b__14_0>d.<>t__builder.Task;
			});
		}
	}
}
