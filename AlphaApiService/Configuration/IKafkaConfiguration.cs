namespace AlphaApiService.Configuration
{
    public interface IKafkaConfiguration
    {
        IEnumerable<KeyValuePair<string, string>> GetConfigurations();
    }
}
