using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static System.Console;
using System.Threading.Tasks;
using System.IO;

namespace ThreadProject
{
    class Program
    {
        //static void BlueFlag()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            WriteLine($"청기");
        //            Thread.Sleep(10);
        //        }
        //    }
        //    catch (ThreadAbortException e)
        //    {
        //        WriteLine(e);
        //        Thread.ResetAbort();
        //    }
        //    finally
        //    {
        //        WriteLine("리소스 해제");
        //    }
        //    WriteLine("추가 작업");  
        //}
        //static Boolean setStop = false;
        //static void BlueFlag()
        //{
        //    while(!setStop)
        //    {
        //        WriteLine("청기");
        //        Thread.Sleep(10);
        //    }
        //    WriteLine("추가 작업");
        //}

        static void BlueFlag()
        {
            try
            {
                while(true)
                {
                    WriteLine("청기");
                    Thread.Sleep(10);
                }
            }
            catch (ThreadInterruptedException e)
            {
                WriteLine(e);
            }
            finally
            {
                WriteLine("리소스 해제");
            }
            WriteLine("추가 작업");
        }
        class Counter
        {
            const int LOOP_COUNT = 1000;
            readonly object thisLock;
            bool lockedCount = false;
            private int count;
            public int Count
            {
                get { return count; }
            }
            public Counter()
            {
                thisLock = new object();
                count = 0;
            }
            //public void Increase()
            //{
            //    int loopCount = LOOP_COUNT;
            //    while(loopCount-- > 0)
            //    {
            //        lock (thisLock) // critical section. thislock 사용영역을 사용하면 다른 쓰레드의 접근을 모두 막는다
            //        {
            //            count += 10;
            //            WriteLine($"Increase : loopcount : {loopCount} count:  {count}");
            //        }
            //        Thread.Sleep(1);
            //    }
            //}

            //public void Increase()
            //{
            //    int loopCount = LOOP_COUNT;
            //    while(loopCount-- > 0)
            //    {
            //        Monitor.Enter(thisLock);
            //        try
            //        {
            //            count += 10;
            //        }
            //        finally
            //        {
            //            Monitor.Exit(thisLock);
            //        }
            //        Thread.Sleep(1);
            //    }
            //}

            public void Increase()
            {
                int loopCount = LOOP_COUNT;
                while(loopCount-- > 0)
                {
                    lock(thisLock)
                    {
                        if(count > 0 || lockedCount == true)
                        {
                            Monitor.Wait(thisLock);
                        }
                        lockedCount = true;
                        count++;
                        lockedCount = false;

                        Monitor.Pulse(thisLock);
                    }
                }
            }

            //public void Decrease()
            //{
            //    int loopCount = LOOP_COUNT;
            //    while (loopCount-- > 0)
            //    {
            //        lock (thisLock)
            //        {
            //            count -= 10;
            //            WriteLine($"Decrease : loopcount : {loopCount} count:  {count}");
            //        }
            //        Thread.Sleep(1);
            //    }
            //}

            public void Decrease()
            {
                int loopCount = LOOP_COUNT;
                while (loopCount-- > 0)
                {
                    lock (thisLock)
                    {
                        if (count > 0 || lockedCount == true)
                        {
                            Monitor.Wait(thisLock);
                        }
                        lockedCount = true;
                        count--;
                        lockedCount = false;

                        Monitor.Pulse(thisLock);
                    }
                }
            }
        }
        
