using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EM.Presentation.Models
{
    public class ReplyVM
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public String Text { get; set; }
        [ScaffoldColumn(false)]
        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }

        [Display(Name ="Reply")]
        public string ParticipantId { get; set; }
        public IEnumerable<SelectListItem> Participants { get; set; }

        [Display(Name ="Comment")]
        public int? CommentId { get; set; }
        public IEnumerable<SelectListItem> Comments { get; set; }
    }
}