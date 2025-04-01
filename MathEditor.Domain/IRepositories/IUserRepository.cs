using MathEditor.Domain.Entities;

namespace MathEditor.Domain.IRepositories;

public interface IUserRepository
{
    /// <summary>
    /// Finds a user by their ID or handle.
    /// </summary>
    Task<ApplicationUser?> FindUserAsync(string handle);

    /// <summary>
    /// Finds a user by their email.
    /// </summary>
    Task<ApplicationUser?> FindUserByEmailAsync(string email);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    Task CreateUserAsync(ApplicationUser user, string password);

    /// <summary>
    /// Updates an existing user by their ID.
    /// </summary>
    Task UpdateUserAsync(string id, ApplicationUser updateData);

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    Task<bool> DeleteUserAsync(string id);
}