        static void Ch_Thread()
        {
            Thread thread = new Thread(new ThreadStart(BlueFlag)); //ThreadStart가 delegate
            WriteLine("Start thread ...");
            thread.Start();

            Thread.Sleep(100);
            //WriteLine("Abortiong thread...");
            //thread.Abort();
            //setStop = true;
            WriteLine("Interrupting thread");
            thread.Interrupt(); // interrupt는 쓰레드가 wait sleep join 상태에 있을때만 걸린다. 미리 걸어놔도 쓰레드는 해당 작업을 끝내고 wsj 상태에 도달했을때 인터럽트가 걸려서 안정적

            WriteLine("Waitng untill thread stop...");
            thread.Join();

            WriteLine("Finished");

            Counter counter = new Counter();
            Thread incThread = new Thread(new ThreadStart(counter.Increase));
            Thread decThread = new Thread(new ThreadStart(counter.Decrease));

            incThread.Start();
            decThread.Start();

            incThread.Join();
            decThread.Join();

            WriteLine(counter.Count);
        }
        static bool IsPrime(long number)
        {
            if (number < 2)
                return false;
            if (number % 2 == 0 && number != 2)
                return false;
            for(long i = 2; i<number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }
        static void Main(string[] args)
        {
            //Ch_Thread();
            /*
            {
                string srcFile = args[0];

                Action<object> FileCopyAction = (object state) =>
                 {
                     string[] paths = (string[])state;
                     File.Copy(paths[0], paths[1]);
                     WriteLine("TaskID:{0}, ThreadID:{1}, {2} was Copied to {3}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId, paths[0], paths[1]);
                 };

                Task t1 = new Task(FileCopyAction, new string[] { srcFile, srcFile + ".copy1" });
                t1.Start();

                Task t2 = Task.Run(() =>
                {
                    FileCopyAction(new string[] { srcFile, srcFile + ".copy2" });
                });

                Task t3 = new Task(FileCopyAction, new string[] { srcFile, srcFile + ".copy3" });
                t3.RunSynchronously();

                t1.Wait();
                t2.Wait();
            }
            */
            //long from = Convert.ToInt64(args[0]);
            //long to = Convert.ToInt64(args[1]);
            //int taskCount = Convert.ToInt32(args[2]);

            //Func<object, List<long>> FindPrimeFunc = (objRange) =>
            // {
            //     long[] range = (long[])objRange;
            //     List<long> found = new List<long>();

            //     for (long i = range[0]; i <= range[1]; i++)
            //     {
            //         if (IsPrime(i))
            //             found.Add(i);
            //     }
            //     return found;
            // };

            //Task<List<long>>[] tasks = new Task<List<long>>[taskCount];
            //long currentFrom = from;
            //long currentTo = from + (to - from) / tasks.Length;

            //for(int i=0; i<tasks.Length; i++)
            //{
            //    WriteLine("Task[{0}] :: {1} ~ {2}", i, currentFrom, currentTo);
            //    tasks[i] = new Task<List<long>>(FindPrimeFunc, new long[] { currentFrom, currentTo });
            //    currentFrom = currentTo + 1;

            //    if (i == tasks.Length - 2)
            //        currentTo = to;
            //    else
            //        currentTo += ((to - from) / tasks.Length);
            //}
            //WriteLine("Please press enter to start");
            //ReadLine();
            //WriteLine("Started ...");

            //DateTime startTime = DateTime.Now;

            //foreach (Task<List<long>> task in tasks)
            //    task.Start();

            //List<long> total = new List<long>();

            //foreach(Task<List<long>> task in tasks)
            //{
            //    task.Wait();
            //    total.AddRange(task.Result.ToArray());
            //}
            //DateTime endTime = DateTime.Now;
            //TimeSpan ellapsed = endTime - startTime;
            //WriteLine("Prime number count between {0} and {1} : {2}", from, to, total.Count);
            //WriteLine("Ellapsed time : {0}", ellapsed);

            long from = Convert.ToInt64(args[0]);
            long to = Convert.ToInt64(args[1]);

            WriteLine("Please press enter to start");
            ReadLine();
            WriteLine("Started...");

            DateTime startTime = DateTime.Now;
            List<long> total = new List<long>();

            Parallel.For(from,to+1,(long i) =>
            {
                if (IsPrime(i))
                    total.Add(i);
            });
            DateTime endTime = DateTime.Now;
            TimeSpan ellapsed = endTime - startTime;

            WriteLine("Prime number count between {0} and {1} : {2}", from, to, total.Count);
            WriteLine("Ellpased time :{0}", ellapsed);
                
        }
    }
}
