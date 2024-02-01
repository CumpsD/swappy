namespace SwappyBot.Commands.Swap
{
    using System.Text.Json.Serialization;

    public class QuoteResponse
    {
        [JsonIgnore]
        public string IngressAmount { get; set; }
        
        [JsonPropertyName("egressAmount")]
        public string EgressAmount { get; set; }

        [JsonPropertyName("intermediateAmount")]
        public string IntermediateAmount { get; set; }
        
        [JsonPropertyName("includedFees")]
        public QuoteFees[] Fees { get; set; }
    }

    public class QuoteFees
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("chain")]
        public string Chain { get; set; }
        
        [JsonPropertyName("asset")]
        public string Asset { get; set; }
        
        [JsonPropertyName("amount")]
        public string Amount { get; set; }
    }
}