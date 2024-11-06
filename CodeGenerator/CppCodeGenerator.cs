using System.Text;
using CodeGenerator.CodeBuilders;
using CppAst;

namespace CodeGenerator;

public class CppCodeGenerator(string diligentCorePath, string baseOutputDir, CppCompilation compilation) : ICodeGenerator
{
    private string _outputDir = Path.Combine(baseOutputDir, "Native");
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
        foreach (var @class in diligentNamespace.Classes.Where(AstUtils.IsAllowedClass))
        {
            if(!CppTypeUtils.CanBeGenerated(@class))
                continue;
            BuildClassHeader(@class);
            BuildClassSource(@class);
        }
    }

    private void BuildClassHeader(CppClass @class)
    {
        var classFileName = Path.GetFileName(@class.SourceFile);
        var builder = new CppBuilder();
        builder.SetPragmaOnce();
        builder.IncludeLiteral("Api.h");
        builder.Include($"<{classFileName}>");
        builder.Line("using namespace Diligent;").Line();

        // Functions can have multiple declarations, but we can only export
        // a single declaration, in this case we will add a index at the end of decl.
        var functionGroups = @class.Functions
            .Where(AstUtils.IsAllowedFunction)
            .GroupBy(x => x.Name)
            .ToArray();
        for (var grpIdx = 0; grpIdx < functionGroups.Count(); ++grpIdx)
        {
            var grp = functionGroups[grpIdx];
            var isVariantCall = grp.Count() > 1;

            for (var funcIdx = 0; funcIdx < grp.Count(); ++funcIdx)
            {
                var func = grp.ElementAt(funcIdx);
                if(AstUtils.IsOperatorFunction(func))
                    continue;
                var methodName = isVariantCall 
                    ? CppTypeUtils.GetFunctionVariantDeclName(@class, func, funcIdx) 
                    : CppTypeUtils.GetFunctionDeclName(@class, func);
                var returnType = CppTypeUtils.GetFunctionReturnType(func);

                builder.Line("EXPORT " + returnType + " " + methodName + ";");
            }
        }
        
        CodeUtils.WriteCode(Path.Combine(_outputDir, CppTypeUtils.GetBaseClassFileName(@class)+".h"), builder);
    }

    private void BuildClassSource(CppClass @class)
    {
        var classHeaderFile = CppTypeUtils.GetBaseClassFileName(@class) + ".h";
        var builder = new CppBuilder();
        builder
            .IncludeLiteral($"./{classHeaderFile}")
            .Line();

        // Functions can have multiple declarations, but we can only export
        // a single declaration, in this case we will add a index at the end of decl.
        var functionGroups = @class.Functions
            .Where(AstUtils.IsAllowedFunction)
            .GroupBy(x => x.Name)
            .ToArray();
        for (var grpIdx = 0; grpIdx < functionGroups.Length; ++grpIdx)
        {
            var grp = functionGroups[grpIdx];
            var isVariantCall = grp.Count() > 1;
            for (var funcIdx = 0; funcIdx < grp.Count(); ++funcIdx)
            {
                var func = grp.ElementAt(funcIdx);
                if(AstUtils.IsOperatorFunction(func))
                    continue;
                
                var methodName = isVariantCall 
                    ? CppTypeUtils.GetFunctionVariantDeclName(@class, func, funcIdx)
                    : CppTypeUtils.GetFunctionDeclName(@class, func);
                var returnType = CppTypeUtils.GetFunctionReturnType(func);
                builder.Line(returnType + " " + methodName);
                builder.Closure(builder =>
                {
                    var line = CppTypeUtils.HandleFunctionReturn(@class, func);
                    builder.Line(line);
                });
            }
        }
        
        CodeUtils.WriteCode(Path.Combine(_outputDir, CppTypeUtils.GetBaseClassFileName(@class)+".cpp"), builder);
    }
}