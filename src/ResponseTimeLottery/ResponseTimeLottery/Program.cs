using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResponseTimeLottery
{
    class Program
    {
        static void Main(string[] args)
        {
            var query1 = "SELECT 1980 --the year that god made me";
            var query2 = "WAITFOR DELAY '0:0:5'";
            

            while (true)
            {
                var connection = new SqlConnection("Server=.;Integrated Security=SSPI;MultipleActiveResultSets=true;");
                connection.Open();

                Task.Factory.StartNew(() => { RunQuery(connection, query1); }, TaskCreationOptions.LongRunning);
                Task.Factory.StartNew(() => { RunQuery(connection, query2); }, TaskCreationOptions.LongRunning);

                Console.WriteLine("Tasks Scheduled...");
                Console.ReadLine();
                connection.Close();
            }
        }


        static void RunQuery(SqlConnection connection, string query)
        {

            Console.WriteLine("Running Query: {0}", query);
            var timer = StartTimer();

            var cmd = connection.CreateCommand();
            cmd.CommandText = query;
            var reader = cmd.ExecuteReader();

            while (reader.Read());
        
            timer.Stop();

            Console.WriteLine("Query: {0} took: {1} ms", query, timer.ElapsedMilliseconds);

        }

        private static Stopwatch StartTimer()
        {
            var sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        
    }
}
