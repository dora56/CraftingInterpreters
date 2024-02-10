namespace Lox;

public class Token(TokenType type, string lexeme, object? literal, int line)
{
    private TokenType Type { get; init; } = type;
    public string Lexeme { get; init; } = lexeme;
    object? Literal { get; init; } = literal;
    public int Line { get; init; } = line;

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}