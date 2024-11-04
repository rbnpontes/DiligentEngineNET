using System.Text;
using CodeGenerator.CodeBuilders;
using CppAst;

namespace CodeGenerator;

public class CSharpCodeGenerator(string diligentCorePath, string outputBaseDir, CppCompilation compilation) : ICodeGenerator
{
    private readonly string _outputDir = Path.Combine(outputBaseDir, "NET");
    public void Setup()
    {
        if (!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);
        else
        {
            foreach(var file in Directory.GetFiles(_outputDir))
                File.Delete(file);
        }
    }

    public void Build()
    {
        var diligentNamespace = compilation.Namespaces.First(x => x.Name == "Diligent");
        foreach (var @class in diligentNamespace.Classes)
            BuildClassCode(@class);
        foreach (var @enum in diligentNamespace.Enums)
            BuildEnumCode(@enum);
        
    }

    private void BuildEnumCode(CppEnum @enum)
    {
        var builder = new CSharpBuilder();
        builder.Namespace("Diligent").Line();

        if (@enum.Name.EndsWith("FLAGS"))
            builder.Line("[Flags]");
        builder
            .Line($"public enum {CSharpUtils.GetFixedEnumName(@enum)} : {CSharpUtils.GetEnumTypeSize(@enum)}")
            .Closure(builder =>
            {
                var enumItems = @enum.Items.ToArray();
                for (var i = 0; i < enumItems.Length; ++i)
                {
                    var enumItemStr = new StringBuilder();
                    var enumItem = enumItems[i];
                    enumItemStr.Append(CSharpUtils.GetFixedEnumItemName(enumItem));
                    enumItemStr.Append(" = ");
                    enumItemStr.Append(CSharpUtils.GetEnumItemValue(enumItem));
                    if (i < enumItems.Length - 1)
                        enumItemStr.Append(',');

                    builder.Line(enumItemStr.ToString());
                }
            });

        var outputCodePath = Path.Combine(_outputDir, $"enum_{CSharpUtils.GetFixedEnumName(@enum)}_binding.cs");
        CodeUtils.WriteCode(outputCodePath, builder);
    }
    private void BuildClassCode(CppClass @class)
    {
        var builder = new CSharpBuilder();
        var classFields = AstUtils.HasClassFields(@class);
        var isConstructable = CSharpUtils.IsConstructable(@class);
        var className = CSharpUtils.GetFixedClassName(@class);

        builder.Line("// ReSharper disable All");
        if (!isConstructable || classFields)
        {
            builder.Using("System.Runtime.InteropServices");
            if (!isConstructable)
                builder.Using("System.Security");
        }
        
        builder.Namespace("Diligent").Line();
        var classDefCall = (CSharpBuilder builder) =>
        {
            if (!CSharpUtils.IsConstructable(@class))
                builder.Class(x => BuildUnmanagedCalls(@class, x), "Interop", "internal static partial");

            if (classFields)
            {
                builder
                    .Line("// internal struct data")
                    .Line($"private __Internal _data;")
                    .Line()
                    .BeginRegion("Internal Struct")
                    .Line("// internal struct data definition")
                    .Line("// this struct must be used by the unmanaged operations")
                    .StructLayout(@class.AlignOf, @class.SizeOf)
                    .Struct(
                        structBuilder => BuildInternalStruct(@class, structBuilder),
                        "__Internal",
                        "internal unsafe")
                    .EndRegion();

                builder.BeginRegion("Class Properties");
                BuildClassProperties(@class, builder);
                builder.EndRegion();

                builder.BeginRegion("Helper Methods");
                builder.Line(
                        $"internal static unsafe {className} FromInternalStruct(__Internal data)")
                    .Closure(funcBuilder => BuildFromInternalStructMethod(@class, funcBuilder))
                    .Line()
                    .Line(
                        $"internal static __Internal GetInternalStruct({className} obj)")
                    .Closure(funcBuilder => BuildGetInternalStructMethod(@class, funcBuilder))
                    .Line()
                    .Line(
                         $"internal static void UpdateInternalStruct({className} target, {className}.__Internal data)")
                     .Closure(BuildUpdateInternalStructMethod);
                builder.EndRegion();
            }
        };

        var baseClass = @class.BaseTypes.FirstOrDefault();
        var classQualifiers = "public partial";
        if (baseClass is not null)
        {
            builder.Class(
                classDefCall,
                CSharpUtils.GetFixedClassName(@class),
                CSharpUtils.GetFixedClassName((CppClass)baseClass.Type),
                classQualifiers);
        }
        else
        {
            builder.Class(
                classDefCall,
                className,
                classQualifiers);
        }
        
        var outputCodePath = Path.Combine(_outputDir, $"class_{className}_binding.cs");
        CodeUtils.WriteCode(outputCodePath, builder);
    }

    private void BuildUnmanagedCalls(CppClass @class, CSharpBuilder builder)
    {
        var grpFunctions = @class.Functions.GroupBy(x => x.Name).ToArray();
        foreach (var grpFunction in grpFunctions)
        {
            var isFunctionVariant = grpFunction.Count() > 1;
            for(var funcIdx = 0; funcIdx < grpFunction.Count(); ++funcIdx)
            {
                var func = grpFunction.ElementAt(funcIdx);
                if(AstUtils.IsOperatorFunction(func))
                    continue;
                builder.DllImport("Constants.LibName");
                if(CSharpUtils.IsUnmanagedSpecialTypeRequiresAttr(func.ReturnType))
                    builder.Line($"[return: {CSharpUtils.GetUnmanagedSpecialAttribute(func.ReturnType)}]");
                builder.Line(
                    isFunctionVariant 
                        ? CSharpUtils.GetUnmanagedVariantCallDecl(@class, func, funcIdx)
                        : CSharpUtils.GetUnmanagedCallDecl(@class, func)
                );
            }
        }
    }

