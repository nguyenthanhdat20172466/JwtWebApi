using JwtWebApi.Data;
using JwtWebApi.Models;
using JwtWebApi.Services.PhotoService;
using Microsoft.AspNetCore.Mvc;

namespace JwtWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly DataContext _context;

        public PhotoController (IPhotoService photoService, DataContext dataContext)
        {
            _photoService = photoService;
            _context = dataContext;
        }

        [HttpPost("add-photo")]

        public async Task<IActionResult> AddPhoto(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                IsMain = false
            };
            _context.Photos.Add(photo);
            _context.SaveChangesAsync();
            return Ok(photo);
        }
    }
}
