using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    [CommandInfo("Вычисляет общий размер всех файлов в каталоге", "Твое Имя")]
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _directoryPath;
        
        public long TotalSize { get; private set; }

        public DirectorySizeCommand(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public void Execute()
        {
            TotalSize = 0;
            string[] files = Directory.GetFiles(_directoryPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                TotalSize += info.Length;
            }
        }
    }
}