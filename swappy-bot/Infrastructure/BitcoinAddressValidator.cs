namespace SwappyBot.Infrastructure
{
    using NBitcoin;

    public static class BitcoinAddressValidator
    {
        public static bool IsValidAddress(string address)
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
    }
}