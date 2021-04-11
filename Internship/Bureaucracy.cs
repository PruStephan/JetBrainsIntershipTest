using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Bureaucracy
{
    public class Department
    {
        private protected int _i, _j, _k;
        private protected bool _visited;
        

        public bool Visited
        {
            get { return _visited; }
        }

        public Department(int i, int j, int k)
        {
            this._i = i;
            this._j = i;
            this._k = i;
            this._visited = false;
        }
        
        public int ExecuteWork(Dictionary<int, bool> seals)
        {
            this._visited = true;
            bool iVal = false;
            if (!seals.TryGetValue(_i, out iVal) && !iVal)
            {
                seals[_i] = false;
            }

            if (seals.ContainsKey(_j))
            { 
                seals[_j] = false;
            }

            return _k;
        }
    }

    public class ConditionalDepartment : Department
    {
        private protected int _s; 
        private protected int _t, _r, _p;


        public new int ExecuteWork(Dictionary<int, bool> seals)
        {
            bool ifExistsVal = false;
            if (seals.TryGetValue(_s, out ifExistsVal) && ifExistsVal)
            {
                return this.ExecuteWork(seals);
            }
            else
            {
                bool tVal = false;
                if (!seals.TryGetValue(_i, out tVal) && !tVal)
                {
                    seals[_t] = true;
                }

                if (seals.ContainsKey(_r))
                {
                    seals[_r] = false;
                }

                return _p;
            }
        }

        public ConditionalDepartment(int i, int j, int k, int s, int t, int r, int p) : base(i, j, k)
        {
            this._s = s;
            this._t = t;
            this._r = r;
            this._p = p;
        }
    }

    public static class Executor
    {
        private static Semaphore sem = new Semaphore(1, 2);
        public static List<Department> departments;
        public static Dictionary<int, bool> seals;

        public static void ClearExecitor()
        {
            departments = new List<Department>();
            seals = new Dictionary<int, bool>();
        }

        public static void AddDepartment(int i, int j, int k)
        {
            departments.Add(new Department(i, j, k));
        }

        public static void AddConditionalDepartment(int i, int j, int k, int s, int t, int r, int p)
        {
            departments.Add(new ConditionalDepartment(i, j, k, s, t, r, p));
        }

        public static void AddSeal(int i)
        {
            seals.Add(i, true);
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH")]
        private static string SaveCondition()
        {
            bool addedSeals  = false, addedCrossed = false;
            string allSeals = "Current Seals: ", crossedSeals = "Crossed Seals: ";
            foreach (var keyVal in seals)
            {
                allSeals += keyVal.Key +  ", ";
                addedSeals = true;
                if (!keyVal.Value)
                {
                    addedCrossed = true;
                    crossedSeals += keyVal.Key + ", ";
                }
            }

            if (addedCrossed)
                crossedSeals = crossedSeals.Remove(crossedSeals.Length - 2);
            
            if (addedSeals)
                allSeals = allSeals.Remove(allSeals.Length - 2);

            return allSeals + "\n" + crossedSeals;
        }
        
        public static List<string> GoThrough(int s, int f, int q)
        {
            sem.WaitOne();
            var initialSeals = seals;
            int  next = s;
            int last = s;
            
            int cycleCount = 0;
            
            List<string> res = new List<string>();
            while (last != f)
            {
                if (cycleCount == departments.Count)
                {
                    res.Add("We are in a cycle. Exiting");
                    return res;
                }

                next  = departments[last].ExecuteWork(seals);
                last = next;

                if (departments[next].Visited)
                {
                    cycleCount++;
                }
                else
                {
                    cycleCount = 0;
                }

                seals = initialSeals;
                if (last == q)
                    res.Add(SaveCondition());
            }
            
            sem.Release();
            return res;
        }
    }
}