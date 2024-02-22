using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Restproject
{
    public partial class Salary : Form
    {
        private readonly Class1 con = new Class1();

        public Salary()
        {
            InitializeComponent();
            LoadNamesIntoComboBox();
        }

        private void LoadNamesIntoComboBox()
        {
            try
            {
                using (con.con)
                {
                    string query = "SELECT name FROM Stuff";
                    SqlCommand command = new SqlCommand(query, con.con);
                    con.con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        comboBox1.Items.Add(reader["name"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading names: " + ex.Message);
            }
        }
    }
}
