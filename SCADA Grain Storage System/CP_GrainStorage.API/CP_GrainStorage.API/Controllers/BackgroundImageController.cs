using CourseProject.BLL.Interfaces;
using CourseProject.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CourseProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackgroundImageController : ControllerBase
    {
        private readonly IBackgroundImageService _service;

        public BackgroundImageController(IBackgroundImageService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBackGroundImage(int id)
        {
            var image = await _service.GetBackgroundImageAsync(id);

            return File(image.Bytes, "image/jpeg");
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult> UploadImageAsync(IFormFile image)
        {
            var memoryStream = new MemoryStream();
            image.CopyTo(memoryStream);
            await _service.UploadImageAsync(memoryStream);

            return NoContent();
        }
    }
}