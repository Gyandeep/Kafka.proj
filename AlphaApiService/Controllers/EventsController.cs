using AlphaApiService.Configuration;
using AlphaApiService.Entities;
using Common.Messages;
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

        [HttpPost("messages")]
        public async Task<IActionResult> ProcessEvent(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                var keys = new string[] { "key1", "key2" };
                Random rnd = new Random();

                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    // JobMessage message = rnd.Next(0, 2) == 1 ? new Message1() { Message = "Hello" } : new Message2() { Count = i };
                    var message = new Message2() { Count = i, Message = $"Hello World - {i + 1}" };
                    var delivery = await producer.ProduceAsync(message.Topic, new Message<string, string>
                    { Key = null, Value = SerializeMessage(message) });
                    eventEntity.Status = delivery.Status.ToString(); 
                }
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
