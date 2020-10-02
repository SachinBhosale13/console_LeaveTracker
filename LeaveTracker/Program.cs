using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace LeaveTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            int empId = 0;
            DataTable dt = new DataTable();
            bool isValid = false;
            int choice = 0;
            Regex regXpositiveIntegers = new Regex("^[1-9]\\d*$");

            try
            {
                //Read Emp Id
                string eId = string.Empty;
                Console.WriteLine("Enter the Employee Id");
                eId = Console.ReadLine();
                Console.WriteLine("-------------------------------------------------------------------------------------------");

                //basic positive integer validation for emp id
                if (!regXpositiveIntegers.IsMatch(eId))
                {
                    throw new ArgumentException("Employee Id can only be positive number", "Employee Id");
                }
                else
                {
                    empId = Convert.ToInt32(eId);
                }

                if (empId > 0)
                {
                    Validator objValidator = new Validator();

                    //validating the entered employee id
                    isValid = objValidator.ValidateEmployee(empId);

                    if (isValid)
                    {
                        LeavesTracker objTracker = new LeavesTracker();
                        bool isManager = objTracker.CheckManager(empId);

                        Console.WriteLine("Please choose one of options below.");
                        Console.WriteLine("1. Create Leave - Assign-To (only current user's manager should be allowed) - Title - Description - Start-Date - End-Date");
                        Console.WriteLine("2. List my Leaves");
                        Console.WriteLine("3. Search Leave");

                        if (isManager)
                        {
                            Console.WriteLine("4. Update leaves (Only Managers)");
                        }

                        string ch = Console.ReadLine();
                        Console.WriteLine("-------------------------------------------------------------------------------------------");

                        switch (ch)
                        {
                            case "1":
                                bool isCreated = false;
                                isCreated = objTracker.CreateLeave(empId);
                                Console.WriteLine("===================================================================");
                                if (isCreated)
                                    Console.WriteLine("Successfully Created Leave.");
                                else
                                    Console.WriteLine("Something went wrong while creating leave.");
                                Console.WriteLine("===================================================================");
                                break;

                            case "2":
                                objTracker.GetMyLeaves(empId);
                                break;

                            case "3":
                                Console.WriteLine("Search Leaves By...");
                                Console.WriteLine("1. Title");
                                Console.WriteLine("2. Status (Pending/Approved/Rejected)");
                                Console.WriteLine("*** Choose 1 or 2");

                                string searchOption = Console.ReadLine();
                                Console.WriteLine("-------------------------------------------------------------------------------------------");
                                if (searchOption == "1" || searchOption == "2")
                                {
                                    objTracker.SearchLeaves(empId, searchOption);
                                }
                                else
                                {
                                    Console.WriteLine("Wrong choice");
                                }
                                break;

                            case "4":
                                if (isManager)
                                {
                                    bool isUpdated = objTracker.UpdateLeaves(empId);
                                    if (isUpdated)
                                        Console.WriteLine("Leave(s) Successfully Updated.");
                                    else
                                        Console.WriteLine("Something Went Wrong.");
                                    Console.WriteLine("===================================================================");
                                }
                                else
                                {
                                    Console.WriteLine("Wrong Choice.");
                                }

                                break;

                            default:
                                Console.WriteLine("Wrong Choice.");
                                break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Employee ID.");
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
