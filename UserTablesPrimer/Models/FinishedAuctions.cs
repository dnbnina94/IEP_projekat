namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("FinishedAuctions")]
    public class FinishedAuctions
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }

        public string AuctionId { get; set; }
        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }
    }
}