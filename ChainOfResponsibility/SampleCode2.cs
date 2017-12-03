//https://www.codeproject.com/Articles/743783/Reusable-Chain-of-responsibility-in-Csharp

using System;
using System.Collections.Generic;

namespace designpatternworkout.ChainOfResponsibility
{
    public class SampleCode2
    {
        public static void Main(string[] args)
        {
            List<Mobile> mobiles=new List<Mobile>{
                new Mobile(Type.Premium, 450), 
                new Mobile(Type.Premium, 800), 
                new Mobile(Type.Basic, 100), 
                new Mobile(Type.Budget, 160),
                new Mobile(Type.Premium, 800) ,
                new Mobile(Type.Budget, 250),
                new Mobile(Type.Budget, 1)
            };

            ISpecification<Mobile> basicSpec = new Specification<Mobile>(o => o.Type == Type.Basic);
            ISpecification<Mobile> budgetSpec = new Specification<Mobile>(o => o.Type == Type.Budget);
            ISpecification<Mobile> premiumSpec = new Specification<Mobile>(o => (o.Type == Type.Premium));


            ISpecification<Mobile> budgetLowCostSpec = new Specification<Mobile>(o => o.Cost < 200 && o.Cost >= 100);
            //To extract all mobile phones that costs greater than or equal to 200
            ISpecification<Mobile> budgetHighCostSpec = new Specification<Mobile>(o => (o.Cost >= 200));
            //To extract all mobile phones that costs less than 500
            ISpecification<Mobile> premiumLowCostSpec = new Specification<Mobile>(o => (o.Cost < 500));
            //To extract all mobile phones that costs greater than or equal to 500    
            ISpecification<Mobile> premiumHighCostSpec = new Specification<Mobile>(o => (o.Cost >= 500));


            var invProcess = new InventoryProcess<Mobile>();            

            IHandler<Mobile> seniorManager = new Approver<Mobile>("SeniorManager", invProcess.placeOrder);
            IHandler<Mobile> manager = new Approver<Mobile>("Manager", invProcess.placeOrder);
            IHandler<Mobile> supervisor = new Approver<Mobile>("Supervisor", invProcess.placeOrder);
            IHandler<Mobile> employee = new Approver<Mobile>("Employee", invProcess.placeOrder);

            employee.SetSpecification(basicSpec);
            //For supervisor spec we combine the budget spec with budget low cost spec to achieve the constraint all budget mobiles that costs less than 200
            //supervisor.SetSpecification(budgetSpec.And<Mobile>(budgetLowCostSpec));
            supervisor.SetSpecification(basicSpec);
            //For manager spec we combine the budget spec with budget high cost spec to achieve the constraint all budget mobiles that costs more than or equal to 200.
            //For manager spec we combine the premium spec with premium low cost spec to achieve the constraint all premium mobiles that costs less than 500. 
            manager.SetSpecification(budgetSpec.And<Mobile>(budgetHighCostSpec).Or<Mobile>(premiumSpec.And<Mobile>(premiumLowCostSpec)));
            //For senior manager spec we combine the premium spec with premium high cost spec to achieve the constraint all premium mobiles that costs more than or equal to 500.   
            seniorManager.SetSpecification(premiumSpec.And<Mobile>(premiumHighCostSpec));
            
            employee.SetSuccessor(supervisor);
            supervisor.SetSuccessor(manager);
            manager.SetSuccessor(seniorManager);
            
            //Default handler
            IHandler<Mobile> defaultApprover = new Approver<Mobile>("Default", invProcess.defaultOrder);
            ISpecification<Mobile> defaultSpec = new Specification<Mobile>(o => true);
            defaultApprover.SetSpecification(defaultSpec);
            seniorManager.SetSuccessor(defaultApprover);

            mobiles.ForEach(o => employee.HandleRequest(o));            
            Console.ReadLine();
        }
    }


    public class InventoryProcess<Mobile>
    {
        public void placeOrder(Mobile o)
        {
            Console.WriteLine("Order placed for mobile {0}", o.ToString());
            //Place the order.
        }

        public void defaultOrder(Mobile o)
        {
            Console.WriteLine("The order is not placed for the mobile {0}", o.ToString());
            //Place the order.
        } 
    }

    #region Mobile
    public enum Type
    {
        Basic, Budget, Premium
    }

    public interface IProcessable
    {
        void Process();
    }

    public class Mobile:IProcessable
    {
        public Type Type{get;set;}

        public double Cost;

        public override string ToString()
        {
            return "Type : "+this.Type +"  and Cost: "+this.Cost;
        }

        public Mobile(Type type, int cost=0)
        {
            this.Type=type;
            this.Cost=cost;
        }

        public void Process()
        {
        }
    }


    #endregion

    #region "Specification Pattern"
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T o);
    }

    public class Specification<T>:ISpecification<T>
    {
        private Func<T,bool> expression;
        public Specification(Func<T,bool> expression)
        {
            if(expression==null)
                throw new ArgumentNullException();
            else
                this.expression=expression;
        }

        public bool IsSatisfiedBy(T o)
        {
            return this.expression(o);
        }
    }

    public static class SpecificationExtensions
    {
        public static Specification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if(left!=null && right!=null)
            {
                return new Specification<T>(o=>left.IsSatisfiedBy(o)&&right.IsSatisfiedBy(o));
            }

            return null;
        }

        public static Specification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if(left!=null && right!=null)
            {
                return new Specification<T>(o=>left.IsSatisfiedBy(o) || right.IsSatisfiedBy(o));
            }
            return null;
        }

        public static Specification<T> Not<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            if(left!=null)
            {
                return new Specification<T>(o=>!left.IsSatisfiedBy(o));
            }
            return null;
        }
    
    }
    #endregion

    #region "Chain of responsibility"
    public interface IHandler<T>
    {
        void SetSuccessor(IHandler<T> handler);
        void HandleRequest(T o);
        void SetSpecification(ISpecification<T> specification);
    } 

    public class Approver<T>:IHandler<T> where T:IProcessable
    {
        private IHandler<T> successor;
        private string name;

        private ISpecification<T> specification;

        Action<T> action;

        public Approver(string name, Action<T> action)
        {
            this.name=name;
            this.action=action;   
        }

        public bool CanHandle(T o)
        {
            if(this.specification!=null&&o!=null)
            {
                return this.specification.IsSatisfiedBy(o);
            }
            return false;
        }

        public void SetSuccessor(IHandler<T> handler)
        {
            this.successor=handler;
        }

        public void HandleRequest(T o)
        {
            if(CanHandle(o))
            {
                //o.Precess();
                Console.WriteLine("{0}: Request Handled by {1} ",o.ToString(), this.name);
                this.action.Invoke(o);
                Console.WriteLine("************************************************");
            }
            if(this.successor!=null)
            {
                this.successor.HandleRequest(o);
            }
        }

        public void SetSpecification(ISpecification<T> specification)
        {
            this.specification=specification;
        }

    }

    #endregion
}