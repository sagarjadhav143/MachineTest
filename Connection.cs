using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MachineTest
{
    class Connection
    {
        public SqlDataAdapter da;
        public SqlDataReader dr;
        public SqlCommand comd;
        public DataTable dt;
        public DataSet ds;
        public ConnectionState state;
        public SqlConnection con;
        int timeout = 60;
        SqlConnection getConnection()
        {
            string sqlConnection = string.Empty;
            sqlConnection = "Data Source=DESKTOP-QD0DK0G;Initial Catalog=Test;Persist Security Info=True;User ID=sa;Password=123456";
            return new SqlConnection(sqlConnection);
        }
        public DataTable operationOnDataBase(string query)
        {
            try
            {
                da = new SqlDataAdapter();
                dt = new DataTable();
                ds = new DataSet();
                con = getConnection();

                state = con.State;
                if (state == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    using (comd = new SqlCommand(query, con))
                    {
                        using (da = new SqlDataAdapter(comd))
                        {
                            comd.CommandTimeout = timeout;
                            da.Fill(dt);
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                }
                return dt;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                
            }
            return dt;
        }
    }
}
