using CourseProject.BLL.Interfaces;
using CourseProject.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CP_GrainStorage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndicatorController : ControllerBase
    {
        private readonly IIndicatorService _service;
        private readonly IHubContext<IndicatorHub> _hubContext;

        public IndicatorController(IIndicatorService service, IHubContext<IndicatorHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetIndicators()
        {
            var indicators = await _service.GetIndicators();

            return Ok(indicators);
        }

        [HttpPost]
        public async Task<ActionResult> CreateIndicator(IndicatorModel model)
        {
            var id = await _service.CreateIndicator(model);

            return Ok(id);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateIndicatorValue(UpdateIndicatorValueModel model)
        {
            await _service.UpdateIndicatorValue(model);
            await _hubContext.Clients.All.SendAsync("receive", model.Id.ToString(), model.Value);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIndicator(Guid id)
        {
            await _service.DeleteIndicator(id);

            return NoContent();
        }
    }
}