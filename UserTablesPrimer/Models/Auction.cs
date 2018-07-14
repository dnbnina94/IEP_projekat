namespace UserTablesPrimer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web;

    [Table("Auction")]
    public partial class Auction
    {
        public string Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Auction name cannot contain over 50 characters.")]
        [Display(Name = "Auction name")]
        public string Name { get; set; }

        [NotMapped]
        [FileTypes("jpg,jpeg,png,gif", ErrorMessage = "Only image files are allowed.")]
        public HttpPostedFileBase ImageToUpload { get; set; }

        [Display(Name = "Image")]
        public byte[] Img { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Auction length must be positive number greater than 0.")]
        [Display(Name = "Auction length (seconds)")]
        public int Length { get; set; }

        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Starting price must be positive number greater than 0.")]
        [Display(Name = "Starting price")]
        public float StartPrice { get; set; }

        public float CurrentPrice { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public int Status { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AspNetUser User { get; set; }

        public string CurrencyId { get; set; }
        [ForeignKey("CurrencyId")]
        public virtual Currencies Currency { get; set; }

        [NotMapped]
        public int minBid { get; set; }

        [NotMapped]
        public List<Bid> Bids { get; set; }

        [NotMapped]
        public String LastBidder { get; set; }

        public string WinnerId { get; set; }
        [ForeignKey("WinnerId")]
        public virtual AspNetUser Winner { get; set; }

        [NotMapped]
        public string timeLeft { get; set; }

    }
}
