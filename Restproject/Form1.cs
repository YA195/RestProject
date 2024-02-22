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
using System.Xml.Linq;


namespace Restproject
{
    public partial class Form1 : Form
    {
        private SqlConnection con;
        private Items itemsForm;  // Declare Items form as a class-level variable


        public Form1()
        {
            InitializeComponent();
            string connectionString = "Data Source=.;Initial Catalog=restrest;User ID=sa;Password=ahmed";
            con = new SqlConnection(connectionString);
           


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            label6.Text = "";
            label7.Text = "0";
            PopulateComboBox();
            loaddata();
            LoadButtonsFromDatabase();  
        }
        public void loaddata()
        {
            try
            {
                con.Open();

                // Assuming you have a table named "Qesm" with a column named "Qesm"
                using (SqlCommand command = new SqlCommand("SELECT TOP 1 Qesm FROM Qesm", con))
                {
                    object firstQesm = command.ExecuteScalar();

                    if (firstQesm != null)
                    {
                        string selectedType = firstQesm.ToString();

                        SqlCommand itemsCommand = new SqlCommand("SELECT item, ps, pm, pl FROM items WHERE qesm LIKE @selectedType", con);
                        itemsCommand.Parameters.AddWithValue("@selectedType", "%" + selectedType + "%");

                        SqlDataAdapter da = new SqlDataAdapter(itemsCommand);
                        DataTable dt = new DataTable();

                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading initial data: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }




        public void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is not empty, is in a clickable range, and is not null or whitespace
            if (e.RowIndex >= 0 && e.ColumnIndex >= 1 && e.ColumnIndex <= 3 &&
                dataGridView1.Rows[e.RowIndex].Cells[0].Value != null &&
                !string.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()) &&
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null &&
                !string.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
            {
                // Identify the item and the corresponding price
                string item = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

                // Check if the price is a valid integer
                if (int.TryParse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out int price))
                {
                    int quantity = 1;
                    int total = price * quantity;

                    // Check if the item and price already exist in DataGridView2
                    bool found = false;
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.Cells[0].Value.ToString() == item + " صغير " &&
                            int.Parse(row.Cells[2].Value.ToString()) == price ||
                            row.Cells[0].Value.ToString() == item + " وسط " &&
                            int.Parse(row.Cells[2].Value.ToString()) == price ||
                            row.Cells[0].Value.ToString() == item + " كبير " &&
                            int.Parse(row.Cells[2].Value.ToString()) == price)
                        {
                            found = true;
                            quantity = int.Parse(row.Cells[1].Value.ToString());
                            quantity++;
                            row.Cells[1].Value = quantity;
                            row.Cells[2].Value = price;
                            total = price * quantity;
                            row.Cells[3].Value = total;
                            break;
                        }
                    }

                    if (!found)
                    {
                        if (e.ColumnIndex == 1)
                        {
                            dataGridView2.Rows.Add(item + " صغير ", 1, price, total);
                        }
                        if (e.ColumnIndex == 2)
                        {
                            dataGridView2.Rows.Add(item + " وسط ", 1, price, total);
                        }
                        if (e.ColumnIndex == 3)
                        {
                            dataGridView2.Rows.Add(item + " كبير ", 1, price, total);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid price. Please enter a valid numeric value.");
                }
            }
            UpdateSumLabel();

            string selectedItem = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();

            // Check if the Items form is open
            Items itemsForm = Application.OpenForms.OfType<Items>().FirstOrDefault(f => !f.IsDisposed);

            if (itemsForm == null)
            {
                // If the Items form is not open, create and show it
                itemsForm = new Items(this, selectedItem);
                itemsForm.FormClosed += (s, args) => itemsForm = null;  // Reset itemsForm when closed
                
            }
            else
            {
                // If the Items form is already open, just reload the data and bring it to the front
                itemsForm.PopulateItemData(selectedItem);
                itemsForm.BringToFront();
            }


        }




        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void UpdateSumLabel()
        {
            int sum = 0;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    sum += Convert.ToInt32(row.Cells[3].Value);
                }
            }

