using System;
using System.Threading.Tasks;

//https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern
namespace designpatternworkout.ChainOfResponsibility
{
    public class SampleCode1
    {
        public static void Main(string[] args)
        {
            Logger logger, logger1, logger2;

            logger=new ConsoleLogger(LogLevel.All);
            logger1=logger.SetNext(new EmailLogger(LogLevel.FunctionalMessage|LogLevel.FunctionalError));
            logger2=logger1.SetNext(new FileLogger(LogLevel.Warning|LogLevel.Error));


            logger.Message("Entering Function ProcessOrder()",LogLevel.Debug);
            logger.Message("Order record retrieved",LogLevel.Info);

            logger.Message("Customer address details missing", LogLevel.Warning);
            logger.Message("Other address details missing",LogLevel.Error);

            logger.Message("Unable to process order ",LogLevel.FunctionalError);
            logger.Message("Unable to process order ",LogLevel.FunctionalMessage);
        }
    }

    [Flags]
    public enum LogLevel
    {
        None = 0,                 //        0
        Info = 1,                 //        1
        Debug = 2,                //       10
        Warning = 4,              //      100
        Error = 8,                //     1000
        FunctionalMessage = 16,   //    10000
        FunctionalError = 32,     //   100000
        All = 63                  //   111111
    }



    public abstract class Logger
    {
        protected LogLevel logMask;
        protected Logger next;

        public Logger(LogLevel mask)
        {
            this.logMask=mask;
        }   

        public Logger SetNext(Logger nextLogger)
        {
            next=nextLogger;
            return nextLogger;
        }

        public void Message(string msg, LogLevel severity)
        {
            if((severity&logMask)!=0)
            {
                WriteMessage(msg);
            }
            if(next!=null)
            {
                next.Message(msg, severity);
            }
        }

        abstract protected void WriteMessage(String msg);

    }

    public class ConsoleLogger :Logger
    {
        public ConsoleLogger(LogLevel mask):base(mask)
        {

        }

        protected override void WriteMessage(string msg)
        {
            Console.WriteLine("Writing to Console :"+msg);
        }
    }


    public class EmailLogger:Logger
    {
        public EmailLogger(LogLevel mask):base(mask)
        {

        }

        protected override void WriteMessage(string msg)
        {
            Console.WriteLine("Sending Emal :"+msg);
        }
    }

    public class FileLogger:Logger
    {
        public FileLogger(LogLevel mask):base(mask)
        {

        }

        protected override void WriteMessage(string msg)
        {
            Console.WriteLine("Writing to Log File :"+msg);
        }
    }




}