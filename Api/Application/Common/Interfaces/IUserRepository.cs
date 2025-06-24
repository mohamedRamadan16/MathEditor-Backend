using System;
using System.Threading.Tasks;
using Api.Domain.Entities;

namespace Api.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindUserAsync(string handleOrId);
        Task<User?> FindUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(Guid id, User user);
        Task DeleteUserAsync(Guid id);
    }
}
