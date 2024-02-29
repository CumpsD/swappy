namespace SwappyBot.Commands
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SwappyBot.Configuration;

    public static class QuoteProvider
    {
        public static async Task<QuoteResponse?> GetQuoteAsync(
            ILogger logger,
            BotConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            double amount,
            AssetInfo assetFrom,
            AssetInfo assetTo)
        {
            using var client = httpClientFactory.CreateClient("Broker");
            
            var quoteRequest =
                $"quote" +
                $"?amount={amount:0}" +
                $"&sourceAsset={assetFrom.Id}" +
                $"&destinationAsset={assetTo.Id}" +
                $"&apiKey={configuration.BrokerApiKey}";
            
            var quoteResponse = await client.GetAsync(quoteRequest);

            if (quoteResponse.IsSuccessStatusCode)
            {
                var quote = await quoteResponse.Content.ReadFromJsonAsync<QuoteResponse>();
                
                var convertedAmount = amount * Math.Pow(10, assetFrom.Decimals);
                quote.IngressAmount = convertedAmount.ToString(CultureInfo.InvariantCulture);
                
                return quote;
            }

            logger.LogError(
                "Broker API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
                quoteResponse.StatusCode,
                await quoteResponse.Content.ReadAsStringAsync(),
                quoteRequest);

            return null;
        }
    }

    public class QuoteResponse
    {
        [JsonIgnore]
        public string IngressAmount { get; set; }
        
        [JsonPropertyName("egressAmount")]
        public string EgressAmount { get; set; }

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