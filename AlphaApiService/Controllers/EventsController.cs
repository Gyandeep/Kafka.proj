using AlphaApiService.Configuration;
using AlphaApiService.Entities;
using AlphaApiService.Messages;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AlphaApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IKafkaConfiguration _kafkaConfig;
        public EventsController(IKafkaConfiguration kafkaConfiguration)
        {
            _kafkaConfig = kafkaConfiguration;
        }

        [HttpPost("message1")]
        public async Task<IActionResult> ProcessEvent(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                var message1 = new Message1()
                {
                    Key = eventEntity.Key,
                    Message = eventEntity.Message,
                };

                var delivery = await producer.ProduceAsync(message1.Topic, new Message<string, string> 
                { Key = message1.Key, Value = SerializeMessage(message1) });
                 eventEntity.Status = delivery.Status.ToString();
            }

            return new JsonResult(eventEntity);
        }

        private string SerializeMessage(JobMessage jobMessage)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>() { new StringEnumConverter() }
            };

            return JsonConvert.SerializeObject(jobMessage, Formatting.None, setting);
        }
    }
}
