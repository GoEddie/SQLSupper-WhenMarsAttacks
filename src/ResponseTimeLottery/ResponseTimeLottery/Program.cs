using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ResponseTimeLottery
{
    class Program
    {
        private static int QueryCompletions;
        private static object _lock;

        private static void Main()
        {
            //MarsConnection();
            MultipleConnections();
        }

        private static void MarsConnection(){

        //QUERIES

            var query1 = "SELECT 1980 --the year that god made me";
            //var query2 = "WAITFOR DELAY '0:0:5'";
            var query2 =
                "select 1000; select top 1000 * from sys.sysindexes cross join sys.sysobjects;create table #ddd(id int); drop table #ddd";

            _lock = new object();

            while (true)
            {
                //Create MARS connection  
                var connection = new SqlConnection("Server=.;Integrated Security=SSPI;MultipleActiveResultSets=true;");
                
                connection.Open();
                
                QueryCompletions = 0;

                //Run queries on seperate threads
                Task.Factory.StartNew(() => { RunQuery(connection, query2); }, TaskCreationOptions.LongRunning);
                Task.Factory.StartNew(() => { RunQuery(connection, query1); }, TaskCreationOptions.LongRunning);
                
                lock (_lock)
                    Console.WriteLine("Tasks Scheduled...");

                //Wait for them both to finish
                while (QueryCompletions < 2)
                    Thread.Sleep(100);

                connection.Close();
            }
        }

        private static void MultipleConnections()
        {
            //QUERIES

            var query1 = "SELECT 1980 --the year that god made me";
            //var query2 = "WAITFOR DELAY '0:0:5'";
            var query2 =
                "select 1000; select top 1000 * from sys.sysindexes cross join sys.sysobjects;create table #ddd(id int); drop table #ddd";

            _lock = new object();

            while (true)
            {
                //Create MARS connection  
                
                var connection1 = new SqlConnection("Server=.;Integrated Security=SSPI;MultipleActiveResultSets=true;");
                var connection2 = new SqlConnection("Server=.;Integrated Security=SSPI;MultipleActiveResultSets=true;");

                
                connection1.Open();
                connection2.Open();

                QueryCompletions = 0;

                //Run queries on seperate threads
                Task.Factory.StartNew(() => { RunQuery(connection1, query2); }, TaskCreationOptions.LongRunning);
                Task.Factory.StartNew(() => { RunQuery(connection2, query1); }, TaskCreationOptions.LongRunning);


                lock (_lock)
                    Console.WriteLine("Tasks Scheduled...");

                //Wait for them both to finish
                while (QueryCompletions < 2)
                    Thread.Sleep(100);

                
                connection1.Close();
                connection2.Close();
            }
        }

        private static void RunQuery(SqlConnection connection, string query)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandTimeout = 60*1000;
            cmd.CommandText = query;

            var timer = StartTimer();
            //run query
            var reader = cmd.ExecuteReader();

            //read off the full response
            while (reader.Read()) ;

            timer.Stop();

            lock (_lock)
            {
                if (timer.ElapsedMilliseconds >= 10)
                    Console.ForegroundColor = ConsoleColor.Red;

                if (timer.ElapsedMilliseconds < 10)
                    Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("Query: {0} took: {1} ms", query, timer.ElapsedMilliseconds);


                Console.ForegroundColor = ConsoleColor.White;
            }


            Interlocked.Increment(ref QueryCompletions);
        }

        private static Stopwatch StartTimer()
        {
            var sw = new Stopwatch();
            sw.Start();
            return sw;
        }
    }
}