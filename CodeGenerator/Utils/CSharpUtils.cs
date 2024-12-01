using System.Text;
using System.Text.RegularExpressions;
using CppAst;

namespace CodeGenerator;

public static class CSharpUtils
{
    private const string InterfacePattern = @"^I[A-Z][a-zA-Z0-9].*";
    public static bool IsConstructable(CppClass @class)
    {
        // On diligent, types that starts with 'I'. Ex: IDeviceContext
        // cannot be constructed, they can be created by the unmanaged code.
        return !Regex.IsMatch(@class.Name, InterfacePattern) || CodeUtils.IsScreamingCase(@class.Name);
    }

    public static string GetFixedClassName(CppClass @class)
    {
        var result = @class.Name;
        if (@class.Name == "IObject")
            result = "DiligentObject";
        else if (!IsConstructable(@class))
            result = result.Remove(0, 1);
        return result;
    }

    public static string GetFixedEnumName(CppEnum @enum)
    {
        return CodeUtils.ConvertScreamingToPascalCase(@enum.Name);
    }

    public static string GetFixedEnumItemName(CppEnumItem enumItem)
    {
        var @enum = (CppEnum)enumItem.Parent;
        var name = CodeUtils.ConvertScreamingToPascalCase(enumItem.Name);
        var originalName = name;

        if (Remapper.EnumItemRemap.TryGetValue(name, out var remappedName))
            return remappedName;
        
        // Diligent Engine uses C enum style instead of new enum class style
        // so we will fix these names

        var enumNameCases = @enum.Name.Split('_');
        foreach (var enumNameCase in enumNameCases)
        {
            int stripIdx = 0;
            for (stripIdx = 0; stripIdx < Math.Min(enumNameCase.Length, name.Length); ++stripIdx)
            {
                if(char.ToLower(enumNameCase[stripIdx]) != char.ToLower(name[stripIdx]))
                    break;
            }
            name = name.Substring(stripIdx);
        }

        if (string.IsNullOrEmpty(name))
            name = originalName;
        
        // if strip generates an invalid enum
        // like the first digit is a number
        // we place 'N' as prefix.
        if (char.IsDigit(name.First()))
            name = 'N' + name;
        return name;
    }

    public static string GetEnumTypeSize(CppEnum @enum)
    {
        var type = GetUnmanagedType(@enum.IntegerType);
        if (type == "char")
            return "sbyte";
        return type;
    }
    public static string GetEnumItemValue(CppEnumItem enumItem)
    {
        var type = GetUnmanagedType(((CppEnum)enumItem.Parent).IntegerType);
        var result = enumItem.Value.ToString();
        if (type == "byte")
            result = ((byte)enumItem.Value).ToString();
        else if (type == "uint")
            result = ((uint)enumItem.Value).ToString();
        else if (type == "ushort")
            result = ((ushort)enumItem.Value).ToString();
        else if (type == "ulong")
            result = ((ulong)enumItem.Value).ToString();
        return result;
    }

    public static string FixPropertyName(string propName)
    {
        if (string.IsNullOrWhiteSpace(propName))
            return propName;

        var firstChar = propName.First();
        if (!char.IsLower(firstChar))
            return propName;
        return char.ToUpper(firstChar) + propName.Substring(1);
    }

    public static string GetPropertyField(CppType type, string propName)
    {
        var result = new StringBuilder();
        type = AstUtils.Resolve(type);
        
        if (type is CppPrimitiveType or CppEnum)
        {
            result.Append("public ");
            result.Append(GetPropertyType(type));
            result.Append(' ');
            result.Append(FixPropertyName(propName));
            result.Append(" { get => _data.");
            result.Append(propName);
            result.Append("; set => _data.");
            result.Append(propName);
            result.Append(" = value; }");
        } 
        else if (type is CppClass classType)
        {
            var className = GetFixedClassName(classType);
            result.Append("public ");
            result.Append(className);
            result.Append(' ');
            result.Append(FixPropertyName(propName));
            result.Append(" { get => ");
            result.Append(className);
            result.Append(".FromInternalStruct(_data.");
            result.Append(propName);
            result.Append("); set => _data.");
            result.Append(propName);
            result.Append(" = ");
            result.Append(className);
            result.Append(".GetInternalStruct(value); }");
        }
        else if (AstUtils.IsVoidPointer(type))
        {
            // void pointer props usually has 'p' prefix
            var fixedPropName = propName;
            if (fixedPropName.StartsWith('p'))
                fixedPropName = fixedPropName.Substring(1);
            result.Append(
                $"public IntPtr {fixedPropName} {{ get => _data.{propName}; set => _data.{propName} = value; }}");
        }
        else if (AstUtils.IsStringPointer(type))
        {
            result.Append("public string ");
            result.Append(propName);
            result.Append(" { get; set; } = string.Empty;");
        }
        return result.ToString();
    }
    
    public static string GetStructField(CppType type, string fieldName)
    {
        var result = new StringBuilder();
        result.Append("public ");
        if (type is CppArrayType arrType)
        {
            var size = arrType.Size;
            type = arrType.ElementType;
            // if multi-dimensional array ?
            if (type is CppArrayType childArrType)
            {
                size *= childArrType.Size;
                type = childArrType.ElementType;
            }
            
            if (type is CppEnum enumType)
                type = enumType.IntegerType;
            
            result.Append("fixed ");
            result.Append(GetStructType(type));
            result.Append(' ');
            result.Append(fieldName);
            result.Append('[');
            result.Append(size);
            result.Append("];");
            return result.ToString();
        }

        if (type is CppTypedef typeDef)
            type = AstUtils.ResolveTypeDef(typeDef);
        
        result.Append(GetStructType(type));
        result.Append(' ');
        result.Append(fieldName);
        result.Append(';');
        return result.ToString();
    }
    public static string GetStructField(CppField field)
    {
        return GetStructField(field.Type, field.Name);
    }

