using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal sealed class EnvironmentService
    {
        internal IEnvironment environment;
        internal IUser user;

        public EnvironmentService()
        {
            environment = new Environment()
            {
                CatalogueUrl = GetUrlFromEnv(),
                Users = GetUsersFromEnv().ToList()
            };

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
    }
}