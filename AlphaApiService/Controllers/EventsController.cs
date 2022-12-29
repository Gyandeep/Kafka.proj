using AlphaApiService.Configuration;
using AlphaApiService.Entities;
using Common;
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
                //var keys = new string[] { "key1", "key2" };
                //Random rnd = new Random();

                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    var message = new WorkerMessage() { Count = i, Message = $"Hello World - {i + 1}" };
                    var delivery = await producer.ProduceAsync(message.Topic, new Message<string, string> { Value = JsonUtility.SerializeMessage<WorkerMessage>(message) });
                    eventEntity.Status = delivery.Status.ToString();
                }

                //var counter = eventEntity.MessagesCount;

                //while (counter-- > 0)
                //{

                //}
            }

            return new JsonResult(eventEntity);
        }

        [HttpPost("queuemessages")]
        public async Task<IActionResult> QueueMessagesAsync(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                Random rnd = new Random();

                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    JobMessage message = null;

                    switch (rnd.Next(0, 3))
                    {
                        case 0:
                            message = new MessageAlpha();
                            break;
                        case 1:
                            message = new MessageBeta();
                            break;
                        case 2:
                            message = new BadMessage();
                            break;
                    }

                    //// special handling to inject bad message
                    //if (i == eventEntity.MessagesCount - 2)
                    //{
                    //    message = new BadMessage();
                    //}

                    var queueMessage = new QueueMessage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = message
                    };
                    
                    var delivery = await producer.ProduceAsync(message.Topic, new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                    eventEntity.Status = delivery.Status.ToString();
                }
            }

            return new JsonResult(eventEntity);
        }

        [HttpPost("queuebadmessages")]
        public async Task<IActionResult> QueueBadMessagesAsync(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    var message = new BadMessage();

                    var queueMessage = new QueueMessage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = message
                    };

                    var delivery = await producer.ProduceAsync(message.Topic, new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                    eventEntity.Status = delivery.Status.ToString();
                }
            }

            return new JsonResult(eventEntity);
        }

        [HttpPost("schedulemessages")]
        public async Task<IActionResult> ScheduleMessagesAsync(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                Random random = new Random();
                
                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    var message = new MessageAlpha();

                    var queueMessage = new QueueMessage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = message,
                        ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(17) //DateTime.UtcNow.AddMinutes(random.Next(1, 60))
                    };
                    var sameDayQueueMessage = new QueueMessage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = message,
                        ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(12)
                    };

                    var delivery = await producer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                    _ = await producer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(sameDayQueueMessage) });
                    eventEntity.Status = delivery.Status.ToString();
                }
            }

            return new JsonResult(eventEntity);
        }

        [HttpPost("schedulesamedaymessages")]
        public async Task<IActionResult> ScheduleSamedayMessagesAsync(EventEntity eventEntity)
        {
            using (var producer = new ProducerBuilder<string, string>(_kafkaConfig.GetConfigurations()).Build())
            {
                Random random = new Random();

                for (int i = 0; i < eventEntity.MessagesCount; i++)
                {
                    var message = new MessageAlpha();

                    //var queueMessage = new QueueMessage()
                    //{
                    //    Id = Guid.NewGuid().ToString(),
                    //    Message = message,
                    //    ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(17) //DateTime.UtcNow.AddMinutes(random.Next(1, 60))
                    //};
                    var sameDayQueueMessage = new QueueMessage()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = message,
                        ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(3)
                    };

                    //var delivery = await producer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                    var delivery = await producer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(sameDayQueueMessage) });
                    eventEntity.Status = delivery.Status.ToString();
                }
            }

            return new JsonResult(eventEntity);
        }
    }
}
