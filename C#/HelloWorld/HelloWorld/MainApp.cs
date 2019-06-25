using System;
using static System.Console;    // 시스템 콘솔에 있는 static 메소드를 바로 쓰겟다.
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace HelloWorld
{
    class Product
    {
        private int price = 100;
        public ref int getPrice()
        {
            return ref price;
        }

        public void PrintPrice()
        {
            WriteLine($"Price : {price}");
        }
    }
    class Car   // C#에선 소멸자 사용을 권하지 않음(가비지 콜렉터가 언제 실행될지 모르므로 정상 작동 예상이 힘듬)
    {
        private string model;
        private string color;

        public Car() // 생성자
        {
            model = "세단";
            color = "흰색";
        }

        public Car(string m, string c)
        {
            model = m;
            color = c;
        }

        public void ShowStatus()
        {
            WriteLine($"Model : {model} , Color : {color}");
        }
        ~Car()
        {
            WriteLine("소멸자 실행");
        }
    }
    class StaticField
    {
        public static int count = 0;
        public static void ShowCount()
        {
            WriteLine($"StaticField.count : {count}");
        }
    }
    class ClassA
    {
        public ClassA()
        {
            StaticField.count++;
        }
    }
    class ClassB
    {
        public ClassB()
        {
            StaticField.count++;
        }

    }
    class ShallowDeepCopy
    {
        public int Field1;
        public int Field2;

        public ShallowDeepCopy DeepCopy()
        {
            ShallowDeepCopy newClass = new ShallowDeepCopy();
            newClass.Field1 = Field1;
            newClass.Field2 = Field2;

            return newClass;
        }
    }
    class Employee
    {
        private string name;
        private string position;

        public void SetName(string name)
        {
            this.name = name;
        }
        public string GetName()
        {
            return name;
        }
        public void SetPosiotn(string position)
        {
            this.position = position;
        }
        public string GetPosition()
        {
            return position;
        }
    }
    class ThisConstructor   //코드 중복 방지를 위해 this() 생성자 사용
    {
        private int a, b, c;
        
        public ThisConstructor()
        {
            a = 1111;
            WriteLine("ThisConstructor()");
        }
        public ThisConstructor(int b) :this()
        {
            this.b = b;
            WriteLine("ThisConstructor(int)");
        }
        public ThisConstructor(int b, int c) : this(b)
        {
            this.c = c;
            WriteLine("ThisConstrcutor(int, int)");
        }
        public void PrintFields()
        {
            WriteLine($"a:{a}, b:{b}, c:{c}");
        }
    }
    class Parent
    {
        protected string name;
        public Parent()
        {
            name = "김철수";
            WriteLine($"{this.name}.Parent()");
        }
        public Parent(string name)
        {
            this.name = name;
            WriteLine($"{this.name}.Parent(string)");
        }
        ~Parent()
        {
            WriteLine($"{this.name}.~Parent()");
        }
        public void ParentMethod()
        {
            WriteLine($"{this.name}.ParentMethod()");
        }
    }

    class Child: Parent
    {
        public Child()
        {
            WriteLine($"{this.name}.Child()");
        }
        public Child(string name) : base(name)
        {
            WriteLine($"{this.name}.Child(string)");
        }
        ~Child()
        {
            WriteLine($"{this.name}.~Child()");
        }
        public void ChildMethod()
        {
            WriteLine($"{this.name}.ChildMethod()");
        }
    }

    class Mammal
    {
        public void Nurse()
        {
            WriteLine("Nursing~~");
        }
    }
    class Dog:Mammal
    {
        public void Bark()
        {
            WriteLine("Barking~~");
        }
    }
    class Human:Mammal
    {
        public void Speak()
        {
            WriteLine("Speking ~~");
        }
    }
    class Car2
    {
        protected string model;
        protected string powerTrain;

        public Car2(string model,string powerTrain)
        {
            this.model = model;
            this.powerTrain = powerTrain;
        }
        public void drive()
        {
            WriteLine("달린다");
        }

    }
    class GasolineCar :Car2
    {
        public GasolineCar(string model,string powerTrain) : base(model,powerTrain)
        {

        }
        public new void drive()
        {
            //base.drive();
            WriteLine($"{model} {powerTrain} 부르릉");
        }
    }
    class HybridCar :Car2
    {
        public HybridCar(string model, string powerTrain) :base(model,powerTrain)
        {

        }
        public new void drive()
        {
            //base.drive();
            WriteLine($"{model} {powerTrain} 스르륵");
        }
    }
    class Configuration
    {
        List<ItemValue> listConfig = new List<ItemValue>();
        public void SetConfig(string item, string value)
        {
            ItemValue iv = new ItemValue();
            iv.SetValue(this, item, value);
        }
        public string GetConfig(string item)
        {
            foreach(ItemValue iv in listConfig)
            {
                if (iv.GetItem() == item)
                    return iv.GetValue();
            }
            return "";
        }

        private class ItemValue
        {
            private string item;
            private string value;

            public void SetValue(Configuration config, string item, string value)
            {
                this.item = item;
                this.value = value;

                bool found = false;
                for(int i=0; i<config.listConfig.Count; i++)
                {
                    if(config.listConfig[i].item == item)
                    {
                        config.listConfig[i] = this;
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    config.listConfig.Add(this);

            }
            public string GetItem()
            {
                return item;
            }
            public string GetValue()
            {
                return value;
            }
        }
    }
    partial class AAA
    {
        public void Method1()
        {
            WriteLine("Method1()");
        }
        public void Method2()
        {
            WriteLine("Method2()");
        }

    }
    partial class AAA
    {
        public void Method3()
        {
            WriteLine("Method3()");
        }
        public void Method4()
        {
            WriteLine("Method4()");
        }
    }
    public static class EnhancedInteger
    {
        public static float Square(this float input)
        {
            return input * input;
        }

        public static int Power(this int input, int exponet)
        {
            int result = input;
            for (int i = 1; i < exponet; i++)
                result *= input;
            return result;
        }
    }
    struct Point3D
    {
        public int x;
        public int y;
        public int z;
        public Point3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    
        public override string ToString() // object의 tostring을 오버라이딩
        {
            return string.Format($"{x}, {y}, {z}");
        }
    }

    abstract class AbstractBase
    {
        protected void PrivateMethodA()
        {
            WriteLine("AbstractBase.privateMethodA()");
        }
        public void PublicMethodA()
        {
            WriteLine("AbstractBase.PublicMethodA()");
        }
        public abstract void AbstractMethodA();
    }

    class Derived : AbstractBase
    {
        public override void AbstractMethodA()
        {
            WriteLine("Derived.AbstractMehtodA()");
            PrivateMethodA();
        }
    }

    class MainApp
    {
        static void day1()
        {
            Console.WriteLine("Hello World!");
            WriteLine("안녕 세상!");

            sbyte a = -10;
            byte b = 40;
            WriteLine($"a={a}, b ={b}");

            short c = -30000;
            ushort d = 60000;
            WriteLine("c={0}, d={1}", c, d);

            int e = -10_000_000;
            uint f = 300_000_000;
            WriteLine($"e={e}, f={f}");

            long g = -500_000_000_000;
            ulong h = 2_000_000_000_000_000_000;
            WriteLine("g={0}, h={1}", g, h);

            // decibal 16(29자리까지 표현)
            float f_a = 3.1415_9265_3589_7932_3846_4643_3832_79f;
            double d_a = 3.1415_9265_3589_7932_3846_4643_3832_79;
            decimal dec_a = 3.1415_9265_3589_7932_3846_4643_3832_79m;
            WriteLine(f_a);
            WriteLine(d_a);
            WriteLine(dec_a);

            string strA = "동해물과 백두산이";
            string strB = "마르고 닳도록";
            WriteLine(strA);
            WriteLine(strB);

            object ob_a = 123;
            object ob_b = 3.14159m;
            object ob_c = true;
            object ob_d = "문자열";
            WriteLine(ob_a);
            WriteLine(ob_b);
            WriteLine(ob_c);
            WriteLine(ob_d);

            int i_a = 123;
            string str_a = i_a.ToString();
            WriteLine(str_a);

            float f_b = 3.14f;
            string str_b = f_b.ToString();
            WriteLine(str_b);

            string str = "123456";
            int i_c = int.Parse(str);
            WriteLine(i_c);

            const int MAX_INT = 2147483647;
            const int MIN_INT = -2147483648;
            WriteLine(MAX_INT);
            WriteLine(MIN_INT);

            WriteLine((int)ColorCode.RED);
            WriteLine((int)ColorCode.BLUE);
            WriteLine(ColorCode.GREEN);
            WriteLine(ColorCode.ORANGE);

            ColorCode cCode = ColorCode.RED;
            WriteLine(cCode == ColorCode.BLUE);
            WriteLine(cCode == ColorCode.RED);

            WriteLine((int)ColorCode2.RED);
            WriteLine((int)ColorCode2.BLUE);
            WriteLine((int)ColorCode2.GREEN);
            WriteLine((int)ColorCode2.ORANGE);

            int? m = null;      //nullable 연산자 '?'
            WriteLine(m.HasValue);
            WriteLine(m != null);

            m = 3;
            WriteLine(m.HasValue);
            WriteLine(m != null);
            WriteLine(m.Value);
            WriteLine(m);

            // var 타입
            // 선언과 동시에 초기화 필요 run때 bind됨
            // 로컬 변수로만 사용 가능
            var var_a = 20;
            WriteLine("Typr:{0}, value:{1}", var_a.GetType(), var_a);

            var var_b = 3.141592;
            WriteLine("typr:{0}, value:{1}", var_b.GetType(), var_b);

            var var_c = "hello world";
            WriteLine("typr:{0}, value:{1}", var_c.GetType(), var_c);

            var var_d = new int[] { 10, 20, 30 };
            WriteLine("typr:{0}, value:", var_d.GetType());
            foreach (var var_e in var_d)
                Write("{0} ", var_e);
            WriteLine();

            string st = "This is string search sample";
            WriteLine(st);

            WriteLine("index of 'search' : {0}", st.IndexOf("search"));
            WriteLine("index of 'h' : {0}", st.IndexOf('h'));

            WriteLine("StartWith 'This' : {0}", st.StartsWith("This"));
            WriteLine("StartWith 'string : {0}", st.StartsWith("string"));

            WriteLine("EndsWith 'This' : {0}", st.EndsWith("This"));
            WriteLine("EndsWith 'sample' : {0}", st.EndsWith("sample"));

            WriteLine("Contains 'search' : {0}", st.Contains("search"));
            WriteLine("Contains 'school' : {0}", st.Contains("school"));

            WriteLine("Replace 'sample' with 'example : {0}", st.Replace("sample", "example"));

            WriteLine("ToLower() : {0}", "Hello World".ToLower());
            WriteLine("ToUpper() : {0}", "Hello World".ToUpper());

            WriteLine("Insert() : {0}", "Hello World".Insert(6, "Wonderful"));
            WriteLine("Remove() : {0}", "Hello Wonderful World".Remove(6, 10));

            WriteLine("Trim() : '{0}'", " I am Tom ".Trim());
            WriteLine("TrimeStart() : '{0}'", " I am Tom ".TrimStart());
            WriteLine("TrimEnd() : '{0}'", " I am Tom ".TrimEnd());

            string strr = "Welcom to the C# World!";
            WriteLine(strr.Substring(15, 2));
            WriteLine(strr.Substring(8));

            string[] arr = strr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries); // 빈 항목은 제거 "  "
            WriteLine("word count : {0}", arr.Length);
            foreach (string element in arr)
                WriteLine("{0} ", element);

            string fmt = "{0,-10}{1,-5}{2,20}";
            WriteLine(fmt, "Type", "Size", "Explain");
            WriteLine(fmt, "byte", "1", "byte 타입");
            WriteLine(fmt, "short", "2", "short 타입");
            WriteLine(fmt, "int", "4", "int 타입");
            WriteLine(fmt, "long", "8", "long 타입");

            WriteLine("10진수: {0:D}", 123);
            WriteLine("10진수: {0:D3}", 123);

            WriteLine("16진수 : 0x{0:X}", 0xFF1234);
            WriteLine("16진수 : 0x{0:X8}", 0xFF1234);

            WriteLine("숫자 : {0:N}", 123456);
            WriteLine("숫자 : {0:N0}", 123456);

            WriteLine("고정소수점 : {0:F}", 123.456);
            WriteLine("고정소수점 : {0:F5}", 123.456);

            WriteLine("공학: {0:E}", 123.456789);

            DateTime dt = DateTime.Now;
            WriteLine("12시간 형식 : {0:yyyy-MM-dd tt hh:mm:ss (ddd)}", dt);
            WriteLine("24시간 형식 : {0:yyyy-MM-dd HH:mm:ss (dddd)}", dt);

            CultureInfo ciKR = new CultureInfo("ko-KR");
            WriteLine();
            WriteLine(dt.ToString("yyyy-MM-dd tt hh:mm:ss (ddd)"), ciKR);
            WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss (dddd)"), ciKR);
            WriteLine(dt.ToString(ciKR));

            CultureInfo ciUS = new CultureInfo("en-US");
            WriteLine();
            WriteLine(dt.ToString("yyyy-MM-dd tt hh:mm:ss (ddd)"), ciUS);
            WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss (dddd)"), ciUS);
            WriteLine(dt.ToString(ciUS));

            string name = "고길동";
            int age = 25;
            WriteLine($"{name,-10}, {age:D3}");

            name = "김유신";
            age = 30;
            WriteLine($"{name},{age,-10:D3}");

            name = "박문수";
            age = 15;
            WriteLine($"{name},{(age > 20 ? "성인" : "미성년자")}");

            ArrayList arA = null;
            arA?.Add("C++");    // 앞에거가 null인지 판단. null일 경우 null반환. 아니면 뒤에꺼 실행
            arA?.Add("C#");
            WriteLine($"Count : {arA?.Count}");
            WriteLine($"{arA?[0]}");
            WriteLine($"{arA?[1]}");

            arA = new ArrayList();
            arA?.Add("C++");
            arA?.Add("C#");
            WriteLine($"Count : {arA?.Count}");
            WriteLine($"{arA?[0]}");
            WriteLine($"{arA?[1]}");

            int? num = null;
            WriteLine($"{num ?? 0}");   // null이면 뒤쪽꺼, 아니면 원래 쓰려고했던거. 널 병합 연산자
            num = 10;
            WriteLine($"{num ?? 0}");

            string strN = null;
            WriteLine($"{strN ?? "Default"}");

            strN = "I study C#";
            WriteLine($"{strN ?? "Default"}");

            Write("요일을 입력하세요 (월 화 수 목 금 토 일) : ");
            string day = ReadLine();
            switch (day)
            {
                case "일":
                    WriteLine("Sunday");
                    break;
                case "월":
                    WriteLine("Monday");
                    break;
                case "화":
                    WriteLine("Tuesday");
                    break;
                case "수":
                    WriteLine("Wednesday");
                    break;
                case "목":
                    WriteLine("Thursday");
                    break;
                case "금":
                    WriteLine("Friday");
                    break;
                case "토":
                    WriteLine("Saturday");
                    break;

            }

            object obj = null;
            // tryparse : 제대로 변경되면 true 안되면 false; parse는 try catch로 잡아줘야함. 정상적인 변환이 아니면 터짐
            string str1 = ReadLine();
            if (int.TryParse(str1, out int int_num))
                obj = int_num;
            else if (float.TryParse(str1, out float float_num))
                obj = float_num;
            else
                obj = str1;

            switch (obj)
            {
                case int i:
                    WriteLine($"{i}는 int 형식입니다");
                    break;

                case float ft:
                    WriteLine($"{ft}는 float 형식입니다");
                    break;
                default:
                    WriteLine($"{obj}는 object 형식입니다");
                    break;

            }

            int[] arr1 = new int[] { 0, 1, 2, 3, 4 };
            foreach (int i in arr1)
                WriteLine(i);
            WriteLine("{0} + {1} = {2}", 7, 8, Calculator.Plus(7, 8));

            int x = 3;
            int y = 5;

            WriteLine($"x:{x} , y:{y}");
            Calculator.Swap(ref x, ref y);
            WriteLine($"x:{x} , y:{y}");

            Product carrot = new Product();
            ref int ref_price = ref carrot.getPrice();
            int normal_price = carrot.getPrice();

            carrot.PrintPrice();
            WriteLine($"Ref Price : {ref_price}");
            WriteLine($"Normal Price : {normal_price}");

            ref_price = 200;
            carrot.PrintPrice();
            WriteLine($"Ref Price : {ref_price}");
            WriteLine($"Normal Price : {normal_price}");

            int input_a = 20;
            int input_b = 3;
            //int c;
            //int d;
            Calculator.Divede(input_a, input_b, out int output_c, out int output_d);
            WriteLine($"a:{input_a}, b:{input_b}, a/b:{output_c}, a%b:{output_d}");
        }
        static void day2()
        {
            int sum = Sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            WriteLine($"Sum:{sum}");
            PrintfProfile("홍길동");
            PrintfProfile("홍길동", "010-1111-2222");
            PrintfProfile(name: "김유신");
            PrintfProfile(phone: "010-1111-2222", name: "이순신");
            WriteLine(ToLowerString("Hello"));
            WriteLine(ToLowerString("World"));
            WriteLine(ToLowerString("C#:Programming"));

            Car myCar = new Car();
            Car yourCar = new Car("SUV", "블랙");
            myCar.ShowStatus();
            yourCar.ShowStatus();

            StaticField.ShowCount();
            new ClassA();
            new ClassA();
            new ClassB();
            new ClassB();
            StaticField.ShowCount();

            WriteLine("Shallow Copy");
            ShallowDeepCopy source = new ShallowDeepCopy();
            source.Field1 = 10;
            source.Field2 = 20;

            ShallowDeepCopy target = source;
            target.Field2 = 30;
            WriteLine($"{source.Field1} {source.Field2}");
            WriteLine($"{target.Field1} {target.Field2}");


            WriteLine("Deep Copy");
            ShallowDeepCopy deepSource = new ShallowDeepCopy();
            deepSource.Field1 = 10;
            deepSource.Field2 = 20;

            ShallowDeepCopy deepTarget = deepSource.DeepCopy();
            deepTarget.Field2 = 30;

            WriteLine($"{deepSource.Field1} {deepSource.Field2}");
            WriteLine($"{deepTarget.Field1} {deepTarget.Field2}");

            Employee worker = new Employee();
            worker.SetName("홍길동");
            worker.SetPosiotn("Guard");
            WriteLine($"{worker.GetName()} {worker.GetPosition()}");

            ThisConstructor a = new ThisConstructor();
            a.PrintFields();
            WriteLine();

            ThisConstructor b = new ThisConstructor(10);
            b.PrintFields();
            WriteLine();

            ThisConstructor c = new ThisConstructor(10, 20);
            c.PrintFields();

            Parent parent = new Parent("홍길동아버지");
            parent.ParentMethod();
            WriteLine();

            Child child = new Child("홍길동");
            child.ParentMethod();
            child.ChildMethod();
            WriteLine();

            Child child2 = new Child();
            child2.ParentMethod();
            child2.ChildMethod();
            WriteLine();

            Mammal mammal = new Mammal();
            mammal.Nurse();
            WriteLine();

            mammal = new Dog();
            mammal.Nurse();
            WriteLine();

            //Dog dog = (Dog)mammal;
            //dog.Nurse();
            //dog.Bark();
            //WriteLine();

            if (mammal is Human)
            {
                Dog dog = (Dog)mammal;
                dog.Nurse();
                dog.Bark();
                WriteLine();
            }

            mammal = new Human();
            mammal.Nurse();
            WriteLine();

            //Human human = (Human)mammal;
            //human.Nurse();
            //human.Speak();

            Human human = mammal as Human;
            if (human != null)
            {
                human.Nurse();
                human.Speak();
            }
            else
                WriteLine("human is not Human");

            GasolineCar gasolineCar = new GasolineCar("소나타", "가솔린엔진");
            gasolineCar.drive();

            HybridCar hybridCar = new HybridCar("프리우스", "가솔린엔진,전기모터");
            hybridCar.drive();

            Configuration config = new Configuration();
            config.SetConfig("Version", "V5.0");
            config.SetConfig("Size", "655,324 KB");

            WriteLine(config.GetConfig("Version"));
            WriteLine(config.GetConfig("Size"));

            config.SetConfig("Version", "V5.1");
            WriteLine(config.GetConfig("Version"));

            AAA aaa = new AAA();
            aaa.Method1();
            aaa.Method2();
            aaa.Method3();
            aaa.Method4();

            WriteLine($"3^2 : {3.14f.Square()} ");
            WriteLine($"3^4 : {3.Power(4)}");
            WriteLine($"2^10 : {2.Power(10)}");

            Point3D p3d1;
            p3d1.x = 10;
            p3d1.y = 20;
            p3d1.z = 40;
            WriteLine(p3d1.ToString());

            Point3D p3d2 = new Point3D(100, 200, 300);
            Point3D p3d3 = p3d2;
            p3d3.z = 400;
            WriteLine(p3d2.ToString());
            WriteLine(p3d3.ToString());
        }

        static int Sum(params int[] args)   // 가변길이 매개 변수
        {
            int sum = 0;
            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    Write(", ");
                Write(args[i]);
                sum += args[i];
            }
            WriteLine();
            return sum;
        }
        static void PrintfProfile(string name, string phone = "") // 명명된 매개 변수
        {
            WriteLine($"Name : {name}, Phone : {phone}");
        }
        static string ToLowerString(string str) // 로컬함수
        {
            var arr = str.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = ToLowerChar(i);
            }
            char ToLowerChar(int i)
            {
                if (arr[i] < 65 || arr[i] > 90)
                    return arr[i];
                else
                    return (char)(arr[i] + 32);
            }
            return new string(arr);
        }
        enum ColorCode { RED, BLUE, GREEN, ORANGE }
        enum ColorCode2 { RED = 10, BLUE, GREEN, ORANGE = 100 }
        static void Main(string[] args)
        {
            //day1();
            //day2();

            var a = ("홍길동", 20);
            WriteLine($"{a.Item1}, {a.Item2}");

            var b = (Name: "이순신", Age: 40);
            WriteLine($"{b.Name}, {b.Age}");

            var (name, age) = b;
            WriteLine($"{name}, {age}");

            b = a;
            WriteLine($"{b.Name}, {b.Age}");

            AbstractBase obj = new Derived();
            obj.AbstractMethodA();
            obj.PublicMethodA();




            


        }
    }
}
