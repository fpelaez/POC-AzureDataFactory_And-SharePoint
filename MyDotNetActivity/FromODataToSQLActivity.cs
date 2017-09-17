using Microsoft.Azure.Management.DataFactories.Runtime;
using Microsoft.Azure.Management.DataFactories.Models;
using SP_Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SP_Provider.Entities;

namespace MyDotNetActivity
{
    public class FromODataToSQLActivity : IDotNetActivity
    {
        public IDictionary<string, string> Execute(IEnumerable<LinkedService> linkedServices, IEnumerable<Dataset> datasets, Activity activity, IActivityLogger logger)
        {
            SPDataProvider dataProvider = new SPDataProvider();


            AzureSqlDatabaseLinkedService sqlInputLinkedService;
            AzureSqlTableDataset sqlInputLocation;

            Dataset sqlInputDataset = datasets.Where(dataset => dataset.Name == "OutputAzureSQLDataset").First();
            sqlInputLocation = sqlInputDataset.Properties.TypeProperties as AzureSqlTableDataset;

            sqlInputLinkedService = linkedServices.Where(
                                linkedService =>
                                linkedService.Name ==
                                sqlInputDataset.Properties.LinkedServiceName).First().Properties.TypeProperties
                                as AzureSqlDatabaseLinkedService;

            string[] lists = { "List-Example1","List-Example2","List-Example3","List-Example4" };
            foreach (string list in lists)
            {
                List<ListItem> items = dataProvider.GetItemsFromList(list);

                //Create an EF project to access to the SQL
                //Use the sqlInputLinkedService.ConnectionString as a connection string to connect to the SQL
                
            }
            return new Dictionary<string, string>();
        }
    }
}
