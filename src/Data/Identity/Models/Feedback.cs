using Data.Enums;
using Data.Identity.Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Identity.Models
{
    public class Feedback
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string FeedbackId { get; set; }

        public string UserId { get; set; }
        public IdentityWebUser User { get; set; }

        public EnumFeedbackCategory FeedbackCategory { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
