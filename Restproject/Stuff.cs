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
    public partial class Stuff : Form

    {
        private SqlConnection con;

        public Stuff()
        {
            InitializeComponent();
            string connectionString = "Data Source=.;Initial Catalog=restrest;User ID=sa;Password=ahmed";
            con = new SqlConnection(connectionString);
        }

        private void Stuff_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'restrestDataSet.Stuff' table. You can move, or remove it, as needed.
            loaddata();
        }
        void loaddata()
        {
            con.Open();
            string query = "SELECT Id, num, position, name, salary, Sec_num from Stuff ";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
            con.Close();
        }

        private bool RecordExists(string id, string name)
        {
            try
            {
                con.Open();
                string sqlCheck = "SELECT COUNT(*) FROM Stuff WHERE Id = @id OR name = @name";

                using (SqlCommand command = new SqlCommand(sqlCheck, con))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            finally
            {
                con.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string name = textBox2.Text;
            string num = textBox5.Text;
            string sec_num = textBox4.Text;
            string position = comboBox1.Text;
            string salary = textBox3.Text;

            if (!RecordExists(id, name))
            {
                if (long.TryParse(id, out long idValue))
                {
                    con.Open();
                    string sqlInsert = "INSERT INTO Stuff (Id, name, num, Sec_num, position, salary) VALUES (@id, @name, @num, @sec_num, @position, @salary)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, con))
                    {
                        command.Parameters.AddWithValue("@id", idValue);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@num", num);
                        command.Parameters.AddWithValue("@sec_num", sec_num);
                        command.Parameters.AddWithValue("@position", position);
                        command.Parameters.AddWithValue("@salary", salary);

                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    MessageBox.Show("Success");
                    ClearFields();
                    loaddata();
                }
                else
                {
                    MessageBox.Show("Invalid ID format. Please enter a numeric ID.");
                }
            }
            else
            {
                MessageBox.Show("Record with the provided Id or Name already exists.");
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            textBox5.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            comboBox1.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string name = textBox2.Text;
            string num = textBox5.Text;
            string sec_num = textBox4.Text;
            string position = comboBox1.Text;
            string salary = textBox3.Text;

            if (RecordExists(id, name))
            {
                con.Open();
                string sqlUpdate = "UPDATE Stuff SET num = @num, name= @name , Sec_num = @sec_num, position = @position, salary = @salary WHERE Id = @id ";

                using (SqlCommand command = new SqlCommand(sqlUpdate, con))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@num", num);
                    command.Parameters.AddWithValue("@sec_num", sec_num);
                    command.Parameters.AddWithValue("@position", position);
                    command.Parameters.AddWithValue("@salary", salary);

                    command.ExecuteNonQuery();
                }
                con.Close();
                MessageBox.Show("تم التعديل بنجاح");
                loaddata();

                // Clear textboxes and combobox after update
                ClearFields();
            }
            else
            {
                MessageBox.Show("خطأ في التعديل");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string id = textBox1.Text;
            string name = textBox2.Text;


            try
            {
                if (RecordExists(id, name))
                {
                    con.Open();
                    string sqlDelete = "DELETE FROM Stuff WHERE Id = @id AND name = @name";

                    SqlCommand command = new SqlCommand(sqlDelete, con);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف العامل بنجاح");
                        loaddata();

                        // Clear textboxes and combobox after delete
                        ClearFields();
                    }
                    else
                    {
                        MessageBox.Show("خطأ في الحذف: لم يتم العثور على السجل");
                    }
                }
                else
                {
                    MessageBox.Show("خطأ في الحذف: السجل غير موجود");
                }
            }
            finally
            {
                con.Close();
            }
        }


        private void ClearFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox3.Clear();
            comboBox1.Text = string.Empty;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            textBox5.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            comboBox1.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
        }
    }
}
