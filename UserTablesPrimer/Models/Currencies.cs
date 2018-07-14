namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Currencies")]
    public partial class Currencies
    {
        [Key]
        public string Name { get; set; }

        public float Value { get; set; }
    }
}