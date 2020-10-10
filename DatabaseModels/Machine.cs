using System;

namespace AVC.DatabaseModels
{
    public class Machine : BaseEntity
    {
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual bool status { get; set; }
        public virtual int gpio { get; set; }
        public virtual int value { get; set; }
        public virtual long totalTime { get; set; }
        public virtual string _totalTime { get => totalTime == 0 ? "" : TimeSpan.FromSeconds(totalTime).ToString(); }

    }
}