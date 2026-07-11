using System;
using System.Reflection;

namespace task07
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }

        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }

    [DisplayName("Пример класса")]
    [Version(1, 0)]
    public class SampleClass
    {
        [DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [DisplayName("Тестовый метод")]
        public void TestMethod()
        {
        }
    }

    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            var classDisplayAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (classDisplayAttr != null)
            {
                Console.WriteLine($"Отображаемое имя класса: {classDisplayAttr.DisplayName}");
            }

            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr != null)
            {
                Console.WriteLine($"Версия: {versionAttr.Major}.{versionAttr.Minor}");
            }

            foreach (var el in type.GetProperties())
            {
                var elAttr = el.GetCustomAttribute<DisplayNameAttribute>();
                if (elAttr != null)
                {
                    Console.WriteLine($"Свойство {el.Name}: {elAttr.DisplayName}");
                }
            }

            foreach (var method in type.GetMethods())
            {
                var methodAttr = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodAttr != null)
                {
                    Console.WriteLine($"Метод {method.Name}: {methodAttr.DisplayName}");
                }
            }
        }
    }
}