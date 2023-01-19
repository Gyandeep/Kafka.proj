using AlphaApiService.Client;
using AlphaApiService.Configuration;
using AlphaApiService.Entities;
using Common;
using Common.Messages;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AlphaApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly KafkaDependentAdmin kafkaDependentAdmin;

        public AdminController(KafkaDependentAdmin kafkaDependentAdmin)
        {
            this.kafkaDependentAdmin = kafkaDependentAdmin;   
        }

        [HttpPost]
        [Route("topic-create")]
        public async Task Post([FromBody] CreateTopicRequest createTopicModel)
        {
            await this.kafkaDependentAdmin.CreateTopicAsync(new List<TopicSpecification> { new TopicSpecification()
            {
                Name = createTopicModel.TopicName,
                NumPartitions = createTopicModel.NumberOfPartitions ?? 1, 
                //ReplicationFactor = createTopicModel.ReplicationFactor ?? 1
            }
            });
        }
    }
}
