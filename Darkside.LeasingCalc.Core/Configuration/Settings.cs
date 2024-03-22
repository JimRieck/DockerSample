using Microsoft.Extensions.Configuration;

namespace Darkside.LeasingCalc.Core.Configuration
{
    public class Settings : ISettings
    {
        protected readonly IConfiguration _configuration;

        public Settings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SqlDbConnectionString => _configuration["ConnectionStrings:SQlDatabase"];
        public string BlobContainerUrl => _configuration["ConnectionStrings:StorageContainer"];
    }
}
