using Microsoft.AspNetCore.Mvc;
using AlphaAPI.Models;
using Confluent.Kafka;

namespace AlphaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> ProduceMessage(Event eventEntity)
        {
           // var builder = new ProducerBuilder<string, string>()

            return new JsonResult("complete");
        }
    }
}
