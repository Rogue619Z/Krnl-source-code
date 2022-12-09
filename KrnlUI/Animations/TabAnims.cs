using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KrnlUI.Animations
{
	// Token: 0x02000012 RID: 18
	internal class TabAnims
	{
		// Token: 0x0600010D RID: 269 RVA: 0x00009172 File Offset: 0x00007372
		public static void NewTabEnter(object sender, MouseEventArgs e)
		{
			((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(175, 175, 175));
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00009198 File Offset: 0x00007398
		public static void NewTabLeave(object sender, MouseEventArgs e)
		{
			((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(122, 122, 122));
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000091B5 File Offset: 0x000073B5
		public static void AnimateNewTab(object sender)
		{
			Grid grid = (Grid)sender;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000091C0 File Offset: 0x000073C0
		private static void AnimEnter(object sender, Color color, Color secondary)
		{
			TabAnims.<AnimEnter>d__5 <AnimEnter>d__;
			<AnimEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimEnter>d__.sender = sender;
			<AnimEnter>d__.color = color;
			<AnimEnter>d__.secondary = secondary;
			<AnimEnter>d__.<>1__state = -1;
			<AnimEnter>d__.<>t__builder.Start<TabAnims.<AnimEnter>d__5>(ref <AnimEnter>d__);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009208 File Offset: 0x00007408
		private static void AnimLeave(object sender, Color color, Color secondary)
		{
			TabAnims.<AnimLeave>d__6 <AnimLeave>d__;
			<AnimLeave>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimLeave>d__.sender = sender;
			<AnimLeave>d__.color = color;
			<AnimLeave>d__.secondary = secondary;
			<AnimLeave>d__.<>1__state = -1;
			<AnimLeave>d__.<>t__builder.Start<TabAnims.<AnimLeave>d__6>(ref <AnimLeave>d__);
		}

		// Token: 0x04000163 RID: 355
		private static int animTime = 20;

		// Token: 0x04000164 RID: 356
		private static int smooth = 5;
	}
}
