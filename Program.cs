using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelLoops
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            list.Add("5");
            list.Add("6");
            list.Add("7");
            list.Add("8");
            list.Add("9");
            list.Add("10");

            Console.WriteLine("ParallelForeach");
            Parallel.ParallelForEach(list, (a) => Console.WriteLine(a));


            Console.WriteLine("ParallelFor");
            Parallel.ParallelFor(0, 10, (a) => Console.WriteLine(a));

            ParallelOptions options = new ParallelOptions();

            options.MaxDegreeOfParallelism = 10;

            Console.WriteLine("ParallelForeachWithOptions");
            Parallel.ParallelForEachWithOptions(list, options, (a) => Console.WriteLine(a));

            Console.Read();
        }
    }
}
