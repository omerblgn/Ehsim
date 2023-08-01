using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace webapi.Helper.Filter
{

    [AttributeUsage(AttributeTargets.All)]
    public class FilterColumnAttribute : Attribute
    {
        public string toFilteredColumn;
        public FilterColumnAttribute(string toFilteredColumn)
        {
            this.toFilteredColumn = toFilteredColumn;
        }
    }
    public static class FilterChanger
    {
        public static string GetFilterColumnAttr(PropertyInfo info)
        {
            object[] attributes = info.GetCustomAttributes(typeof(FilterColumnAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                var displayName = (FilterColumnAttribute)attributes[0];
                return displayName.toFilteredColumn;
            }
            return null;
        }

    }


}
