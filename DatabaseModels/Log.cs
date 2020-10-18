using System;

namespace AVC.DatabaseModels
{
    public class Log : BaseEntity
    {
        public virtual string ip { get; set; }
        public virtual GPIO gpio { get; set; }
        public virtual string display { get => DateTimeOffset.FromUnixTimeSeconds(timeCreate).LocalDateTime.ToString(); }
    }
}