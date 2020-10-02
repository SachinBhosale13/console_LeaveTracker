using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace LeaveTracker
{
    public interface ICsvParser
    {
        DataTable GetDataTableFromCSV(string csvFilePath);
        bool UpdateCsvFromDataTable(DataTable dt, string FilePath);
    }

    public class CsvParser : ICsvParser
    {
        public DataTable GetDataTableFromCSV(string csvFilePath)
        {            
            try
            {
                StreamReader sr = new StreamReader(csvFilePath);
                string[] headers = sr.ReadLine().Split(',');
                DataTable dt = new DataTable();
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
                sr.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateCsvFromDataTable(DataTable dt, string FilePath)
        {
            try
            {
                string fileDirectory = Path.GetDirectoryName(FilePath);

                string tempFilePath = fileDirectory + "\\" + "tempLeaves.csv";

                StreamWriter sw = new StreamWriter(tempFilePath, false);

                //headers  
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sw.Write(dt.Columns[i]);
                    if (i < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                        if (i < dt.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();

                try
                {
                    File.Copy(tempFilePath, FilePath,true);
                    File.Delete(tempFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
