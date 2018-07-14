namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("Order")]
    public partial class Order
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }

        public int Tokens { get; set; }

        public float Price { get; set; }

        public int Status { get; set; }

        public string CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currencies Currency { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedOn { get; set; }
    }
}