using MathEditor.Domain.Entities;
using MathEditor.Domain.IRepositories;
using MathEditor.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace MathEditor.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<ApplicationUser?> FindUserAsync(string handle)
    {
        if (string.IsNullOrEmpty(handle))
        {
            throw new ArgumentNullException(nameof(handle), "Handle must not be null or empty.");
        }

        // Check if handle is a valid GUID (ID)
        if (Guid.TryParse(handle, out _))
        {
            return await _userManager.FindByIdAsync(handle);
        }

        // Otherwise, search by handle (case-insensitive)
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Handle != null && u.Handle.ToLower() == handle.ToLower());
    }


    public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentNullException(nameof(userId), "User ID must not be null or empty.");
        }

        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email), "Email must not be null or empty.");
        }

        return await _userManager.FindByEmailAsync(email);
    }

    public async Task CreateUserAsync(ApplicationUser user, string password)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User must not be null.");
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Password must not be null or empty.");
        }

        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task UpdateUserAsync(string id, ApplicationUser updateData)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "User ID must not be null or empty.");
        }

        if (updateData == null)
        {
            throw new ArgumentNullException(nameof(updateData), "Update data must not be null.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }

        // Update fields
        user.UserName = updateData.UserName ?? user.UserName;
        user.Email = updateData.Email ?? user.Email;
        user.NormalizedUserName = updateData.NormalizedUserName ?? user.NormalizedUserName;
        user.NormalizedEmail = updateData.NormalizedEmail ?? user.NormalizedEmail;
        user.EmailConfirmed = updateData.EmailConfirmed;
        user.PhoneNumber = updateData.PhoneNumber ?? user.PhoneNumber;
        user.PhoneNumberConfirmed = updateData.PhoneNumberConfirmed;
        user.TwoFactorEnabled = updateData.TwoFactorEnabled;
        user.LockoutEnd = updateData.LockoutEnd;
        user.LockoutEnabled = updateData.LockoutEnabled;
        user.AccessFailedCount = updateData.AccessFailedCount;

        // Custom fields
        user.Handle = updateData.Handle ?? user.Handle;
        user.Name = updateData.Name ?? user.Name;
        user.Image = updateData.Image ?? user.Image;
        user.Role = updateData.Role ?? user.Role;
        user.Disabled = updateData.Disabled;
        user.EmailVerified = updateData.EmailVerified;
        user.LastLogin = updateData.LastLogin;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "User ID must not be null or empty.");
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return true;
    }

}
