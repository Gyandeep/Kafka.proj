using AlphaApiService.Client;
using AlphaApiService.Configuration;

namespace AlphaApiService.Dependency
{
    public static class DependencyExtension
    {
        public static void AddDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IKafkaConfiguration, KafkaConfiguration>();
            serviceCollection.AddSingleton<AdminClientHandle>();
            serviceCollection.AddSingleton<KafkaDependentAdmin>();
        }
    }
}
