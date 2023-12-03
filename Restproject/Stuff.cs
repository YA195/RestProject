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
            this.stuffTableAdapter.Fill(this.restrestDataSet.Stuff);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string id = textBox1.Text;
            string name = textBox2.Text;
            string num = textBox5.Text;
            string sec_num = textBox4.Text;
            string position = comboBox1.Text;
            string salary = textBox3.Text;

            con.Open();
            string sqlInsert = "INSERT INTO Stuff (Id, name, num, Sec_num, position, salary) VALUES (@id, @name, @num, @sec_num, @position, @salary)";


            using (SqlCommand command = new SqlCommand(sqlInsert, con))
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
            MessageBox.Show("success");
            this.stuffTableAdapter.Fill(this.restrestDataSet.Stuff);

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString(); 
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString(); 
            textBox5.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString(); 
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString(); 
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString(); 
            comboBox1.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
