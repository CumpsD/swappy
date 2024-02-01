namespace SwappyBot.Commands.Swap
{
    using System.Text.Json.Serialization;

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