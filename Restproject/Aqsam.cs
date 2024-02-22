using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Restproject
{
    public partial class Aqsam : Form
    {
        private Class1 con = new Class1();
        private Form1 form1; // Reference to Form1

        public Aqsam(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1; // Store the reference to Form1
            // Attach the CellClick event to handle button clicks
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
            string query = "SELECT Qesm from Qesm ";
            SqlDataAdapter da = new SqlDataAdapter(query, con.con);
            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
            con.con.Close();
        }

        private void Aqsam_Load(object sender, EventArgs e)
        {
            loaddata();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string qesm = textBox1.Text;

            // Check if the item already exists
            if (!ItemExists(qesm))
            {
                con.con.Open();
                string sqlInsert = "INSERT INTO Qesm (Qesm) VALUES (@qesm)";

                using (SqlCommand command = new SqlCommand(sqlInsert, con.con))
                {
                    command.Parameters.AddWithValue("@qesm", qesm);

                    command.ExecuteNonQuery();
                }
                con.con.Close();
                MessageBox.Show("Success");
                loaddata();
                form1.LoadButtonsFromDatabase();
            }
            else
            {
                MessageBox.Show("Item already exists in the database.");
            }
        }

        private bool ItemExists(string qesm)
        {
            try
            {
                con.con.Open();
                string sqlCheck = "SELECT COUNT(*) FROM Qesm WHERE Qesm = @qesm";

                using (SqlCommand command = new SqlCommand(sqlCheck, con.con))
                {
                    command.Parameters.AddWithValue("@qesm", qesm);

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
                DialogResult result = MessageBox.Show("هل تريد حذف القسم ؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Check the user's choice
                if (result == DialogResult.Yes)
                {
                    // Call a method to delete the row from the database
                    DeleteRowFromDatabase(qesmToDelete);

                    // Reload the data to update the DataGridView
                    loaddata();
                    form1.LoadButtonsFromDatabase();
                }
            }
        }


        private void DeleteRowFromDatabase(string qesm)
        {
            con.con.Open();
            string sqlDelete = "DELETE FROM Qesm WHERE Qesm = @qesm";

            using (SqlCommand command = new SqlCommand(sqlDelete, con.con))
            {
                command.Parameters.AddWithValue("@qesm", qesm);

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
    }
}
