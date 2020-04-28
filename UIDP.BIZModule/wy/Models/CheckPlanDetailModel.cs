using System;
using System.Collections.Generic;
using System.Text;

namespace UIDP.BIZModule.wy.Models
{
    public class CheckPlanDetailModel
    {
        public string PLAN_DETAIL_ID { get; set; }
        public string PLAN_ID { get; set; }
        public string[] JCQY { get; set; }
        public string JCNR { get; set; }
        public string JCLX { get; set; }
        public int PCCS { get; set; }
        public string CJR { get; set; }
        public string CJSJ { get; set; }
        public string BJR { get; set; }
        public string BJSJ { get; set; }
        public int IS_DELETE { get; set; }
        public string ALLPLACENAME { get; set; }
        public string JCNAME { get; set; }
    }
}
