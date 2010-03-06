using System;


namespace Izume.Mobile.Odeo.Common
{
    /// <summary>
    /// Represents item in task list.
    /// </summary>
    public class TasklistItem
    {
        public enum ItemState
        {
            New = 0,
            Incomplete,
            Complete,
            Failed,
            Cancelled
        }

        // internal fields
        string m_taskId;
        string m_name;
        string m_description;
        ItemState m_state;
        long m_total;
        long m_completed;
        
        // properties
        public string TaskId
        {
            get { return m_taskId; }
            set { m_taskId = value; }
        }

        public ItemState State
        {
            get { return m_state; }
            set { m_state = value; }
        }
        

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }


        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }


        public long Total
        {
            get { return m_total; }
            set { m_total = value; }
        }

        public long Completed
        {
            get { return m_completed; }
            set { m_completed = value; }
        }

        public int PercentComplete
        {
            get 
            { 
                double ratio = (this.Completed / (double) this.Total);
                return (int)(ratio * 100);
            }
        }


        // ctor
        public TasklistItem(string taskId, string taskName)
        {
            this.TaskId = taskId;
            this.Name = taskName;
            this.Description = String.Empty;
            this.State = ItemState.New;
            this.Completed = 0;            
        }

        public TasklistItem(string taskId, string name, ItemState state, int total, int completed)
        {
            this.TaskId = taskId;
            this.Name = name;
            this.Description = String.Empty;
            this.State = state;
            this.Total = total;
            this.Completed = completed;
        }

    }
}
