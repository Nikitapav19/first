using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace PluginSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Передайте путь к папке с плагинами через аргументы.");
                return;
            }

            string folderPath = args[0];
            var pluginTypes = new List<Type>();

            foreach (string file in Directory.GetFiles(folderPath, "*.dll"))
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(file);
                    foreach (Type type in asm.GetTypes())
                    {
                        if (typeof(ICommand).IsAssignableFrom(type) && type.GetCustomAttribute<PluginLoadAttribute>() != null)
                        {
                            pluginTypes.Add(type);
                        }
                    }
                }
                catch { } // Это для игнора файлов, которые не являются .NET библиотеками
            }

            var sortedPlugins = SortPlugins(pluginTypes);

            foreach (Type type in sortedPlugins)
            {
                ICommand plugin = (ICommand)Activator.CreateInstance(type);
                plugin.Execute();
            }
        }

        static List<Type> SortPlugins(List<Type> plugins)
        {
            var sorted = new List<Type>();
            var visited = new HashSet<Type>();
            var visiting = new HashSet<Type>();

            void Visit(Type type)
            {
                if (visited.Contains(type)) return;

                visiting.Add(type);

                var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                if (attr != null && attr.Dependencies != null)
                {
                    foreach (string depName in attr.Dependencies)
                    {
                        var depType = plugins.FirstOrDefault(p => p.Name == depName);
                        if (depType != null) Visit(depType);
                    }
                }

                visiting.Remove(type);
                visited.Add(type);
                sorted.Add(type);
            }

            foreach (var plugin in plugins) Visit(plugin);

            return sorted;
        }
    }
}