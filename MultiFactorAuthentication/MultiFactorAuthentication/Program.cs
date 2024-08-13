using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiFactorAuthentication.Services;

namespace MultiFactorAuthentication
{
    class Program
    {
        private static TimeSpan x;
        static void Main(string[] args)
        {
            x = TimeSpan.FromMinutes(0);
            var y = default(TimeSpan);
            Console.WriteLine(y.ToString());
            Console.WriteLine(x.ToString());
            Console.WriteLine(x == y);
            Console.ReadKey();
        }
    }
}
