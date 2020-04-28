using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UIDP.BIZModule.wy.Models;
using UIDP.ODS.wy;

namespace UIDP.BIZModule.wy
{
    public class TaskDetailConfigModule
    {
        TaskDetailConfigDB db = new TaskDetailConfigDB();

        public Dictionary<string,object> GetTaskDetailConfig()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                List<TaskDetailConfigNode> list = CreateTaskDetailConfigNode(db.GetTaskDetailConfig());
                if (list.Count > 0)
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                    r["items"] = list;
                }
                else
                {
                    r["code"] = 2001;
                    r["message"] = "成功,但是没有数据";
                }
            }
            catch(Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }

        private List<TaskDetailConfigNode> CreateTaskDetailConfigNode(DataTable dt)
        {
            List<TaskDetailConfigNode> list = new List<TaskDetailConfigNode>();
            foreach(DataRow dr in dt.Select("ParentID is null or ParentID=''"))
            {
                TaskDetailConfigNode node = new TaskDetailConfigNode()
                {
                    ID = dr["ID"].ToString(),
                    ParentID = dr["ParentID"].ToString(),
                    Code = dr["Code"].ToString(),
                    Name = dr["Name"].ToString()
                };
                CreateChildrenTaskDetailConfigNode(node, dt);
                list.Add(node);
            }
            return list;
        }

        private void CreateChildrenTaskDetailConfigNode(TaskDetailConfigNode Parentnode,DataTable dt)
        {
            Parentnode.children = new List<TaskDetailConfigNode>();
            foreach(DataRow dr in dt.Select("ParentID='" + Parentnode.ID + "'"))
            {
                TaskDetailConfigNode childrennode = new TaskDetailConfigNode()
                {
                    ID = dr["ID"].ToString(),
                    ParentID = dr["ParentID"].ToString(),
                    Code = dr["Code"].ToString(),
                    Name = dr["Name"].ToString()
                };
                CreateChildrenTaskDetailConfigNode(childrennode, dt);
                Parentnode.children.Add(childrennode);
            }
        }

        public Dictionary<string, object> CreateTaskDetailConfig(Dictionary<string, object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.CreateTaskDetailConfig(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string,object> UpdateTaskDetailConfig(Dictionary<string,object> d)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.UpdateTaskDetailConfig(d);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch(Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string,object> DeleteTaskDetailConfig(string ID)
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                string b = db.DeleteTaskDetailConfig(ID);
                if (b == "")
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                }
                else
                {
                    r["message"] = b;
                    r["code"] = -1;
                }
            }
            catch (Exception e)
            {
                r["message"] = e.Message;
                r["code"] = -1;
            }
            return r;
        }

        public Dictionary<string, object> GetParentCodeConfig()
        {
            Dictionary<string, object> r = new Dictionary<string, object>();
            try
            {
                DataTable dt =db.GetParentCodeConfig();
                if (dt.Rows.Count > 0)
                {
                    r["message"] = "成功";
                    r["code"] = 2000;
                    r["items"] = dt;
                }
                else
                {
                    r["code"] = 2001;
                    r["message"] = "成功,但是没有数据";
                }
            }
            catch (Exception e)
            {
                r["code"] = -1;
                r["message"] = e.Message;
            }
            return r;
        }
    }
}
