using CourseProject.BLL.Interfaces;
using CourseProject.BLL.Models;
using CourseProject.Core.Entities;
using CourseProject.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseProject.BLL.Services
{
    public class BackgroundImageService : IBackgroundImageService
    {
        private readonly CourseProjectDbContext context;

        public BackgroundImageService(CourseProjectDbContext context)
        {
            this.context = context;
        }

        public async Task<ImageModel> GetBackgroundImageAsync(int id)
        {
            var img = await context.BackgroundImages.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Image with such id does not exist");

            var imageModel = new ImageModel()
            {
                Id = img.Id,
                Bytes = img.Bytes
            };

            return imageModel;
        }

        public async Task UploadImageAsync(MemoryStream stream)
        {
            var image = new BackgroundImage()
            {
                Bytes = stream.ToArray()
            };

            context.Add(image);
            await context.SaveChangesAsync();
        }
    }
}