namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Parameters")]
    public partial class Parameters
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}