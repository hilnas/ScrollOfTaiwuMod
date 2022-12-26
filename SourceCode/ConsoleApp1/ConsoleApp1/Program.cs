using System;

namespace ConsoleApp1
{




    class sum
    {
        public int value;
        int num1;
        int num2;
        int num3;
        public sum(int a, int b, int c)
        {
            this.num1 = a;
            this.num2 = b;
            this.num3 = c;
        }

        ~sum()
        {
            value = this.num1 * this.num2 * this.num3;
            Console.WriteLine(value);
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            sum s = new sum(4, 5, 9);
        }
    }

}
