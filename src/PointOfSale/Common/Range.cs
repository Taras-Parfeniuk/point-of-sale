namespace PointOfSale.Common
{
    public class Range
    {
        public Range(decimal? amountFrom, decimal? amountTo)
        {
            From = amountFrom;
            To = amountTo;
        }

        public decimal? From { get; }
        public decimal? To { get; }

        public bool IsInRange(decimal value)
        {
            return (From == default || value >= From) && (To == default || value <= To);
        }
    }
}
