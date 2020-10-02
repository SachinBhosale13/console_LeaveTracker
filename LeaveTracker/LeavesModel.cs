using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveTracker
{
    public class LeavesModel
    {
        public int ID { get; set; }
        public int CreaterId { get; set; }
        public string CreaterName { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
    }
}
