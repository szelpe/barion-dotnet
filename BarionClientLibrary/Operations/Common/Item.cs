namespace BarionClientLibrary.Operations.Common
{
    public class Item
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ItemTotal { get; set; }
        public string? SKU { get; set; }
    }
}