using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "this field ca not be null")]
        public string Text { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }
        [Required]
        public string ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }

        public int? EventId { get; set; }
        public Evenement Evenement { get; set; }
    }

}
