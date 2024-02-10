using System.Text;

namespace Lox;

public class AstPrinter : Expr.IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }
    
    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }
    
    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }
    
    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return (expr.Value == null ? "nil" : expr.Value.ToString())!;
    }
    
    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }

    private string Parenthesize(string lexeme, params Expr[] exprs)
    {
        var builder = new StringBuilder();
        builder.Append("(").Append(lexeme);
        exprs.ToList().ForEach(expr =>
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        });
        builder.Append(")");
        return builder.ToString();
    }
}