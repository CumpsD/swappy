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
            // https://chainflip-swap.chainflip.io/quote?amount=1500000000000000000&srcAsset=ETH&destAsset=BTC
            using var client = httpClientFactory.CreateClient("Quote");

            // var commissionPercent = (double)configuration.CommissionBps / 100;
            // var commission = amount * (commissionPercent / 100);
            // var ingressAmount = amount - commission;
            // var convertedAmount = ingressAmount * Math.Pow(10, assetFrom.Decimals);
            
            var convertedAmount = amount * Math.Pow(10, assetFrom.Decimals);

            var quoteRequest =
                $"quote?amount={convertedAmount:0}&srcAsset={assetFrom.Ticker}&destAsset={assetTo.Ticker}&brokerCommissionBps={configuration.CommissionBps}";
            var quoteResponse = await client.GetAsync(quoteRequest);

            if (quoteResponse.IsSuccessStatusCode)
            {
                var quote = await quoteResponse.Content.ReadFromJsonAsync<QuoteResponse>();
                quote.IngressAmount = convertedAmount.ToString(CultureInfo.InvariantCulture);
                return quote;
            }

            logger.LogError(
                "Quote API returned {StatusCode}: {Error}\nRequest: {QuoteRequest}",
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