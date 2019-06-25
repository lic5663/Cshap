using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Runtime.CompilerServices;

namespace AttributeChapter
{
    class MyClass
    {
        [Obsolete("OldMethod는 폐기되었습니다. NewMethod()를 이용하세요")]
        public void OldMethod()
        {
            WriteLine("I'm old");
        }

        [Obsolete("쓰지마라")]
        public void NewMethod()
        {
            WriteLine("I'm new");
        }
    }
    public static class Trace
    {
        public static void WriteLine(string message,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        {
            Console.WriteLine($"{file}(Line:{line}) {member}:{message}");
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    class History : System.Attribute
    {
        private string programmer;
        public double version;
        public string changes;

        public History(string programmer)
        {
            this.programmer = programmer;
            version = 1.0;
            changes = "First release";
        }

        public string GetProgrammer()
        {
            return programmer;
        }
    }

    [History("Sean", version = 0.1, changes = "2017-11-01 Create class stub")]
    [History("Bob", version = 0.2, changes = "2017-12-03 Added Func() Method")]
    class YourClass
    {
        public void Func()
        {
            WriteLine("Func()");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            MyClass obj = new MyClass();

            obj.OldMethod();
            obj.NewMethod();

            Trace.WriteLine("안녀엉");

            Type type = typeof(YourClass);
            Attribute[] attributes = Attribute.GetCustomAttributes(type);

            WriteLine("MyClass change history...");

            foreach (Attribute a in attributes)
            {
                History h = a as History;
                if (h != null)
                    WriteLine("Ver:{0}, Programmer:{1}, Changes:{2}", h.version, h.GetProgrammer(), h.changes);
            }
        }
    }
}
