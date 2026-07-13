using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public class CalculatorGenerator
    {
        public static ICalculator CreateCalculator()
        {
            string code = @"
            using task11;
            public class Calculator : ICalculator
            {
                public int Add(int a, int b) => a + b;
                public int Minus(int a, int b) => a - b;
                public int Mul(int a, int b) => a * b;
                public int Div(int a, int b) => a / b;
            }";

            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToList();

            var compilation = CSharpCompilation.Create("DynamicCalcAssembly",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            Type calcType = assembly.GetType("Calculator");

            return (ICalculator)Activator.CreateInstance(calcType);
        }
    }
}