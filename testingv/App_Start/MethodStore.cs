using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GSTWebsiteClasses.VendorManagement
{
    public class MethodStore
    {
        //private readonly SqlConnection connectionDB;
        private readonly SqlConnection Con;
        private SqlBulkCopy BulkCopy;
        private SqlCommand Cmd;
        private SqlTransaction Transaction;
        public string ConnectionString { get; private set; }
        
        //public MethodStore(SqlConnection con)
        //{
        //    connectionDB = con;
        //}
        public MethodStore()

        {
           
            ConnectionString = ConfigurationManager.ConnectionStrings["VM"].ConnectionString;
            Con = new SqlConnection(ConnectionString);
        }
        public string Error { get; set; }

        public SqlDataReader ExecuteReader(string query, params object[] args)
        {
            SqlDataReader dr = null;
            Con.Open();
            try
            {
                Cmd = new SqlCommand(query, Con) {CommandText = query};

                for (var i = 0; i < args.Length - 1; i = i + 2)
                    Cmd.Parameters.AddWithValue(args[i].ToString(), args[i + 1]);

                dr = Cmd.ExecuteReader();
                return dr;
            }
            catch (Exception ex)
            {
                ConnectionClose(dr);
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return null;
            }
        }
        public bool ExecuteQuery(string query, params object[] args)
        {
            Con.Open();
            Transaction = Con.BeginTransaction();
            try
            {
                Cmd = new SqlCommand(query, Con)
                {
                    Transaction = Transaction,
                    CommandText = query
                };


                for (var i = 0; i < args.Length - 1; i = i + 2)
                    Cmd.Parameters.AddWithValue(args[i].ToString(), args[i + 1]);

                Cmd.ExecuteNonQuery();
                Transaction.Commit();
                Transaction.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                ConnectionClose();
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return false;
            }
        }

        public bool ExecuteQuery(string query)
        {
            try
            {
                Cmd = new SqlCommand(query, Con);
                Con.Open();
                Cmd.ExecuteNonQuery();
                
                ConnectionClose();
            }
            catch (Exception ex)
            {
                ConnectionClose();
                Error = string.Format("Query can not be executed due to: Message {0} StackTrace {1}", ex.Message, ex.StackTrace);
                return false;
            }
            return true;
        }

        public bool sp_ExecuteQuery(params object[] arr)
        {
            Con.Open();
            try
            {
                Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Connection = Con;
                if (arr.Length > 1)
                {
                    Cmd.CommandText = arr[0].ToString();
                }
                for (int i = 1; i < arr.Length; i = i + 2)
                {
                    Cmd.Parameters.AddWithValue(arr[i].ToString(), arr[i + 1]);
                }
                Cmd.ExecuteNonQuery();
                Con.Close();
                return true;
            }
            catch (Exception ex)
            {
                ConnectionClose();
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return false;
            }

        }

        
        public SqlDataReader sp_ExcecuteReader(params object[] arr)
        {
            SqlDataReader dr = null;
            Con.Open();
            try
            {
                Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Connection = Con;
                if (arr.Length > 1)
                {
                    Cmd.CommandText = arr[0].ToString();
                }
                for (int i = 1; i < arr.Length; i = i + 2)
                {
                    Cmd.Parameters.AddWithValue(arr[i].ToString(), arr[i + 1]);
                }

                dr = Cmd.ExecuteReader();
                return dr;
              
            }
            catch(Exception ex)
            {
                ConnectionClose(dr);
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return null;
            }
          
        }
        public SqlDataReader sp_ExcecuteReaderzeroArg(params object[] arr)
        {
            SqlDataReader dr = null;
            Con.Open();
            try
            {
                Cmd = new SqlCommand();
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Connection = Con;
                if (arr.Length > 0)
                {
                    Cmd.CommandText = arr[0].ToString();
                }
                for (int i = 1; i < arr.Length; i = i + 2)
                {
                    Cmd.Parameters.AddWithValue(arr[i].ToString(), arr[i + 1]);
                }

                dr = Cmd.ExecuteReader();
                return dr;

            }
            catch (Exception ex)
            {
                ConnectionClose(dr);
                return null;
            }

        }
        public bool ExecuteQueryInBulk(string query, params object[] args)
        {
            Con.Open();
            Transaction = Con.BeginTransaction();
            try
            {
                Cmd = new SqlCommand(query, Con) {Transaction = Transaction};

                var parameter = new SqlParameter(args[0].ToString(), args[1])
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = args[2].ToString()
                };
                Cmd.Parameters.Add(parameter);

                Cmd.CommandText = query;

                Cmd.ExecuteNonQuery();
                Transaction.Commit();
                Transaction.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                ConnectionClose();
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return false;
            }
        }

        public bool BulkInsert(DataTable dt, string tableName)
        {
            Con.Open();
            Transaction = Con.BeginTransaction();
            try
            {
                BulkCopy = new SqlBulkCopy(Con, SqlBulkCopyOptions.Default, Transaction)
                {
                    DestinationTableName = tableName
                };
                BulkCopy.WriteToServer(dt);
                BulkCopy.Close();
                Transaction.Commit();
                Transaction.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                Transaction.Rollback();
                Transaction.Dispose();
                ConnectionClose();
                Error = $"Query can not be executed due to: Message {ex.Message} StackTrace {ex.StackTrace}";
                return false;
            }
        }

        public void ConnectionClose(SqlDataReader dr)
        {
            if (dr != null)
                dr.Close();

            Con.Close();
        }
        public void ConnectionClose()
        {
            Con.Close();
        }
       
        public  bool CheckDatabaseExists(string NameOfDatabase)
        {
            SqlDataReader dr;
            try
            {
                string DBQuery = string.Format("SELECT [name] FROM master.dbo.sysdatabases where [name] =@NameOfDatabase;");

                dr = ExecuteReader(DBQuery, "@NameOfDatabase", NameOfDatabase);
                if (dr != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
    }
}