using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Threading;

namespace AsyncAndAwait
{
    class Program
    {
        async static private void MyMethodAsync(int count)
        {
            WriteLine("C");
            WriteLine("D");

            await Task.Run(() =>
            {
                for (int i = 0; i <= count; i++)
                {
                    WriteLine($"{i}/{count} ...");
                    Thread.Sleep(100);
                }
            });
            WriteLine("G");
            WriteLine("H");
        }
        static void Caller()
        {
            WriteLine("A");
            WriteLine("B");
            MyMethodAsync(3);
            WriteLine("E");
            WriteLine("F");
        }
        static void Main(string[] args)
        {
            Caller();
            WriteLine("######################");
            ReadLine();
        }
    }
}
