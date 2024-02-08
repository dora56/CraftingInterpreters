using System.Text;

namespace Tool;

public static class GenerateAst
{
    public static void Main(string[] args)
    {
        if(args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generate_ast <output directory>");
            Environment.Exit(64);
        }
        var outputDir = args[0];
        DefineAst(
            outputDir,
            "Expr",
            [
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : Object Value",
                "Unary    : Token Operator, Expr Right"
            ]);
    }

    private static void DefineAst(
        string outputDir,
        string baseName,
        IEnumerable<string> types)
    {
        var path = $"{outputDir}/{baseName}.cs";
        using var writer = new StreamWriter(path, false);
        writer.WriteLine("namespace Lox;");
        writer.WriteLine();
        writer.WriteLine("using System;");
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine();
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        
        DefineVisitor(writer, baseName, types);
        
        // AST classes
        types.ToList().ForEach(type =>
        {
            var className = type.Split(":")[0].Trim();
            var fields = type.Split(":")[1].Trim();
            DefineType(writer, baseName, className, fields);
        });
        
        // The base accept() method
        writer.WriteLine();
        writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
        writer.WriteLine("}");

    }

    private static void DefineVisitor(StreamWriter writer, string baseName, IEnumerable<string> types)
    {
        writer.WriteLine("    public interface IVisitor<T>");
        writer.WriteLine("    {");
        types.ToList().ForEach(type =>
        {
            var typeName = type.Split(":")[0].Trim();
            writer.WriteLine($"        T Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        });
        writer.WriteLine("    }");
    }

    private static void DefineType(
        TextWriter writer,
        string baseName,
        string className,
        string fieldList)
    {
        writer.WriteLine("    public class " + className + " : " + baseName);
        writer.WriteLine("    {");
        
        // Constructor
        writer.WriteLine("        public " + className + "(" + fieldList + ")");
        writer.WriteLine("        {");
        // Store parameters in fields
        var fields = fieldList.Split(", ").ToList();
        fields.ForEach(field =>
        {
            var name = field.Split(" ")[1];
            writer.WriteLine("            " + name + " = " + name + ";");
        });
        
        writer.WriteLine("        }");
        
        // Visitor pattern
        writer.WriteLine();
        writer.WriteLine("        public override T Accept<T>(IVisitor<T> visitor)");
        writer.WriteLine("        {");
        writer.WriteLine($"            return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("        }");
        
        // fields
        writer.WriteLine();
        fields.ForEach(field =>
            writer.WriteLine($"        public {field} {{ get; internal set; }}"));
        
        writer.WriteLine("    }");
    }
}