namespace Lox;

public abstract class Expr
{
}

public class Binary(Expr left, Token op, Expr right) : Expr
{
    public Expr Left { get; } = left;
    public Token Op { get; } = op;
    public Expr Right { get; } = right;
}
