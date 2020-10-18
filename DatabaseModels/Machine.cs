using System.Collections.Generic;

namespace AVC.DatabaseModels
{
    public class Machine : BaseEntity
    {
        public virtual string ip { get; set; }
        public virtual string name { get; set; }
        public virtual bool status { get; set; }
        public virtual List<GPIO> gpio { get; set; }
    }
}