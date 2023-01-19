namespace Admin.UI.Controllers
{
    using Admin.UI.Client;
    using Admin.UI.Models;

    using Common;
    using Common.Messages;

    using Confluent.Kafka;
    using Confluent.Kafka.Admin;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly KafkaDependentAdmin kafkaDependentAdmin;
        private readonly KafkaDependentProducer<string, string> kafkaDependentProducer;

        public AdminController(ILogger<AdminController> logger, KafkaDependentAdmin kafkaDependentAdmin,
            KafkaDependentProducer<string, string> producer)
        {
            _logger = logger;
            this.kafkaDependentAdmin = kafkaDependentAdmin ?? throw new ArgumentNullException(nameof(kafkaDependentAdmin));
            this.kafkaDependentProducer = producer ?? throw new ArgumentNullException(nameof(producer));
        }

        [HttpPost]
        [Route("topic-create")]
        public async Task Post([FromBody]CreateTopicRequest createTopicModel)
        {
            await this.kafkaDependentAdmin.CreateTopicAsync(new List<TopicSpecification> { new TopicSpecification()
            {
                Name = createTopicModel.TopicName,
                NumPartitions = createTopicModel.NumberOfPartitions ?? 1, 
                //ReplicationFactor = createTopicModel.ReplicationFactor ?? 1
            }
            });
        }

        [HttpGet]
        [Route("metadata")]
        public JsonResult Get()
        {
            var metadata = this.kafkaDependentAdmin.GetMetadata();
            return new JsonResult(metadata);
        }

        [HttpPost("schedulesamedaymessages")]
        public async Task<IActionResult> ScheduleSamedayMessagesAsync(QueueMessageRequest queueMessageRequest)
        {
            for (int i = 0; i < queueMessageRequest.MessagesCount; i++)
            {
                var message = new MessageAlpha();

                var sameDayQueueMessage = new QueueMessage()
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = message,
                    ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(3)
                };

                //var delivery = await producer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                var delivery = await this.kafkaDependentProducer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(sameDayQueueMessage) });
                queueMessageRequest.Status = delivery.Status.ToString();
            }

            return new JsonResult(queueMessageRequest);
        }

        [HttpPost("schedulefuturemessages")]
        public async Task<IActionResult> ScheduleFutureMessagesAsync(QueueMessageRequest eventEntity)
        {
            for (int i = 0; i < eventEntity.MessagesCount; i++)
            {
                var message = new MessageAlpha();
                var sameDayQueueMessage = new QueueMessage()
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = message,
                    ScheduledDateTimeUtc = DateTime.UtcNow.AddMinutes(16)
                };

                var delivery = await kafkaDependentProducer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(sameDayQueueMessage) });
                eventEntity.Status = delivery.Status.ToString();
            }

            return new JsonResult(eventEntity);
        }

        [HttpPost("queueimmediatemessages")]
        public async Task<IActionResult> QueueImmediateMessageAsync(QueueMessageRequest eventEntity)
        {
            for (int i = 0; i < eventEntity.MessagesCount; i++)
            {
                var message = new MessageAlpha();

                var sameDayQueueMessage = new QueueMessage()
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = message,
                    ScheduledDateTimeUtc = DateTime.UtcNow
                };

                var delivery = await kafkaDependentProducer.ProduceAsync("schedulingqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(sameDayQueueMessage) });
                eventEntity.Status = delivery.Status.ToString();
            }

            return new JsonResult(eventEntity);
        }

        [HttpGet]
        [Route("topic-stats")]
        public JsonResult GetTopicStats()
        {
            var metadata = kafkaDependentProducer.ReadContent();
            return new JsonResult(metadata);
        }
    }
}