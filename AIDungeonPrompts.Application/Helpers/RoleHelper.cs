using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Application.Helpers
{
	public static class RoleHelper
	{
		public static bool CanEdit(RoleEnum role)
		{
			const RoleEnum editRoles = RoleEnum.TagEdit | RoleEnum.FieldEdit | RoleEnum.Delete;
			return (editRoles & role) != 0;
		}
	}
}
