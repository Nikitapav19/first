using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dllPath = Path.Combine(currentDirectory, "FileSystemCommands.dll");

            Assembly assembly = Assembly.LoadFrom(dllPath);
            string targetFolder = currentDirectory; 

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    if (type.Name == "DirectorySizeCommand")
                    {
                        ICommand sizeCommand = (ICommand)Activator.CreateInstance(type, targetFolder);
                        sizeCommand.Execute();
                        
                        PropertyInfo sizeProp = type.GetProperty("TotalSize");
                        long size = (long)sizeProp.GetValue(sizeCommand);
                        Console.WriteLine($"Размер каталога: {size} байт.");
                    }
                    else if (type.Name == "FindFilesCommand")
                    {
                        ICommand findCommand = (ICommand)Activator.CreateInstance(type, targetFolder, "*.dll");
                        findCommand.Execute();
                        
                        PropertyInfo filesProp = type.GetProperty("FoundFiles");
                        var files = (System.Collections.IEnumerable)filesProp.GetValue(findCommand);
                        
                        Console.WriteLine("Найденные файлы DLL:");
                        foreach (var file in files)
                        {
                            Console.WriteLine(file);
                        }
                    }
                }
            }
        }
    }
}