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
        public double MinimumAmount { get; }
        public double MaximumAmount { get; }
        public double[] SuggestedAmounts { get; }
        public string FormatString { get; }
        public Func<string, bool> AddressValidator { get; }
        public Func<string, string> AddressConverter { get; }

        public AssetInfo(
            string id, 
            string ticker,
            string name, 
            string network, 
            int decimals,
            double minimumAmount, 
            double maximumAmount,
            double[] suggestedAmounts,
            Func<string, bool> addressValidator,
            Func<string, string> addressConverter)
        {
            Id = id;
            Ticker = ticker;
            Name = name;
            Network = network;
            Decimals = decimals;
            MinimumAmount = minimumAmount;
            MaximumAmount = maximumAmount;
            SuggestedAmounts = suggestedAmounts;
            AddressValidator = addressValidator;
            AddressConverter = addressConverter;
            
            FormatString = $"0.00{new string('#', decimals - 2)}";

        }
    }
}