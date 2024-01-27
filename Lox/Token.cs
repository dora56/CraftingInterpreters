namespace Lox;

public class Token
{
    private TokenType type { get; init; }
    string lexeme { get; init; }
    Object? literal { get; init; }
    int line { get; init; }
    
    public Token(TokenType type, String lexeme, Object? literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }
    
    public override string ToString()
    {
        return $"{type} {lexeme} {literal}";
    }
}