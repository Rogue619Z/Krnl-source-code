using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace KrnlUI.Controls
{
	// Token: 0x0200000F RID: 15
	public partial class ScrollContainer : UserControl
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x000081B8 File Offset: 0x000063B8
		// (set) Token: 0x060000E4 RID: 228 RVA: 0x00008224 File Offset: 0x00006424
		public List<UIElement> Children
		{
			get
			{
				List<UIElement> list = new List<UIElement>();
				foreach (object obj in ((Grid)base.Content).Children)
				{
					UIElement item = (UIElement)obj;
					list.Add(item);
				}
				return list;
			}
			set
			{
				foreach (UIElement element in value)
				{
					((Grid)base.Content).Children.Add(element);
				}
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00008284 File Offset: 0x00006484
		public ScrollContainer()
		{
			this.InitializeComponent();
			Storyboard.SetTargetProperty(this.ScrollPosAnimator, new PropertyPath("Margin", Array.Empty<object>()));
			this.ScrollStoryboard.Children.Add(this.ScrollPosAnimator);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00008314 File Offset: 0x00006514
		private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (this.VisualChildrenCount == 1 && base.IsLoaded)
			{
				ScrollViewer scrollViewer = (ScrollViewer)base.Content;
				Storyboard.SetTarget(this.ScrollPosAnimator, scrollViewer);
				this.ScrollStoryboard.Stop();
				MessageBox.Show(scrollViewer.ViewportHeight.ToString());
				int num = this.ScrollLength * (e.Delta / 120);
				this.AccPad += num;
				if (this.AccPad > 0)
				{
					this.ScrollPosAnimator.To = new Thickness?(new Thickness(0.0, 0.0, 0.0, 0.0));
					this.AccPad = 0;
				}
				else
				{
					this.ScrollPosAnimator.To = new Thickness?(new Thickness(0.0, (double)this.AccPad, 0.0, 0.0));
				}
				this.ScrollStoryboard.Begin();
			}
		}

		// Token: 0x04000142 RID: 322
		private int AccPad;

		// Token: 0x04000143 RID: 323
		public int ScrollLength = 40;

		// Token: 0x04000144 RID: 324
		private Storyboard ScrollStoryboard = new Storyboard();

		// Token: 0x04000145 RID: 325
		private ThicknessAnimation ScrollPosAnimator = new ThicknessAnimation
		{
			Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
			DecelerationRatio = 1.0
		};
	}
}
