using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace BuyingCatalogueTests.utils.EnvData
{
    internal class EnvironmentService
    {
        internal IEnvironment environment;
        internal IUser user;

        public EnvironmentService()
        {
            var dse = new Deserializer();

            using (var yamlStream = File.OpenRead(GetDataFile()))
            {
                using (var yamlReader = new StreamReader(yamlStream, Encoding.UTF8, true))
                {
                    environment = dse.Deserialize<Environment>(yamlReader);
                }
            }

            // Get the user to be used in each test
            user = environment.Users[new Random().Next(environment.Users.Count)];
        }

        private string GetDataFile()
        {
            return TestContext.Parameters.Get("envData", $"{AppContext.BaseDirectory}utils\\EnvData\\Data\\Test.yml");
        }
    }
}