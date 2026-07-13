using System;
using CommandLib;

namespace FileSystemCommands
{
    [PluginLoad]
    public class FindFilesCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("[FindFiles] выполнена.");
        }
    }
}