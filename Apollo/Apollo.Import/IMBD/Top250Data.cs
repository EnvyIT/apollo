using System.Collections.Generic;

namespace Apollo.Import.IMBD
{
    public class Top250Data
    {
        public List<Top250DataDetail> Items { get; set; }

        public string ErrorMessage { get; set; }
    }
}
