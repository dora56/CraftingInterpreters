namespace Lox;

public class Token(TokenType type, string lexeme, object? literal, int line)
{
    public TokenType Type { get; init; } = type;
    public string Lexeme { get; init; } = lexeme;
    public object? Literal { get; init; } = literal;
    public int Line { get; init; } = line;

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}