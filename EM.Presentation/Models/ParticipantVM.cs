using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EM.Presentation.Models
{
    public class ParticipantVM
    {
        [Key]
        public string ParticipantId { get; set; }
        public string ParticipantName { get; set; }
        [ScaffoldColumn(false)]
        public DateTime CreatedOn { get; set; }
    }
}