using Microsoft.EntityFrameworkCore;
using JEGASolutions.ReportBuilder.Core.Entities.Models;
using JEGASolutions.ReportBuilder.Core.Interfaces;
using JEGASolutions.ReportBuilder.Data;

namespace JEGASolutions.ReportBuilder.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync(int tenantId)
        {
            return await _context.Users
                .Where(u => u.TenantId == tenantId && u.DeletedAt == null)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(User user)
        {
            user.MarkAsUpdated();
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id, int tenantId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId && u.DeletedAt == null);
            
            if (user == null)
                return false;

            user.MarkAsDeleted();
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

