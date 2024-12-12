using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CP_GrainStorage.API.Controllers;

namespace CP_GrainStorage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly IHubContext<IndicatorHub> _hubContext;

        public TimeController(IHubContext<IndicatorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("start-time-updates")]
        public IActionResult StartTimeUpdates()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var currentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    await _hubContext.Clients.All.SendAsync("receiveTime", currentTime);
                    await Task.Delay(1000);
                }
            });

            return Ok("Time updates started");
        }
    }

}
