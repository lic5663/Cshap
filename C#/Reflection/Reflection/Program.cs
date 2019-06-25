using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Reflection.Emit;

namespace Reflection
{
    class Program
    {
        static void PrintInterfaces (Type type)
        {
            WriteLine("---- Interfaces ----");

            Type[] interfaces = type.GetInterfaces();
            foreach(Type i in interfaces)
                WriteLine("Name : {0}", i.Name);

            WriteLine();
            
        }

        static void PrintFields(Type type)
        {
            WriteLine("---- Fields ----");

            FieldInfo[] fields = type.GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Public |
                BindingFlags.Static |
                BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                String accessLevel = "protected";
                if (field.IsPublic)
                    accessLevel = "public";
                else if (field.IsPrivate)
                    accessLevel = "private";

                WriteLine("Access:{0}, Type:{0}, Name:{2}", accessLevel, field.FieldType.Name, field.Name);
            }

            WriteLine();
        }

        static void PrintMethods(Type type)
        {
            WriteLine("---- Methods ----");
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                Write("Type : {0}, Name : {1}, Parameter", method.ReturnType.Name, method.Name);

                ParameterInfo[] args = method.GetParameters();
                for (int i=0; i<args.Length;i++)
                {
                    Write("{0}", args[i].ParameterType.Name);
                    if (i < args.Length - 1)
                        Write(", ");
                }
                WriteLine();
            }
            WriteLine();
        }

        static void PrintProperties(Type type)
        {
            WriteLine("---- Properties ----");
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
                WriteLine("Type:{0}, Name:{1}", property.PropertyType.Name, property.Name);

            WriteLine();
        }

        static void Main(string[] args)
        {
            int a = 0;

            Type type = a.GetType();

            PrintInterfaces(type);
            PrintFields(type);
            PrintProperties(type);
            PrintMethods(type);

            AssemblyBuilder newAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("CalculatorAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder newModule = newAssembly.DefineDynamicModule("Calculator");
            TypeBuilder newType = newModule.DefineType("Sum1To100");

            MethodBuilder newMethod = newType.DefineMethod(
                "Calculate",
                MethodAttributes.Public,
                typeof(int),
                new Type[0]);

            ILGenerator generator = newMethod.GetILGenerator();

            generator.Emit(OpCodes.Ldc_I4, 1);
            for(int i = 2; i <= 100; i++)
            {
                generator.Emit(OpCodes.Ldc_I4, i);
                generator.Emit(OpCodes.Add);
            }
            generator.Emit(OpCodes.Ret);
            newType.CreateType();

            object sum1To100 = Activator.CreateInstance(newType);
            MethodInfo Calculate = sum1To100.GetType().GetMethod("Calculate");
            WriteLine(Calculate.Invoke(sum1To100, null));
        }
    }
}
