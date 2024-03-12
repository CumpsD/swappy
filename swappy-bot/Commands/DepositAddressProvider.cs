namespace SwappyBot.Commands
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using FluentResults;
    using Microsoft.Extensions.Logging;
    using SwappyBot.Configuration;

    public static class DepositAddressProvider
    {
        public static async Task<Result<DepositAddressResponse>> GetDepositAddressAsync(
            ILogger logger,
            BotConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            decimal amount,
            AssetInfo assetFrom,
            AssetInfo assetTo,
            string destinationAddress)
        {
            using var client = httpClientFactory.CreateClient("Broker");

            var swapRequest =
                $"swap" +
                $"?amount={amount}" +
                $"&sourceAsset={assetFrom.Id}" +
                $"&destinationAsset={assetTo.Id}" +
                $"&destinationAddress={destinationAddress}" +
                $"&apiKey={configuration.BrokerApiKey}";
            
            var swapResponse = await client.GetAsync(swapRequest);
            
            if (swapResponse.IsSuccessStatusCode)
            {
                var body = await swapResponse.Content.ReadAsStringAsync();

                logger.LogInformation(
                    "Broker API returned {StatusCode}: {Body}\nRequest: {SwapRequest}",
                    swapResponse.StatusCode,
                    body,
                    swapRequest);

                var swap = JsonSerializer.Deserialize<DepositAddressResponse>(body);
                return swap == null
                    ? Result.Fail("Something has gone wrong while starting a swap.")
                    : Result.Ok(swap);
            }

            var error = await swapResponse.Content.ReadAsStringAsync();
            var problem = JsonSerializer.Deserialize<ProblemDetailsResponse>(error);

            logger.LogError(
                "Broker API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
                swapResponse.StatusCode,
                error,
                swapRequest);

            return Result.Fail(
                problem == null 
                    ? "Something has gone wrong while starting a swap."
                    : problem.Detail);
        }
    }
    
    public class DepositAddressResponse
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [JsonPropertyName("issuedBlock")]
        public ulong IssuedBlock { get; set; }
        
        [JsonPropertyName("network")]
        public string Network { get; set; }
        
        [JsonPropertyName("channelId")]
        public ulong ChannelId { get; set; }
        
        [JsonPropertyName("explorerUrl")]
        public string ExplorerUrl { get; set; }
    }
}