using System;
using System.Collections.Generic;
using static System.Console;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ProjectException
{
    delegate void OnlineShopping(string location);
    class InvalidArgumentException : Exception
    {
        public InvalidArgumentException() { }
        public InvalidArgumentException(string message) : base(message) { }
        public object Argument
        {
            get; set;
        }
        public string Range
        {
            get; set;
        }
    }
    class FilterableException : Exception
    {
        public int ErrorNo
        { get; set; }
    }
    delegate int MyDelegate(int a, int b);
    delegate int Compare1(int a, int b);
    delegate int Compare<T>(T a, T b);
    delegate void Notify(string message);
    delegate int dCalculate(int a, int b);
    delegate void EventHandler(string message);
    class Notifier
    {
        public Notify EventOccured;
    }
    class EventListener
    {
        private string name;
        public EventListener(string name)
        {
            this.name = name;
        }
        public void SomethingHappend(string message)
        {
            WriteLine($"{name}.SomethingHappend : {message}");
        }
    }
    class Calculator
    {
        public int Plus(int a, int b)
        {
            return a + b;
        }
        static public int Minus(int a, int b)
        {
            return a - b;
        }
    }
    class MyNotifier
    {
        public event EventHandler DoAlarm; // 선언된 내부에서만 접근 가능하게 만들어주는 event
        public void Get369(int num)
        {
            int temp = num % 10;
            if(temp!=0 && temp%3 == 0)
            {
                DoAlarm(String.Format("{0}: 짝", num));
            }
        }
    }
    class Indexer : IEnumerable , IEnumerator
    {
        private int[] array;
        private int position = -1;
        public Indexer()
        {
            array = new int[3];
        }
        
        public int this[int index]
        {
            get { return array[index]; }
            set
            {
                if (index >= array.Length)
                {
                    Array.Resize<int>(ref array, index + 1);
                    WriteLine("Array Resized : {0}", array.Length);
                }
                array[index] = value;
            }
        }
        public int Length
        {
            get { return array.Length; }
        }

        // IEnumerator 멤버
        public object Current
        {
            get { return array[position]; }
        }

        public void Reset()
        {
            position = -1;
        }

        public bool MoveNext()
        {
            if (position == array.Length -1)
            {
                Reset();
                return false;
            }
            position++;
            return (position < array.Length);
        }
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < array.Length; i++)
                yield return array[i];
        }
    }
    class MyEnumerator
    {
        int[] numbers = { 1, 2, 3, 4 };
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                yield return numbers[i];
            }
        }
    }

    class Program
    {
        static void SimpleFunc(int arg)
        {
            if (arg <= 10)
                WriteLine($"arg : {arg}");
            else
                throw new Exception("인자값이 10보다 큽니다");
        }
        static int Divide(int divisor, int dividend)
        {
            try
            {
                WriteLine("Divede() 시작");
                return divisor / dividend;
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine("Divide() 예외 발생");
                throw e;
            }
            finally
            {
                WriteLine("Divide() 종료");
            }
        }
        static uint MergeARGB(uint alpha, uint red, uint green, uint blue)
        {
            uint[] args = new uint[] { alpha, red, green, blue };
            foreach (uint arg in args)
            {
                if (arg > 255)
                    throw new InvalidArgumentException()
                    {
                        Argument = arg,
                        Range = "0~255"
                    };
            }

            return (alpha << 24 & 0xFF000000) |
                    (red << 16 & 0x00FF0000) |
                    (green << 8 & 0x0000FF00) |
                    (blue & 0x000000FF);
        }
        static int AscendCompare<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b);
        }
        static int DescendCompare<T>(T a, T b) where T : IComparable<T>
        {
            return b.CompareTo(a);
        }
        static void BubbleSort<T>(T[] DataSet, Compare<T> Comparer)
        {
            int i = 0;
            int j = 0;
            T temp;

            for (i = 0; i < DataSet.Length; i++)
            {
                for (j = 0; j < DataSet.Length - (i + 1); j++)
                {
                    if (Comparer(DataSet[j], DataSet[j + 1]) > 0)
                    {
                        temp = DataSet[j + 1];
                        DataSet[j + 1] = DataSet[j];
                        DataSet[j] = temp;
                    }
                }
            }
        }
        static void BubbleSort2(int[] DataSet, Compare1 Comparer)
        {
            int i = 0;
            int j = 0;
            int temp;

            for (i = 0; i < DataSet.Length; i++)
            {
                for (j = 0; j < DataSet.Length - (i + 1); j++)
                {
                    if (Comparer(DataSet[j], DataSet[j + 1]) > 0)
                    {
                        temp = DataSet[j + 1];
                        DataSet[j + 1] = DataSet[j];
                        DataSet[j] = temp;
                    }
                }
            }
        }
        static void OrderGoods(string location)
        {
            WriteLine($"장바구니내 물건을 {location}으로 가져다 주세요");
        }
        static void SpecialOrder(string location)
        {
            WriteLine($"{location}에 사람이 없으면 문앞에 두시고 문자주세요");
        }

        static public void MyHandler(string message)
        {
            WriteLine(message);
        }
        delegate int rCalculator(int a, int b);
        delegate string Concatenate(string[] args);
        static void ch11()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 5; i++)
                list.Add(i);

            foreach (int i in list)
                Write($"{i} ");
            WriteLine();

            list.RemoveAt(2);

            foreach (int i in list)
                Write($"{i} ");
            WriteLine();

            list.Insert(2, 2);

            foreach (int i in list)
                Write($"{i} ");
            WriteLine();

            Dictionary<string, int> dic = new Dictionary<string, int>();
            dic["국어"] = 90;
            dic["영어"] = 85;
            dic["수학"] = 95;
            dic["물리"] = 100;
            dic["화학"] = 95;

            foreach (KeyValuePair<string, int> item in dic)
                WriteLine($"{item.Key} : {item.Value}");
        }
        static void ch12()//예외처리
        {
            int[] arr = { 1, 2, 3 };
            try
            {
                for (int i = 0; i < 5; i++)
                    WriteLine(arr[i]);
            }
            catch (IndexOutOfRangeException e)
            {
                WriteLine($"예외 발생 : {e.Message}");
            }
            WriteLine("종료");

            try
            {
                SimpleFunc(5);
                SimpleFunc(12);
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            try
            {
                int? a = null;
                int b = a ?? throw new ArgumentException();
            }
            catch (ArgumentException e)
            {
                WriteLine(e);
            }
            try
            {
                int[] array = new[] { 1, 2, 3 };
                int index = 4;
                int value = array[index >= 0 && index < 3 ? index : throw new IndexOutOfRangeException()];
            }
            catch (IndexOutOfRangeException e)
            {
                WriteLine(e);
            }

            try
            {
                Write("제수 입력 :");
                String temp = ReadLine();
                int divisor = Convert.ToInt32(temp);

                Write("피제수 입력 :");
                temp = ReadLine();
                int dividend = int.Parse(temp);

                WriteLine("{0}/{1} = {2}", divisor, dividend, Divide(divisor, dividend));
            }
            catch (FormatException e)
            {
                WriteLine("에러 : " + e.Message);
            }
            catch (DivideByZeroException e)
            {
                WriteLine("에러 : " + e.Message);
            }
            finally
            {
                WriteLine("프로그램 종료");
            }

            try
            {
                WriteLine("0x{0:X8}", MergeARGB(255, 100, 100, 100));
                WriteLine("0x{0:X8}", MergeARGB(1, 165, 190, 125));
                WriteLine("0x{0:X8}", MergeARGB(0, 255, 255, 260));
            }
            catch (InvalidArgumentException e)
            {
                WriteLine(e.Message);
                WriteLine($"Argument : {e.Argument}, Range : {e.Range}");
            }

            Write("Enter number between 0~10 :");
            string input = ReadLine();
            try
            {
                int num = int.Parse(input);
                if (num < 0 || num > 10)
                    throw new FilterableException()
                    {
                        ErrorNo = num
                    };
                else
                    WriteLine($"Output : {num}");
            }
            catch (FilterableException e) when (e.ErrorNo < 0)
            {
                WriteLine("음수는 허용되지 않는다.");
            }
            catch (FilterableException e) when (e.ErrorNo > 10)
            {
                WriteLine("10보다 큰 수는 허용되지 않습니다.");
            }

        }
        static void ch13()//deligate (대리자)
        {
            Calculator Calc = new Calculator();
            MyDelegate Callback;

            Callback = new MyDelegate(Calc.Plus);
            WriteLine(Callback(3, 4));

            Callback = new MyDelegate(Calculator.Minus);
            WriteLine(Callback(8, 3));

            int[] array = { 3, 7, 4, 2, 10 };
            WriteLine("Sorting Ascending...");
            BubbleSort<int>(array, new Compare<int>(AscendCompare));
            for (int i = 0; i < array.Length; i++)
                Write($"{array[i]} ");
            WriteLine();

            WriteLine("Sorting Descending...");
            BubbleSort<int>(array, new Compare<int>(DescendCompare));
            for (int i = 0; i < array.Length; i++)
                Write($"{array[i]} ");
            WriteLine();

            string[] array2 = { "hi", "good", "bye", "hello" };
            BubbleSort<string>(array2, new Compare<string>(DescendCompare));
            for (int i = 0; i < array2.Length; i++)
                Write($"{array2[i]} ");
            WriteLine();

            OnlineShopping shopper = new OnlineShopping(OrderGoods) + new OnlineShopping(SpecialOrder); // delegate 체인
            shopper("우리집");

            Notifier notifier = new Notifier();
            EventListener listener1 = new EventListener("Listener1");
            EventListener listener2 = new EventListener("Listener2");
            EventListener listener3 = new EventListener("Listener3");

            notifier.EventOccured = listener1.SomethingHappend;
            notifier.EventOccured += listener2.SomethingHappend;
            notifier.EventOccured += listener3.SomethingHappend;
            notifier.EventOccured("You've got mail");
            WriteLine();

            notifier.EventOccured -= listener2.SomethingHappend;
            notifier.EventOccured("Download completed");
            WriteLine();

            Notify notify1 = new Notify(listener1.SomethingHappend);
            Notify notify2 = new Notify(listener2.SomethingHappend);
            notifier.EventOccured = (Notify)Delegate.Combine(notify1, notify2);
            notifier.EventOccured("Fire");
            WriteLine();

            notifier.EventOccured = (Notify)Delegate.Remove(notifier.EventOccured, notify2);
            notifier.EventOccured("Game Over");

            dCalculate calc;
            calc = delegate (int a, int b)
            {
                return a + b;
            };
            WriteLine("3+4 : {0}", calc(3, 4));

            int[] Aarray = { 3, 7, 4, 2, 9 };
            BubbleSort2(Aarray, delegate (int a, int b)
            {
                if (a > b)
                    return 1;
                else if (a == b)
                    return 0;
                else
                    return -1;
            });
            for (int i = 0; i < Aarray.Length; i++)
                Write($"{Aarray[i]} ");
            WriteLine();

            BubbleSort2(Aarray, delegate (int a, int b)
            {
                if (a < b)
                    return 1;
                else if (a == b)
                    return 0;
                else
                    return -1;
            });
            for (int i = 0; i < Aarray.Length; i++)
                Write($"{Aarray[i]} ");
            WriteLine();

            MyNotifier mynotifier = new MyNotifier();
            mynotifier.DoAlarm += new EventHandler(MyHandler);

            for(int i=1; i<30; i++)
            {
                mynotifier.Get369(i);
            }
           // mynotifier.DoAlarm("이거는 안됨");

        }
        static void ch14()// 람다식
        {
            rCalculator cal = (a, b) => a + b;
            WriteLine($"{3} + {4} : {cal(3, 4)}");

            string[] strArr = { "Microsoft", "C#", "Language" };
            Concatenate concat = (arr) =>
            {
                string result = "";
                foreach (string s in arr)
                    result += s;
                return result;
            };
            WriteLine(concat(strArr));

            // Func는 맨끝이 리턴타입
            Func<int> func1 = () => 10;
            WriteLine($"func1() : {func1()}");

            Func<int, int> func2 = (x) => x * 2;
            WriteLine($"func2(4) : {func2(4)}");

            Func<double, double, double> func3 = (x, y) => x / y;
            WriteLine($"func3(23,6) : {func3(23, 6)}");

            Func<string[], string> func4 = (arr) =>
             {
                 string result = "";
                 foreach (string s in arr)
                     result += s;
                 return result;
             };
            WriteLine($"func4(strArr) : {func4(strArr)}");

            // Action Delegate 리턴이 보이드
            Action act1 = () => WriteLine("Action()");
            act1();

            int result2 = 0;
            Action<int> act2 = (x) => result2 = x * x;
            act2(3);
            WriteLine($"result : {result2}");

            Action<double, double> act3 = (x, y) =>
             {
                 double d = x / y;
                 WriteLine($"Action<T1,T2>({x}, {y}) : {d}");
             };
            act3(10.0, 4.0);

            var obj = new MyEnumerator();
            foreach (int i in obj)
                WriteLine(i);

            Indexer list = new Indexer();
            for (int i = 0; i < 5; i++)
                list[i] = i;
            for (int i = 0; i < list.Length; i++)
                WriteLine(list[i]);
            foreach (int e in list)
                WriteLine(e);
        }
    
        static void Main(string[] args)
        {
            //ch11();
            //ch12();
            //ch13();
            ch14();
        }
    }
}
