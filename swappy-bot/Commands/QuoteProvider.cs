namespace SwappyBot.Commands
{
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using FluentResults;
    using Microsoft.Extensions.Logging;
    using SwappyBot.Configuration;

    public static class QuoteProvider
    {
        public static async Task<Result<QuoteResponse>> GetQuoteAsync(
            ILogger logger,
            BotConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            decimal amount,
            AssetInfo assetFrom,
            AssetInfo assetTo)
        {
            using var client = httpClientFactory.CreateClient("Broker");
            
            var quoteRequest =
                $"quotes" +
                $"?amount={amount}" +
                $"&sourceAsset={assetFrom.Id}" +
                $"&destinationAsset={assetTo.Id}" +
                $"&apiKey={configuration.BrokerApiKey}";
            
            var quoteResponse = await client.GetAsync(quoteRequest);

            if (quoteResponse.IsSuccessStatusCode)
            {
                var body = await quoteResponse.Content.ReadAsStringAsync();

                logger.LogInformation(
                    "Broker API returned {StatusCode}: {Body}\nRequest: {QuoteRequest}",
                    quoteResponse.StatusCode,
                    body,
                    quoteRequest);

                var quote = JsonSerializer.Deserialize<QuoteResponse[]>(body);
                
                if (quote == null || quote.Length == 0)
                {
                    logger.LogError(
                        "Broker API returned an empty quote for request: {QuoteRequest}",
                        quoteRequest);

                    return Result.Fail("Something has gone wrong while fetching a quote.");
                }

                var bestQuote = quote.OrderByDescending(x => x.EgressAmount).First();
                return Result.Ok(bestQuote);
            }

            var error = await quoteResponse.Content.ReadAsStringAsync();
            var problem = JsonSerializer.Deserialize<ProblemDetailsResponse>(error);
            
            logger.LogError(
                "Broker API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
                quoteResponse.StatusCode,
                error,
                quoteRequest);

            return Result.Fail(
                problem == null 
                    ? "Something has gone wrong while fetching a quote." 
                    : problem.Detail);
        }
    }

    
    public class QuoteResponse
    {
        [JsonPropertyName("type")]
        public string SwapType { get; set; }
        
        [JsonPropertyName("ingressAmount")]
        public decimal IngressAmount { get; set; }
        
        [JsonPropertyName("egressAmount")]
        public decimal EgressAmount { get; set; }
        
        [JsonPropertyName("estimatedPrice")]
        public decimal EstimatedPrice { get; set; }
        
        [JsonPropertyName("numberOfChunks")]
        public int? NumberOfChunks { get; set; }
        
        [JsonPropertyName("chunkIntervalBlocks")]
        public int? ChunkIntervalBlocks { get; set; }
    }
}