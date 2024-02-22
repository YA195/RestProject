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
    public partial class Items : Form
    {
        private Class1 con = new Class1();
        public Form1 form1;  // Reference to Form1
        private string selectedItem;

        // Updated constructor to accept both parameters
        public Items(Form1 form1, string selectedItem)
        {
            InitializeComponent();
            this.form1 = form1;  // Store the reference to Form1
            this.selectedItem = selectedItem;  // Store the selected item
            PopulateItemData(selectedItem);  // Populate data using the selected item
        }

        private void Items_Load(object sender, EventArgs e)
        {
            loadcombobox();
        }

        public void PopulateItemData(string selectedItem)
        {
            // Open the connection
            con.con.Open();

            try
            {
                // Assuming you have a table named "items" with columns "item", "ps", "pm", "pl"
                using (SqlCommand command = new SqlCommand("SELECT item, ps, pm, pl, kit,qesm FROM items WHERE item = @selectedItem", con.con))
                {
                    command.Parameters.AddWithValue("@selectedItem", selectedItem);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the text boxes with item data
                            textBox1.Text = reader["item"].ToString();
                            textBox3.Text = reader["ps"].ToString();
                            textBox4.Text = reader["pm"].ToString();
                            textBox2.Text = reader["pl"].ToString();
                            comboBox1.Text = reader["kit"].ToString();
                            comboBox2.Text = reader["qesm"].ToString();


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading item data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.con.Close();
            }
        }

        public void loadcombobox()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            // Open the connection
            con.con.Open();

            try
            {
                // Assuming you have a table named "YourItemsTable" and a column named "ItemName"
                using (SqlCommand command = new SqlCommand("SELECT Kit FROM Kit", con.con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Kit = reader["Kit"].ToString();
                            comboBox1.Items.Add(Kit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.con.Close();
            }
            con.con.Open();
            try
            {
                // Assuming you have a table named "YourItemsTable" and a column named "ItemName"
                using (SqlCommand command = new SqlCommand("SELECT Qesm FROM Qesm", con.con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Qesm = reader["Qesm"].ToString();
                            comboBox2.Items.Add(Qesm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error loading items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.con.Close();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string item = textBox1.Text;
            string ps = textBox3.Text;
            string pm = textBox4.Text;
            string pl = textBox2.Text;
            string kit = comboBox1.SelectedItem?.ToString();
            string qesm = comboBox2.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(kit) || string.IsNullOrEmpty(qesm))
            {
                MessageBox.Show("برجاء ملئ البيانات المطلوبه");
                return;
            }

            try
            {
                con.con.Open();

                // Check if the item with the same (item) and (qesm) already exists
                string checkQuery = "SELECT COUNT(*) FROM items WHERE item = @item AND qesm = @qesm";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con.con))
                {
                    checkCmd.Parameters.AddWithValue("@item", item);
                    checkCmd.Parameters.AddWithValue("@qesm", qesm);

                    int existingCount = (int)checkCmd.ExecuteScalar();

                    if (existingCount > 0)
                    {
                        MessageBox.Show("هذا العنصر موجود بالفعل.");
                        return;  // Exit the method, do not proceed with insertion
                    }
                }

                // Continue with insertion if the item doesn't exist
                string insertQuery = "INSERT INTO items (item, qesm, ps, pm, pl, kit) VALUES (@item, @qesm, @ps, @pm, @pl, @kit)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, con.con))
                {
                    cmd.Parameters.AddWithValue("@item", item);
                    cmd.Parameters.AddWithValue("@qesm", qesm);
                    cmd.Parameters.AddWithValue("@ps", ps);
                    cmd.Parameters.AddWithValue("@pm", pm);
                    cmd.Parameters.AddWithValue("@pl", pl);
                    cmd.Parameters.AddWithValue("@kit", kit);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تمت اضافة العنصر بنجاح.");
                        form1.loaddata();
                    }
                    else
                    {
                        MessageBox.Show("فشلت الإضافة، حاول مرة أخرى.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
            finally
            {
                if (con.con.State == ConnectionState.Open)
                {
                    con.con.Close();
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string item = textBox1.Text;
            string ps = textBox3.Text;
            string pm = textBox4.Text;
            string pl = textBox2.Text;
            string kit = comboBox1.SelectedItem?.ToString();
            string qesm = comboBox2.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(kit) || string.IsNullOrEmpty(qesm))
            {
                MessageBox.Show("برجاء ملئ البيانات المطلوبه");
                return;
            }

            try
            {
                con.con.Open();

                // Check if the item with the same (item) and (qesm) already exists
                string checkQuery = "SELECT COUNT(*) FROM items WHERE item = @item AND qesm = @qesm";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con.con))
                {
                    checkCmd.Parameters.AddWithValue("@item", item);
                    checkCmd.Parameters.AddWithValue("@qesm", qesm);

                    int existingCount = (int)checkCmd.ExecuteScalar();

                    if (existingCount == 0)
                    {
                        MessageBox.Show("هذا العنصر غير موجود، يرجى استخدام زر الإضافة.");
                        return;  // Exit the method, the item doesn't exist for update
                    }
                }

                // Continue with update
                string updateQuery = "UPDATE items SET ps = @ps, pm = @pm, pl = @pl, kit = @kit WHERE item = @item AND qesm = @qesm";
                using (SqlCommand cmd = new SqlCommand(updateQuery, con.con))
                {
                    cmd.Parameters.AddWithValue("@item", item);
                    cmd.Parameters.AddWithValue("@qesm", qesm);
                    cmd.Parameters.AddWithValue("@ps", ps);
                    cmd.Parameters.AddWithValue("@pm", pm);
                    cmd.Parameters.AddWithValue("@pl", pl);
                    cmd.Parameters.AddWithValue("@kit", kit);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم تحديث العنصر بنجاح.");
                        form1.loaddata();
                    }
                    else
                    {
                        MessageBox.Show("فشل التحديث، حاول مرة أخرى.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
            finally
            {
                if (con.con.State == ConnectionState.Open)
                {
                    con.con.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string item = textBox1.Text;
            string qesm = comboBox2.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(item) || string.IsNullOrEmpty(qesm))
            {
                MessageBox.Show("برجاء تحديد العنصر للحذف.");
                return;
            }

            try
            {
                con.con.Open();

                // Check if the item with the specified (item) and (qesm) exists
                string checkQuery = "SELECT COUNT(*) FROM items WHERE item = @item AND qesm = @qesm";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con.con))
                {
                    checkCmd.Parameters.AddWithValue("@item", item);
                    checkCmd.Parameters.AddWithValue("@qesm", qesm);

                    int existingCount = (int)checkCmd.ExecuteScalar();

                    if (existingCount == 0)
                    {
                        MessageBox.Show("هذا العنصر غير موجود، يرجى تحديد عنصر صحيح.");
                        return;  // Exit the method, the item doesn't exist for deletion
                    }
                }

                // Continue with deletion
                string deleteQuery = "DELETE FROM items WHERE item = @item AND qesm = @qesm";
                using (SqlCommand cmd = new SqlCommand(deleteQuery, con.con))
                {
                    cmd.Parameters.AddWithValue("@item", item);
                    cmd.Parameters.AddWithValue("@qesm", qesm);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف العنصر بنجاح.");
                        form1.loaddata();
                        // You may want to clear the input controls after successful deletion
                        textBox1.Text = string.Empty;
                        textBox2.Text = string.Empty;
                        textBox3.Text = string.Empty;
                        textBox4.Text = string.Empty;
                        comboBox1.SelectedIndex = -1;
                        comboBox2.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("فشل الحذف، حاول مرة أخرى.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
            finally
            {
                if (con.con.State == ConnectionState.Open)
                {
                    con.con.Close();
                }
            }
        }
    }

}
