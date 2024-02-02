namespace SwappyBot.Commands
{
    using System;
    using System.Linq;
    using SimpleBase;

    public static class ConvertToSs58Extension
    {
        // First 00 = Polkadot
        // Console.WriteLine("008dc56c5e01ddbbea748ab139d18e5cfc894955967a62599311d3ad7bb7571dca".ConvertToSs58());
        // 14CtQ8HLKxSwtwYGc7mvSMEW7wt6maGhhEqrWezg8Y3XpZuV

        // Console.WriteLine("00d513a8eb412b6bdffb39d9ebbe4edd7afad16b7bf9fa89fa225e084896757b35".ConvertToSs58());
        // 15pP3JL915qcrRpYu7H1dqn87MS3WE8nc6ivKYabD54HaBcu
        
        public static string ConvertToSs58(this string hex)
        {
            var publicKeyBytes = StringToByteArray(hex);
            var ss58AddressBytes = Ss58Hash(publicKeyBytes);
            return Base58.Bitcoin.Encode(ss58AddressBytes);
        }
        
        private static byte[] StringToByteArray(string hex) =>
            Enumerable
                .Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

        private static byte[] Ss58Hash(byte[] data)
        {
            var dataLength = data.Length;
            const int prefixLength = 7;
            var ssPrefix = "SS58PRE"u8.ToArray();

            var ssPrefixed = new byte[dataLength + prefixLength];
            Buffer.BlockCopy(ssPrefix, 0, ssPrefixed, 0, prefixLength);
            Buffer.BlockCopy(data, 0, ssPrefixed, prefixLength, dataLength);
            
            var hash = Blake2Core.Blake2B.ComputeHash(ssPrefixed);

            var ss58 = new byte[data.Length + 2];
            Buffer.BlockCopy(data, 0, ss58, 0, dataLength);
            ss58[dataLength] = hash[0];
            ss58[dataLength + 1] = hash[1];

            return ss58;
        }
    }
}