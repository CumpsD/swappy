namespace SwappyBot.Commands.Swap
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class DepositAddress
    {
        public string Address { get; }

        public double IssuedBlock { get; }

        public double ChannelId { get; }

        private DepositAddress(
            AssetInfo asset, 
            DepositAddressResult result)
        {
            Address = asset.AddressConverter(result.Address);
            IssuedBlock = result.IssuedBlock;
            ChannelId = result.ChannelId;
        }

        public static async Task<DepositAddress?> GetDepositAddressAsync(
            IHttpClientFactory httpClientFactory,
            AssetInfo assetFrom,
            AssetInfo assetTo,
            string destinationAddress,
            int commissionBps)
        {
            using var client = httpClientFactory.CreateClient("Deposit");

            var response = await client.PostAsJsonAsync(
                string.Empty,
                new DepositAddressRequest(assetFrom, assetTo, destinationAddress, commissionBps));

            var result = await response.Content.ReadFromJsonAsync<DepositAddressResponse>();

            return new DepositAddress(
                assetFrom,
                result.Result);
        }
    }
    
    public class DepositAddressResponse
    {
        [JsonPropertyName("result")]
        public DepositAddressResult Result { get; set; }
    }

    public class DepositAddressResult
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }
        
        [JsonPropertyName("issued_block")]
        public double IssuedBlock { get; set; }
        
        [JsonPropertyName("channel_id")]
        public double ChannelId { get; set; }
    }
    
    public class DepositAddressRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; } = "1";

        [JsonPropertyName("jsonrpc")] 
        public string JsonRpcVersion { get; } = "2.0";

        [JsonPropertyName("method")] 
        public string Method { get; } = "broker_request_swap_deposit_address";

        [JsonPropertyName("params")] 
        public dynamic[] Parameters { get; }
        
        public DepositAddressRequest(
            AssetInfo assetFrom, 
            AssetInfo assetTo, 
            string destinationAddress, 
            int commissionBps)
        {
            Parameters =
            [
                new ChainId(assetFrom.Network, assetFrom.Ticker),
                new ChainId(assetTo.Network, assetTo.Ticker),
                destinationAddress,
                commissionBps
            ];
        }
    }
    
    public class ChainId
    {
        [JsonPropertyName("chain")] 
        public string Network { get; }
        
        [JsonPropertyName("asset")] 
        public string Asset { get; }

        public ChainId(
            string network,
            string asset)
        {
            Network = network;
            Asset = asset;
        }
    }
}