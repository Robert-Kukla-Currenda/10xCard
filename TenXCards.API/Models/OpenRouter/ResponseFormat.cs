namespace TenXCards.API.Models.OpenRouter
{
    public class ResponseFormat
    {
        public string Type { get; set; } = "json_schema";
        public JsonSchema JsonSchema { get; set; } = new();
    }

    public class JsonSchema
    {
        public string Name { get; set; } = "chatCompletionResponse";
        public bool Strict { get; set; } = true;
        public Dictionary<string, string> Schema { get; set; } = new();
    }

    public class ChatPayload
    {
        public string Model { get; set; } = string.Empty;
        public List<Message> Messages { get; set; } = new();
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
        public ResponseFormat ResponseFormat { get; set; } = new();
    }
}