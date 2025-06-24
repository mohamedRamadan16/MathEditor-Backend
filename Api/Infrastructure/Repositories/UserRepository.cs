using System;
using System.Threading.Tasks;
using Api.Application.Common.Interfaces;
using Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> FindUserAsync(string handleOrId)
        {
            if (Guid.TryParse(handleOrId, out var id))
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return await _context.Users.FirstOrDefaultAsync(u => u.Handle != null && u.Handle.ToLower() == handleOrId.ToLower());
        }

        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(Guid id, User user)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) throw new KeyNotFoundException();
            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.Handle = user.Handle;
            existing.UpdatedAt = user.UpdatedAt;
            existing.Disabled = user.Disabled;
            existing.EmailVerified = user.EmailVerified;
            existing.LastLogin = user.LastLogin;
            existing.Image = user.Image;
            existing.Role = user.Role;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
