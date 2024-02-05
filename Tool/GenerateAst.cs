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
                "Unary    : Token operator, Expr Right"
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
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        types.ToList().ForEach(type =>
        {
            var className = type.Split(":")[0].Trim();
            var fields = type.Split(":")[1].Trim();
            DefineType(writer, baseName, className, fields);
        });
        writer.WriteLine("}");

    }

    private static void DefineType(
        TextWriter writer,
        string baseName,
        string className,
        string fieldList)
    {
        writer.WriteLine("    public class " + className + " : " + baseName);
        writer.WriteLine("    {");
        writer.WriteLine("        public " + className + "(" + fieldList + ")");
        
        var fields = fieldList.Split(", ").ToList();
        fields.ForEach(field =>
        {
            var name = field.Split(" ")[1];
            writer.WriteLine("            " + field + ")");
        });
        
        writer.WriteLine("        }");
        
        fields.ForEach(field =>
        {
            var name = field.Split(" ")[1];
            writer.WriteLine("        public " + field + " { get; }");
        });
        
        writer.WriteLine("    }");
        
        writer.WriteLine();
        fields.ForEach(field =>
            writer.WriteLine($"        public {field} {{ get; internal set; }}"));
        writer.WriteLine("    }");
    }
}