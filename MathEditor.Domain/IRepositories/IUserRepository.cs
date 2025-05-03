using MathEditor.Domain.Entities;

namespace MathEditor.Domain.IRepositories;

public interface IUserRepository
{
    Task<ApplicationUser?> FindUserAsync(string handle);

    Task<ApplicationUser?> FindUserByIdAsync(string userId);

    Task<ApplicationUser?> FindUserByEmailAsync(string email);

    Task CreateUserAsync(ApplicationUser user, string password);

    Task UpdateUserAsync(string id, ApplicationUser updateData);

    Task<bool> DeleteUserAsync(string id);
}
