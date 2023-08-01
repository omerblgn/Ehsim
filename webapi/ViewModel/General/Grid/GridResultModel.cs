using System.Collections.Generic;

namespace webapi.ViewModel.General.Grid
{
    public class GridResultModel<T>
    {
        public int DataCount { get; set; }
        public IEnumerable<T> List { get; set; }

        public string EkAlan1 { get; set; }
        public string EkAlan2 { get; set; }
    }
}
