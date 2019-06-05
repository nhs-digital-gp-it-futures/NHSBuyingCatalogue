using Bogus;

namespace BuyingCatalogueTests.utils
{
    internal static class TestDataGenerator
    {
        internal static ContactDetails GenerateTestContact()
        {
            // Generates a new instance of the ContactDetails class using faker data
            Faker<ContactDetails> details = new Faker<ContactDetails>("en_GB")
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.EmailAddress, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.JobTitle, f => f.Name.JobTitle());

            return details;
        }
    }

    internal class ContactDetails
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string JobTitle { get; set; }
    }
}
