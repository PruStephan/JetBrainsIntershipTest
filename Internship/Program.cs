using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static Bureaucracy.Executor;

namespace Bureaucracy
{
    public class Test
    {
        private static char delimiter = '\n';

        public static string res1 = "Current Seals: 1, 2\nCrossed Seals: 1, 2";
        public static string res2 = "Current Seals: 1, 2\n" +
                             "Crossed Seals: 1, 2\n" +
                             "Current Seals: 1, 2, 3\n" +
                             "Crossed Seals: 1, 2, 3\n" +
                             "We are in a cycle. Exiting";
        private static void InitDepartments()
        {
            ClearExecitor();
            AddDepartment(1, 2, 1);
            AddConditionalDepartment(2, 2, 2,  1, 3, 4, 3 );
            AddDepartment(3, 2, 4);
            AddDepartment(1, 2, 5);
        }

        private static void InitCycleDepartments()
        {
            ClearExecitor();
            AddDepartment(1, 2, 1);
            AddConditionalDepartment(2, 2, 2, 1, 3, 4, 3 );
            AddConditionalDepartment(3, 2, 4, 2, 3, 2, 3);
            AddDepartment(1, 2, 2);
        }

        public static void TestExecutor()
        {
            Console.WriteLine("TESTING EXECUTOR");
            InitDepartments();
            var res = GoThrough(0, 2, 2);
            string myRes1 = res.Aggregate((i, j) => i + delimiter + j);

            Debug.Assert(myRes1.Equals(res1));
        }

        public static void TestCycleExecutor()
        {
            Console.WriteLine("TESTING EXECUTOR WITH CYCLES");   
            InitCycleDepartments();
            var res = GoThrough(0, 4, 2);
            string myRes2 = res.Aggregate((i, j) => i + delimiter + j);
            Console.WriteLine(myRes2);
            Debug.Assert(myRes2.Equals(res2));
        }

        public static void TestMultithreadExecutor()
        {
            Console.WriteLine("TESTING EXECUTOR WITH PARALLEL REQUESTS");
            InitDepartments();
            Thread thread1 = new Thread(() => 
                Console.WriteLine(GoThrough(0, 2, 2).Aggregate((i, j) => i + delimiter + j)));
            Thread thread2 = new Thread(() => 
                Console.WriteLine(GoThrough(0, 4, 3).Aggregate((i, j) => i + delimiter + j)));
            thread1.Start();
            thread2.Start();
            // thread1.Join();
            // thread2.Join();
        }
        
        static void Main(string[] args)
        {
            // TestExecutor();
            // TestCycleExecutor();
            TestMultithreadExecutor();
        }
    }
}