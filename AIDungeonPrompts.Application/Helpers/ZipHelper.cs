namespace AIDungeonPrompts.Application.Helpers
{
	public static class ZipHelper
	{
		private static readonly byte[] ZipBytes1 = { 0x50, 0x4b, 0x03, 0x04 };
		private static readonly byte[] ZipBytes2 = { 0x50, 0x4b, 0x05, 0x06 };
		private static readonly byte[] ZipBytes3 = { 0x50, 0x4b, 0x07, 0x08 };

		public static bool IsCompressedData(byte[] data)
		{
			foreach (var headerBytes in new[] { ZipBytes1, ZipBytes2, ZipBytes3 })
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
				//throw new ArgumentOutOfRangeException(nameof(dataBytes),
				//	$"Passed databytes length ({dataBytes.Length}) is shorter than the headerbytes ({headerBytes.Length})");
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
