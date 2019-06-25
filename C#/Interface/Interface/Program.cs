using System;
using static System.Console;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Interface
{
    interface ILogger   // 접근제한자 안쓰면 internal
    {
        void WriteLog(string message);
    }

    interface IFormattableLogger : ILogger
    {
        void WriteLog(string format, params Object[] args);
    }

    class ConsoleLogger : ILogger
    {
        public void WriteLog(string messsage)
        {
            WriteLine("{0} {1}", DateTime.Now.ToLocalTime(), messsage);
        }
    }

    class ConsoleLogger2 : IFormattableLogger
    {
        public void WriteLog(string message)
        {
            WriteLine("{0} {1}", DateTime.Now.ToLocalTime(), message);
        }
        public void WriteLog(string format, params Object[] args)
        {
            string message = String.Format(format, args);
            WriteLine("{0} {1}", DateTime.Now.ToLocalTime(), message);
        }
    }

    class FileLogger : ILogger
    {
        private StreamWriter writer;
        public FileLogger(string path)
        {
            writer = File.CreateText(path);
            writer.AutoFlush = true;
        }
        public void WriteLog(string message)
        {
            writer.WriteLine("{0} {1}", DateTime.Now.ToLocalTime(), message);
        }
    }

    class ClimateMonitor
    {
        private ILogger logger;
        public ClimateMonitor(ILogger logger)
        {
            this.logger = logger;
        }
        public void strat()
        {
            while (true)
            {
                Write("온도 입력 :");
                string temperature = ReadLine();
                if (temperature == "")
                    break;
                logger.WriteLog("현재 온도 : " + temperature);
            }
        }
    }

    class BirthdayInfo
    {
        private string name;
        private DateTime birthday;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public DateTime Birthday
        {
            get { return birthday; }
            set { birthday = value; }
        }
        public int Age
        {
            get { return new DateTime(DateTime.Now.Subtract(birthday).Ticks).Year; }
        }
    }
    class BirthdayInfo2
    {
        public string Name  // 내부적으로 컴파일러가 임의로 변수명 설정해서 생성
        {
            get;
            set;
        } = "모름";
        public DateTime Birthday
        {
            get;
            set;
        } = new DateTime(1, 1, 1);
        public int Age
        {
            get { return new DateTime(DateTime.Now.Subtract(Birthday).Ticks).Year; }
        }
    }
    interface INamedValue
    {
        string Name
        {
            get;
            set;
        }
        string value
        {
            get;
            set;
        }
    }
    class NamedValue : INamedValue
    {
        public string Name
        {
            get;
            set;
        }
        public string value
        {
            get;
            set;
        }
    }
    abstract class Product
    {
        private static int serial = 0;
        public string SerialID
        {
            get { return String.Format("{0:d5}", serial++); }
        }
        abstract public DateTime ProductDate
        {
            get;
            set;
        }
    }
    class MyProduct : Product
    {
        public override DateTime ProductDate { get; set; }
    }
    
    class Program
    {
        private static bool CheckPassed(int score)
        {
            if (score >= 60)
                return true;
            else
                return false;
        }
        static void CopyArray<T>(T[] source, T[] target)
        {
            for (int i = 0; i < source.Length; i++)
                target[i] = source[i];
        }
        static void ch8() // interface
        {
            WriteLine("FileLogger Start");
            ClimateMonitor monitor = new ClimateMonitor(new FileLogger("C:/temp/MyLog.txt"));
            monitor.strat();

            WriteLine("ConsoleLogger Start");
            ClimateMonitor monitor2 = new ClimateMonitor(new ConsoleLogger());
            monitor2.strat();

            IFormattableLogger logger = new ConsoleLogger2();
            logger.WriteLog("The world is not flat");
            logger.WriteLog("{0} + {1} = {2}", 1, 1, 2);
        }
        static void ch9() // Property
        {
            BirthdayInfo birth = new BirthdayInfo();
            birth.Name = "홍길동";
            birth.Birthday = new DateTime(1990, 5, 10);
            WriteLine($"Name : {birth.Name}");
            WriteLine($"Birthday : {birth.Birthday.ToShortDateString()}");
            WriteLine($"Age : {birth.Age}");

            BirthdayInfo2 birth2 = new BirthdayInfo2();
            birth2.Name = "홍길동";
            birth2.Birthday = new DateTime(1990, 5, 10);
            WriteLine($"Name : {birth2.Name}");
            WriteLine($"Birthday : {birth2.Birthday.ToShortDateString()}");
            WriteLine($"Age : {birth2.Age}");

            BirthdayInfo2 birth3 = new BirthdayInfo2()
            {
                Name = "길동",
                Birthday = new DateTime(1969, 5, 2)
            };
            WriteLine($"Name : {birth3.Name}");
            WriteLine($"Birthday : {birth3.Birthday.ToShortDateString()}");
            WriteLine($"Age : {birth3.Age}");

            BirthdayInfo2 birth4 = new BirthdayInfo2();
            WriteLine($"Name : {birth4.Name}");
            WriteLine($"Birthday : {birth4.Birthday.ToShortDateString()}");
            WriteLine($"Age : {birth4.Age}");
            var a = new { Name = "홍길동", Age = 20 }; //readonly
            WriteLine($"Name : {a.Name} , Age : {a.Age}");

            NamedValue name = new NamedValue() { Name = "이름", value = "홍길동" };
            NamedValue height = new NamedValue() { Name = "키", value = "170cm" };
            NamedValue weight = new NamedValue() { Name = "체중", value = "80kg" };
            WriteLine($"{name.Name} : {name.value}");
            WriteLine($"{height.Name} : {height.value}");
            WriteLine($"{weight.Name} : {weight.value}");

            Product product1 = new MyProduct()
            {
                ProductDate = new DateTime(2018, 09, 09)
            };
            WriteLine("Product : {0}, Product Date: {1}", product1.SerialID, product1.ProductDate);
            Product product2 = new MyProduct()
            {
                ProductDate = new DateTime(2018, 03, 03)
            };
            WriteLine("Product : {0}, Product Date: {1}", product2.SerialID, product2.ProductDate);
        }
        static void ch10() // 배열 컬렉션 인덱서
        {
            string[] array1 = new string[3] { "C++", "C#", "Java" };
            foreach (string subject in array1)
                Write($"{subject} ");
            WriteLine();

            string[] array2 = new string[] { "C++", "C#", "Java" };
            foreach (string subject in array2)
                Write($"{subject} ");
            WriteLine();

            string[] array3 = { "C++", "C#", "Java" };
            foreach (string subject in array3)
                Write($"{subject} ");
            WriteLine();

            int[] scores = new int[] { 90, 75, 80, 94, 50 };
            foreach (int score in scores)
                Write($"{score} ");
            WriteLine();

            Array.Sort(scores);
            foreach (int score in scores)
                Write($"{score} ");
            WriteLine();

            WriteLine($"Number of dimensions : {scores.Rank}");

            WriteLine("Binary Search : 80 is at {0}", Array.BinarySearch<int>(scores, 80));
            WriteLine("Linear Search : 94 is at {0}", Array.IndexOf(scores, 94));

            WriteLine("Everyone passed? : {0}", Array.TrueForAll<int>(scores, CheckPassed));
            WriteLine($"Old length of scores : {scores.GetLength(0)}");
            Array.Resize<int>(ref scores, 10);
            WriteLine($"New length of scores : {scores.Length}");

            foreach (int score in scores)
                Write($"{score} ");
            WriteLine();

            Array.Clear(scores, 3, 7);
            foreach (int score in scores)
                Write($"{score} ");
            WriteLine();

            int[][] jagged = new int[3][];
            jagged[0] = new int[5] { 1,2,3,4,5};
            jagged[1] = new int[] { 10, 20, 30 };
            jagged[2] = new int[] { 100, 200 };

            foreach(int[] arr in jagged)
            {
                Write($"Length : {arr.Length}, ");
                foreach (int e in arr)
                    Write($"{e} ");
                WriteLine();
            }
            WriteLine();

            int[][] jagged2 = new int[2][] { new int[] { 100, 200 }, new int[4] { 6, 7, 8, 9 } };
            foreach (int[] arr in jagged2)
            {
                Write($"Length : {arr.Length}, ");
                foreach (int e in arr)
                    Write($"{e} ");
                WriteLine();
            }
            WriteLine();

            ArrayList list = new ArrayList();
            for (int i = 0; i < 5; i++)
                list.Add(i);

            foreach (object obj in list)
                Write($"{obj} ");
            WriteLine();

            list.RemoveAt(2);

            foreach (object obj in list)
                Write($"{obj} ");
            WriteLine();

            list.Insert(2, 2);

            foreach (object obj in list)
                Write($"{obj} ");
            WriteLine();

            list.Add("abc");
            list.Add("def");
            foreach (object obj in list)
                Write($"{obj} ");
            WriteLine();

            Queue que = new Queue();
            que.Enqueue(1);
            que.Enqueue(2);
            que.Enqueue(3);
            que.Enqueue(4);
            que.Enqueue(5);

            while (que.Count > 0)
                WriteLine(que.Dequeue());

            Hashtable hashtable = new Hashtable();  // key 하나에 값 하나. 여러개 넣고자 할때는 배열로 생성
            hashtable["하나"] = "one";
            hashtable["둘"] = "two";
            hashtable["셋"] = "three";
            hashtable["넷"] = "four";
            hashtable["다섯"] = "five";

            WriteLine(hashtable["하나"]);
            WriteLine(hashtable["둘"]);
            WriteLine(hashtable["셋"]);
            WriteLine(hashtable["넷"]);
            WriteLine(hashtable["다섯"]);

            int[] source = { 1, 2, 3, 4, 5 };
            int[] target = new int[source.Length];

            CopyArray<int>(source, target);
            foreach (int i in target)
                WriteLine(i);

            string[] source2 = { "C++", "C#", "Java", "Python" };
            string[] target2 = new string[source2.Length];
            CopyArray<string>(source2, target2);
            foreach (string str in target2)
                WriteLine(str);

        }




        static void Main(string[] args)
        {
            //ch8();
            //ch9();
            ch10();
        }
    }
}
