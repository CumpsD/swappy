namespace SwappyBot.Commands
{
    using System.Collections.Generic;
    using Discord.Interactions;
    using Nethereum.Util;
    using SwappyBot.Infrastructure;

    public static class Assets
    {
        public enum AllAssets
        {
            [ChoiceDisplay("Bitcoin (BTC)")] btc,
            [ChoiceDisplay("Polkadot (DOT)")] dot,
            [ChoiceDisplay("Ethereum (ETH)")] eth,
            [ChoiceDisplay("Chainflip (FLIP)")] flip,
            [ChoiceDisplay("ethUSDC (USDC)")] usdc,
        }
        
        public static readonly Dictionary<string, AssetInfo> SupportedAssets = new()
        {
            {
                "btc",
                new AssetInfo(
                    "btc",
                    "BTC",
                    "Bitcoin",
                    "Bitcoin",
                    8,
                    0.0007,
                    0.65,
                    [0.001, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.5],
                    x => AddressValidator.IsValidAddress(x, "btc"),
                    x => x)
            },

            {
                "dot",
                new AssetInfo(
                    "dot",
                    "DOT",
                    "Polkadot",
                    "Polkadot",
                    10,
                    4,
                    4_100,
                    [10, 20, 50, 150, 300, 700, 1000, 2000, 4000],
                    x => true,
                    hex => hex.ConvertToSs58())
            },

            {
                "eth",
                new AssetInfo(
                    "eth",
                    "ETH",
                    "Ethereum",
                    "Ethereum",
                    18,
                    0.01,
                    11,
                    [0.02, 0.04, 0.1, 0.2, 0.5, 1, 2, 5, 10],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()),
                    x => x)
            },

            {
                "flip",
                new AssetInfo(
                    "flip",
                    "FLIP",
                    "Chainflip",
                    "Ethereum",
                    18,
                    4,
                    5_700,
                    [10, 20, 50, 150, 300, 1000, 2000, 4000, 5500],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()),
                    x => x)
            },

            {
                "usdc",
                new AssetInfo(
                    "usdc",
                    "USDC",
                    "ethUSDC",
                    "Ethereum",
                    6,
                    20,
                    25_000,
                    [25, 50, 100, 500, 1000, 2500, 5000, 10000, 20000],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()),
                    x => x)
            },
        };
    }
}