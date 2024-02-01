namespace SwappyBot.Commands.Swap
{
    using System;

    public static class RoundTrip
    {
        private static readonly string[] Zeros = new string[1000];

        static RoundTrip()
        {
            for (var i = 0; i < Zeros.Length; i++)
                Zeros[i] = new string('0', i);
        }

        public static string ToRoundTrip(this double value)
        {
            var str = value.ToString("r");
            int x = str.IndexOf('E');
            if (x < 0) return str;

            int x1 = x + 1;
            String exp = str.Substring(x1, str.Length - x1);
            int e = int.Parse(exp);

            String s = null;
            int numDecimals = 0;
            if (value < 0)
            {
                int len = x - 3;
                if (e >= 0)
                {
                    if (len > 0)
                    {
                        s = str.Substring(0, 2) + str.Substring(3, len);
                        numDecimals = len;
                    }
                    else
                        s = str.Substring(0, 2);
                }
                else
                {
                    // remove the leading minus sign
                    if (len > 0)
                    {
                        s = str.Substring(1, 1) + str.Substring(3, len);
                        numDecimals = len;
                    }
                    else
                        s = str.Substring(1, 1);
                }
            }
            else
            {
                int len = x - 2;
                if (len > 0)
                {
                    s = str[0] + str.Substring(2, len);
                    numDecimals = len;
                }
                else
                    s = str[0].ToString();
            }

            if (e >= 0)
            {
                e = e - numDecimals;
                String z = (e < Zeros.Length ? Zeros[e] : new String('0', e));
                s = s + z;
            }
            else
            {
                e = (-e - 1);
                String z = (e < Zeros.Length ? Zeros[e] : new String('0', e));
                if (value < 0)
                    s = "-0." + z + s;
                else
                    s = "0." + z + s;
            }

            return s;
        }
    }
}