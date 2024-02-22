using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Restproject
{
    public partial class Customers : Form
    {
        private Class1 con = new Class1();

        public Customers()
        {
            InitializeComponent();
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            loaddata();
        }
        void loaddata()
        {
            con.con.Open();
            string query = "SELECT * from Customers ";
            SqlDataAdapter da = new SqlDataAdapter(query, con.con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
            con.con.Close();
        }
    }
}
