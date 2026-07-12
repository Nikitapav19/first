using System;

namespace CommandLib
{
    public interface ICommand
    {
        void Execute();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandInfoAttribute : Attribute
    {
        public string Description { get; }
        public string Author { get; }

        public CommandInfoAttribute(string description, string author)
        {
            Description = description;
            Author = author;
        }
    }
}