namespace backend.Models
{
    public class PromoUsage
    {
        public Guid id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public Guid PromoId { get; set; }

        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;

        public virtual Promo Promo { get; set; } = null!;
    }
}
