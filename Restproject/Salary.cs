using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Restproject
{
    public partial class Salary : Form
    {
        public SqlConnection con;

        public Salary()
        {
            InitializeComponent();
            string connectionString = "Data Source=.;Initial Catalog=restrest;User ID=sa;Password=ahmed";
            con = new SqlConnection(connectionString);
        }

        private void LoadNamesIntoComboBox()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                string query = "SELECT name FROM Stuff";
                SqlCommand command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading names: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
                return;

            DateTime fromDate = dateTimePicker1.Value;
            DateTime toDate = dateTimePicker2.Value;
            string selectedStuff = comboBox1.SelectedItem.ToString();

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();


                string salaryQuery = "SELECT salary FROM Stuff WHERE name = @StuffName";
                SqlCommand salaryCmd = new SqlCommand(salaryQuery, con);
                salaryCmd.Parameters.AddWithValue("@StuffName", selectedStuff);
                object result = salaryCmd.ExecuteScalar();
                decimal hourlyRate = result != DBNull.Value ? Convert.ToDecimal(result) : 0;



                string query = "SELECT TimeToAttend, TimeToGo, Day, Hours " +
                               "FROM Attendance AS a " +
                               "JOIN Stuff AS s ON a.StuffId = s.Id " +
                               "WHERE s.name = @StuffName AND Day BETWEEN @FromDate AND @ToDate";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@StuffName", selectedStuff);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                TimeSpan totalHours = TimeSpan.Zero;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Hours"] != DBNull.Value)
                    {
                        TimeSpan hours = TimeSpan.Parse(row["Hours"].ToString());
                        totalHours += hours;
                    }
                }

                label4.Text = $"{totalHours.Hours:00}:{totalHours.Minutes:00}";

                decimal totalSalary = hourlyRate * (decimal)totalHours.TotalHours;

                label2.Text = $" {totalSalary:F1}";


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void Salary_Load(object sender, EventArgs e)
        {
            LoadNamesIntoComboBox();
        }
    }
}
