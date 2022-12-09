using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KrnlUI
{
	// Token: 0x02000007 RID: 7
	internal class Pipes
	{
		// Token: 0x060000AD RID: 173
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WaitNamedPipe(string name, int timeout);

		// Token: 0x060000AE RID: 174 RVA: 0x0000786C File Offset: 0x00005A6C
		public static bool PipeActive()
		{
			bool result;
			try
			{
				if (!Pipes.WaitNamedPipe(Path.GetFullPath("\\\\.\\pipe\\" + Pipes.PipeName), 0))
				{
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000078B4 File Offset: 0x00005AB4
		public static bool PassString(string input)
		{
			bool result = false;
			Task.Run(delegate()
			{
				if (Pipes.PipeActive())
				{
					try
					{
						using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", "krnlpipe", PipeDirection.Out))
						{
							namedPipeClientStream.Connect();
							byte[] bytes = Encoding.UTF8.GetBytes(input);
							namedPipeClientStream.Write(bytes, 0, bytes.Length);
							namedPipeClientStream.Dispose();
						}
						return;
					}
					catch (Exception)
					{
						result = false;
						return;
					}
				}
				result = false;
			}).GetAwaiter().GetResult();
			return result;
		}

		// Token: 0x04000107 RID: 263
		private static string PipeName = "krnlpipe";
	}
}
