using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace DETP
{
    class sqlhelp
    {

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        private static SqlCommand cmd;
        private static SqlConnection cn;
        private static SqlDataAdapter da;
        public static DataTable datatable1 = new DataTable();
        public static DataTable datatable2 = new DataTable();
        public static DataTable datatable3 = new DataTable();


        public sqlhelp()
        {
            var configuration = GetConfiguration();
            cn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public static void insert(params string[] values)
        {
            string qry = "insert into " + values[0] + " values(";
            for (int i = 1; i < values.Length; i++)
                qry = qry + " '" + values[i] + "',";
            qry = qry.Remove(qry.Length - 1, 1) + ")";

            try
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
                //MessageBox.Show("Error\n\n" + ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public static int insert1(params string[] values)
        {
            string qry = "insert into " + values[0] + " OUTPUT INSERTED.ID values(";
            for (int i = 1; i < values.Length; i++)
                qry = qry + " '" + values[i] + "',";
            qry = qry.Remove(qry.Length - 1, 1) + ")";

            try
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                cmd = new SqlCommand(qry, cn);
                var res = (int)cmd.ExecuteScalar();
                return res;
            }
            catch (Exception ex)
            {
                throw;
                //MessageBox.Show("Error\n\n" + ex);
            }
            finally
            {
                cn.Close();
            }
        }

        public static DataTable fetch1(string qry)
        {
            DataSet dataset1 = new();
            try
            {

                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                cmd = new SqlCommand(qry, cn);
                da = new SqlDataAdapter(cmd);
                dataset1.Reset();
                dataset1.Clear();
                da.Fill(dataset1);

                datatable1 = dataset1.Tables[0];
                return dataset1.Tables[0];
            }
            catch(Exception ex)
            {
                return null;
            }
            cn.Close();
        }

        public static void fetch2(string qry)
        {
            DataSet dataset1 = new DataSet();

            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd = new SqlCommand(qry, cn);
            da = new SqlDataAdapter(cmd);
            dataset1.Reset();
            dataset1.Clear();
            da.Fill(dataset1);

            //if (dataset1.Tables.Count > 0 && dataset1.Tables[0].Rows.Count > 0)
            //{
            //}
            datatable2 = dataset1.Tables[0];
            cn.Close();

        }

        public static void fetch_no_output(string qry)
        {
            DataSet dataset1 = new DataSet();

            if (cn.State == ConnectionState.Closed)
                cn.Open();
            cmd = new SqlCommand(qry, cn);
            da = new SqlDataAdapter(cmd);
            dataset1.Reset();
            dataset1.Clear();
            da.Fill(dataset1);
            if (dataset1.Tables.Count > 0 && dataset1.Tables[0].Rows.Count > 0)
            {
                datatable3 = dataset1.Tables[0];
                cn.Close();
            }
        }

        public static void performAction(string qry)
        {
            try
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                cmd = new SqlCommand(qry, cn);
                cmd.ExecuteNonQuery();

                cn.Close();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}