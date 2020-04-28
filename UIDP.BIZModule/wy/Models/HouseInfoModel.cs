using System;
using System.Collections.Generic;
using System.Text;

namespace UIDP.BIZModule.wy.Models
{
    public class HouseInfoModel
    {
        public string FWID { get; set; }
        public int? FWSX { get; set; }
        public string FWBH { get; set; }
        public string FWMC { get; set; }
        public decimal? JZMJ { get; set; }
        public string LSFGS { get; set; }
        public string ZLWZ { get; set; }
        public string ZLMJ { get; set; }
        public string JGLX { get; set; }
        public decimal? ZCYZ { get; set; }
        public string SSQY { get; set; }
        public string WATER_NUMBER { get; set; }
        public string ELE_NUMBER { get; set; }
        public string CZ_SHID { get; set; }
        //public string LEASE_ID { get; set; }
        //public string FEE_ID { get; set; }
        public string CJR { get; set; }
        public string CJSJ { get; set; }
        public string BJR { get; set; }
        public string BJSJ { get; set; }
        public int IS_DELETE { get; set; }
        public decimal? ZFK { get; set; }
        public List<file> PMT { get; set; }
        public string newFilePath { get; set; }
        public string LS { get; set; }
        public string JG { get; set; }

        public string SS { get; set; }

        public string CID { get; set; }
    }
}
