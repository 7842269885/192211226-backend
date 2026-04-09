using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrowsmartAPI.Models
{
    public class ProfileImageUploadRequest
    {
        [FromForm(Name = "userId")]
        public int UserId { get; set; }

        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
