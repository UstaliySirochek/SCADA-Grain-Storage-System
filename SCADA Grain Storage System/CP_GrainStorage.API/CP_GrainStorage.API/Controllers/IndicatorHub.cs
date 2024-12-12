using CourseProject.BLL.Interfaces;
using Microsoft.AspNetCore.SignalR;
using CP_GrainStorage.API.Controllers;

namespace CP_GrainStorage.API.Controllers
{
    public class IndicatorHub : Hub
    {
        private readonly IIndicatorService indicatorService;

        public IndicatorHub(IIndicatorService service)
        {
            indicatorService = service;
        }

        public async Task SendValue(string id, string value)
        {
            await indicatorService.UpdateIndicatorValue(new()
            {
                Id = Guid.Parse(id),
                Value = value
            });

            await this.Clients.Others.SendAsync("receive", id, value);
        }

        public async Task SendTime()
        {
            var currentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await Clients.All.SendAsync("receiveTime", currentTime);
        }
    }
}