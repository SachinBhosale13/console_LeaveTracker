using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using LeaveTracker.Utils;
using System.Text.RegularExpressions;
using System.IO;

namespace LeaveTracker
{
    public interface ILeavesTracker
    {
        bool CreateLeave(int empId);
        void GetMyLeaves(int empId);
        bool UpdateLeaves(int empId);
        void SearchLeaves(int empId, string searchOption);
        bool CheckManager(int empId);
    }
    public class LeavesTracker : ILeavesTracker
    {
        string leavesCsvPath = ConfigurationSettings.AppSettings["LeavesCsvPath"];
        string empCsvPath = ConfigurationSettings.AppSettings["EmployeesCsvPath"];

        public bool CreateLeave(int empId)
        {
            try
            {
                Console.WriteLine("Enter Title");
                string Title = Console.ReadLine();

                Console.WriteLine("Enter Description");
                string Description = Console.ReadLine();

                Console.WriteLine("Enter Start Date(DD-MM-YYYY)");
                string StartDate = Console.ReadLine();

                Regex regXDate = new Regex("^(((0[1-9]|[12]\\d|3[01])[\\s\\.\\-\\/](0[13578]|1[02])[\\s\\.\\-\\/]((19|[2-9]\\d)\\d{2}))|((0[1-9]|[12]\\d|30)[\\s\\.\\-\\/](0[13456789]|1[012])[\\s\\.\\-\\/]((19|[2-9]\\d)\\d{2}))|((0[1-9]|1\\d|2[0-8])[\\s\\.\\-\\/]02[\\s\\.\\-\\/]((19|[2-9]\\d)\\d{2}))|(29[\\s\\.\\-\\/]02[\\s\\.\\-\\/]((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$");

                if (!regXDate.IsMatch(StartDate))
                {
                    throw new ArgumentException("Invalid Start Date", "Start Date");
                }

                Console.WriteLine("Enter End Date(DD-MM-YYYY)");
                string EndDate = Console.ReadLine();

                if (!regXDate.IsMatch(EndDate))
                {
                    throw new ArgumentException("Invalid End Date", "End Date");
                }

                CsvParser objCsvParser = new CsvParser();

                DataTable dt = new DataTable();

                dt = objCsvParser.GetDataTableFromCSV(empCsvPath);

                DataRow drEmp = dt.Select("Id=" + empId)[0];

                string Creator = drEmp.Field<string>("Name");
                string ManagerId = drEmp.Field<string>("ManagerId");
                string Manager = dt.Select("Id=" + ManagerId)[0].Field<string>("Name");

                string lastLeaveRow = string.Empty;

                lastLeaveRow = File.ReadLines(leavesCsvPath).First();

                if (string.IsNullOrEmpty(lastLeaveRow))
                {
                    throw new Exception("Something went wrong.");
                }
                else
                {
                    string lastLeaveId = string.Empty;
                    int newID = 0;

                    lastLeaveId = lastLeaveRow.Split(',')[0];

                    if (int.TryParse(lastLeaveId, out newID))
                    {
                        newID++;

                        string newRow = newID + "," + empId + "," + Creator + "," + ManagerId + "," + Manager + "," + Title + "," + Description + "," + StartDate + "," + EndDate + ",Pending";

                        using (FileStream fs = new FileStream(leavesCsvPath, FileMode.Append, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                sw.WriteLine(newRow);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Something went wrong.");
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void GetMyLeaves(int empId)
        {
            //List<LeavesModel> lstLeaves = new List<LeavesModel>();
            try
            {
                CsvParser objCsvParser = new CsvParser();

                DataTable dt = new DataTable();

                dt = objCsvParser.GetDataTableFromCSV(leavesCsvPath);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> lstColumnNames = new List<string>();
                    string[] columnNames = { };

                    foreach (DataColumn dc in dt.Columns)
                    {
                        lstColumnNames.Add(dc.ColumnName);
                    }

                    columnNames = lstColumnNames.ToArray();

                    dt.TableName = "My Leaves";

                    DataView dv = new DataView(dt);
                    dv.RowFilter = "CreatorId=" + empId;

                    Console.WriteLine("===================================================================");

                    PrintDataExtensions.Print(dv, columnNames);

                    Console.WriteLine("===================================================================");
                }

                //return lstLeaves;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateLeaves(int empId)
        {
            try
            {
                CsvParser objCsvParser = new CsvParser();

                DataTable dt = new DataTable();

                dt = objCsvParser.GetDataTableFromCSV(leavesCsvPath);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> lstColumnNames = new List<string>();
                    string[] columnNames = { };

                    foreach (DataColumn dc in dt.Columns)
                    {
                        lstColumnNames.Add(dc.ColumnName);
                    }

                    columnNames = lstColumnNames.ToArray();

                    dt.TableName = "Leaves Assigned to me";

                    DataRow[] drEmp = dt.Select("ManagerId = " + empId);

                    Console.WriteLine("===================================================================");

                    //DataView dv = new DataView(dt);
                    //dv.RowFilter = "ManagerId=" + empId;
                    //PrintDataExtensions.Print(dv, columnNames);

                    PrintDataExtensions.Print(drEmp, columnNames);

                    Console.WriteLine("Please choose one of the options below.");
                    Console.WriteLine("1. Update Specific Leave by entering Leave ID");
                    Console.WriteLine("2. Approve All");
                    Console.WriteLine("3. Reject All");

                    string updateLeaveOption = Console.ReadLine();

                    int oldTblRowCount = dt.Rows.Count;

                    switch (updateLeaveOption)
                    {
                        case "1":
                            Console.WriteLine("Enter Leave ID:");
                            string leaveID = Console.ReadLine();

                            bool isLeaveExist = false;

                            foreach (var dr in drEmp)
                            {
                                if (dr.Field<string>("ID") == leaveID)
                                {
                                    isLeaveExist = true;
                                    break;
                                }
                            }

                            if (!isLeaveExist)
                            {
                                Console.WriteLine("Leave with ID=" + leaveID + " not found.");
                                return false;
                            }
                            else
                            {
                                Console.WriteLine("Choose option to change the status.");
                                Console.WriteLine("1. Approve");
                                Console.WriteLine("2. Reject");
                                Console.WriteLine("3. Pending");

                                string newStatusOption = Console.ReadLine();

                                if (string.IsNullOrEmpty(newStatusOption))
                                {
                                    Console.WriteLine("Error while accepting option.");
                                }
                                else
                                {
                                    

                                    if (newStatusOption == "1" || newStatusOption == "2" || newStatusOption == "3")
                                    {
                                        string newStatus = newStatusOption == "1" ? "Approved" : newStatusOption == "2" ? "Rejected" : "Pending";

                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (dr["ID"].ToString() == leaveID)
                                            {
                                                dr.BeginEdit();
                                                dr["Status"] = newStatus;
                                                dr.EndEdit();

                                                dt.AcceptChanges();
                                                break;
                                            }
                                        }

                                        if (oldTblRowCount == dt.Rows.Count)
                                        {
                                            bool isUpdated = objCsvParser.UpdateCsvFromDataTable(dt, leavesCsvPath);

                                            if (isUpdated)
                                                return true;
                                            else
                                                return false;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Something went wrong.");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Wrong choice.");
                                    }

                                }
                            }

                            break;
                        case "2":

                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["ManagerId"].ToString() == empId.ToString())
                                {
                                    dr.BeginEdit();
                                    dr["Status"] = "Approved";
                                    dr.EndEdit();

                                    dt.AcceptChanges();                                    
                                }
                            }

                            if (oldTblRowCount == dt.Rows.Count)
                            {
                                bool isUpdated = objCsvParser.UpdateCsvFromDataTable(dt, leavesCsvPath);

                                if (isUpdated)
                                    return true;
                                else
                                    return false;
                            }
                            else
                            {
                                Console.WriteLine("Something went wrong.");
                                return false;
                            }

                            break;

                        case "3":

                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["ManagerId"].ToString() == empId.ToString())
                                {
                                    dr.BeginEdit();
                                    dr["Status"] = "Rejected";
                                    dr.EndEdit();

                                    dt.AcceptChanges();
                                }
                            }

                            if (oldTblRowCount == dt.Rows.Count)
                            {
                                bool isUpdated = objCsvParser.UpdateCsvFromDataTable(dt, leavesCsvPath);

                                if (isUpdated)
                                    return true;
                                else
                                    return false;
                            }
                            else
                            {
                                Console.WriteLine("Something went wrong.");
                                return false;
                            }
                            break;

                        default:
                            Console.WriteLine("Wrong Choice.");
                            break;
                    }

                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SearchLeaves(int empId, string searchOption)
        {
            try
            {
                CsvParser objCsvParser = new CsvParser();
                DataTable dt = new DataTable();
                dt = objCsvParser.GetDataTableFromCSV(leavesCsvPath);

                DataView dv = new DataView(dt);

                List<string> lstColumnNames = new List<string>();
                string[] columnNames = { };
                foreach (DataColumn dc in dt.Columns)
                {
                    lstColumnNames.Add(dc.ColumnName);
                }
                columnNames = lstColumnNames.ToArray();

                if (searchOption == "1")
                {
                    Console.WriteLine("Enter Title");
                    string titleToSearch = Console.ReadLine();

                    dt.TableName = "Searched Leaves By Title = " + titleToSearch;

                    dv.RowFilter = "CreatorId = " + empId + "and Title like '%" + titleToSearch + "%'";

                    Console.WriteLine("===========================================================================================");
                    if (dv.Count > 0)
                    {
                        PrintDataExtensions.Print(dv, columnNames);
                        Console.WriteLine("* " + dv.Count + " record(s) found.");
                    }
                    else
                    {
                        Console.WriteLine("There are no matching records.");
                    }
                    Console.WriteLine("===========================================================================================");
                }
                else if (searchOption == "2")
                {
                    Console.WriteLine("Please select the status to search");
                    Console.WriteLine("1. Pending");
                    Console.WriteLine("2. Approved");
                    Console.WriteLine("3. Rejected");
                    string statusChoice = Console.ReadLine();
                    Console.WriteLine("-------------------------------------------------------------------------------------------");

                    switch (statusChoice)
                    {
                        case "1":
                            dt.TableName = "Pending Leaves.";
                            dv.RowFilter = "CreatorId = " + empId + "and Status = 'Pending'";
                            break;
                        case "2":
                            dt.TableName = "Approved Leaves.";
                            dv.RowFilter = "CreatorId = " + empId + "and Status = 'Approved'";
                            break;
                        case "3":
                            dt.TableName = "Rejected Leaves.";
                            dv.RowFilter = "CreatorId = " + empId + "and Status = 'Rejected'";
                            break;
                        default:
                            Console.WriteLine("Wrong Choice.");
                            break;
                    }

                    Console.WriteLine("===========================================================================================");
                    if (dv.Count > 0)
                    {
                        PrintDataExtensions.Print(dv, columnNames);
                        Console.WriteLine("* " + dv.Count + " record(s) found.");
                    }
                    else
                    {
                        Console.WriteLine("There are no matching records.");
                    }
                    Console.WriteLine("===========================================================================================");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool CheckManager(int empId)
        {
            try
            {
                CsvParser objCsvParser = new CsvParser();
                DataTable dt = new DataTable();
                dt = objCsvParser.GetDataTableFromCSV(empCsvPath);

                DataRow[] drEmp = dt.Select("ManagerId='" + empId + "'");

                if (drEmp.Count() > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
