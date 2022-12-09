using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace KrnlUI.Animations
{
	// Token: 0x02000011 RID: 17
	internal class ButtonAnims
	{
		// Token: 0x060000FF RID: 255 RVA: 0x00008F6B File Offset: 0x0000716B
		public static void ButtonExitEnter(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimEnter(sender, Color.FromRgb(231, 16, 34), Color.FromRgb(200, 200, 200));
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008F95 File Offset: 0x00007195
		public static void ButtonExitLeave(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimLeave(sender, Color.FromRgb(34, 34, 34), Color.FromRgb(122, 122, 122));
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00008FB3 File Offset: 0x000071B3
		public static void ButtonExitDown(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimDown(sender, Color.FromRgb(194, 16, 29));
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00008FC9 File Offset: 0x000071C9
		public static void ButtonExitUp(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimUp(sender, Color.FromRgb(231, 16, 34));
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008FDF File Offset: 0x000071DF
		public static void ButtonTopEnter(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimEnter(sender, Color.FromRgb(57, 57, 57), Color.FromRgb(200, 200, 200));
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00009006 File Offset: 0x00007206
		public static void ButtonTopLeave(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimLeave(sender, Color.FromRgb(34, 34, 34), Color.FromRgb(122, 122, 122));
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00009024 File Offset: 0x00007224
		public static void ButtonTopDown(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimDown(sender, Color.FromRgb(74, 74, 74));
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00009037 File Offset: 0x00007237
		public static void ButtonTopUp(object sender, MouseEventArgs e)
		{
			ButtonAnims.AnimUp(sender, Color.FromRgb(57, 57, 57));
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000904C File Offset: 0x0000724C
		private static void AnimEnter(object sender, Color color, Color secondary)
		{
			ButtonAnims.<AnimEnter>d__10 <AnimEnter>d__;
			<AnimEnter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimEnter>d__.sender = sender;
			<AnimEnter>d__.color = color;
			<AnimEnter>d__.secondary = secondary;
			<AnimEnter>d__.<>1__state = -1;
			<AnimEnter>d__.<>t__builder.Start<ButtonAnims.<AnimEnter>d__10>(ref <AnimEnter>d__);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00009094 File Offset: 0x00007294
		private static void AnimLeave(object sender, Color color, Color secondary)
		{
			ButtonAnims.<AnimLeave>d__11 <AnimLeave>d__;
			<AnimLeave>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimLeave>d__.sender = sender;
			<AnimLeave>d__.color = color;
			<AnimLeave>d__.secondary = secondary;
			<AnimLeave>d__.<>1__state = -1;
			<AnimLeave>d__.<>t__builder.Start<ButtonAnims.<AnimLeave>d__11>(ref <AnimLeave>d__);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000090DC File Offset: 0x000072DC
		private static void AnimDown(object sender, Color color)
		{
			ButtonAnims.<AnimDown>d__12 <AnimDown>d__;
			<AnimDown>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimDown>d__.sender = sender;
			<AnimDown>d__.color = color;
			<AnimDown>d__.<>1__state = -1;
			<AnimDown>d__.<>t__builder.Start<ButtonAnims.<AnimDown>d__12>(ref <AnimDown>d__);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000911C File Offset: 0x0000731C
		private static void AnimUp(object sender, Color color)
		{
			ButtonAnims.<AnimUp>d__13 <AnimUp>d__;
			<AnimUp>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AnimUp>d__.sender = sender;
			<AnimUp>d__.color = color;
			<AnimUp>d__.<>1__state = -1;
			<AnimUp>d__.<>t__builder.Start<ButtonAnims.<AnimUp>d__13>(ref <AnimUp>d__);
		}

		// Token: 0x04000161 RID: 353
		private static int animTime = 20;

		// Token: 0x04000162 RID: 354
		private static int smooth = 5;
	}
}
