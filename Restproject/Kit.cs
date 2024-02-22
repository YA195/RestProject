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
    public partial class Kit : Form
    {
        private Class1 con = new Class1();


        public Kit()
        {
            InitializeComponent();
            dataGridView1.CellClick += DataGridView1_CellClick;

            // Add a new column for the delete button in column 3
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn
            {
                Name = "DeleteButton",
                Text = "Delete",
                UseColumnTextForButtonValue = true
            };
            dataGridView1.Columns.Insert(1, deleteButtonColumn);
        }
        void loaddata()
        {
            con.con.Open();
            string query = "SELECT Kit from Kit ";
            SqlDataAdapter da = new SqlDataAdapter(query, con.con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
            con.con.Close();
        }

        private void Kit_Load(object sender, EventArgs e)
        {
            loaddata();
        }
       
        private bool ItemExists(string kit)
        {
            try
            {
                con.con.Open();
                string sqlCheck = "SELECT COUNT(*) FROM Kit WHERE Kit = @kit";

                using (SqlCommand command = new SqlCommand(sqlCheck, con.con))
                {
                    command.Parameters.AddWithValue("@kit", kit);

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
            finally
            {
                con.con.Close();
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the DeleteButton column
            if (e.ColumnIndex == dataGridView1.Columns["DeleteButton"].Index && e.RowIndex >= 0)
            {
                // Get the value from the selected row's Qesm column
                string qesmToDelete = dataGridView1.Rows[e.RowIndex].Cells["Column1"].Value.ToString();

                // Display a confirmation message
                DialogResult result = MessageBox.Show("هل تريد حذف المطبخ ؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Check the user's choice
                if (result == DialogResult.Yes)
                {
                    // Call a method to delete the row from the database
                    DeleteRowFromDatabase(qesmToDelete);

                    // Reload the data to update the DataGridView
                    loaddata();
                }
            }
        }


        private void DeleteRowFromDatabase(string kit)
        {
            con.con.Open();
            string sqlDelete = "DELETE FROM Kit WHERE Kit = @kit";

            using (SqlCommand command = new SqlCommand(sqlDelete, con.con))
            {
                command.Parameters.AddWithValue("@kit", kit);

                command.ExecuteNonQuery();
            }
            con.con.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button1_Click(sender, e);
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kit = textBox1.Text;

            if (!ItemExists(kit))
            {
                con.con.Open();
                string sqlInsert = "INSERT INTO Kit (Kit) VALUES (@kit)";

                using (SqlCommand command = new SqlCommand(sqlInsert, con.con))
                {
                    command.Parameters.AddWithValue("@kit", kit);

                    command.ExecuteNonQuery();
                }
                con.con.Close();
                MessageBox.Show("تم اضافة المطبخ بنجاح.");
                loaddata();
            }
            else
            {
                MessageBox.Show("المطبخ موجود بالفعل");
            }
        }
    }
}
