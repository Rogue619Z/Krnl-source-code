using System;
using System.Runtime.InteropServices;

namespace KrnlUI
{
	// Token: 0x02000003 RID: 3
	internal class FileAPI
	{
		// Token: 0x06000007 RID: 7
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern int SHFileOperation(ref FileAPI.SHFILEOPSTRUCT FileOp);

		// Token: 0x06000008 RID: 8 RVA: 0x000020C8 File Offset: 0x000002C8
		public static bool MoveToRecycleBin(string path)
		{
			bool result;
			try
			{
				FileAPI.SHFILEOPSTRUCT shfileopstruct = new FileAPI.SHFILEOPSTRUCT
				{
					wFunc = FileAPI.FileOperationType.FO_DELETE,
					pFrom = path + "\0\0",
					fFlags = (FileAPI.FileOperationFlags.FOF_SILENT | FileAPI.FileOperationFlags.FOF_NOCONFIRMATION | FileAPI.FileOperationFlags.FOF_ALLOWUNDO | FileAPI.FileOperationFlags.FOF_NOERRORUI)
				};
				FileAPI.SHFileOperation(ref shfileopstruct);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x02000014 RID: 20
		[Flags]
		public enum FileOperationFlags : ushort
		{
			// Token: 0x04000166 RID: 358
			FOF_SILENT = 4,
			// Token: 0x04000167 RID: 359
			FOF_NOCONFIRMATION = 16,
			// Token: 0x04000168 RID: 360
			FOF_ALLOWUNDO = 64,
			// Token: 0x04000169 RID: 361
			FOF_SIMPLEPROGRESS = 256,
			// Token: 0x0400016A RID: 362
			FOF_NOERRORUI = 1024,
			// Token: 0x0400016B RID: 363
			FOF_WANTNUKEWARNING = 16384
		}

		// Token: 0x02000015 RID: 21
		public enum FileOperationType : uint
		{
			// Token: 0x0400016D RID: 365
			FO_MOVE = 1U,
			// Token: 0x0400016E RID: 366
			FO_COPY,
			// Token: 0x0400016F RID: 367
			FO_DELETE,
			// Token: 0x04000170 RID: 368
			FO_RENAME
		}

		// Token: 0x02000016 RID: 22
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct SHFILEOPSTRUCT
		{
			// Token: 0x04000171 RID: 369
			public IntPtr hwnd;

			// Token: 0x04000172 RID: 370
			[MarshalAs(UnmanagedType.U4)]
			public FileAPI.FileOperationType wFunc;

			// Token: 0x04000173 RID: 371
			public string pFrom;

			// Token: 0x04000174 RID: 372
			public string pTo;

			// Token: 0x04000175 RID: 373
			public FileAPI.FileOperationFlags fFlags;

			// Token: 0x04000176 RID: 374
			[MarshalAs(UnmanagedType.Bool)]
			public bool fAnyOperationsAborted;

			// Token: 0x04000177 RID: 375
			public IntPtr hNameMappings;

			// Token: 0x04000178 RID: 376
			public string lpszProgressTitle;
		}
	}
}
