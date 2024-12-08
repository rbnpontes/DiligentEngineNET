using System.Text;
using CppAst;

namespace CodeGenerator;

public class CppTypeUtils
{
    private static string BuildFunctionName(CppClass @class, CppFunction func)
    {
        var className = CodeUtils.ToSnakeCase(@class.Name);
        var baseFuncName = CodeUtils.ToSnakeCase(func.Name);
        var result = new StringBuilder();

        if (className.StartsWith("i"))
            className = className.Remove(0, 1);
        result.Append(className);
        result.Append('_');
        result.Append(baseFuncName);
        return result.ToString();
    }

    private static string BuildFunctionArgs(CppClass @class, CppFunction func)
    {
        var result = new StringBuilder();
        result.Append("(");
        result.Append(@class.Name + "*");
        result.Append(" _this");

        var parameters = func.Parameters.ToArray();
        if (parameters.Any())
            result.Append(", ");
        for (var i = 0; i < parameters.Length; ++i)
        {
            var funcParam = parameters[i];
            result.Append(GetFunctionArgType(funcParam.Type));
            result.Append(" arg");
            result.Append(i);
            if (i < parameters.Length - 1)
                result.Append(", ");
        }
        
        result.Append(")");
        return result.ToString();
    }

    public static string GetFunctionName(CppClass @class, CppFunction func)
    {
        return BuildFunctionName(@class, func);
    }
    
    public static string GetFunctionVariantDeclName(CppClass @class, CppFunction func, int idx)
    {
        var result = new StringBuilder();
        var funcName = BuildFunctionName(@class, func) + "_v" + idx;
        result.Append(BuildFunctionName(@class, func));
        result.Append("_v");
        result.Append(idx);
        result.Append(BuildFunctionArgs(@class, func));
        return result.ToString();
    }
    public static string GetFunctionDeclName(CppClass @class, CppFunction func)
    {
        var result = new StringBuilder();
        result.Append(BuildFunctionName(@class, func));
        result.Append(BuildFunctionArgs(@class, func));
        return result.ToString();
    }

    public static string GetFunctionArgType(CppType type)
    {
        var result = type.GetDisplayName();
        switch (type.TypeKind)
        {
            case CppTypeKind.Array:
            {
                var arrType = (CppArrayType)type;
                result = $"{arrType.ElementType.GetDisplayName()}*";
            }
                break;
            case CppTypeKind.StructOrClass:
                result += "*";
                break;
        }

        if (AstUtils.IsFunctionPointer(type))
            result = "void*";
        
        return result;
    }
    public static string GetFunctionReturnType(CppFunction func)
    {
        var currentType = func.ReturnType;
        var result = string.Empty;
        
        switch (currentType.TypeKind)
        {
            case CppTypeKind.Pointer:
            {
                var pointerType = func.ReturnType as CppPointerType;
                result = pointerType.ElementType.GetDisplayName() + "*";
            }
                break;
            case CppTypeKind.Function:
                result = "void*";
                break;
            case CppTypeKind.Array:
                throw new NotImplementedException();
                break;
            case CppTypeKind.Reference:
            {
                var refType = func.ReturnType as CppReferenceType;
                if (refType.ElementType is CppQualifiedType qualifiedType)
                    result = qualifiedType.ElementType.GetDisplayName() + "*";
                else
                    result = refType.ElementType.GetDisplayName() + "*";
            }
                break;
            case CppTypeKind.Primitive:
            case CppTypeKind.Enum:
            case CppTypeKind.Typedef:
            case CppTypeKind.StructOrClass:
                result = func.ReturnType.GetDisplayName();
                break;
            default:
                throw new NotImplementedException();
        }
        
        return result;
    }

    public static string GetBaseClassFileName(CppClass @class)
    {
        return $"class_{@class.Name}_binding";
    }

    public static bool CanBeGenerated(CppClass @class)
    {
        return @class.Name.StartsWith("I");
    }

    public static string GetFunctionArgAccess(CppParameter parameter, int argIdx)
    {
        var funcParamType = parameter.Type;
        var result = $"arg{argIdx}";
        switch (funcParamType.TypeKind)
        {
            case CppTypeKind.StructOrClass:
                result = '*' + result;
                break;
        }

        if (AstUtils.IsFunctionPointer(funcParamType))
            result = $"reinterpret_cast<{funcParamType.GetDisplayName()}>({result})";
        
        return result;
    }
    
    public static string CreateFunctionCall(CppClass @class, CppFunction func)
    {
        StringBuilder result = new();
        result.Append("_this->");
        result.Append(func.Name);
        result.Append("(");
        for (var i = 0; i < func.Parameters.Count; ++i)
        {
            var funcParam = func.Parameters[i];
            result.Append(GetFunctionArgAccess(funcParam, i));
            if (i < func.Parameters.Count - 1)
                result.Append(", ");
        }
        result.Append(")");
        return result.ToString();
    }

    public static string HandleFunctionReturn(CppClass @class, CppFunction func)
    {
        var resultCall = CreateFunctionCall(@class, func);
        var returnType = func.ReturnType;
        var typeKind = returnType.TypeKind;
        var result = resultCall;

        if (func.ReturnType.GetDisplayName() == "void")
            return result + ";";
        
        switch (typeKind)
        {    
            case CppTypeKind.Typedef:
            case CppTypeKind.Pointer:
            case CppTypeKind.Enum:
            case CppTypeKind.Primitive:
            case CppTypeKind.StructOrClass:
                result = $"return {result};";
                break;
            case CppTypeKind.Reference:
            {
                var refType = (CppReferenceType)returnType;
                if (refType.ElementType is CppQualifiedType qualifiedType)
                {
                    if (qualifiedType.Qualifier == CppTypeQualifier.Const)
                        result = $"const_cast<{qualifiedType.ElementType.GetDisplayName()}*>(&{result})";
                    else
                        throw new NotImplementedException();
                }
                else
                    result = $"&{result}";

                result = $"return {result};";
            }
                break;
            default:
                throw new NotImplementedException();
        }

        return result;
    }
}