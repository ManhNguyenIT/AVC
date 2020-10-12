using System;

namespace AVC.DatabaseModels
{
    public class Log : BaseEntity
    {
        public virtual Machine machine { get; set; }
        public virtual long start { get; set; }
        public virtual long finish { get; set; }
        public virtual string total { get => (DateTimeOffset.FromUnixTimeSeconds(finish).LocalDateTime - DateTimeOffset.FromUnixTimeSeconds(start).LocalDateTime).ToString(); }
    }
}