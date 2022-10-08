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
            // NOTE: There could be at least 3 approaches how to read emails from the file
            // with their own pros and cons. Let's consider them:


            // 1. Read all emails at once using the `File.ReadAllLines` method.
            // This is the currently implemented approach.

            // Pros:
            //  - The simplest one-line solution.

            // Cons: 
            //  - In the case of very large files we can face with memory issues like OutOfMemoryException.


            // 2. Read and process emails in chunks using `File.ReadLines` method.

            // Pros: 
            //  - OutOfMemoryException will be avoided.

            // Cons:
            //  - It's more complex solution.
            //  - Even if OutOfMemoryException will be avoided
            //  the application does not control how much memory it will consume
            //  and should rely on the Garbage Collector to free unused memory.
            //  Relying on the Garbage CollectorColector can also produce performance issues.


            // 3. Allocate a fixed buffer in memory.
            // Fill it with emails and process the email chunk.

            // Pros:
            //  - The most effective in terms of memory consumption and performance solution.

            // Cons:
            //  - The most complex solution.

            return File.ReadAllLines(fileName)
                    .Where(line => line.Trim() != "");
        }
    }
}
