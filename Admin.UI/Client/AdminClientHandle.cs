namespace Admin.UI.Client
{
    using Confluent.Kafka;

    public class AdminClientHandle : IDisposable
    {
        private readonly IAdminClient adminClient;

        public AdminClientHandle(IConfiguration config)
        {
            var adminConfig = new AdminClientConfig();
            config.GetSection("Kafka:AdminSettings").Bind(adminConfig);
            adminClient = new AdminClientBuilder(adminConfig).Build();
        }

        public Handle Handle => adminClient.Handle;

        public void Dispose()
        {
            adminClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
