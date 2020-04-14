using System.Threading.Tasks;
using BTProb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTProb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FilesController : BaseController
    {
        private readonly FileService _fileService;

        public FilesController(FileService service)
        {
            _fileService = service;
        }

        [HttpPost]
        public async Task<IActionResult> WriteFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file");
            }

            _fileService.WriteFileToDisk(file, file.FileName);

            return Ok("Writing to disk");
        }
    }
}