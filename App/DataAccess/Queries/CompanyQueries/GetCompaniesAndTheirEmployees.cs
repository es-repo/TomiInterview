using App.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace App.DataAccess.Queries
{
    public static partial class CompanyQueries
    {
        public static List<(string Company, List<string> Employees)> GetCompaniesAndTheirEmployees(this AppDbContext dbContext)
        {
            return GetCompaniesAndTheirEmployees(
                dbContext.Companies.AsNoTracking(),
                dbContext.Users.AsNoTracking());
        }

        public static List<(string Company, List<string> Employers)> GetCompaniesAndTheirEmployees(IQueryable<Company> companies, IQueryable<User> users)
        {
            return companies
                .Join(
                    users,
                    company => company.Id,
                    user => user.EmployerId,
                    (company, user) =>
                        new
                        {
                            CompanyName = company.Name,
                            UserName = user.Name,
                        }
                    )
                .GroupBy(o => o.CompanyName)
                .Select(g => new
                {
                    CompanyName = g.Key,
                    UserNames = g.Select(u => u.UserName),
                })
                .ToList()
                .Select(o => (Company: o.CompanyName, Employees: o.UserNames.ToList()))
                .ToList();
        }
    }
}
