using App.DataAccess;
using App.DataAccess.Queries;
using System.Collections;

namespace DataAccess.Tests.Queries.UserQueriesTests
{
    public static class GetEmailsUsersDontHaveTest
    {
        public sealed record Args 
        {
            public IQueryable<User> Users { get; init; } = Enumerable.Empty<User>().AsQueryable();
            public IEnumerable<string> Emails { get; init; } = Enumerable.Empty<string>();
        }

        sealed class TestCases : IEnumerable<object[]>
        {
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return EmptyListOfEmails_EmptyListExpected_1();
                yield return EmptyListOfUsers_FullListExpected_2();
                yield return EmailsAndUsersProided_EmailsWhichUsersDontHaveExpected_3();
            }

            static object[] EmptyListOfEmails_EmptyListExpected_1()
            {
                var args = new Args
                {
                    Users = new List<User>
                    {
                        new User
                        {
                            Name = "User 1 1",
                            Email = "user_1_1@email.com"
                        },

                        new User
                        {
                            Name = "User 1 2",
                            Email = "user_1_2@email.com"
                        }
                    }.AsQueryable(),

                    Emails = new List<string>
                    {
                    }
                };

                var expected = new List<string>
                {
                };

                return new object[] { args, expected };
            }

            static object[] EmptyListOfUsers_FullListExpected_2()
            {
                var args = new Args
                {
                    Users = new List<User>
                    {
                    }.AsQueryable(),

                    Emails = new List<string>
                    {
                        "user_1_1@email.com",
                        "user_9_1@email.com",
                        "user_9_2@email.com",
                    }
                };

                var expected = args.Emails.ToList();

                return new object[] { args, expected };
            }

            static object[] EmailsAndUsersProided_EmailsWhichUsersDontHaveExpected_3()
            {
                var args = new Args
                {
                    Users = new List<User>
                    {
                        new User
                        {
                            Name = "User 1 1",
                            Email = "user_1_1@email.com"
                        },

                        new User
                        {
                            Name = "User 1 2",
                            Email = "user_1_2@email.com"
                        }
                    }.AsQueryable(),

                    Emails = new List<string>
                    {
                        "user_1_1@email.com",
                        "user_9_1@email.com",
                        "user_9_2@email.com",
                    }
                };

                var expected = new List<string> 
                {
                    "user_9_1@email.com",
                    "user_9_2@email.com",
                };

                return new object[] { args, expected };
            }
        }

        [Theory]
        [ClassData(typeof(TestCases))]
        public static void Test(Args args, List<string> expected)
        {
            var actual = UserQueries.GetEmailsUsersDontHave(args.Users, args.Emails);

            Assert.Equal(expected, actual);
        }
    }
}
