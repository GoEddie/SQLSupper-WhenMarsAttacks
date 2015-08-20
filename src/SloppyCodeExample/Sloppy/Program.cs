using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sloppy
{
    class Program
    {
        static void Main(string[] args)
        {

            var conn = new SqlConnection("SERVER=.;Integrated Security=SSPI;");
            //var conn = new SqlConnection("SERVER=.;Integrated Security=SSPI;MultipleActiveResultSets=true");
            conn.Open();
            
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM sys.sysindexes cross join sys.sysobjects o";
            var reader = cmd.ExecuteReader();

            var cmd2 = conn.CreateCommand();
            cmd2.CommandText = "SELECT 123, 'MARS IS AWESOME', 456";
            var reader2 = cmd2.ExecuteReader();


            Console.WriteLine("WOWSERS");
            Console.ReadLine();


            

        }
    }
}
