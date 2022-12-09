using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KrnlUI.Properties
{
	// Token: 0x02000009 RID: 9
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	public class Resources
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000079D7 File Offset: 0x00005BD7
		internal Resources()
		{
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000BD RID: 189 RVA: 0x000079DF File Offset: 0x00005BDF
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("KrnlUI.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00007A0B File Offset: 0x00005C0B
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00007A12 File Offset: 0x00005C12
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00007A1A File Offset: 0x00005C1A
		public static string _default
		{
			get
			{
				return Resources.ResourceManager.GetString("_default", Resources.resourceCulture);
			}
		}

		// Token: 0x0400010B RID: 267
		private static ResourceManager resourceMan;

		// Token: 0x0400010C RID: 268
		private static CultureInfo resourceCulture;
	}
}
