using System.Threading.Tasks;
using AIDungeonPrompts.Domain.Entities;

namespace AIDungeonPrompts.Application.Abstractions.Identity
{
	public interface ICurrentUserService
	{
		Task SetCurrentUser(int userId);

		bool TryGetCurrentUser(out User? user);
	}
}
