namespace TenXCards.API.Models.OpenRouter
{
    public class Message
    {
        public MessageRole Role { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}