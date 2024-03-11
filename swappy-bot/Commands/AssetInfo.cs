namespace SwappyBot.Commands
{
    using System;

    public class AssetInfo
    {
        public string Id { get; }
        public string Ticker { get; }
        public string Name { get; }
        public string Network { get; }
        public int Decimals { get; }
        public decimal MinimumAmount { get; }
        public decimal[] SuggestedAmounts { get; }
        public string FormatString { get; }
        public Func<string, bool> AddressValidator { get; }

        public AssetInfo(
            string id, 
            string ticker,
            string name, 
            string network, 
            int decimals,
            decimal minimumAmount, 
            decimal[] suggestedAmounts,
            Func<string, bool> addressValidator)
        {
            Id = id;
            Ticker = ticker;
            Name = name;
            Network = network;
            Decimals = decimals;
            MinimumAmount = minimumAmount;
            SuggestedAmounts = suggestedAmounts;
            AddressValidator = addressValidator;
            
            FormatString = $"0.00{new string('#', decimals - 2)}";
        }
    }
}