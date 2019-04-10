using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.Domain.Entities
{
    public class Participant : User
    {
        public virtual ICollection<Comment> comments { get; set; }
        public virtual ICollection<Reply> Replies { get; set; }
    }
}
