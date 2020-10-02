using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;

namespace LeaveTracker
{
    public interface IValidator
    {        
        bool ValidateEmployee(int id);
    }
    public class Validator:IValidator
    {
        public bool ValidateEmployee(int empId)
        {
            try
            {
                DataTable dt = new DataTable();
                string empCsvPath = ConfigurationSettings.AppSettings["EmployeesCsvPath"];

                CsvParser objCsvParser = new CsvParser();

                dt = objCsvParser.GetDataTableFromCSV(empCsvPath);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int rowEmpId = Convert.ToInt32(dt.Rows[i]["Id"]);
                    if (empId == rowEmpId)
                    {
                        return true;                       
                    }
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
