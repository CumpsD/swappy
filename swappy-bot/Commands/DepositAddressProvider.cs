namespace SwappyBot.Commands
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using SwappyBot.Configuration;

    public static class DepositAddressProvider
    {
        public static async Task<DepositAddressResponse?> GetDepositAddressAsync(
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
                $"?amount={amount:0}" +
                $"&sourceAsset={assetFrom.Id}" +
                $"&destinationAsset={assetTo.Id}" +
                $"&destinationAddress={destinationAddress}" +
                $"&apiKey={configuration.BrokerApiKey}";
            
            var swapResponse = await client.GetAsync(swapRequest);
            
            if (swapResponse.IsSuccessStatusCode)
                return await swapResponse.Content.ReadFromJsonAsync<DepositAddressResponse>();

            logger.LogError(
                "Broker API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
                swapResponse.StatusCode,
                await swapResponse.Content.ReadAsStringAsync(),
                swapRequest);

            return null;
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