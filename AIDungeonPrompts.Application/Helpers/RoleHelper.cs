using AIDungeonPrompts.Domain.Enums;

namespace AIDungeonPrompts.Application.Helpers
{
	public static class RoleHelper
	{
		public static bool CanEdit(RoleEnum role)
		{
			const RoleEnum editRoles = RoleEnum.TagEdit | RoleEnum.FieldEdit;
			return (editRoles & role) != 0;
		}

		public static bool CanDelete(RoleEnum role)
		{
			const RoleEnum deleteRoles = RoleEnum.Delete;
			return (deleteRoles & role) != 0;
		}
	}
}
