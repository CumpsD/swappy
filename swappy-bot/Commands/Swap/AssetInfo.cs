namespace SwappyBot.Commands.Swap
{
    using System;

    public record AssetInfo(
        string Id, 
        string Ticker,
        string Name, 
        string Network, 
        int Decimals,
        double MinimumAmount, 
        double MaximumAmount,
        double[] SuggestedAmounts,
        Func<string, bool> AddressValidator);
}