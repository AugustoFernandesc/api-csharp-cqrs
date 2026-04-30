using Microsoft.EntityFrameworkCore;
using MinhaApiCQRS.Application.Interfaces;
using MinhaApiCQRS.Domain.Entities;
using MinhaApiCQRS.Infrastructure.Data;
using Shared.Pagination;

namespace MinhaApiCQRS.Infrastructure.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ConnectionContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _context.Employees.FirstOrDefaultAsync(employee => employee.Email == email);

    public async Task<bool> IsEmailAllReadyInUse(string email, Guid? id = null)
    {
        if (id.HasValue)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email && e.Id != id.Value);
        }

        return await _context.Employees.AnyAsync(e => e.Email == email);
    }

}
