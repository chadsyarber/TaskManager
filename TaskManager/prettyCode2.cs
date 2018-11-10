using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Windows.Forms;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace RenAndStimpyDatabase
{
    class Controller
    {
        /*
         * This Class is used to pass TaskItems into the database
         * */
        private TaskItem task { get; set; }
        private string connection = @"Server=10.42.31.38;datasource=localhost;port=3306;Database='taskmanager';username=root;password=root; Allow Zero Datetime=True; Convert Zero Datetime=True";
        private string command { get; set; }
        private string username { get; set; }
        private DateTime today { get; set; }

        private string getUser()
        {
            return username;
        }
        private void setUser()
        {
            username = Environment.UserName;
        }
        private void setToday()
        {
            today = DateTime.Now;
        }
        private DateTime getToday()
        {
            return today;
        }

        public Controller(TaskItem task)
        {
            this.task = task;
            setUser();
            setToday();
        }
        public Controller()
        { }
        public void setCommand(string sqlCommand)
        {
            this.command = sqlCommand;
        }
        public string getCommand()
        {
            return command;
        }
        /*
         * Database structure
         * 
         * task_num (auto int)
         * assignment_group (text)
         * caller_id (text)
         * category (text)
         * impact (small int)
         * short_description (longtext)
         * subcategory (text)
         * urgency (small int)
         * repeat_interval (time)
         * rules (longtext)
         * beginAfter (time)
         * */
        //Make a task
        public void createTask()
        {
            MySqlConnection conn = new MySqlConnection();
            MySqlCommand cmd = new MySqlCommand();

            conn.ConnectionString = connection;
            
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"INSERT INTO tasks VALUES(NULL, @assignment_group, @caller_id, @category, @impact, @short_description, @subcategory, @urgency, @repeat_interval, @rules, @beginAfter, @weekDay)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@assignment_group", task.getAssignment_group());
                cmd.Parameters.AddWithValue("@caller_id", task.getCaller_id());
                cmd.Parameters.AddWithValue("@category", task.getCategory());
                cmd.Parameters.AddWithValue("@impact", task.getImpact());
                cmd.Parameters.AddWithValue("@short_description", task.getShort_Description());
                cmd.Parameters.AddWithValue("@subcategory", task.getSubcategory());
                cmd.Parameters.AddWithValue("@urgency", task.getUrgency());
                cmd.Parameters.AddWithValue("@repeat_interval", task.getRepeatInterval());
                cmd.Parameters.AddWithValue("@rules", task.getRules());
                cmd.Parameters.AddWithValue("@beginAfter", task.getBeginAfter());
                cmd.Parameters.AddWithValue("@weekDay", task.getWeekday());
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();

                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"INSERT INTO phink VALUES(@username, @today, @type)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", getUser());
                cmd.Parameters.AddWithValue("@today", getToday());
                cmd.Parameters.AddWithValue("@type", "Create");
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.Number + " Type Error\n" + e.Message + "\n" + e.StackTrace);
            }

        }
        public List<TaskItem> retrieveTasks()
        {
            MySqlConnection conn = new MySqlConnection();
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader reader;

            conn.ConnectionString = connection;
            List<TaskItem> manyTasks = new List<TaskItem>();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT * FROM tasks";
                reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    manyTasks.Add(new TaskItem(reader.GetString("assignment_group"), reader.GetString("caller_id"), reader.GetString("category"), reader.GetInt32("impact"), reader.GetString("short_description"), reader.GetString("subcategory"), reader.GetInt32("urgency"), reader.GetTimeSpan("repeat_interval"), reader.GetString("rules"), reader.GetTimeSpan("beginAfter"), reader.GetInt32("weekDay")));
                }

            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.Number + " Type Error\n" + e.Message);
            }
            cmd.Dispose();
            conn.Close();
            return manyTasks;
        }
        public void replaceTask(TaskItem replaceTask, string origDescription)
        {
            MySqlConnection conn = new MySqlConnection();
            MySqlCommand cmd = new MySqlCommand();

            conn.ConnectionString = connection;

            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"UPDATE tasks SET assignment_group= @assignment_group, caller_id= @caller_id, category= @category, impact= @impact, short_description= @short_description, subcategory= @subcategory, urgency= @urgency, repeat_interval= @repeat_interval, rules= @rules, beginAfter= @beginAfter, weekDay= @weekDay WHERE short_description= @short";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@short", origDescription);
                cmd.Parameters.AddWithValue("@assignment_group", replaceTask.getAssignment_group());
                cmd.Parameters.AddWithValue("@caller_id", replaceTask.getCaller_id());
                cmd.Parameters.AddWithValue("@category", replaceTask.getCategory());
                cmd.Parameters.AddWithValue("@impact", replaceTask.getImpact());
                cmd.Parameters.AddWithValue("@short_description", replaceTask.getShort_Description());
                cmd.Parameters.AddWithValue("@subcategory", replaceTask.getSubcategory());
                cmd.Parameters.AddWithValue("@urgency", replaceTask.getUrgency());
                cmd.Parameters.AddWithValue("@repeat_interval", replaceTask.getRepeatInterval());
                cmd.Parameters.AddWithValue("@rules", replaceTask.getRules());
                cmd.Parameters.AddWithValue("@beginAfter", replaceTask.getBeginAfter());
                cmd.Parameters.AddWithValue("@weekDay", replaceTask.getWeekday());
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"INSERT INTO phink VALUES(@username, @today, @type)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", getUser());
                cmd.Parameters.AddWithValue("@today", getToday());
                cmd.Parameters.AddWithValue("@type", "Replace");
                cmd.ExecuteNonQuery();

            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.Number + " Type Error\n" + e.Message);
            }
            cmd.Dispose();
            conn.Close();
        }
        public void deleteTask(string short_description)
        {
            MySqlConnection conn = new MySqlConnection();
            MySqlCommand cmd = new MySqlCommand();

            conn.ConnectionString = connection;
            setUser();
            setToday();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = @"DELETE FROM tasks WHERE short_description= @short_description";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@short_description", short_description);
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"INSERT INTO phink VALUES(@username, @today, @type)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", getUser());
                cmd.Parameters.AddWithValue("@today", getToday());
                cmd.Parameters.AddWithValue("@type", "Delete");
                cmd.ExecuteNonQuery();
            }
            catch(MySqlException e)
            {
                MessageBox.Show(e.Number + " Type Error\n" + e.Message);
            }
            cmd.Dispose();
            conn.Close();
            
        }

    }
}
