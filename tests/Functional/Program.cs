using AIlins.Collections.Generic;
using System;

namespace Functional
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiValueDictionary<int, long> t = new MultiValueDictionary<int, long>();
            t[1] = 10;
            t[1] = 20;
            t[1] = 10;
            t[1] = 30;
            foreach (var item in t)
            {
                Console.WriteLine($"Key {item.Key} - {item.Value}");
            }
            Console.ReadKey();
        }
    }
}
