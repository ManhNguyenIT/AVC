using System;
using System.Collections.Generic;

namespace AVC.DatabaseModels
{
    public class Machine : BaseEntity
    {
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual bool status { get; set; }
        public virtual List<GPIO> gpio { get; set; }
        public virtual long _totalON { get; set; }
        public virtual long _totalOFF { get; set; }
        public virtual string TotalON { get => _totalON == 0 ? "" : TimeSpan.FromSeconds(_totalON).ToString(); }
        public virtual string TotalOFF { get => _totalOFF == 0 ? "" : TimeSpan.FromSeconds(_totalOFF).ToString(); }
    }
}