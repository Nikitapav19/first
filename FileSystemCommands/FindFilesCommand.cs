using System.Collections.Generic;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    [CommandInfo("Ищет файлы по заданной маске", "Твое Имя")]
    public class FindFilesCommand : ICommand
    {
        private string _directoryPath;
        private string _searchPattern;
        
        public List<string> FoundFiles { get; private set; }

        public FindFilesCommand(string directoryPath, string searchPattern)
        {
            _directoryPath = directoryPath;
            _searchPattern = searchPattern;
            FoundFiles = new List<string>();
        }

        public void Execute()
        {
            FoundFiles.Clear();
            string[] files = Directory.GetFiles(_directoryPath, _searchPattern, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FoundFiles.Add(file);
            }
        }
    }
}