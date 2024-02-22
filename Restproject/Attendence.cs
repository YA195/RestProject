using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Restproject
{
    public partial class Attendence : Form
    {
        private readonly Class1 con = new Class1();

        public Attendence()
        {
            InitializeComponent();
        }

        private void Attendence_Load(object sender, EventArgs e)
        {
            LoadStuffData();
            LoadAttendanceData();
        }

        private void LoadStuffData()
        {
            try
            {
                string query = "SELECT Id, name, num, position FROM Stuff";
                SqlDataAdapter da = new SqlDataAdapter(query, con.con);
                DataTable dt = new DataTable();

                con.con.Open();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stuff data: " + ex.Message);
            }
            finally
            {
                con.con.Close();
            }
        }

        private void LoadAttendanceData()
        {
            try
            {
                string query = "SELECT s.Id, s.name, s.num, s.position " +
                               "FROM Stuff s " +
                               "INNER JOIN Attendance a ON s.Id = a.StuffId " +
                               "WHERE a.TimeToGo IS NULL";
                SqlDataAdapter da = new SqlDataAdapter(query, con.con);
                DataTable dt = new DataTable();

                con.con.Open();
                da.Fill(dt);

                dataGridView2.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance data: " + ex.Message);
            }
            finally
            {
                con.con.Close();
            }
        }

        private void RecordAttendance(long stuffId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                con.con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Attendance (StuffId, TimeToAttend) VALUES (@StuffId, @TimeToAttend)", con.con);
                cmd.Parameters.AddWithValue("@StuffId", stuffId);
                cmd.Parameters.AddWithValue("@TimeToAttend", currentTime);
                cmd.ExecuteNonQuery();

                MessageBox.Show("تم الحضور.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording attendance: " + ex.Message);
            }
            finally
            {
                con.con.Close();
            }
        }

        private void RecordTimeToGo(long stuffId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                DateTime adjustedTime = AdjustDateTime(currentTime);

                con.con.Open();

                // Retrieve TimeToAttend for the given stuffId
                SqlCommand cmdGetTimeToAttend = new SqlCommand("SELECT TimeToAttend FROM Attendance WHERE StuffId = @StuffId AND TimeToGo IS NULL", con.con);
                cmdGetTimeToAttend.Parameters.AddWithValue("@StuffId", stuffId);
                DateTime timeToAttend = (DateTime)cmdGetTimeToAttend.ExecuteScalar();

                // Calculate the difference between TimeToAttend and adjustedTime
                TimeSpan timeDifference = adjustedTime - timeToAttend;

                // Update the TimeToGo, Day, and Hours columns in the database
                SqlCommand cmdUpdate = new SqlCommand("UPDATE Attendance SET TimeToGo = @TimeToGo, Day = @Day, Hours = @Hours WHERE StuffId = @StuffId AND TimeToGo IS NULL", con.con);
                cmdUpdate.Parameters.AddWithValue("@StuffId", stuffId);
                cmdUpdate.Parameters.AddWithValue("@TimeToGo", adjustedTime);
                cmdUpdate.Parameters.AddWithValue("@Day", adjustedTime.Date); // Fill the Day column with the adjusted date

                // Format the time difference as a string in HH:mm:ss format
                string formattedTimeDifference = $"{timeDifference.Hours:00}:{timeDifference.Minutes:00}:{timeDifference.Seconds:00}";
                cmdUpdate.Parameters.AddWithValue("@Hours", formattedTimeDifference);

                cmdUpdate.ExecuteNonQuery();

                MessageBox.Show("Time to go recorded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error recording time to go: " + ex.Message);
            }
            finally
            {
                con.con.Close();
            }
        }





        private DateTime AdjustDateTime(DateTime inputDateTime)
        {
            TimeSpan morningStartTime = new TimeSpan(7, 0, 0);
            TimeSpan eveningEndTime = new TimeSpan(23, 59, 59);
            TimeSpan earlyMorningEndTime = new TimeSpan(6, 59, 59);

            TimeSpan timeOfDay = inputDateTime.TimeOfDay;

            if (timeOfDay >= morningStartTime && timeOfDay <= eveningEndTime)
            {
                return inputDateTime;
            }
            else if (timeOfDay <= earlyMorningEndTime)
            {
                return inputDateTime.Date.AddDays(-1).Add(timeOfDay);
            }
            else
            {
                return inputDateTime;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                long stuffId = Convert.ToInt64(row.Cells["Id"].Value);

                // Check if the person has already attended
                bool alreadyAttended = IsAlreadyAttended(stuffId);

                if (!alreadyAttended)
                {
                    RecordAttendance(stuffId);
                    LoadAttendanceData();
                }
                else
                {
                    MessageBox.Show("هذا الشخص موجود بالفعل.");
                }
            }
        }
        private bool IsAlreadyAttended(long stuffId)
        {
            bool attended = false;

            try
            {
                con.con.Open();
                string query = "SELECT COUNT(*) FROM Attendance WHERE StuffId = @StuffId AND TimeToGo IS NULL";
                SqlCommand cmd = new SqlCommand(query, con.con);
                cmd.Parameters.AddWithValue("@StuffId", stuffId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                attended = (count > 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking attendance: " + ex.Message);
            }
            finally
            {
                con.con.Close();
            }

            return attended;
        }


        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                long stuffId = Convert.ToInt64(row.Cells["IDD"].Value);

                RecordTimeToGo(stuffId);
                LoadAttendanceData();
            }
        }
    }
}
