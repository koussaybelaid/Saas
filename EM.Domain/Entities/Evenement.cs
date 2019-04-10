using EM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain.Entities
{
    public class Evenement
    {
        [Key]
        public int EventId { get; set; }

        public string Picture { get; set; }
        public string theme { get; set; }
        public string location { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

    }
}
