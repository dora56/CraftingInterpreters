namespace Lox;

public class Scanner(string source)
{
    private readonly List<Token> _tokens = new();
    private int _start;
    private int _current;
    private int _line = 1;

    private bool IsAtEnd()
    {
        return _current >= source.Length;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();

        switch (c)
        {
            case '(':
                AddToken(TokenType.LEFT_PAREN);
                break;
            case ')':
                AddToken(TokenType.RIGHT_PAREN);
                break;
            case '{':
                AddToken(TokenType.LEFT_BRACE);
                break;
            case '}':
                AddToken(TokenType.RIGHT_BRACE);
                break;
            case ',':
                AddToken(TokenType.COMMA);
                break;
            case '.':
                AddToken(TokenType.DOT);
                break;
            case '-':
                AddToken(TokenType.MINUS);
                break;
            case '+':
                AddToken(TokenType.PLUS);
                break;
            case ';':
                AddToken(TokenType.SEMICOLON);
                break;
            case '*':
                AddToken(TokenType.STAR);
                break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd())
                    {
                        Advance();
                    }
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }

                break;
            case ' ':
            case '\r':
            case '\t':
                break;
            case '"':
                String();
                break;
            case 'o':
                if (Match('r'))
                {
                    AddToken(TokenType.OR);
                }

                break;
            default:
                if (IsDigit(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    Lox.Error(_line, "Unexpected character.");
                }

                break;
        }
    }

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS},
        { "else", TokenType.ELSE},
        { "false", TokenType.FALSE},
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN},
        { "if", TokenType.IF },
        { "nil", TokenType.NIL},
        { "or", TokenType.OR},
        { "print", TokenType.PRINT},
        { "return", TokenType.RETURN},
        { "super", TokenType.SUPER},
        { "this", TokenType.THIS},
        { "true", TokenType.TRUE},
        { "var", TokenType.VAR},
        { "while", TokenType.WHILE}
    };


    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();
        
        var text = source.Substring(_start, _current - _start);
        var type = Keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);
        AddToken(type);
    }

    private bool IsAlpha(char c)
    {
        return c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
    }

    private bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }
    private bool IsDigit(char c)
    {
        return c is >= '0' and <= '9';
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }
        
        AddToken(TokenType.NUMBER, double.Parse(source.Substring(_start, _current - _start)));
    }
    
    private char PeekNext()
    {
        if (_current + 1 >= source.Length)
        {
            return '\0';
        }

        return source[_current + 1];
    }
    
    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Unterminated string.");
            return;
        }

        Advance();
        
        var value = source.Substring(_start + 1, _current - _start - 2);
        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : source[_current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }
        if (source[_current] != expected)
        {
            return false;
        }

        _current++;
        return true;
    }
    private char Advance()
    {
        return source[_current++];
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        var text = source.Substring(_start, _current);
        _tokens.Add(new Token(type, text, literal, _line));
    }
}