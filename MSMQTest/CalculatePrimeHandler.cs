using System;

namespace MSMQTest
{
    public class CalculatePrimeHandler : IEventHandler<SlowCalculateBigPrimeEvent>
    {
        public void Execute(SlowCalculateBigPrimeEvent e)
        {
            Slow();
        }
        public void Slow()
        {
            Console.WriteLine("Calculate Largest Prime");
            long nthPrime = FindPrimeNumber(999); //set higher value for more time
            Console.WriteLine($"Largest prime {nthPrime}");
        }

        public long FindPrimeNumber(int n)
        {
            int count=0;
            long a = 2;
            while(count<n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while(b * b <= a)
                {
                    if(a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if(prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }

    }
}