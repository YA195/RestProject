using System;
using System.Data.SqlClient;

namespace Restproject
{
    public class Class1
    {
        public SqlConnection con;

        // Constructor
        public Class1()
        {
            string ConnectionString = "Data Source=.;Initial Catalog=restrest;User ID=sa;Password=ahmed";
            con = new SqlConnection(ConnectionString);
        }
    }
}
