namespace Lox;

public abstract class Expr
{
    public interface IVisitor<out T>
    {
        T VisitBinaryExpr(Binary expr);
        T VisitGroupingExpr(Grouping expr);
        T VisitLiteralExpr(Literal expr);
        T VisitUnaryExpr(Unary expr);
    }
    public sealed class Binary(Expr left, Token @operator, Expr right) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }

        public Expr Left { get; internal set; } = left;
        public Token Operator { get; internal set; } = @operator;
        public Expr Right { get; internal set; } = right;
    }
    public sealed class Grouping(Expr expression) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }

        public Expr Expression { get; internal set; } = expression;
    }
    public sealed class Literal(object value) : Expr
    {
        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }

        public object? Value { get; internal set; } = value;
    }
    public sealed class Unary : Expr
    {
        public Unary(Token @operator, Expr right)
        {
            Operator = @operator;
            Right = right;
        }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public Token Operator { get; internal set; }
        public Expr Right { get; internal set; }
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
}
