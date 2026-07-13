using System;
using CommandLib;

namespace FileSystemCommands
{
    [PluginLoad("FindFilesCommand")]
    public class DirectorySizeCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("[DirectorySize] выполнена.");
        }
    }
}