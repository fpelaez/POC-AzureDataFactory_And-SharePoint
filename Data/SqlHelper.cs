using SP_Provider.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class SqlHelper
    {
        EntityConnectionStringBuilder entityBuilder = null;

        public SqlHelper(string connectionString)
        {
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.ConnectionString = connectionString;
            string providerString = sqlBuilder.ToString();

            entityBuilder = new EntityConnectionStringBuilder();
            entityBuilder.Provider = "System.Data.SqlClient";
            entityBuilder.ProviderConnectionString = providerString;
            entityBuilder.Metadata = @"res://*/Model2.csdl|res://*/Model2.ssdl|res://*/Model2.msl";
        }

        public void SaveSPItems(Data.Entity1 allItems)
        {
            using (var db = new Entities1(entityBuilder.ToString()))
            {
                db.Entity1Set.Add(allItems);
                db.SaveChanges();
            }
        }
    }
}