    private void BuildInternalStruct(CppClass @class, CSharpBuilder builder)
    {
        var fields = AstUtils.GetAllClassFields(@class);
        var specialMethods2Gen = new List<(CppType, string, int)>();
        foreach (var field in fields)
        {
            if (CSharpUtils.RequiresSpecialSetStructMethod(field))
            {
                var arrayType = (CppArrayType)field.Type;
                var targetType = (CppClass)AstUtils.Resolve(arrayType.ElementType);

                for (var i = 0; i < arrayType.Size; ++i)
                {
                    builder.FieldOffset((int)field.Offset + targetType.SizeOf * i);
                    builder.Line(CSharpUtils.GetStructField(targetType, $"{field.Name}_{i}"));
                }
                
                specialMethods2Gen.Add((targetType, field.Name, arrayType.Size));
                continue;    
            }
            
            builder.FieldOffset((int)field.Offset);
            if (CSharpUtils.IsUnmanagedSpecialTypeRequiresAttr(field.Type))
                builder.Line($"[{CSharpUtils.GetUnmanagedSpecialAttribute(field.Type)}]");
            builder.Line(CSharpUtils.GetStructField(field));
        }

        if (specialMethods2Gen.Any())
            builder.Line();
        foreach (var pair in specialMethods2Gen)
        {
            (var targetType, var fieldName, var fieldItemsCount) = pair;
            builder.Line($"public {CSharpUtils.GetStructType(targetType)} Get{fieldName}(int idx)");
            builder.Closure(funcBuilder =>
            {
                funcBuilder.Line(
                    $"if (idx < 0 || idx > {fieldItemsCount}) throw new ArgumentOutOfRangeException(nameof(idx));");
                funcBuilder.Line($"{CSharpUtils.GetStructType(targetType)}? result = null;");
                funcBuilder.Line("switch (idx)");
                funcBuilder.Closure(switchBuilder =>
                {
                    for (var i = 0; i < fieldItemsCount; ++i)
                    {
                        switchBuilder.Line($"case {i}:");
                        switchBuilder.Line($"\tresult = {fieldName}_{i};");
                        switchBuilder.Line("break;");
                    }
                });
                funcBuilder.Line();
                funcBuilder.Line("if (result is null) throw new NullReferenceException();");
                funcBuilder.Line().Line("return result.Value;");
            });
            // generate special method set
            builder.Line($"public void Set{fieldName}(int idx, {CSharpUtils.GetStructType(targetType)} value)");
            builder.Closure(funcBuilder =>
            {
                funcBuilder.Line(
                    $"if (idx < 0 || idx > {fieldItemsCount}) throw new ArgumentOutOfRangeException(nameof(idx));");
                funcBuilder.Line("switch (idx)");
                funcBuilder.Closure(switchBuilder =>
                {
                    for (var i = 0; i < fieldItemsCount; ++i)
                    {
                        switchBuilder.Line($"case {i}:");
                        switchBuilder.Line($"\t{fieldName}_{i} = value;");
                        switchBuilder.Line("break;");
                    }
                });
            });
        }
    }

    private void BuildClassProperties(CppClass @class, CSharpBuilder builder)
    {
        foreach (var field in @class.Fields)
        {
            if (CSharpUtils.RequiresSpecialSetStructMethod(field))
                continue;
            var propDef = CSharpUtils.GetPropertyField(field.Type, field.Name);
            if(string.IsNullOrEmpty(propDef))
                continue;

            builder.Line(propDef);
        }
    }

    private void BuildFromInternalStructMethod(CppClass @class, CSharpBuilder builder)
    {
        var className = CSharpUtils.GetFixedClassName(@class);
        builder
            .Line($"var result = new {className}();")
            .Line($"{className}.UpdateInternalStruct(result, data);");

        if (AstUtils.HasBaseClass(@class))
        {
            var parentClass = AstUtils.GetClassParent(@class);
            var parentClassName = CSharpUtils.GetFixedClassName(parentClass);
            var parentClassFields = AstUtils.GetAllClassFields(parentClass);

            builder.Line("// update child data");
            builder.Line($"var childData = {parentClassName}.GetInternalStruct(result);");
            foreach (var classField in parentClassFields)
            {
                var fieldType = AstUtils.Resolve(classField.Type);
                if (fieldType is CppArrayType arrayType)
                {
                    builder.Line("// copy manually when type is fixed");
                    builder.Line($"for (var i = 0; i < {arrayType.Size}; ++i)");
                    builder.Line($"\tchildData.{classField.Name}[i] = data.{classField.Name}[i];");
                    continue;
                }
                builder.Line($"childData.{classField.Name} = data.{classField.Name};");
            }
            builder.Line($"{parentClassName}.UpdateInternalStruct(result, childData);");
        }
        
        builder.Line("return result;");
    }

    private void BuildGetInternalStructMethod(CppClass @class, CSharpBuilder builder)
    {
        builder.Line("return obj._data;");
    }

    private void BuildUpdateInternalStructMethod(CSharpBuilder builder)
    {
        builder.Line("target._data = data;");
    }
}