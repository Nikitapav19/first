using System;
using System.Reflection;

namespace MetadataScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Передайте путь к dll в аргументы");
                return;
            }
            Assembly assembly = Assembly.LoadFrom(args[0]);

            foreach (Type type in assembly.GetTypes())
            {
                Console.WriteLine("Класс: " + type.FullName);

                foreach (var attr in type.GetCustomAttributes())
                {
                    Console.WriteLine("Атрибут: " + attr.GetType().Name);
                }

                foreach (ConstructorInfo ctor in type.GetConstructors())
                {
                    Console.Write("Конструктор: " + type.Name + "(");
                    PrintParameters(ctor.GetParameters());
                    Console.WriteLine(")");
                }

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (MethodInfo method in methods)
                {
                    Console.Write("Метод: " + method.ReturnType.Name + " " + method.Name + "(");
                    PrintParameters(method.GetParameters());
                    Console.WriteLine(")");
                }
                
                Console.WriteLine();
            }
        }

        static void PrintParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write(parameters[i].ParameterType.Name + " " + parameters[i].Name);
                if (i < parameters.Length - 1) Console.Write(", ");
            }
        }
    }
}