using System;

namespace AVC.DatabaseModels
{
    public class Total
    {
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual int gpio { get; set; }
        public virtual long totalTime { get; set; }
        public virtual string _totalTime { get => totalTime == 0 ? "" : TimeSpan.FromSeconds(totalTime).ToString(); }
    }
}