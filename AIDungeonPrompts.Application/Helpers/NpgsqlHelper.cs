using System;
using System.Linq;
using System.Text;

namespace AIDungeonPrompts.Application.Helpers
{
	public static class NpgsqlHelper
	{
		public static readonly string EscapeChar = @"\";
		public static string SafeIlike(string query)
		{
			var chars = new[] { '%', '_', '/' };
			var stringBuilder = new StringBuilder(query);
			for (var i = 0; i < stringBuilder.Length; i++)
			{
				if (!chars.Contains(stringBuilder[i]))
				{
					continue;
				}
				stringBuilder.Insert(i, EscapeChar);
				i++;
			}
			return stringBuilder.ToString();
		}
	}
}
