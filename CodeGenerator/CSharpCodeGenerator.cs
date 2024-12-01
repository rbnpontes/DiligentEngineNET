using System.Text;
using CodeGenerator.CodeBuilders;
using CppAst;

namespace CodeGenerator;

public class CSharpCodeGenerator(string diligentCorePath, string outputBaseDir, CppCompilation compilation)
    : ICodeGenerator
{
    private readonly string _outputDir = Path.Combine(outputBaseDir, "NET");

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
        var enums = compilation.Enums.Concat(diligentNamespace.Enums);
        foreach (var @class in classes)
            BuildClassCode(@class);
        foreach (var @enum in enums)
            BuildEnumCode(@enum);
        BuildConstantsCode(diligentNamespace.Fields.ToArray());
    }

    private void BuildEnumCode(CppEnum @enum)
    {
        var builder = new CSharpBuilder();
        builder.Namespace("Diligent").Line();

        if (AstUtils.IsEnumFlag(@enum))
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
            if (!isConstructable)
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
                        $"internal static {className} FromInternalStruct(__Internal data)")
                    .Closure(funcBuilder => BuildFromInternalStructMethod(@class, funcBuilder))
                    .Line()
                    .Line(
                        $"internal static unsafe __Internal GetInternalStruct({className} obj)")
                    .Closure(funcBuilder => BuildGetInternalStructMethod(@class, funcBuilder))
                    .Line()
                    .Line(
                        $"internal static unsafe void UpdateInternalStruct({className} target, {className}.__Internal data)")
                    .Closure(funcBuilder => BuildUpdateInternalStructMethod(@class, funcBuilder));
                builder.EndRegion();
            }
        };

        var baseClass = @class.BaseTypes.FirstOrDefault();
        string classQualifiers;
        if (isConstructable)
            classQualifiers = "public partial";
        else
            classQualifiers = "internal partial";

        if (baseClass is not null)
        {
            builder.Class(
                classDefCall,
                className,
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

    private void BuildConstantsCode(CppField[] fields)
    {
        // filter allowed fields
        fields = fields
            .Where(x =>
            {
                if (x.StorageQualifier == CppStorageQualifier.Extern)
                    return false;
                if (x.Name == "True" || x.Name == "False")
                    return false;
                return x.InitValue is not null || x.InitExpression is not null;
            })
            .ToArray();
        var builder = new CSharpBuilder();
        builder.Namespace("Diligent").Line();


        builder.Class(classBuilder =>
        {
            foreach (var field in fields)
            {
                var fieldType = (CppQualifiedType)field.Type;
                var elementType = AstUtils.Resolve(fieldType.ElementType);

                if (elementType.TypeKind == CppTypeKind.Primitive)
                    BuildPrimitiveField(field, classBuilder);
                else if (field.Name.StartsWith("IID_"))
                    BuildInterfaceField(field, classBuilder);
            }
        }, "Constants", "public static partial");

        var outputCodePath = Path.Combine(_outputDir, "constants.cs");
        CodeUtils.WriteCode(outputCodePath, builder);

        void BuildPrimitiveField(CppField field, CSharpBuilder builder)
        {
            var fieldName = CodeUtils.ConvertScreamingToPascalCase(field.Name);
            builder.Line(
                $"public const {CSharpUtils.GetUnmanagedType(field.Type)} {fieldName} = {field.InitValue.Value};");
        }

        // special handle case
        void BuildInterfaceField(CppField field, CSharpBuilder builder)
        {
            var fieldName = field.Name;
            var ctorArgs = new StringBuilder();
            var args = field.InitExpression.Arguments;
            for (var i = 0; i < args.Count; ++i)
            {
                var arg = args[i];
                switch (arg.Kind)
                {
                    case CppExpressionKind.IntegerLiteral:
                    {
                        var literalExp = (CppLiteralExpression)arg;
                        ctorArgs.Append(literalExp.Value);
                    }
                        break;
                    case CppExpressionKind.InitList:
                    {
                        var listExp = (CppInitListExpression)arg;
                        var listArgs = listExp.Arguments;
                        ctorArgs.Append('[');
                        for (var j = 0; j < listArgs.Count; ++j)
                        {
                            var listArg = listArgs[j];
                            if (listArg.Kind != CppExpressionKind.IntegerLiteral)
                                throw new NotImplementedException();

                            var listIntArg = (CppLiteralExpression)listArg;
                            ctorArgs.Append(listIntArg.Value);

                            if (j < listArgs.Count - 1)
                                ctorArgs.Append(", ");
                        }

                        ctorArgs.Append(']');
                    }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (i < args.Count - 1)
                    ctorArgs.Append(", ");
            }

            builder.Line($"public static readonly INTERFACE_ID {fieldName} = new ({ctorArgs});");
        }
    }

    private void BuildUnmanagedCalls(CppClass @class, CSharpBuilder builder)
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
                if (AstUtils.IsOperatorFunction(func))
                    continue;
                builder.DllImport("Constants.LibName");
                if (CSharpUtils.IsUnmanagedSpecialTypeRequiresAttr(func.ReturnType))
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
        var props2Skip = new HashSet<string>(ExclusionList.PropertiesToSkip);
        foreach (var field in @class.Fields)
        {
            var fieldWithClass = @class.Name + "::" + field.Name;
            if(props2Skip.Contains(fieldWithClass))
                continue;
            if(field.Name.StartsWith("pp"))
                continue;
            
            if (CSharpUtils.RequiresSpecialSetStructMethod(field))
                continue;
            if (AstUtils.IsFixedStringType(field.Type))
            {
                BuildFixedStringProperty(field);
                continue;
            }

            if (AstUtils.IsClassPointer(field.Type))
            {
                var classType = AstUtils.ResolveClassPointer(field.Type);
                if (AstUtils.InheritsDiligentObject(classType))
                    BuildDiligentObjectProperty(field, classType);
                else
                    BuildObjectProperty(field, classType);
                
                continue;
            }

            if (AstUtils.IsArrayType(field.Type) && !AstUtils.IsMultiDimensionalArray(field.Type))
            {
                BuildFixedArrayProperty(field);
                continue;
            }
            
            // Array members has pField and NumField or can be FieldCount
            if (field.Name.StartsWith("Num") || field.Name.EndsWith("Count"))
            {
                if(BuildArrayProperty(field))
                    continue;
            }
            
            var propDef = CSharpUtils.GetPropertyField(field.Type, field.Name);
            if (string.IsNullOrEmpty(propDef))
                continue;

            builder.Line(propDef);
        }

        void BuildFixedStringProperty(CppField field)
        {
            var arrayType = (CppArrayType)field.Type;
            builder
                .Line($"public unsafe string {field.Name}")
                .Closure(propBodyBuilder =>
                {
                    propBodyBuilder
                        .Line("get")
                        .Closure(getBuilder =>
                        {
                            getBuilder
                                .Line($"fixed (sbyte* ptr = _data.{field.Name})")
                                .Line($"\treturn new string(ptr, 0, {arrayType.Size});");
                        })
                        .Line("set")
                        .Closure(setBuilder =>
                        {
                            setBuilder
                                .Line($"for (var i = 0; i < int.Min(value.Length, {arrayType.Size}); ++i)")
                                .Line($"\t_data.{field.Name}[i] = (sbyte)value[i];");
                        });
                });
        }

        void BuildDiligentObjectProperty(CppField field, CppClass fieldClass)
        {
            var fieldName = field.Name;
            // field names usually starts with 'p' prefix
            if (fieldName.StartsWith('p'))
                fieldName = fieldName.Substring(1);
            // In this case, class name doesn't need to be normalized.
            builder
                .Line($"public {fieldClass.Name}? {fieldName}")
                .Closure(propBodyBuilder =>
                {
                    propBodyBuilder
                        .Line("get")
                        .Closure(getBuilder =>
                        {
                            getBuilder.Line(
                                $"NativeObjectRegistry.TryGetObject<{fieldClass.Name}>(_data.{field.Name}, out var output);"
                            ).Line("return output;");
                        })
                        .Line($"set => _data.{field.Name} = value?.Handle ?? IntPtr.Zero;");
                })
                .Line($"public IntPtr {fieldName}Ptr")
                .Closure(propBodyBuilder =>
                {
                    propBodyBuilder
                        .Line($"get => _data.{field.Name};")
                        .Line($"set => _data.{field.Name} = value;");
                });
        }

        void BuildObjectProperty(CppField field, CppClass fieldClass)
        {
            var fieldName = field.Name;
            if (fieldName.StartsWith('p'))
                fieldName = fieldName.Substring(1);

            var fieldNameSingular = CodeUtils.ToSingular(fieldName);
            var countField = @class.Fields.FirstOrDefault(x =>
            {
                var name = x.Name.ToLower();
                return name == (fieldName + "Count").ToLower()
                    || name == (fieldNameSingular + "Count").ToLower()
                    || name == ("Num" + fieldName).ToLower()
                    || name == ("Num" + fieldNameSingular).ToLower();
            });
            // skip this field is an array
            if (countField is not null)
                return;
            
            builder.Line($"public {fieldClass.Name}? {fieldName} {{ get; set; }}");
        }
        
        void BuildFixedArrayProperty(CppField field)
        {
            var arrayType = (CppArrayType)field.Type;
            var propType = CSharpUtils.GetPropertyType(arrayType.ElementType);
            var isEnum = AstUtils.IsEnumType(arrayType.ElementType);

            builder.Line($"public unsafe {propType}[] {field.Name}");
            builder.Closure(propBodyBuilder =>
            {
                propBodyBuilder
                    .Line("get")
                    .Closure(getBuilder =>
                    {
                        var accessor = $"_data.{field.Name}[i]";
                        if (isEnum)
                            accessor = $"({propType}){accessor}";
                        getBuilder
                            .Line($"var result = new {propType}[{arrayType.Size}];")
                            .Line("for (var i = 0; i < result.Length; ++i)")
                            .Line($"\tresult[i] = {accessor};")
                            .Line("return result;");
                    })
                    .Line("set")
                    .Closure(setBuilder =>
                    {
                        var accessor = $"value[i]";
                        if (isEnum)
                        {
                            var enumType = (CppEnum)arrayType.ElementType;
                            accessor = $"({CSharpUtils.GetPropertyType(enumType.IntegerType)}){accessor}";
                        }

                        setBuilder
                            .Line($"for (var i = 0; i < int.Min(value.Length, {arrayType.Size}); ++i)")
                            .Line($"\t_data.{field.Name}[i] = {accessor};");
                    });
            });
        }

        bool BuildArrayProperty(CppField field)
        {
            var propName = field.Name
                .Replace("Num", string.Empty)
                .Replace("Count", string.Empty);
            var propNamePlural = CodeUtils.ToPlural(propName);
            var targetField = @class.Fields.FirstOrDefault(x => x.Name == propName 
                                                                || x.Name == ('p' + propName) 
                                                                || x.Name == ('p' + propNamePlural)
                                                                || x.Name == "pp" + propName);
            // Sometimes, when the 'Num' prefix exists but the field is not found,
            // it indicates that the field doesn't represent an array. 
            if (targetField is null || !AstUtils.IsClassPointer(targetField.Type))
                return false;

            var isPlural = targetField.Name == 'p' + propNamePlural;
            var finalPropName = isPlural ? propNamePlural : propName;
            var privatePropName = "_"+CodeUtils.ToCamelCase(finalPropName);
            var classType = AstUtils.ResolveClassPointer(targetField.Type);

            builder
                .Line($"private {classType.Name}[] {privatePropName} = [];")
                .Line($"public {classType.Name}[] {finalPropName}")
                .Closure(propBodyBuilder =>
                {
                    propBodyBuilder
                        .Line($"get => {privatePropName};")
                        .Line("set")
                        .Closure(setBuilder =>
                        {
                            setBuilder
                                .Line($"{privatePropName} = value;")
                                .Line($"_data.{field.Name} = (uint)value.Length;");
                        });
                });

            return true;
        }
    }

    private void BuildFromInternalStructMethod(CppClass @class, CSharpBuilder builder)
    {
        var className = CSharpUtils.GetFixedClassName(@class);
        builder
            .Line($"var result = new {className}();")
            .Line($"{className}.UpdateInternalStruct(result, data);");
        builder.Line("return result;");
    }

    private void BuildGetInternalStructMethod(CppClass @class, CSharpBuilder builder)
    {
        builder.Line("var result = obj._data;");
        if (AstUtils.HasBaseClass(@class))
        {
            var parentClass = AstUtils.GetClassParent(@class);
            var parentClassName = CSharpUtils.GetFixedClassName(parentClass);
            var parentClassFields = AstUtils.GetAllClassFields(parentClass);

            builder.Line("// update child data into current data");
            builder.Line($"var childData = {parentClassName}.GetInternalStruct(obj);");
            foreach (var classField in parentClassFields)
            {
                var fieldType = AstUtils.Resolve(classField.Type);
                if (fieldType is CppArrayType arrayType)
                {
                    builder.Line("// copy manually when type is fixed");
                    builder.Line($"for (var i = 0; i < {arrayType.Size}; ++i)");
                    builder.Line($"\tresult.{classField.Name}[i] = childData.{classField.Name}[i];");
                    continue;
                }

                builder.Line($"result.{classField.Name} = childData.{classField.Name};");
            }
        }

        builder.Line("return result;");
    }

    private void BuildUpdateInternalStructMethod(CppClass @class, CSharpBuilder builder)
    {
        builder.Line("target._data = data;");

        if (!AstUtils.HasBaseClass(@class))
            return;
        var parentClass = AstUtils.GetClassParent(@class);
        var parentClassName = CSharpUtils.GetFixedClassName(parentClass);
        var parentClassFields = AstUtils.GetAllClassFields(parentClass);

        builder.Line("// update child data");
        builder.Line($"var childData = {parentClassName}.GetInternalStruct(target);");
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

        builder.Line($"{parentClassName}.UpdateInternalStruct(target, childData);");
    }
}