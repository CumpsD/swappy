namespace SwappyBot.Commands.Swap
{
    using System.Text.Json.Serialization;

    public class DepositAddress
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
}