using System;
using System.Collections.Generic;
using System.Linq;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal sealed class EnvironmentService
    {
        internal IEnvironment environment;
        internal IUser user;
        internal string Browser;

        public EnvironmentService()
        {
            environment = new Environment()
            {
                CatalogueUrl = GetUrlFromEnv(),
                Users = GetUsersFromEnv().ToList()                
            };

            // Get the browser to run the tests against
            Browser = GetBrowserFromEnv();

            // Get the user to be used in each test
            user = environment.Users[new Random().Next(environment.Users.Count)];
        }

        private string GetUrlFromEnv()
        {
            return System.Environment.GetEnvironmentVariable("CatalogueUrl", EnvironmentVariableTarget.User) ?? throw new ArgumentNullException("'CatalogueUrl' not found");
        }

        private IEnumerable<IUser> GetUsersFromEnv()
        {
            var userString = System.Environment.GetEnvironmentVariable("CatalogueUsers", EnvironmentVariableTarget.User) ?? throw new ArgumentNullException("'CatalogueUsers' not found");

            var userList = userString.Split(';');

            return userList.Select(s => {
                var splitUser = s.Split(',');
                return new User { UserName = splitUser[0].Trim(), Password = splitUser[1].Trim() };
            });            
        }

        private string GetBrowserFromEnv()
        {
            return System.Environment.GetEnvironmentVariable("CatalogueBrowser", EnvironmentVariableTarget.User) ?? throw new ArgumentNullException("'CatalogueBrowser' not found");
        }
    }
}