namespace SwappyBot.Commands
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using FluentResults;
    using Microsoft.Extensions.Logging;
    using SwappyBot.Configuration;

    public static class StatusProvider
    {
        public static async Task<Result<SwapStatusResponse>> GetStatusAsync(
            ILogger logger,
            BotConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            string depositChannel)
        {
            using var client = httpClientFactory.CreateClient("Broker");
         
            var depositParts = depositChannel!.Split('-');

            var issuedBlock = depositParts[0];
            var network = depositParts[1];
            var channelId = depositParts[2];
            
            var statusRequest =
                $"status-by-deposit-channel" +
                $"?issuedBlock={issuedBlock}" +
                $"&network={network}" +
                $"&channelId={channelId}" +
                $"&apiKey={configuration.BrokerApiKey}";
            
            var statusResponse = await client.GetAsync(statusRequest);

            if (statusResponse.IsSuccessStatusCode)
            {
                var body = await statusResponse.Content.ReadAsStringAsync();

                logger.LogInformation(
                    "Broker API returned {StatusCode}: {Body}\nRequest: {StatusRequest}",
                    statusResponse.StatusCode,
                    body,
                    statusRequest);

                if (string.IsNullOrWhiteSpace(body))
                    return Result.Fail("Something has gone wrong while fetching swap status.");

                var status = JsonSerializer.Deserialize<SwapStatusResponse>(body);
                if (status == null)
                    return Result.Fail("Something has gone wrong while fetching swap status.");

                status.Body = body;
                return Result.Ok(status);
            }

            var error = await statusResponse.Content.ReadAsStringAsync();
            var problem = JsonSerializer.Deserialize<ProblemDetailsResponse>(error);
            
            logger.LogError(
                "Broker API returned {StatusCode}: {Error}\nRequest: {StatusRequest}",
                statusResponse.StatusCode,
                error,
                statusRequest);

            return Result.Fail(
                problem == null 
                    ? "Something has gone wrong while fetching swap status." 
                    : problem.Detail);
        }
    }

    public class SwapStatusResponse
    {
        [JsonPropertyName("id")] 
        public ulong Id { get; set; }

        [JsonPropertyName("status")]
        public SwapStatus Status { get; set; }
        
        [JsonIgnore]
        public string Body { get; set; }
    }
    
    public class SwapStatus 
    {
        [JsonPropertyName("state")]
        public string State { get; set; }
        
        [JsonPropertyName("isDepositChannelExpired")]
        public bool DepositChannelExpired { get; set; }
        
        [JsonPropertyName("depositAmount")]
        public string? DepositAmount { get; set; }

        [JsonPropertyName("egressAmount")]
        public string? EgressAmount { get; set; }

        [JsonPropertyName("swapId")]
        public string? SwapId { get; set; }
    }
}