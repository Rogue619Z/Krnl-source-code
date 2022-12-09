using System;
using System.Runtime.InteropServices;

namespace KrnlUI
{
	// Token: 0x02000005 RID: 5
	public class Injector
	{
		// Token: 0x0600000D RID: 13
		[DllImport("injector.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern Injector.inject_status inject(string dll_path);

		// Token: 0x02000017 RID: 23
		public enum inject_status
		{
			// Token: 0x0400017A RID: 378
			failure = -1,
			// Token: 0x0400017B RID: 379
			success,
			// Token: 0x0400017C RID: 380
			loadimage_fail,
			// Token: 0x0400017D RID: 381
			no_rbx_proc
		}
	}
}
