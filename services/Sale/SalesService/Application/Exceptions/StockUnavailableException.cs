namespace SalesService.Application.Exceptions
{
    public class StockUnavailableException : Exception
    {
        public StockUnavailableException(string message) : base(message) { }
    }
}
