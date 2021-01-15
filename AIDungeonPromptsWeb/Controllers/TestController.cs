using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AIDungeonPrompts.Web.Controllers
{
	public class TestController : Controller
	{
		private readonly IWebHostEnvironment _environment;

		public TestController(IWebHostEnvironment environment)
		{
			_environment = environment;
		}

		[HttpGet("[controller]/[action]")]
		public Task<string> Get()
		{
			var folder = Path.Combine(_environment.WebRootPath, "backups");
			Directory.CreateDirectory(folder);
			var file = Path.Combine(folder, "file.txt");
			return System.IO.File.ReadAllTextAsync(file);
		}

		[HttpGet("[controller]/[action]")]
		public async Task<IActionResult> Index()
		{
			var folder = Path.Combine(_environment.WebRootPath, "backups");
			Directory.CreateDirectory(folder);
			var file = Path.Combine(folder, "file.txt");
			using var stream = System.IO.File.CreateText(file);
			await stream.WriteLineAsync("Hello World");
			return Ok();
		}
	}
}
