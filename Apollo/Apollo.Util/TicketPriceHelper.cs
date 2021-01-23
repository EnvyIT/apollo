using System.Collections.Generic;
using System.Linq;

namespace Apollo.Util
{
    public static class TicketPriceHelper
    {
        public static decimal CalculatePrice(decimal basePrice, IEnumerable<double> priceFactors)
        {
            return priceFactors.Sum(priceFactor => (decimal) priceFactor * basePrice);
        }
    }
}