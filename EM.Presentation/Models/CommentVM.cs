using EM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EM.Presentation.Models
{
    public class CommentVM
    {
       
        public int Id { get; set; }
        [Required(ErrorMessage ="this field ca not be null")]
        public string Text { get; set; }
        
        public DateTime CreatedOn { get; set; }

        [Display(Name ="Participant")]
        public string ParticipantId { get; set; }

        public IEnumerable<SelectListItem> Participants { get; set; }

        public ICollection<Reply> Replies { get; set; }



    }
}