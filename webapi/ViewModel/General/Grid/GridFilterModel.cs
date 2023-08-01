using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webapi.ViewModel.General.Grid
{
    public class GridFilterModel
    {
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
        public string allColumn { get; set; }
        public List<ExpressionFilter> ExpressionFilter { get; set; }
        public bool IsExcelExport { get; set; }
        public bool IsPdfExport { get; set; }
    }
    public class ExpressionFilter
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public Comparison Comparison { get; set; }
    }
    public enum Comparison
    {
        Equal,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        NotEqual,
        Contains, //for strings  
        StartsWith, //for strings  
        EndsWith, //for strings  ,
        SortAsc,
        SortDesc
    }
}
