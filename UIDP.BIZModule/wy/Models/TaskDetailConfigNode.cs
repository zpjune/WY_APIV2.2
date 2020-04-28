using System;
using System.Collections.Generic;
using System.Text;

namespace UIDP.BIZModule.wy.Models
{
    public class TaskDetailConfigNode
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<TaskDetailConfigNode> children { get; set; }
    }
}

