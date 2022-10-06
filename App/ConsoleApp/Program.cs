using App.DataAccess;
using App.DataAccess.Queries;
using Microsoft.Extensions.Configuration;

namespace App.ConsoleApp
{
    internal class Program
    {
        public static void Main()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            var connectionString = config.GetConnectionString("App");

            using (var dbContext = new AppDbContext(connectionString))
            {
                OutputCompaniesAndTheirEmployees(dbContext);

                var emails = ReadEmailsFromFile("emails.txt");
                OutputEmailsUsersDontHave(dbContext, emails);
            }
        }

        private static void OutputCompaniesAndTheirEmployees(AppDbContext dbContext)
        {
            Console.WriteLine("Companies and their employees:");

            var companiesAndTheirEmployees = dbContext.GetCompaniesAndTheirEmployees();

            foreach (var companyAndTheirEmployees in companiesAndTheirEmployees)
            {
                Console.WriteLine(companyAndTheirEmployees.Company);

                foreach (var emploeer in companyAndTheirEmployees.Employees)
                {
                    Console.WriteLine("\t" + emploeer);
                }

                Console.WriteLine();
            }
        }

        private static void OutputEmailsUsersDontHave(AppDbContext dbContext, IEnumerable<string> emails)
        {
            Console.WriteLine("Emails users don't have:");

            var emailUsersDontHave = dbContext.GetEmailsUsersDontHave(emails);

            foreach (var email in emailUsersDontHave)
            {
                Console.WriteLine(email);
            }
        }

        private static IEnumerable<string> ReadEmailsFromFile(string fileName)
        {
            // NOTE: In case very large files all lines may not fit into memory.
            // Modify the implementation to read lines by chunks. 
            return File.ReadAllLines(fileName)
                    .Where(line => line.Trim() != "");
        }
    }
}
