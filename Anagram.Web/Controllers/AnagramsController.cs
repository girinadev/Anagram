using Anagram.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using Anagram.Web.Services;

namespace Anagram.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnagramsController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICombinationsService _permutationsService;

        public AnagramsController(IHostingEnvironment environment, ICombinationsService permutationsService)
        {
            _hostingEnvironment = environment;
            _permutationsService = permutationsService;
        }

        // POST api/anagrams
        [HttpPost]
        public ActionResult Post([FromBody] AnagramModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(model);
                }

                var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "dict", $"{model.Language}-dict.txt");
                if (!System.IO.File.Exists(path))
                {
                    return NotFound($"Dictionary {model.Language} file doesn't exist");
                }

                var result = _permutationsService.GetByString(model.Value);
                var dict = System.IO.File.ReadAllLines(path).ToHashSet();
                result = _permutationsService.FilterByDictionary(result, dict);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
