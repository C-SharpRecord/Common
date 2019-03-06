using Common.UniqueIdGenerator;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            UniqueGenerator ug = new UniqueGenerator(1, 1, 1);

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine(ug.GetNextId());
            }

            CalculationDetail(ug.GetNextId());

            Console.WriteLine("end..");
            Console.Read();
        }

        static void CalculationDetail(long value)
        {
            Console.WriteLine($"value:{value}");
            Console.WriteLine($"timestmap:{value >> 22}");
            Console.WriteLine($"DatacenterId:{(value << 42)>>59}");
            Console.WriteLine($"WorkId:{(value << 47) >> 59}");
            Console.WriteLine($"Sequence:{(value << 52) >> 52}");

        }
    }
}
