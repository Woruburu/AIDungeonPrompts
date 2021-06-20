using System.IO;
using System.IO.Compression;
using System.Linq;

namespace AIDungeonPrompts.Application.Helpers
{
	public static class ZipHelper
	{
		private static readonly string[] ExpectedFiles =
		{
			"contextModifier.js", "inputModifier.js", "outputModifier.js", "shared.js"
		};

		private static readonly byte[] ZipBytes1 = {0x50, 0x4b, 0x03, 0x04};
		private static readonly byte[] ZipBytes2 = {0x50, 0x4b, 0x05, 0x06};
		private static readonly byte[] ZipBytes3 = {0x50, 0x4b, 0x07, 0x08};

		public static bool CheckFileContents(byte[] bytes)
		{
			try
			{
				using var memoryStream = new MemoryStream(bytes);
				using var zip = new ZipArchive(memoryStream);
				return zip.Entries.Any(e => ExpectedFiles.Contains(e.Name));
			}
			catch
			{
				return false;
			}
		}

		public static bool IsCompressedData(byte[] data)
		{
			foreach (var headerBytes in new[] {ZipBytes1, ZipBytes2, ZipBytes3})
			{
				if (HeaderBytesMatch(headerBytes, data))
				{
					return true;
				}
			}

			return false;
		}

		private static bool HeaderBytesMatch(byte[] headerBytes, byte[] dataBytes)
		{
			if (dataBytes.Length < headerBytes.Length)
			{
				return false;
			}

			for (var i = 0; i < headerBytes.Length; i++)
			{
				if (headerBytes[i] == dataBytes[i])
				{
					continue;
				}

				return false;
			}

			return true;
		}
	}
}