            label7.Text = $"{sum}";
        }

       
        private void PopulateComboBox()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand ("SELECT phoneNum FROM Customers;", con);
            SqlDataReader reader = cmd.ExecuteReader ();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["phoneNum"].ToString());
            }
            con.Close();
        }

        private void button16_Click(object sender, EventArgs e) // ISERT BUTTON NEW CUSTOMER
        {
            string num = comboBox1.Text;
            string name = textBox1.Text;
            string address1 = textBox2.Text;
            string address2 = textBox3.Text;
            string address3 = textBox4.Text;
           
                con.Open();
                string sqlInsert = "INSERT INTO Customers (name, phoneNum, First_Add, Sec_Add, Third_Add) VALUES (@name, @phoneNum, @First_Add, @Sec_Add, @Third_Add)";


                using (SqlCommand command = new SqlCommand(sqlInsert, con))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@phoneNum", num);
                    command.Parameters.AddWithValue("@First_Add", address1);
                    command.Parameters.AddWithValue("@Sec_Add", address2);
                    command.Parameters.AddWithValue("@Third_Add", address3);

                    command.ExecuteNonQuery();
                }
            con.Close();
            MessageBox.Show("success");

        }

        private void button17_Click(object sender, EventArgs e)// UPDATE BUTTON CUSTOMERS INFORMATION
        {
            
            label6.Text = $" {comboBox1.Text.Length}";

            string num = comboBox1.Text;
            string name = textBox1.Text;
            string address1 = textBox2.Text;
            string address2 = textBox3.Text;
            string address3 = textBox4.Text;

            con.Open();

            SqlCommand command = new SqlCommand("UPDATE Customers SET name=@name, phoneNum=@phoneNum, First_Add=@First_Add, Sec_Add=@Sec_Add, Third_Add=@Third_Add  WHERE phoneNum=@phoneNum",con);
            
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@phoneNum", num);
            command.Parameters.AddWithValue("@First_Add", address1);
            command.Parameters.AddWithValue("@Sec_Add", address2);
            command.Parameters.AddWithValue("@Third_Add", address3);
            command.ExecuteNonQuery();
            

            con.Close();
            MessageBox.Show("تم التعديل");
        }

        private void button14_Click(object sender, EventArgs e)// BUTTON MAINFORM ----> CUSTOMERSFORM
        {

            Customers customers = new Customers();
            customers.Show();

        }

        private void button15_Click(object sender, EventArgs e) // CLEAR BUTTON THE (textboxes) CUSTOMER INFO
        {
            label6.Text = $" {comboBox1.Text.Length}";

            comboBox1.Text = String.Empty;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label6.Text = $" {comboBox1.Text.Length}";

            string PhoneNum = comboBox1.Text;
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT name,  First_Add, Sec_Add, Third_Add FROM Customers WHERE phoneNum=@phoneNum", con);
            cmd.Parameters.AddWithValue("@phoneNum", PhoneNum);
            SqlDataReader reader = cmd.ExecuteReader();


            if (reader.Read())
            {
                textBox1.Text = reader["Name"].ToString();
                textBox2.Text = reader["First_Add"].ToString();
                textBox3.Text = reader["Sec_Add"].ToString();
                textBox4.Text = reader["Third_Add"].ToString();
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
            con.Close();

        }

        

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            label6.Text =  $" {comboBox1.Text.Length}";


            string PhoneNum = comboBox1.Text;
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT name,  First_Add, Sec_Add, Third_Add FROM Customers WHERE phoneNum=@phoneNum", con);
            cmd.Parameters.AddWithValue("@phoneNum", PhoneNum);
            SqlDataReader reader = cmd.ExecuteReader();


            if (reader.Read())
            {
                textBox1.Text = reader["Name"].ToString();
                textBox2.Text = reader["First_Add"].ToString();
                textBox3.Text = reader["Sec_Add"].ToString();
                textBox4.Text = reader["Third_Add"].ToString();
            }
            else
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
            con.Close();

            if (comboBox1.Text.Length >= 11)
            {

                // If it is, prevent further input
                comboBox1.Text = comboBox1.Text.Substring(0, 11);
                comboBox1.Select(11, 0); // Move the cursor to the end
                textBox1.Focus();
            }

        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
            
            
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.customersTableAdapter.FillBy(this.customersDataSet.Customers);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                dataGridView2.Rows.RemoveAt(e.RowIndex);

                // After removing a row, update the sum label
                UpdateSumLabel();
            }
        }

       

        private void عمالهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stuff stuff = new Stuff();
            stuff.Show();

        }

        public void LoadButtonsFromDatabase()
        {
            // Clear existing buttons before reloading
            panel3.Controls.Clear();

            con.Open();

            // Assuming you have a table named "YourTable" and a column named "Type"
            using (SqlCommand command = new SqlCommand("SELECT DISTINCT Qesm FROM Qesm", con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string type = reader["Qesm"].ToString();
                        CreateButton(type);
                    }
                }
            }
            con.Close();
        }


        private void CreateButton(string type)
        {
            Button button = new Button
            {
                Text = type,
                Width = 100,  // Set your preferred width
                Height = 73,  // Set your preferred height
                Tag = type,
                Dock = DockStyle.Top,
                // You can use Tag to store additional information if needed
            };



            panel3.Controls.Add(button);
            button.Click += Button_Click;  // Attach an event handler

        }
        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string selectedType = clickedButton.Tag.ToString(); // or clickedButton.Text, depending on how you set it

            con.Open();

            SqlCommand command = new SqlCommand("SELECT item, ps, pm, pl FROM items WHERE qesm LIKE @selectedType",con);
            command.Parameters.AddWithValue("@selectedType", "%" + selectedType + "%");

            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();

            da.Fill(dt);
            dataGridView1.DataSource = dt;

            con.Close();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void الاقسامToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            Aqsam form = new Aqsam(this); // Pass a reference to Form1
            form.ShowDialog();
        }

        private void المطToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Kit form = new Kit(); // Pass a reference to Form1
            form.ShowDialog();
        }

        private void الاصنافToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Assuming you want to pass the selected item from dataGridView1
            string selectedItem = dataGridView1.CurrentRow.Cells["item"].Value.ToString();

            // Check if the Items form is not already open
            if (itemsForm == null || itemsForm.IsDisposed)
            {
                // Create an instance of Items and pass Form1 and the selected item
                itemsForm = new Items(this, selectedItem);

                // Set the form properties as needed, for example:
                itemsForm.TopLevel = false;
                itemsForm.Dock = DockStyle.Fill;

                // Add the ItemsForm to panel4
                panel4.Controls.Add(itemsForm);

                // Subscribe to the FormClosed event to set itemsForm to null when closed
                itemsForm.FormClosed += (s, args) => itemsForm = null;

                // Show the ItemsForm
                itemsForm.Show();
            }
           
                // If the Items form is already open, just reload the data and bring it to the front
                itemsForm.PopulateItemData(selectedItem);
                itemsForm.BringToFront();
            
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            UpdateSumLabel();

        }

        private void حضورToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Attendence form = new Attendence(); // Pass a reference to Form1
            form.ShowDialog();
        }

        private void اجورToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salary form = new Salary(); // Pass a reference to Form1
            form.ShowDialog();
        }
    }
}
