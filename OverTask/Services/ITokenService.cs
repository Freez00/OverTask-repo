using OverTask.Models;

namespace OverTask.Services
{
	public interface ITokenService
	{
		Task<string> GenerateToken(ApplicationUser user);
	}
}
