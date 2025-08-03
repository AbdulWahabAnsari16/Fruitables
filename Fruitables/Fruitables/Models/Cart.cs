namespace Fruitables.Models
{
    public class Cart
    {
        public IEnumerable<addtocart> addtocarts  { get; set; }
        public string coupons { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
    }
}
