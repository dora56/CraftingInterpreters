using System.Linq.Expressions;
using System.Text;

namespace Lox;

public static class Lox
{
    public static void Main(string[] args)
    {
        switch (args.Length)
        {
            case > 1:
                Console.WriteLine("Usage: dotnet run [script]");
                Environment.Exit(64);
                break;
            case 1:
                RunFile(args[0]);
                break;
            default:
                RunPrompt();
                break;
        }
    }
    
    private static void RunFile(string path)
    {
        var source = File.ReadAllText(Path.GetFullPath(path), Encoding.UTF8);
        Run(source);
        if (HadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
            Run(line);
            HadError = false;
        }
    }

    private static void Run(String source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expression = parser.Parse();
        
        if (HadError)
        {
            return;
        }
   
        Console.WriteLine(new AstPrinter().Print(expression));
    }

    private static bool HadError { get; set; }
    
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        // if (token.Type == TokenType.EOF)
        // {
        //     Report(token.Line, " at end", message);
        // }
        // else
        // {
        //     Report(token.Line, " at '" + token.Lexeme + "'", message);
        // }
        Report(token.Line, " at '" + token.Lexeme + "'", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
    
}