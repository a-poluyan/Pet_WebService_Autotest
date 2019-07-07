using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_WebService_Autotest
{
    static class RandomString
    {
        private static readonly Random random = new Random();
        private static readonly char[] letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();


        // Метод получения рандомной строки указанной длины length
        public static string Get(int length)
        {
            StringBuilder builder = new StringBuilder(length);

            for (int i = 0; i < length; ++i)
                builder.Append(letters[random.Next(0, letters.Length - 1)]);

            return builder.ToString();
        }


    }
}
