using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FasterDevelopFM.Extension.EntityFrameworkExtension
{
    public static class EntityFrameworkExtension
    {
        //sql查询
        public static DataTable SqlQuery(this DatabaseFacade facade, string sql, params object[] parameters)
        {
            var conn = facade.GetDbConnection();
            var dt = new DataTable();
            try
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                var reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                conn.Close();
                return dt;
            }
            catch (DataException e)
            {
                System.Data.DataRow[] rowsInError;
                System.Text.StringBuilder sbError = new System.Text.StringBuilder();
                // Test if the table has errors. If not, skip it.
                if (dt.HasErrors)
                {
                    // Get an array of all rows with errors.
                    rowsInError = dt.GetErrors();
                    // Print the error of each column in each row.
                    for (int i = 0; i < rowsInError.Length; i++)
                    {
                        foreach (System.Data.DataColumn column in dt.Columns)
                        {
                            sbError.Append(column.ColumnName + " " + rowsInError[i].GetColumnError(column));
                        }
                        rowsInError[i].ClearErrors();
                    }
                }
                Console.WriteLine(sbError.ToString());
                throw e;
            }
        }
        //sql查询泛型列表
        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var dt = SqlQuery(facade, sql, parameters);
            return dt.ToList<T>();
        }

        //反射转换泛型列表方法  支持匿名
        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            return list;
        }

    }
}