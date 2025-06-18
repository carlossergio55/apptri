using Aplicacion.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Persistencia.Contexts;
using Persistencia.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Persistencia.Repository.Common.Aplicacion
{
    public class AppRepositoryAsync<T> : RepositoryBase<T>, IRepositoryAsync<T> where T : class
    {
        private readonly AplicationDbContext _dbContext;
     

        public AppRepositoryAsync(AplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
          
        }
	
		public async Task<List<T>> CallFunctionReFCursor(string nameFunction, params object[] param)
        {
            List<T> vList = new();
            await _dbContext.Database.GetDbConnection().OpenAsync();
            using (var transaccion = await _dbContext.Database.GetDbConnection().BeginTransactionAsync())
            {
                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Transaction = transaccion;
                    for (int i = 0; i < param.Length; i++)
                    {
                        if (param[i].GetType().Name.Contains("DateTime"))
                        {
                            cmd.Parameters.Add(new NpgsqlParameter
                            {
                                Value = param[i],
                                DbType = (Convert.ToDateTime(param[i]).Hour == 0 &&
                                                            Convert.ToDateTime(param[i]).Minute == 0 &&
                                                            Convert.ToDateTime(param[i]).Second == 0) ? DbType.Date : DbType.DateTime
                            });
                        }
                        else
                        {
                            cmd.Parameters.Add(new NpgsqlParameter { Value = param[i] });
                        }
                    }

                    cmd.CommandText = nameFunction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    await cmd.PrepareAsync();

                    var refcursor = await cmd.ExecuteScalarAsync();

                    cmd.CommandText = $"FETCH ALL in \"{refcursor}\";";
                    cmd.CommandType = CommandType.Text;
                    await cmd.PrepareAsync();


                    using (IDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        vList = DataReaderMapToList<T>(dr);
                    }
                }
                //  await t.CommitAsync();
            };

            return vList;
        }

        private List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            DataTable dtSchema = dr.GetSchemaTable();

            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!dtSchema.AsEnumerable().Any(a => a.Field<string>("ColumnName").Equals(CHelper.ToUnderscoreCase(prop.Name))))
                    {
                        continue;
                    }

                    if (!object.Equals(dr[CHelper.ToUnderscoreCase(prop.Name)], DBNull.Value))
                    {
                        prop.SetValue(obj, prop.PropertyType.FullName.Contains("Date") ?
                                                Convert.ToDateTime(dr[CHelper.ToUnderscoreCase(prop.Name)]) :
                                                dr[CHelper.ToUnderscoreCase(prop.Name)], null);
                    }
                    else
                    {
                        prop.SetValue(obj, null, null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

    }
}
