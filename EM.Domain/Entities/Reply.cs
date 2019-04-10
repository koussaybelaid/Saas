using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain.Entities
{
   public class Reply
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Text { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? CreatedOn { get; set; }
        [Required]
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }
}
