using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AIApplication.Service
{
    public class OllamaService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _embeddingModel;
        private readonly string _chatModel;

        public OllamaService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _baseUrl = config["AISettings:BaseUrl"];
            _embeddingModel = config["AISettings:EmbeddingModel"];
            _chatModel = config["AISettings:ChatModel"];
        }

        public async Task<List<float>> GetEmbedding(string text)
        {
            var body = new
            {
                model = _embeddingModel,
                input = text
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _http.PostAsync($"{_baseUrl}/embeddings", content);

            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);

            using var doc = JsonDocument.Parse(json);

            var embedding = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding");

            var list = new List<float>();

            foreach (var item in embedding.EnumerateArray())
            {
                list.Add(item.GetSingle());
            }

            return list;
        }

        public async Task<string> GenerateResponse(string text)
        {
            try
            {
                var body = new
                {
                    model = _chatModel,
                    messages = new[]
                    {
                        new { role = "user", content = text }
                    }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");

                var response = await _http.PostAsync( $"{_baseUrl}/chat/completions", content);

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);

                //var result = doc.RootElement.GetProperty("response").GetString() ?? doc.RootElement.GetProperty("error").GetString();
                var result = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return result ?? "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
