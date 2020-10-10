using System;

namespace AVC.DatabaseModels
{
    public class Log : BaseEntity
    {
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual int gpio { get; set; }
        public virtual int value { get; set; }
        public virtual bool status { get; set; }
        public virtual long start { get; set; }
        public virtual long finish { get; set; }
        public virtual string _start { get => start == 0 ? "" : DateTimeOffset.FromUnixTimeSeconds(start).LocalDateTime.ToString(); }
        public virtual string _finish { get => finish == 0 ? "" : DateTimeOffset.FromUnixTimeSeconds(finish).LocalDateTime.ToString(); }
    }
}