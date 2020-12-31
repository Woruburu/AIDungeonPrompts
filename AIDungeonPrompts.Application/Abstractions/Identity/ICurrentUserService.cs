using System.Threading.Tasks;
using AIDungeonPrompts.Application.Queries.GetUser;

namespace AIDungeonPrompts.Application.Abstractions.Identity
{
	public interface ICurrentUserService
	{
		Task SetCurrentUser(int userId);

		bool TryGetCurrentUser(out GetUserViewModel? user);
	}
}
