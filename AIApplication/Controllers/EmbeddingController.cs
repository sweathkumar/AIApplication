using AIApplication.Service;
using Microsoft.AspNetCore.Mvc;

namespace AIApplication.Controllers
{
    public class EmbeddingController : ControllerBase
    {
        private readonly OllamaService _ollama;

        public EmbeddingController(OllamaService ollama)
        {
            _ollama = ollama;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string text)
        {
            try
            {
                var vector = await _ollama.GetEmbedding(text);

                if (vector != null)
                    return Ok(new { embedding = vector });
                else
                    return Ok(new { embedding = "Nothing Returned!" });
            }
            catch (Exception ex)
            {
                return Ok(new { embedding = ex.Message });
            }
        }
    }
}
