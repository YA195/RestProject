﻿using System;
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

        public Form1()
        {
            InitializeComponent();
            string connectionString = "Data Source=.;Initial Catalog=restrest;User ID=sa;Password=ahmed";
            con = new SqlConnection(connectionString);
           


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'customersDataSet.Customers' table. You can move, or remove it, as needed.
            comboBox1.Text = "";
            label6.Text = "";
            label7.Text = "0";
            PopulateComboBox();
            loaddata();
        }
        void loaddata()
        {
            con.Open();
            string query = "SELECT item, ps, pm, pl FROM items WHERE qesm LIKE '%pizza%'";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
           loaddata();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            string query = "SELECT item, ps, pm, pl FROM items WHERE qesm LIKE '%sharqy%'";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();

            da.Fill(dt);
            dataGridView1.DataSource = dt;
            con.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
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
                        if (row.Cells[0].Value.ToString() == item + " صغير "&& int.Parse(row.Cells[2].Value.ToString()) == price || row.Cells[0].Value.ToString() == item + " وسط " && int.Parse(row.Cells[2].Value.ToString()) == price || row.Cells[0].Value.ToString() == item + " كبير " && int.Parse(row.Cells[2].Value.ToString()) == price)
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

                            dataGridView2.Rows.Add(item+" صغير ", 1, price, total);

                        }
                        if (e.ColumnIndex == 2)
                        {

                            dataGridView2.Rows.Add(item+" وسط ", 1, price, total);

                        }
                        if (e.ColumnIndex == 3)
                        {

                            dataGridView2.Rows.Add(item+" كبير ", 1, price, total);

                        }


                    }
                }
                else
                {
                    MessageBox.Show("Invalid price. Please enter a valid numeric value.");
                }
            }
            UpdateSumLabel();
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

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            UpdateSumLabel();
        }

        private void عمالهToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stuff stuff = new Stuff();
            stuff.Show();

        }
    }
}