    public static string GetStructType(CppType type)
    {
        if (type.TypeKind == CppTypeKind.StructOrClass)
        {
            var classType = (CppClass)type;
            return GetFixedClassName(classType) + ".__Internal";
        }

        return GetUnmanagedType(type);
    }

    public static string GetPropertyType(CppType type)
    {
        return GetStructType(type);
    }
    
    public static bool RequiresSpecialSetStructMethod(CppField field)
    {
        if (field.Type is not CppArrayType arrType)
            return false;

        if (AstUtils.IsMultiDimensionalArray(field.Type))
            return false;
        
        var targetType = AstUtils.Resolve(arrType.ElementType);
        if (targetType.TypeKind is CppTypeKind.Primitive or CppTypeKind.Enum)
            return false;
        return true;
    }
    
    public static bool IsUnmanagedSpecialTypeRequiresAttr(CppType type)
    {
        if (type.TypeKind == CppTypeKind.Typedef)
            type = AstUtils.ResolveTypeDef((CppTypedef)type);
        
        if (Equals(type, CppPrimitiveType.Bool))
            return true;
        return false;
    }
    public static string GetUnmanagedSpecialAttribute(CppType type)
    {
        if (type.TypeKind == CppTypeKind.Typedef)
            type = AstUtils.ResolveTypeDef((CppTypedef)type);
        
        if (Equals(type, CppPrimitiveType.Bool))
            return "MarshalAs(UnmanagedType.U1)";
        return string.Empty;
    }
    public static string GetUnmanagedType(CppType type)
    {
        var result = string.Empty;
        switch (type.TypeKind)
        {
            case CppTypeKind.Pointer:
            case CppTypeKind.Array:
            case CppTypeKind.Function:
            case CppTypeKind.Reference:
            case CppTypeKind.StructOrClass:
                result = "IntPtr";
                break;
            case CppTypeKind.Primitive:
            {
                if (Equals(type, CppPrimitiveType.Bool))
                    result = "bool";
                else if (Equals(type, CppPrimitiveType.Int))
                    result = "int";
                else if (Equals(type, CppPrimitiveType.Short))
                    result = "short";
                else if (Equals(type, CppPrimitiveType.Void))
                    result = "void";
                else if (Equals(type, CppPrimitiveType.UnsignedChar))
                    result = "byte";
                else if (Equals(type, CppPrimitiveType.UnsignedInt))
                    result = "uint";
                else if (Equals(type, CppPrimitiveType.UnsignedShort))
                    result = "ushort";
                else if (Equals(type, CppPrimitiveType.UnsignedLongLong))
                    result = "ulong";
                else if (Equals(type, CppPrimitiveType.Float))
                    result = "float";
                else if (Equals(type, CppPrimitiveType.Double) || Equals(type, CppPrimitiveType.LongDouble))
                    result = "double";
                else if (Equals(type, CppPrimitiveType.Char))
                    result = "sbyte";
                else
                    throw new NotImplementedException();
            }
                break;
            case CppTypeKind.Typedef:
            {
                var typeDefType = (CppTypedef)type;
                result = GetUnmanagedType(typeDefType.ElementType);
            }
                break;
            case CppTypeKind.Enum:
                result = GetFixedEnumName((CppEnum)type);
                break;
            case CppTypeKind.Qualified:
                result = GetUnmanagedType(((CppQualifiedType)type).ElementType);
                break;
            default:
                throw new NotImplementedException();
        }

        if (string.IsNullOrEmpty(result))
            throw new NullReferenceException();
        
        return result;
    }

    private static StringBuilder GetUnmanagedCallHeader(CppClass @class, CppFunction func)
    {
        var result = new StringBuilder();
        result.Append("public static partial ");
        result.Append(GetUnmanagedType(func.ReturnType));
        result.Append(' ');
        result.Append(CppTypeUtils.GetFunctionName(@class, func));

        return result;
    }

    private static StringBuilder BuildUnmanagedCallArgs(CppClass @class, CppFunction func, StringBuilder text)
    {
        text.Append("(IntPtr _this");

        if (func.Parameters.Any())
            text.Append(", ");
        
        for (var i = 0; i < func.Parameters.Count; ++i)
        {
            var paramType = func.Parameters[i].Type;
            if (IsUnmanagedSpecialTypeRequiresAttr(paramType))
                text.Append($"[{GetUnmanagedSpecialAttribute(paramType)}] ");
            text.Append(GetUnmanagedType(paramType));
            text.Append(' ');
            text.Append("arg");
            text.Append(i);
            if (i < func.Parameters.Count - 1)
                text.Append(", ");
        }

        text.Append(");");
        return text;
    }
    public static string GetUnmanagedCallDecl(CppClass @class, CppFunction func)
    {
        var result = GetUnmanagedCallHeader(@class, func);
        return BuildUnmanagedCallArgs(@class, func, result).ToString();
    }

    public static string GetUnmanagedVariantCallDecl(CppClass @class, CppFunction func, int idx)
    {
        var result = GetUnmanagedCallHeader(@class, func);
        result.Append("_v");
        result.Append(idx);
        return BuildUnmanagedCallArgs(@class, func, result).ToString();
    }
}