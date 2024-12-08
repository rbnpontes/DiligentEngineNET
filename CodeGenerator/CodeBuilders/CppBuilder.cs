using System.Text;

namespace CodeGenerator.CodeBuilders;

public class FunctionDesc
{
    public string Name { get; set; } = string.Empty;
    public string ReturnType { get; set; } = "void";
    public string[] Arguments { get; set; } = [];
}

public class CppBuilder(int indentation = 0)
{
    private CodeBlockChunk _chunk = new(indentation);
    //private List<string> _headers = new();
    private string _topComment = string.Empty;
    private bool _pragmaOnce;

    public CppBuilder SetPragmaOnce()
    {
        _pragmaOnce = true;
        return this;
    }

    public CppBuilder SetTopComment(string comment)
    {
        _topComment = comment;
        return this;
    }

    public CppBuilder Include(string includeName)
    {
        return Line($"#include {includeName}");
    }

    public CppBuilder IncludeLiteral(string includeName)
    {
        return Include($"\"{includeName}\"");
    }

    public CppBuilder Namespace(Action<CppBuilder> namespaceBody, string ns)
    {
        _chunk.Add($"namespace {ns}");
        return Closure(namespaceBody);
    }

    public CppBuilder Comment(string comment)
    {
        return Line("//" + comment);
    }

    public CppBuilder Line()
    {
        return Line(string.Empty);
    }

    public CppBuilder Line(string line)
    {
        _chunk.Add(line);
        return this;
    }

    private string GetMethodDeclaration(FunctionDesc functionDesc)
    {
        StringBuilder result = new();
        result.Append(functionDesc.ReturnType);
        result.Append(' ');
        result.Append(functionDesc.Name);
        result.Append('(');
        for (var i = 0; i < functionDesc.Arguments.Length; ++i)
        {
            result.Append(functionDesc.Arguments[i]);
            if (i < functionDesc.Arguments.Length - 1)
                result.Append(", ");
        }

        result.Append(')');

        return result.ToString();
    }

    public CppBuilder MethodDecl(FunctionDesc functionDesc)
    {
        _chunk.Add($"{GetMethodDeclaration(functionDesc)};");
        return this;
    }

    public CppBuilder Method(Action<CppBuilder> methodBody, FunctionDesc functionDesc)
    {
        _chunk.Add($"{GetMethodDeclaration(functionDesc)}");
        Closure(methodBody);
        Line();
        return this;
    }

    public CppBuilder MethodCall(string name, string[] args)
    {
        StringBuilder sb = new();
        sb.Append(name);
        sb.Append('(');
        for (var i = 0; i < args.Length; ++i)
        {
            sb.Append(args[i]);
            if (i < args.Length - 1)
                sb.Append(", ");
        }

        sb.Append(");");
        return Line(sb.ToString());
    }

    public CppBuilder Closure(Action<CppBuilder> closureBody)
    {
        var closure = new CppBuilder(_chunk.Indentation + 1);
        closureBody(closure);

        _chunk.Add("{");
        _chunk.Add(closure._chunk);
        _chunk.Add("}");
        return this;
    }

    public CppBuilder IfDef(Action<CppBuilder> closureBody, string[] macros)
    {
        var macrosDef = new StringBuilder();
        for (var i = 0; i < macros.Length; ++i)
        {
            macrosDef.Append("defined(");
            macrosDef.Append(macros[i]);
            macrosDef.Append(')');
            if (i < macros.Length - 1)
                macrosDef.Append(" || ");
        }

        Line($"#if {macrosDef}");
        closureBody(this);
        Line("#endif");
        return this;
    }

    public CppBuilder Var(string varType, string varName)
    {
        _chunk.Add($"{varType} {varName};");
        return this;
    }

    public CppBuilder Var(string varType, string varName, string varValue)
    {
        _chunk.Add($"{varType} {varName} = {varValue};");
        return this;
    }

    public CppBuilder Return()
    {
        _chunk.Add("return;");
        return this;
    }

    public CppBuilder Return(string resultValue)
    {
        _chunk.Add($"return {resultValue};");
        return this;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        if (!string.IsNullOrEmpty(_topComment))
        {
            sb.AppendLine("/*");
            sb.AppendLine(_topComment);
            sb.AppendLine("*/");
        }

        if (_pragmaOnce)
            sb.AppendLine("#pragma once");
        sb.AppendLine();

        sb.AppendLine(_chunk.GetString());
        return sb.ToString();
    }

    protected static void Assign(CppBuilder from, CppBuilder to)
    {
        to._chunk = from._chunk;
        to._topComment = from._topComment;
        to._pragmaOnce = from._pragmaOnce;
    }
}