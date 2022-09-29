namespace AlphaApiService.Configuration
{
    public class KafkaConfiguration : IKafkaConfiguration
    {
        private IEnumerable<KeyValuePair<string, string>> _configurations;

        public KafkaConfiguration()
        {
            LoadConfig();
        }

        public IEnumerable<KeyValuePair<string, string>> GetConfigurations()
        {
            return _configurations;
        }

        private void LoadConfig()
        {
            var configuration = new ConfigurationBuilder()
                .AddIniFile(@"./Configuration/kafka.properties")
                .Build();

            _configurations = configuration.AsEnumerable();
        }
    }
}
