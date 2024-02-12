namespace Lox;

public class Parser(IReadOnlyList<Token> tokens)
{
    public sealed class ParseException : Exception;

    private int _current;

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseException error)
        {
            return null;
        }
    }
    
    private Expr Expression()
    {
        return Equality();
    }

    private Expr Equality()
    {
        var expr = Comparison();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var @operator = Previous();
            var right = Comparison();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private bool Match(params TokenType[] types)
    {
        if (!Array.Exists(types, Check)) return false;
        Advance();
        return true;

    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private void Advance()
    {
        if(!IsAtEnd()) _current++;
        Previous();
    }

    private Token Previous()
    {
        return tokens[_current - 1];
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }
    
    private Token Peek()
    {
        return tokens[_current];
    }
    
    private Expr Comparison()
    {
        var expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var @operator = Previous();
            var right = Term();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private Expr Term()
    {
        var expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            var @operator = Previous();
            var right = Factor();
            expr = new Expr.Binary(expr, @operator, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        var expr = Unary();
        if (!Match(TokenType.SLASH, TokenType.STAR)) return expr;
        var @operator = Previous();
        var right = Unary();
        return new Expr.Binary(expr, @operator, right);
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            var @operator = Previous();
            var right = Unary();
            return new Expr.Unary(@operator, right);
        }
        return Primary();
    }

    private Expr Primary()
    {
        if(Match(TokenType.FALSE)) return new Expr.Literal(false);
        if(Match(TokenType.TRUE)) return new Expr.Literal(true);
        if (Match(TokenType.NIL)) return new Expr.Literal(null);

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal!);
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }
        throw Error(Peek(), "Expect expression.");
    }
    
    private void Consume(TokenType type, string message)
    {
        if (!Check(type)) throw Error(Peek(), message);
        Advance();
    }

    private ParseException Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseException();
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;
            switch (Peek().Type)
            {
                case TokenType.CLASS
                    or TokenType.FOR
                    or TokenType.FUN 
                    or TokenType.IF
                    or TokenType.PRINT
                    or TokenType.RETURN
                    or TokenType.VAR
                    or TokenType.WHILE:
                    return;
            }

            Advance();
        }
    }
    
}