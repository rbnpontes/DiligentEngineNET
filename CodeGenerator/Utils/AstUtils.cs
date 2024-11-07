using System.Collections.Immutable;
using CppAst;

namespace CodeGenerator;

public static class AstUtils
{
    public static CppType Resolve(CppType type)
    {
        if (type is CppTypedef typeDef)
            return ResolveTypeDef(typeDef);
        return type;
    }
    public static CppType ResolveTypeDef(CppTypedef typeDef)
    {
        CppType result = typeDef.ElementType;
        if (result.TypeKind == CppTypeKind.Typedef)
            result = ResolveTypeDef((CppTypedef)result);
        return result;
    }

    public static bool IsMultiDimensionalArray(CppType type)
    {
        if (type is not CppArrayType arrayType)
            return false;
        return arrayType.ElementType is CppArrayType;
    }

    public static CppType GetMultiDimensionalArrayType(CppType type)
    {
        return ((CppArrayType)((CppArrayType)type).ElementType).ElementType;
    }
    public static bool IsOperatorFunction(CppFunction func)
    {
        return func.Name.Contains("operator");
    }

    public static bool IsFunctionPointer(CppType type)
    {
        if (type.TypeKind == CppTypeKind.Typedef)
            type = ResolveTypeDef((CppTypedef)type);
        if (type.TypeKind != CppTypeKind.Pointer)
            return false;
        
        var pointerType = (CppPointerType)type;
        return pointerType.ElementType.TypeKind == CppTypeKind.Function;
    }

    public static bool IsAllowedClass(CppClass @class)
    {
        return !ExclusionList.Classes.Contains(@class.Name);
    }
    
    public static bool IsAllowedFunction(CppFunction function)
    {
        if (function.IsFunctionTemplate)
            return false;
        return !ExclusionList.Methods.Contains(function.Name);
    }

    public static bool IsArrayType(CppType type)
    {
        return type is CppArrayType;
    }

    public static bool IsEnumType(CppType type)
    {
        return type is CppEnum;
    }
    
    public static CppPrimitiveType? GetPrimitiveType(CppType type)
    {
        type = Resolve(type);
        return type as CppPrimitiveType;
    }
    public static bool IsStringType(CppType type)
    {
        if (!IsArrayType(type))
            return false;
        var arrayType = (CppArrayType)type;
        var primitiveType = GetPrimitiveType(arrayType.ElementType);
        if (primitiveType is null)
            return false;
        return primitiveType.Kind == CppPrimitiveKind.Char;
    }

    public static bool HasClassFields(CppClass @class)
    {
        var result = false;
        var currentClass = @class;
        while (currentClass is not null)
        {
            result = currentClass.Fields.Count > 0;
            if (result)
                break;
            currentClass = currentClass.BaseTypes.FirstOrDefault()?.Type as CppClass;
        }
        
        return result;
    }

    public static bool HasBaseClass(CppClass @class)
    {
        return @class.BaseTypes.Any();
    }

    public static CppClass GetClassParent(CppClass @class)
    {
        return (CppClass)@class.BaseTypes[0].Type;
    }
    
    public static IImmutableList<CppField> GetAllClassFields(CppClass @class)
    {
        var fields = new List<CppField>();
        var currentClass = @class;
        while (currentClass is not null)
        {
            fields.AddRange(currentClass.Fields);
            currentClass = currentClass.BaseTypes.FirstOrDefault()?.Type as CppClass;
        }

        return fields.OrderBy(x => x.Offset).ToImmutableList();
    }
}