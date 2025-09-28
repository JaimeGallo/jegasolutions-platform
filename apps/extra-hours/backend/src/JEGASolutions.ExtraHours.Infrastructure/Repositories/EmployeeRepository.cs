using Microsoft.EntityFrameworkCore;
using JEGASolutions.ExtraHours.Core.Entities.Models;
using JEGASolutions.ExtraHours.Core.Interfaces;
using JEGASolutions.ExtraHours.Core.Dto;
using JEGASolutions.ExtraHours.Data;

namespace JEGASolutions.ExtraHours.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeesByManagerIdAsync(long managerId)
        {
            return await _context.employees
                .Include(e => e.manager)
                .ThenInclude(m => m!.User)
                .Where(e => e.manager_id == managerId)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(long id)
        {
            return await _context.employees
                .Include(e => e.manager)
                .ThenInclude(m => m!.User)
                .FirstOrDefaultAsync(e => e.id == id);
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.employees.AnyAsync(e => e.id == id);
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.employees
                .Include(e => e.manager)
                .ThenInclude(m => m!.User)
                .ToListAsync();
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await _context.employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var employee = await _context.employees.FindAsync(id);
            if (employee != null)
            {
                _context.employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Empleado no encontrado");
            }
        }

        public async Task<bool> EmployeeExistsAsync(long id)
        {
            return await _context.employees
                .AsNoTracking()
                .AnyAsync(e => e.id == id);
        }

        public async Task<Employee> UpdateEmployeeAsync(long id, UpdateEmployeeDTO dto)
        {
            Console.WriteLine($"Updating employee with ID: {id}");
            Console.WriteLine($"Received DTO: Name={dto.Name}, Position={dto.Position}, Salary={dto.Salary}, ManagerId={dto.ManagerId}");

            var employee = await _context.employees
                .Include(e => e.manager)
                .FirstOrDefaultAsync(e => e.id == id);

            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado");

            employee.name = dto.Name ?? employee.name;
            employee.position = dto.Position ?? employee.position;
            employee.salary = (double?)(dto.Salary ?? (decimal?)employee.salary);

            if (dto.ManagerId.HasValue)
            {
                // Verificar que el nuevo manager exista
                var manager = await _context.managers.FindAsync(dto.ManagerId.Value);
                if (manager == null)
                    throw new KeyNotFoundException("Manager no encontrado");

                // Actualizar la relación
                employee.manager_id = dto.ManagerId.Value;
                employee.manager = manager;
            }

            await _context.SaveChangesAsync();

            // Recargar la entidad para obtener los datos actualizados
            await _context.Entry(employee).ReloadAsync();

            return employee;
        }
    }
}