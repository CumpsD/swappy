namespace SwappyBot.Commands
{
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
            decimal amount,
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
                return await quoteResponse.Content.ReadFromJsonAsync<QuoteResponse>();

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
        [JsonPropertyName("ingressAmount")]
        public decimal IngressAmount { get; set; }
        
        [JsonPropertyName("egressAmount")]
        public decimal EgressAmount { get; set; }
    }
}