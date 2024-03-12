namespace SwappyBot.Commands
{
    using System.Text.Json.Serialization;

    public class ProblemDetailsResponse
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }
}