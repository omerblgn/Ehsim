using Microsoft.AspNetCore.Http;
using webapi.Helper.Filter;
using webapi.ViewModel.General.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace webapi.Base.Base.Grid
{
    public static class GridPagination
    {
        public static GridFilterModel ToRequestFilter(this HttpRequest request)
        {
            var filterGrid = new GridFilterModel
            {

            };
            if (request.Form != null && request.Form.ContainsKey("pageSize") && request.Form.ContainsKey("pageIndex"))
            {
                int pagesize = 0;
                Int32.TryParse(request.Form["pageSize"].FirstOrDefault(), out pagesize);
                int pageIndex = 0;
                Int32.TryParse(request.Form["pageIndex"].FirstOrDefault(), out pageIndex);


                foreach (var req in request.Form)
                {
                    if (req.Key.Contains("pageSize"))
                    {
                        filterGrid.pageSize = pagesize;
                    }
                    else if (req.Key.Contains("pageIndex"))
                    {
                        filterGrid.pageIndex = pageIndex;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(req.Value.ToString()))
                        {
                            if (filterGrid.ExpressionFilter == null)
                                filterGrid.ExpressionFilter = new List<ExpressionFilter>();
                            string currentKey = req.Key;
                            if (req.Key.Contains("filter["))
                                currentKey = req.Key.Replace("filter[", "");
                            if (currentKey.Contains("]"))
                                currentKey = currentKey.Replace("]", "");


                            if (!String.IsNullOrEmpty(currentKey))
                            {
                                if (currentKey == "sortField" && !string.IsNullOrEmpty(req.Value))
                                {
                                    filterGrid.ExpressionFilter.Add(new ExpressionFilter
                                    {
                                        Comparison = Comparison.SortAsc,
                                        PropertyName = currentKey,
                                        Value = req.Value.ToString()
                                    });
                                }
                                else if (currentKey == "sortOrder" && !string.IsNullOrEmpty(req.Value))
                                {
                                    var sorterField = filterGrid.ExpressionFilter.FirstOrDefault(x => x.Comparison == Comparison.SortAsc);
                                    if (sorterField != null)
                                    {
                                        if (req.Value == "desc")
                                            sorterField.Comparison = Comparison.SortDesc;
                                    }
                                }
                                else if (currentKey == "AllColumn" && !string.IsNullOrEmpty(req.Value))
                                {
                                    filterGrid.ExpressionFilter.Add(new ExpressionFilter
                                    {
                                        Comparison = Comparison.Contains,
                                        PropertyName = currentKey,
                                        Value = req.Value.ToString()
                                    });
                                }
                                else
                                {
                                    string[] keyOfSpace = req.Value.ToString().Split(null);
                                    foreach (var key in keyOfSpace)
                                    {
                                        if (!String.IsNullOrEmpty(key))
                                        {
                                            filterGrid.ExpressionFilter.Add(new ExpressionFilter
                                            {
                                                Comparison = Comparison.Contains,
                                                PropertyName = currentKey,
                                                Value = key
                                            });
                                        }

                                    }

                                }
                            }

                        }
                    }
                }
                //return filterGrid;
            }


            try
            {
                if (request.Form != null && request.Form.ContainsKey("exportData"))
                {
                    //var filterJsGrid = new GridFilterModel
                    //{

                    //};
                    if (request.Form.ContainsKey("columndict"))
                    {
                        filterGrid.IsExcelExport = true;
                    }
                    filterGrid.pageIndex = 0;
                    filterGrid.pageSize = 0;
                    //return filterJsGrid;
                }
            }
            catch (Exception)
            {

            }
            return filterGrid;
        }

        public static GridResultModel<T> ToDataListRequest<T>(this IQueryable<T> data, GridFilterModel gridFilterModel)
        {
            return ToDataListRequestDone(data, gridFilterModel);
        }
        public static GridResultModel<T> ToDataListRequest<T>(this IEnumerable<T> data, GridFilterModel gridFilterModel)
        {
            return ToDataListRequestDone(data.AsQueryable(), gridFilterModel);
        }
        public static GridResultModel<T> ToDataListRequest<T>(this List<T> data, GridFilterModel gridFilterModel)
        {
            return ToDataListRequestDone(data, gridFilterModel);
        }
        public static GridResultModel<T> ToDataListRequestDone<T>(this IQueryable<T> data, GridFilterModel gridFilterModel)
        {

            if (gridFilterModel.ExpressionFilter != null && gridFilterModel.ExpressionFilter.Count() > 0)
            {
                BinaryExpression binaryExpression = null;
                ParameterExpression param = Expression.Parameter(typeof(T), "t");
                string orderbyColumn = "";
                bool isAscending = true;
                foreach (var filter in gridFilterModel.ExpressionFilter)
                {
                    if (!String.IsNullOrEmpty(filter.Value.ToString()) && !string.IsNullOrWhiteSpace(filter.Value.ToString()))
                    {
                        if (filter.Comparison == Comparison.Contains && filter.PropertyName != "exportData" && filter.PropertyName != "excelPdfFileName" && filter.PropertyName != "pdf" && filter.PropertyName != "columndict")
                        {

                            //var currentKey = char.ToUpper(filter.PropertyName[0]) + filter.PropertyName.Substring(1);
                            var property = typeof(T).GetProperty(filter.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            var currentKey = property.Name;

                           
                            if (property != null)
                            {
                                var customColumn = FilterChanger.GetFilterColumnAttr(property);
                                if (customColumn != null)
                                {
                                    property = typeof(T).GetProperty(customColumn);
                                    currentKey = customColumn;
                                }
                                MemberExpression member = Expression.Property(param, currentKey);
                                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                ConstantExpression constant = Expression.Constant(filter.Value.ToLower());

                                if (property.PropertyType.ToString().Contains("Int64") || property.PropertyType.ToString().Contains("Int32") || property.PropertyType.ToString().Contains("Int16") || property.PropertyType.ToString().Contains("float") || property.PropertyType.ToString().Contains("Double"))
                                {
                                    Regex regex = new Regex(@"^\d+$");
                                    Expression isequal = null;
                                    if (regex.IsMatch(filter.Value))
                                    {
                                        var case1 = Expression.Property(param, currentKey);
                                        var case2 = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);
                                        var firstCallForContains = Expression.Call(case1, "ToString", null);
                                        MethodCallExpression result1 = Expression.Call(firstCallForContains, method, constant);
                                        BinaryExpression lastExp = null;
                                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            Expression nullCheck = Expression.NotEqual(member, Expression.Constant(null));
                                            lastExp = Expression.AndAlso(nullCheck, result1);
                                        }
                                        else
                                        {

                                            if (property.PropertyType.ToString().Contains("Int64"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt64(filter.Value), typeof(Int64)));
                                            if (property.PropertyType.ToString().Contains("Int32"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt32(filter.Value), typeof(Int32)));
                                            if (property.PropertyType.ToString().Contains("Int16"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt16(filter.Value), typeof(Int16)));
                                            if (property.PropertyType.ToString().Contains("Double"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToDouble(filter.Value), typeof(Double)));
                                            if (property.PropertyType.ToString().Contains("float"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToDouble(filter.Value), typeof(float)));
                                            lastExp = Expression.OrElse(result1, isequal);
                                        }

                                        if (binaryExpression == null)
                                        {
                                            binaryExpression = lastExp;
                                        }
                                        else
                                        {
                                            binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                        }
                                    }
                                }
                                else if (property.PropertyType.ToString().Contains("DateTime"))
                                {
                                    //var firstCallForToDate = Expression.Call(Expression.Property(param, currentKey), "ToShortDateString", null, null);
                                    //MethodCallExpression result2 = Expression.Call(firstCallForToDate, method, constant);
                                    //var const2=Expression.Constant(filter.Value, typeof(object));
                                    //Expression nullCheck = Expression.NotEqual(member, const2);
                                    //BinaryExpression lastExp = Expression.AndAlso(nullCheck, result2);
                                    //if (binaryExpression == null)
                                    //{
                                    //    binaryExpression = lastExp;
                                    //}
                                    //else
                                    //{
                                    //    binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                    //}
                                    //binaryExpressionsList.Add(lastExp);
                                    //methodCallExpressionsList.Add(result2);
                                }
                                else
                                {
                                    MethodInfo miTrim = typeof(string).GetMethod("Trim", Type.EmptyTypes);
                                    MethodInfo miLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                                    // Trim (x.Number.Trim)
                                    Expression trimMethod = Expression.Call(member, miTrim);
                                    //// LowerCase (x.Number.Trim.ToLower)
                                    Expression toLowerMethod = Expression.Call(trimMethod, miLower);

                                    // The target value ( == "301")
                                    Expression target = Expression.Constant(filter.Value.ToLower(), typeof(string));

                                    MethodCallExpression result3 = Expression.Call(toLowerMethod, method, constant);

                                    Expression nullCheck = Expression.NotEqual(member, Expression.Constant(null, typeof(object)));
                                    Expression emptyCheck = Expression.NotEqual(member, Expression.Constant("", typeof(object)));
                                    BinaryExpression bin1 = Expression.AndAlso(nullCheck, emptyCheck);
                                    BinaryExpression lastExp = Expression.AndAlso(bin1, result3);

                                    if (binaryExpression == null)
                                    {
                                        binaryExpression = lastExp;
                                    }
                                    else
                                    {
                                        binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                    }
                                    //binaryExpressionsList.Add(lastExp);

                                }
                            }
                        }
                        else if (filter.Comparison == Comparison.SortDesc || filter.Comparison == Comparison.SortAsc)
                        {
                            orderbyColumn = filter.Value;

                            //var currentKey = char.ToUpper(filter.Value[0]) + filter.Value.Substring(1);
                            var orderproperty = typeof(T).GetProperty(orderbyColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            //var orderproperty = typeof(T).GetProperty(filter.Value);
                            var customColumn = FilterChanger.GetFilterColumnAttr(orderproperty);
                            if (customColumn != null)
                                orderbyColumn = customColumn;
                            if (filter.Comparison == Comparison.SortAsc)
                            {
                                isAscending = true;
                            }

                            if (filter.Comparison == Comparison.SortDesc)
                            {
                                isAscending = false;
                            }
                        }
                    }
                }

                Expression<Func<T, bool>> finalExpression = null;
                if (binaryExpression != null)
                    finalExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, param);
                var newListForInternal = finalExpression != null ? data.Where(finalExpression) : data;

                if (!String.IsNullOrEmpty(orderbyColumn) && newListForInternal.Count() > 0)
                {
                    string ordd = orderbyColumn;
                    if (orderbyColumn.ToLower().Contains("tarih"))
                    {
                        ordd = orderbyColumn.Replace("String", "");
                    }
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(T)).Find(ordd, true);
                    if (prop != null)
                    {
                        var oredrparam = Expression.Parameter(typeof(T));
                        var neworderProp = typeof(T).GetProperty(prop.Name);
                        var orderExpr = Expression.Lambda<Func<T, object>>(
                            Expression.Convert(Expression.Property(oredrparam, neworderProp), typeof(Object)),
                            oredrparam
                        );

                        List<T> orderedList = null;
                        if (isAscending)
                            orderedList = newListForInternal
                                .OrderBy(orderExpr)
                                .Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1))
                                .Take(gridFilterModel.pageSize).ToList();
                        else
                            orderedList = newListForInternal
                                .OrderByDescending(orderExpr)
                                .Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).
                                Take(gridFilterModel.pageSize).ToList();

                        string datacountOrderList = newListForInternal.Count().ToString();
                        return new GridResultModel<T>
                        {
                            DataCount = Convert.ToInt32(datacountOrderList),
                            List = orderedList
                        };
                    }

                }

                string datacountInternal = newListForInternal.Count().ToString();
                if (gridFilterModel == null || gridFilterModel.IsPdfExport || gridFilterModel.IsExcelExport)
                {
                    return new GridResultModel<T>
                    {
                        DataCount = Convert.ToInt32(datacountInternal),
                        List = newListForInternal.ToList()
                    };
                }
                var newEndedList = newListForInternal.Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).Take(gridFilterModel.pageSize).ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacountInternal),
                    List = newEndedList
                };
            }
            string datacount = data.Count().ToString();
            if (gridFilterModel.pageIndex == 0 && gridFilterModel.pageSize == 0)
            {
                var newlist = data.ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacount),
                    List = newlist
                };
            }
            else
            {
                var newlist = data.Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).Take(gridFilterModel.pageSize).ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacount),
                    List = newlist
                };
            }

        }

        public static GridResultModel<T> ToDataListRequestDone<T>(this List<T> data, GridFilterModel gridFilterModel)
        {

            if (gridFilterModel.ExpressionFilter != null && gridFilterModel.ExpressionFilter.Count() > 0)
            {
                BinaryExpression binaryExpression = null;
                ParameterExpression param = Expression.Parameter(typeof(T), "t");
                string orderbyColumn = "";
                bool isAscending = true;
                foreach (var filter in gridFilterModel.ExpressionFilter)
                {
                    if (!String.IsNullOrEmpty(filter.Value.ToString()) && !string.IsNullOrWhiteSpace(filter.Value.ToString()))
                    {

                        if (filter.Comparison == Comparison.Contains && filter.PropertyName != "exportData" && filter.PropertyName != "excelPdfFileName" && filter.PropertyName != "pdf" && filter.PropertyName != "columndict")
                        {
                            var currentKey = char.ToUpper(filter.PropertyName[0]) + filter.PropertyName.Substring(1);
                            var property = typeof(T).GetProperty(currentKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            
                            if (property != null)
                            {
                                var customColumn = FilterChanger.GetFilterColumnAttr(property);
                                if (customColumn != null)
                                {
                                    property = typeof(T).GetProperty(customColumn);
                                    currentKey = customColumn;
                                }
                                MemberExpression member = Expression.Property(param, currentKey);
                                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                ConstantExpression constant = Expression.Constant(filter.Value.ToLower());

                                if (property.PropertyType.ToString().Contains("Int64") || property.PropertyType.ToString().Contains("Int32") || property.PropertyType.ToString().Contains("Int16") || property.PropertyType.ToString().Contains("float") || property.PropertyType.ToString().Contains("Double"))
                                {
                                    Regex regex = new Regex(@"^\d+$");
                                    Expression isequal = null;
                                    if (regex.IsMatch(filter.Value))
                                    {
                                        var case1 = Expression.Property(param, currentKey);
                                        var case2 = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);
                                        var firstCallForContains = Expression.Call(case1, "ToString", null);
                                        MethodCallExpression result1 = Expression.Call(firstCallForContains, method, constant);
                                        BinaryExpression lastExp = null;
                                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                        {
                                            Expression nullCheck = Expression.NotEqual(member, Expression.Constant(null));
                                            lastExp = Expression.AndAlso(nullCheck, result1);
                                        }
                                        else
                                        {

                                            if (property.PropertyType.ToString().Contains("Int64"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt64(filter.Value), typeof(Int64)));
                                            if (property.PropertyType.ToString().Contains("Int32"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt32(filter.Value), typeof(Int32)));
                                            if (property.PropertyType.ToString().Contains("Int16"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToInt16(filter.Value), typeof(Int16)));
                                            if (property.PropertyType.ToString().Contains("Double"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToDouble(filter.Value), typeof(Double)));
                                            if (property.PropertyType.ToString().Contains("float"))
                                                isequal = Expression.Equal(member, Expression.Constant(Convert.ToDouble(filter.Value), typeof(float)));
                                            lastExp = Expression.OrElse(result1, isequal);
                                        }

                                        if (binaryExpression == null)
                                        {
                                            binaryExpression = lastExp;
                                        }
                                        else
                                        {
                                            binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                        }
                                    }
                                }
                                else if (property.PropertyType.ToString().Contains("DateTime"))
                                {
                                    //var firstCallForToDate = Expression.Call(Expression.Property(param, currentKey), "ToShortDateString", null, null);
                                    //MethodCallExpression result2 = Expression.Call(firstCallForToDate, method, constant);
                                    //var const2=Expression.Constant(filter.Value, typeof(object));
                                    //Expression nullCheck = Expression.NotEqual(member, const2);
                                    //BinaryExpression lastExp = Expression.AndAlso(nullCheck, result2);
                                    //if (binaryExpression == null)
                                    //{
                                    //    binaryExpression = lastExp;
                                    //}
                                    //else
                                    //{
                                    //    binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                    //}
                                    //binaryExpressionsList.Add(lastExp);
                                    //methodCallExpressionsList.Add(result2);
                                }
                                else
                                {
                                    MethodInfo miTrim = typeof(string).GetMethod("Trim", Type.EmptyTypes);
                                    MethodInfo miLower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                                    // Trim (x.Number.Trim)
                                    Expression trimMethod = Expression.Call(member, miTrim);
                                    //// LowerCase (x.Number.Trim.ToLower)
                                    Expression toLowerMethod = Expression.Call(trimMethod, miLower);

                                    // The target value ( == "301")
                                    Expression target = Expression.Constant(filter.Value.ToLower(), typeof(string));

                                    MethodCallExpression result3 = Expression.Call(toLowerMethod, method, constant);

                                    Expression nullCheck = Expression.NotEqual(member, Expression.Constant(null, typeof(object)));
                                    Expression emptyCheck = Expression.NotEqual(member, Expression.Constant("", typeof(object)));
                                    BinaryExpression bin1 = Expression.AndAlso(nullCheck, emptyCheck);
                                    BinaryExpression lastExp = Expression.AndAlso(bin1, result3);

                                    if (binaryExpression == null)
                                    {
                                        binaryExpression = lastExp;
                                    }
                                    else
                                    {
                                        binaryExpression = Expression.OrElse(binaryExpression, lastExp);
                                    }
                                    //binaryExpressionsList.Add(lastExp);

                                }
                            }
                        }
                        else if (filter.Comparison == Comparison.SortDesc || filter.Comparison == Comparison.SortAsc)
                        {
                            orderbyColumn = filter.Value;

                            //var currentKey = char.ToUpper(filter.Value[0]) + filter.Value.Substring(1);
                            var orderproperty = typeof(T).GetProperty(orderbyColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            //var orderproperty = typeof(T).GetProperty(filter.Value);
                            var customColumn = FilterChanger.GetFilterColumnAttr(orderproperty);
                            if (customColumn != null)
                                orderbyColumn = customColumn;
                            if (filter.Comparison == Comparison.SortAsc)
                            {
                                isAscending = true;
                            }

                            if (filter.Comparison == Comparison.SortDesc)
                            {
                                isAscending = false;
                            }
                        }
                    }
                }

                Expression<Func<T, bool>> finalExpression = null;
                if (binaryExpression != null)
                    finalExpression = Expression.Lambda<Func<T, bool>>(binaryExpression, param);
                var newListForInternal = finalExpression != null ? data.ToList().AsQueryable().Where(finalExpression) : data.AsQueryable();

                if (!String.IsNullOrEmpty(orderbyColumn))
                {
                    string ordd = orderbyColumn;
                    if (orderbyColumn.ToLower().Contains("tarih"))
                    {
                        ordd = orderbyColumn.Replace("String", "");
                    }
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(T)).Find(ordd, true);
                    if (prop != null)
                    {
                        var oredrparam = Expression.Parameter(typeof(T));
                        var neworderProp = typeof(T).GetProperty(prop.Name);
                        var orderExpr = Expression.Lambda<Func<T, object>>(
                            Expression.Convert(Expression.Property(oredrparam, neworderProp), typeof(Object)),
                            oredrparam
                        );

                        List<T> orderedList = null;
                        if (isAscending)
                            orderedList = newListForInternal.ToList().AsQueryable()
                                .OrderBy(orderExpr)
                                .Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1))
                                .Take(gridFilterModel.pageSize).ToList();
                        else
                            orderedList = newListForInternal.ToList().AsQueryable()
                                .OrderByDescending(orderExpr)
                                .Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).
                                Take(gridFilterModel.pageSize).ToList();

                        string datacountOrderList = newListForInternal.Count().ToString();
                        return new GridResultModel<T>
                        {
                            DataCount = Convert.ToInt32(datacountOrderList),
                            List = orderedList
                        };
                    }

                }

                string datacountInternal = newListForInternal.Count().ToString();
                if (gridFilterModel == null || gridFilterModel.IsPdfExport || gridFilterModel.IsExcelExport)
                {
                    return new GridResultModel<T>
                    {
                        DataCount = Convert.ToInt32(datacountInternal),
                        List = newListForInternal.ToList()
                    };
                }
                var newEndedList = newListForInternal.Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).Take(gridFilterModel.pageSize).ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacountInternal),
                    List = newEndedList
                };
            }
            string datacount = data.Count().ToString();
            if (gridFilterModel.pageIndex == 0 && gridFilterModel.pageSize == 0)
            {
                var newlist = data.ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacount),
                    List = newlist
                };
            }
            else
            {
                var newlist = data.Skip(gridFilterModel.pageSize * (gridFilterModel.pageIndex - 1)).Take(gridFilterModel.pageSize).ToList();
                return new GridResultModel<T>
                {
                    DataCount = Convert.ToInt32(datacount),
                    List = newlist
                };
            }

        }


        public static Expression<Func<T, bool>> ConstructAndExpressionTree<T>(List<ExpressionFilter> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
            {
                exp = ExpressionRetriever.GetExpression<T>(param, filters[0]);
            }
            else
            {
                exp = ExpressionRetriever.GetExpression<T>(param, filters[0]);
                for (int i = 1; i < filters.Count; i++)
                {
                    exp = Expression.And(exp, ExpressionRetriever.GetExpression<T>(param, filters[i]));
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        public static class ExpressionRetriever
        {
            private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
            private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

            public static Expression GetExpression<T>(ParameterExpression param, ExpressionFilter filter)
            {
                MemberExpression member = Expression.Property(param, filter.PropertyName);
                ConstantExpression constant = Expression.Constant(filter.Value);
                switch (filter.Comparison)
                {
                    case Comparison.Equal:
                        return Expression.Equal(member, constant);
                    case Comparison.GreaterThan:
                        return Expression.GreaterThan(member, constant);
                    case Comparison.GreaterThanOrEqual:
                        return Expression.GreaterThanOrEqual(member, constant);
                    case Comparison.LessThan:
                        return Expression.LessThan(member, constant);
                    case Comparison.LessThanOrEqual:
                        return Expression.LessThanOrEqual(member, constant);
                    case Comparison.NotEqual:
                        return Expression.NotEqual(member, constant);
                    case Comparison.Contains:
                        return Expression.Call(member, containsMethod, constant);
                    case Comparison.StartsWith:
                        return Expression.Call(member, startsWithMethod, constant);
                    case Comparison.EndsWith:
                        return Expression.Call(member, endsWithMethod, constant);
                    default:
                        return null;
                }
            }
        }





    }
    public static class QueryHelper
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method =>
            method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
            method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        public static bool PropertyExists<T>(this IQueryable<T> source, string propertyName)
        {
            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static IQueryable<T> OrderByProperty<T>(
           this IQueryable<T> source, string propertyName)
        {
            if (typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                BindingFlags.Public | BindingFlags.Instance) == null)
            {
                return null;
            }
            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod =
              OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }

        public static IQueryable<T> OrderByPropertyDescending<T>(
            this IQueryable<T> source, string propertyName)
        {
            if (typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                BindingFlags.Public | BindingFlags.Instance) == null)
            {
                return null;
            }
            ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
            LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
            MethodInfo genericMethod =
              OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            object ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }
    }
}
