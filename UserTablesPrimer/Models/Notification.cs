namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("Notification")]
    public partial class Notification
    {
        [Key]
        public string Id { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }

        public int Read { get; set; }

        public DateTime CreatedOn { get; set; }
        
    }
}