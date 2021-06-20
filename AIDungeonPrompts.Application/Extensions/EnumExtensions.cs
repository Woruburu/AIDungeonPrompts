using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AIDungeonPrompts.Application.Extensions
{
	public static class EnumExtensions
	{
		public static string? GetEnumDisplayName(this Enum enumType) =>
			enumType.GetType().GetMember(enumType.ToString())[0]
				.GetCustomAttribute<DisplayAttribute>()
				.Name;
	}
}
