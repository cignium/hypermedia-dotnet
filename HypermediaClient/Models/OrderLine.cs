namespace HypermediaClient {
    public class OrderLine {
        public decimal Amount => UnitPrice * Units;
        public decimal UnitPrice { get; set; }
        public string Text { get; set; }
        public decimal Units { get; set; }
    }
}