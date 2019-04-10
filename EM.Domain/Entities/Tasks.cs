using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain.Entities
{
    public class Tasks
    {
        [Key]
        [Column(Order = 0)]
        public int idTasks { get; set; }

        [ForeignKey("Evenement")]
        [Column(Order = 1)]

        public int EventId { get; set; }

        //[Key]
        [ForeignKey("User")]
        [Column(Order = 2)]
        public string Id { get; set; }

        public string type { get; set; }

        public virtual Evenement Evenement { get; set; }
        public virtual User User { get; set; }

    }
}
