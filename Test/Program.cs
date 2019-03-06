using Common.UniqueIdGenerator;
using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            UniqueGenerator ug = new UniqueGenerator(1, 1, 1);

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(ug.GetNextId());
            }

            Console.WriteLine("end..");
            Console.Read();
        }
    }
}
