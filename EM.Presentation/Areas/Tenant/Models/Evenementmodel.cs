using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EM.Presentation.Areas.Tenant.Models
{
    public class Evenementmodel
    {
        [Key]
        public int EventId { get; set; }

        public string Picture { get; set; }
        public string theme { get; set; }
        public string location { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        //public virtual ICollection<Tasks> Tasks { get; set; }
    }
}