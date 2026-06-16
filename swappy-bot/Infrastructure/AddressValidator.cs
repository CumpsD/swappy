namespace SwappyBot.Infrastructure
{
    using System;
    using NBitcoin;
    using SimpleBase;

    public static class AddressValidator
    {
        public static bool IsValidBitcoinAddress(string address)
        {
            try
            {
                Network.Main.Parse(address);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidSolanaAddress(string address)
        {
            try
            {
                return Base58.Bitcoin.Decode(address).Length == 32;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidTronAddress(string address)
        {
            const byte mainnetPrefix = 0x41; // 'T' in Base58Check

            if (string.IsNullOrWhiteSpace(address) || address.Length != 34 || address[0] != 'T')
                return false;

            try
            {
                // 0x41 + 20-byte body = 21 bytes after the 4-byte checksum is stripped
                Span<byte> buffer = stackalloc byte[21];

                return
                    Base58.Bitcoin.TryDecodeCheck(address, buffer, out var version, out var bytesWritten) &&
                    version == mainnetPrefix &&
                    bytesWritten == 20;
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException)
            {
                return false;
            }
        }
    }
}