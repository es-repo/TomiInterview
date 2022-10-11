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
            // NOTE: To find out which emails are not used by users we can select one of the two general approaches:

            // 1. Load the emails to the Data Base server and check on the Data Base server side which emails are used.

            // 2. Load all user emails from the Data Base server to the Application server and filter out
            // emails that are used on the Application server side.


            // Let's consider how we can implement both approaches and try to find which issues we can face.

            // 1. To implement the first approach we can consider following ways: 

            //      1) The most obvious way is to generate an SQL query of the form: SELECT ... WHERE ... IN (email1, email2, email3, ...).
            //      But in the case of very large email lists the query may reach the limit when a SQL Data Base server will refuse to process it.
            //      There are two kinds of limits:
            // 
            //          - The SQL batch size limit. Which equals to 65,536 * (network packet size) for MS SQL server.
            //          It will be equal to 256Mb if the network packet size is default 4kb.
            //          Source: https://learn.microsoft.com/en-us/sql/sql-server/maximum-capacity-specifications-for-sql-server?redirectedfrom=MSDN&view=sql-server-ver16
            // 
            //          - The limit of values in the IN(...) clause.
            //          MS SQL server doesn't specify this limit but experimentally known it should be about 30000.
            //          Source: https://dba.stackexchange.com/a/228982
            // 
            //      Assuming the average size of an email address is about 40 bytes we can not worry about SQL batch size limit but
            //      just limit the count of emails in IN (...) clause to 30000. 
            // 
            //      Pros: 
            //          - This way of the implementation of the problem is unit-testable.
            // 
            //      Cons: 
            //          - If we have to process more than 30000 emails we have to make more than one round trip to the Data Base server.
            //

            //      2) Another way is to upload all emails to a temp table and then generate an SQL query of the form SELECT ... WHERE ... IN(SELECT * FROM the_temp_table).
            //      In the case of MS SQL server we can use table-value parameter instead of an temp table. See: https://stackoverflow.com/a/71523982 
            // 
            //      Pros: 
            //          - Avoiding limitation of values in IN(...) clause.
            // 
            //      Cons: 
            //          - On uploading emails to the temp table or using table-value parameter we should still worry about the SQL batch size limit.
            //          So more thanthen one round trip to the Data Base server may be unavoidable.
            //          - This way of the implementation of the problem is not unit-testable.
            //          - It requires to create a DB migration for the temp table or table-value parameter type.
            //          Which means more burden on the deployment.
            // 
            //      
            //      3) The third way is to use SQL UNION operator. The SQL statement should look about like this:  
            // 
            //      SELECT *
            //          FROM(
            //              VALUES('user_1_1@email.com'), ('user_1_2@email.com'), ('user_1_1@email.com'), ('user_9_1@email.com'), ('user_9_2@email.com')
            //              ) AS Emails(a)
            //
            //          EXCEPT
            //
            //      SELECT Email FROM Users
            //
            //      Pros and Cons are same as in the second way except that the query can be generated on the application side and does not require DB migration.
            //          
            //          
            //
            // 2. To implement the second approach we can load all user emails from the Data Base into memory
            // and then compare them with emails from the file. In this approach we have to be sure that
            // all emails from the data base fits into the memory.
            // 
            // Pros:
            //  - Simple.
            //  - Unit-testable.
            // 
            // Cons: 
            //  - Works only if the amount of emails in the database base is small enough to fit into memory.
            //
            const int whereInLimit = 3; // 30000 

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
                .Except(existedEmails)
                .ToList();
        }
    }
}
