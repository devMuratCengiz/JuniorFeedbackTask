using JuniorFeedbackTask.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JuniorFeedbackTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ILogger<FeedbackController> _logger;
        private readonly PublisherService _rabbitmqService;

        public FeedbackController(ILogger<FeedbackController> logger, PublisherService rabbitmqService)
        {
            _logger = logger;
            _rabbitmqService = rabbitmqService;
        }

        [HttpPost]
        public IActionResult SendFeedback(FeedbackDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz Model: {@ModelState}", ModelState);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        PropertyName = x.Key,
                        ErrorMessage = x.Value.Errors.First().ErrorMessage
                    }).ToList();

                return BadRequest(new { errors });
            }
            try
            {
                _logger.LogInformation("Feedback alındı: {@Model}", model);
                _rabbitmqService.SendToQueue(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Feedback gönderilirken hata oluştu.");
                return StatusCode(500, new { message = "Sunucuda bir hata oluştu." });
            }
        }


    }
}
