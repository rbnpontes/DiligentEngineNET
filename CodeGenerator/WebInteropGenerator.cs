using System.Text;
using CodeGenerator.CodeBuilders;
using CppAst;

namespace CodeGenerator;

public class WebInteropGenerator(string outputBaseDir, CppCompilation compilation) : ICodeGenerator
{
    private readonly string _outputDir = Path.Combine(outputBaseDir, "NET", "WebInterop");

    public void Setup()
    {
        if (!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);
        else
        {
            foreach (var file in Directory.GetFiles(_outputDir))
                File.Delete(file);
        }
    }

    public void Build()
    {
        var diligentNamespace = compilation.Namespaces.First(x => x.Name == "Diligent");
        var classes = compilation.Classes.Concat(diligentNamespace.Classes).Where(AstUtils.IsAllowedClass);

        foreach (var @class in classes)
            BuildClassCode(@class);
    }

    private void BuildClassCode(CppClass @class)
    {
        if (CSharpUtils.IsConstructable(@class))
            return;

        var className = CSharpUtils.GetFixedClassName(@class);
        var baseClass = @class.BaseTypes.FirstOrDefault();

        var builder = new CSharpBuilder();
        builder
            .Line("// ReSharper disable All")
            .Using("System.Runtime.InteropServices.JavaScript")
            .Namespace("Diligent")
            .Line();

        var classBuilderCall = (CSharpBuilder classBuilder) =>
        {
            classBuilder.Class(
                interopBuilder => BuildInteropCall(@class, interopBuilder),
                "WebInterop", "internal static partial");
        };
        var classQualifiers = "internal partial";
        if (baseClass is not null)
        {
            builder.Class(
                classBuilderCall, 
                className, 
                CSharpUtils.GetFixedClassName((CppClass)baseClass.Type),
                classQualifiers
            );
        }
        else
        {
            builder.Class(classBuilderCall, className, classQualifiers);
        }

        var outputCodePath = Path.Combine(_outputDir, $"web_{className}_interop.cs");
        CodeUtils.WriteCode(outputCodePath, builder);
    }

    private void BuildInteropCall(CppClass @class, CSharpBuilder builder)
    {
        var grpFunctions = @class.Functions
            .Where(AstUtils.IsAllowedFunction)
            .GroupBy(x => x.Name)
            .ToArray();

        foreach (var grpFunction in grpFunctions)
        {
            var isFunctionVariant = grpFunction.Count() > 1;
            for (var funcIdx = 0; funcIdx < grpFunction.Count(); ++funcIdx)
            {
                var func = grpFunction.ElementAt(funcIdx);
                if(AstUtils.IsOperatorFunction(func))
                    continue;
                
                var funcName = isFunctionVariant
                    ? CppTypeUtils.GetFunctionVariantName(@class, func, funcIdx)
                    : CppTypeUtils.GetFunctionName(@class, func);
                var retType = GetMarshalType(func.ReturnType);

                builder.Line($"[JSImport(\"_{funcName}\", Constants.WebLibName)]");
                if (IsRequiredMarshalAsAttribute(func.ReturnType))
                    builder.Line($"[return: {GetMarshalAsAttribute(func.ReturnType)}]");
                builder.Line($"internal static partial {retType} {funcName}({BuildCallArgs(@class, func)});");
            }
        }
    }

    private string GetMarshalType(CppType type)
    {
        type = AstUtils.Resolve(type);
        var result = CSharpUtils.GetUnmanagedType(type);
        switch (type.TypeKind)
        {
            case CppTypeKind.Enum:
                var enumType = (CppEnum)type;
                result = GetMarshalType(enumType.IntegerType);
                break;
            case CppTypeKind.Primitive:
            {
                if(Equals(type, CppPrimitiveType.UnsignedInt))
                    result = "int";
                else if(Equals(type, CppPrimitiveType.UnsignedLong) || Equals(type, CppPrimitiveType.UnsignedLongLong))
                    result = "long";
                else if(Equals(type, CppPrimitiveType.UnsignedShort))
                    result = "short";
                else if(Equals(type, CppPrimitiveType.Char))
                    result = "char";
            }
                break;
        }
        
        return result;
    }

    private bool IsRequiredMarshalAsAttribute(CppType type)
    {
        type = AstUtils.Resolve(type);
        var result = false;
        switch (type.TypeKind)
        {
            case CppTypeKind.Enum:
                var enumType = (CppEnum)type;
                result = IsRequiredMarshalAsAttribute(enumType.IntegerType);
                break;
            case CppTypeKind.Primitive:
            {
                if(Equals(type, CppPrimitiveType.UnsignedLongLong) || Equals(type, CppPrimitiveType.UnsignedLong))
                    result = true;
            }
                break;
        }
        
        return result;
    }

    private string GetMarshalAsAttribute(CppType type)
    {
        type = AstUtils.Resolve(type);
        var result = string.Empty;
        switch (type.TypeKind)
        {
            case CppTypeKind.Enum:
                var enumType = (CppEnum)type;
                result = GetMarshalAsAttribute(enumType.IntegerType);
                break;
            case CppTypeKind.Primitive:
            {
                if (Equals(type, CppPrimitiveType.UnsignedLongLong) || Equals(type, CppPrimitiveType.UnsignedLong))
                    result = "JSMarshalAs<JSType.Number>";
            }
                break;
        }
        
        return result;
    }

    private StringBuilder BuildCallArgs(CppClass @class, CppFunction func)
    {
        var result = new StringBuilder();
        result.Append("IntPtr _this");

        if (func.Parameters.Any())
            result.Append(", ");

        for (var i = 0; i < func.Parameters.Count; ++i)
        {
            var paramType = func.Parameters[i].Type;
            if(IsRequiredMarshalAsAttribute(paramType))
                result.Append($"[{GetMarshalAsAttribute(paramType)}] ");
            result.Append(GetMarshalType(paramType));
            result.Append(' ');
            result.Append("arg");
            result.Append(i);
            if(i < func.Parameters.Count-1)
                result.Append(", ");
        }
        
        return result;
    }
}