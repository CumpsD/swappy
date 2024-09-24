namespace SwappyBot.Infrastructure
{
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
    }
}