using System;
using System.Collections.Generic;

namespace designpatternworkout.Composite
{
    /// <summary>
    /// - Compose Objects into tree structures to represetn while-part hierarchies.
    /// - Composite lets client treat individual objects and compisitions of objects uniformly
    /// - Recursive Composition
    /// 1 to many "HAS A " up the "IS A" hierarchy
    /// 
    /// when to use
    /// =====================
    /// Composite should be used when clients ignore the difrence between cmpositions and individual objects.
    /// If Programers find that they are using multiple objects in the same way, and often have nearly identical code to handle each of them then composite is good choice. 
    /// </summary>
    public class SampleCode
    {
        public static void Main(String[] args)
        {
    
        Developer dev1 = new Developer(100, "Lokesh Sharma", "Pro Developer");
        Developer dev2 = new Developer(101, "Vinay Sharma", "Developer");
        CompanyDirectory engDirectory = new CompanyDirectory();
        engDirectory.AddEmployee(dev1);
        engDirectory.AddEmployee(dev2);
          
        Manager man1 = new Manager(200, "Kushagra Garg", "SEO Manager");
        Manager man2 = new Manager(201, "Vikram Sharma ", "Kushagra's Manager");
          
        CompanyDirectory accDirectory = new CompanyDirectory();
        accDirectory.AddEmployee(man1);
        accDirectory.AddEmployee(man2);
      
        CompanyDirectory directory = new CompanyDirectory();
        directory.AddEmployee(engDirectory);
        directory.AddEmployee(accDirectory);
        directory.ShowEmployeeDetails();
    
        }
    }

/// <summary>
/// Interface Component
/// </summary>
    public interface Employee
    {
         void ShowEmployeeDetails();
    } 


    /// <summary>
    /// Leaf
    /// </summary>
    public class Developer : Employee
    {
        public string Name { get; set; }

        public long EmpId { get; set; }

        public string Position {get; set;}

        public Developer(long empId, string name, string position)
        {
            Name=name;
            EmpId=empId;
            Position=position;
        }

        public void ShowEmployeeDetails()
        {
            Console.WriteLine(" {0}  {1}",EmpId,Name);
        }
    }

    /// <summary>
    /// Leaf
    /// </summary>
    public class Manager:Employee
    {
        public Manager(string name, long empId, string position) 
        {
            this.Name = name;
                this.EmpId = empId;
                this.Position = position;
               
        }
                public string Name { get; set; }

        public long EmpId { get; set; }

        public string Position {get; set;}

        public Manager(long empId, string name, string position)
        {
            Name=name;
            EmpId=empId;
            Position=position;
        }

        public void ShowEmployeeDetails()
        {
            Console.WriteLine(" {0}  {1}",EmpId,Name);
        }
    }



    /// <summary>
    /// Composite
    /// </summary>
    public class CompanyDirectory:Employee
    {
        private List<Employee> employeeList=new List<Employee>();

        public void ShowEmployeeDetails()
        {
            foreach(Employee e in employeeList)
            {
                e.ShowEmployeeDetails();
            }
        }
        public void AddEmployee(Employee e)
        {
            employeeList.Add(e);
        }

        public void RemoveEmployee(Employee e)
        {
            employeeList.Remove(e);
        }
    }

}