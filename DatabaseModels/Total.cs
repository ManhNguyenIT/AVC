using System;

namespace AVC.DatabaseModels
{
    public class Total : BaseEntity
    {
        public virtual string date { get; set; }
        public virtual Machine machine { get; set; }
        public virtual long _totalON { get; set; }
        public virtual long _totalOFF { get; set; }
        public virtual string TotalON { get => _totalON == 0 ? "" : TimeSpan.FromSeconds(_totalON).ToString(); }
        public virtual string TotalOFF { get => _totalOFF == 0 ? "" : TimeSpan.FromSeconds(_totalOFF).ToString(); }
    }
}