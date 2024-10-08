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
            [ChoiceDisplay("ethUSDT (USDT)")] usdt,
            [ChoiceDisplay("arbEthereum (ETH)")] arbeth,
            [ChoiceDisplay("arbUSDC (USDC)")] arbusdc,
            [ChoiceDisplay("Solana (SOL)")] sol,
            [ChoiceDisplay("solUSDC (USDC)")] solusdc,
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
                    0.0007m,
                    [0.005m, 0.01m, 0.02m, 0.05m, 0.1m, 0.2m, 0.5m, 1.0m, 2.0m],
                    AddressValidator.IsValidBitcoinAddress)
            },

            {
                "dot",
                new AssetInfo(
                    "dot",
                    "DOT",
                    "Polkadot",
                    "Polkadot",
                    10,
                    4m,
                    [10m, 20m, 50m, 150m, 300m, 700m, 1000m, 2000m, 4000m, 7000m],
                    _ => true)
            },

            {
                "eth",
                new AssetInfo(
                    "eth",
                    "ETH",
                    "Ethereum",
                    "Ethereum",
                    18,
                    0.01m,
                    [0.02m, 0.04m, 0.1m, 0.2m, 0.5m, 1m, 2m, 5m, 10m, 20m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },

            {
                "flip",
                new AssetInfo(
                    "flip",
                    "FLIP",
                    "Chainflip",
                    "Ethereum",
                    18,
                    4m,
                    [10m, 20m, 50m, 150m, 300m, 1000m, 2000m, 4000m, 6500m, 9000m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },

            {
                "usdc",
                new AssetInfo(
                    "usdc",
                    "USDC",
                    "ethUSDC",
                    "Ethereum",
                    6,
                    20m,
                    [100m, 500m, 1000m, 2500m, 5000m, 10000m, 25000m, 40000m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },
            
            {
                "usdt",
                new AssetInfo(
                    "usdt",
                    "USDT",
                    "ethUSDT",
                    "Ethereum",
                    6,
                    20m,
                    [100m, 500m, 1000m, 2500m, 5000m, 10000m, 25000m, 40000m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },
            
            {
                "arbeth",
                new AssetInfo(
                    "arbeth",
                    "ETH",
                    "arbEthereum",
                    "Arbitrum",
                    18,
                    0.01m,
                    [0.02m, 0.04m, 0.1m, 0.2m, 0.5m, 1m, 2m, 5m, 10m, 20m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },
            
            {
                "arbusdc",
                new AssetInfo(
                    "arbusdc",
                    "USDC",
                    "arbUSDC",
                    "Arbitrum",
                    6,
                    20m,
                    [100m, 500m, 1000m, 2500m, 5000m, 10000m, 25000m, 40000m],
                    x => AddressUtil.Current.IsNotAnEmptyAddress(x) &&
                         AddressUtil.Current.IsValidAddressLength(x) &&
                         AddressUtil.Current.IsValidEthereumAddressHexFormat(x) &&
                         (AddressUtil.Current.IsChecksumAddress(x) || x == x.ToLower() || x[2..] == x[2..].ToUpper()))
            },
            
            
            {
                "sol",
                new AssetInfo(
                    "sol",
                    "SOL",
                    "Solana",
                    "Solana",
                    9,
                    0.01m,
                    [0.2m, 0.4m, 1m, 2m, 5m, 10m, 20m, 50m, 100m, 200m],
                    AddressValidator.IsValidSolanaAddress)
            },
            
            {
                "solusdc",
                new AssetInfo(
                    "solusdc",
                    "USDC",
                    "solUSDC",
                    "Solana",
                    6,
                    20m,
                    [100m, 500m, 1000m, 2500m, 5000m, 10000m, 25000m, 40000m],
                    AddressValidator.IsValidSolanaAddress)
            },
        };
    }
}