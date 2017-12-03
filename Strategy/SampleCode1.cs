using System;

namespace designpatternworkout.Strategy
{
    //https://en.wikipedia.org/wiki/Strategy_pattern
    public class SampleCode1
    {
        public static void Main(string[] args)
        {
            CalculateClient client=new CalculateClient(new Minus());

            Console.WriteLine("Minus: " + client.Calculate(7, 1));
            //Change the strategy
            client.Strategy = new Plus();
    
            Console.WriteLine("Plus: " + client.Calculate(7, 1));
        }
    }

    public interface ICalculate
    {
        int Calculate(int val1, int val2);
    }

    public class Minus:ICalculate
    {
        public int Calculate(int val1,int val2)
        {
            return val1-val2;
        }
    }

    public class Plus:ICalculate
    {
        public int Calculate(int val1,int val2)
        {
            return val1+val2;
        }
    }

    public class CalculateClient
    {
        public ICalculate Strategy {get;set;}

        public CalculateClient(ICalculate strategy)
        {
            Strategy=strategy;
        }

        public int Calculate(int val1,int val2)
        {
            return Strategy.Calculate(val1,val2);
        }
    }
}