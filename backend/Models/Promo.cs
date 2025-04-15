using backend.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Promo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsPercentage { get; set; } = true;


        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; } 

        public decimal? MaxDiscountAmount { get; set; }


        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxUsage { get; set; }

        [Range(0, int.MaxValue)]
        public int UsageCount { get; set; } = 0;

        public virtual ICollection<PromoUsage> PromoUsages { get; set; } = [];
    }
}
