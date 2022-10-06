using Microsoft.EntityFrameworkCore;

namespace App.DataAccess.Queries
{
    public static partial class UserQueries
    {
        public static List<string> GetEmailsUsersDontHave(
            this AppDbContext dbContext,
            IEnumerable<string> emails)
        {
            return GetEmailsUsersDontHave(dbContext.Users.AsNoTracking(), emails);
        }

        public static List<string> GetEmailsUsersDontHave(
            IQueryable<User> users,
            IEnumerable<string> emails)
        {
            // NOTE: The code below will generate SQL WHERE ... IN ... clause.
            // Need to set a sanity limit for number of items in the IN list.
            // 
            // Alternative solution with better performance which will require only one round trip to data base server
            // will be to use Table-Values parameter: https://stackoverflow.com/a/71523982
            const int whereInLimit = 3; // 32768 

            return emails
                .Chunk(whereInLimit)
                .Select(emailsChunk => GetEmailsUsersDontHaveNotChunked(users, emailsChunk))
                .SelectMany(email => email)
                .ToList();
        }

        internal static List<string> GetEmailsUsersDontHaveNotChunked(
            IQueryable<User> users,
            string[] emails)
        {
            var existedEmails = users
                .Select(user => user.Email)
                .Where(email => emails.Contains(email))
                .ToList();

            return emails
                .Where(email => !existedEmails.Contains(email))
                .ToList();
        }
    }
}
