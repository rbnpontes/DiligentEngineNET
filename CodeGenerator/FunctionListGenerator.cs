using System.Text;
using CppAst;

namespace CodeGenerator;

public class FunctionListGenerator(string outputBaseDir, CppCompilation compilation): ICodeGenerator
{
    private readonly string _outputDir = Path.Combine(outputBaseDir, "Native");
    private readonly List<string> _functionsList = ["malloc", "free", "memset"];
    public void Setup()
    {
        if(!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);
    }

    public void Build()
    {
        var diligentNamespace = compilation.Namespaces.First(x => x.Name == "Diligent");
        CollectFunctions(diligentNamespace);

        var result = new StringBuilder();
        result.AppendLine("[");
        for (var i = 0; i < _functionsList.Count; i++)
        {
            result.Append("\t\"_");
            result.Append(_functionsList[i]);
            result.Append('"');
            if(i < _functionsList.Count - 1)
                result.Append(',');
            result.AppendLine();
        }
        result.AppendLine("]");
        
        CodeUtils.WriteCode(
            Path.Combine(_outputDir, "exported-functions.json"),
            result);
    }

    private void CollectFunctions(CppNamespace ns)
    {
        foreach (var @class in ns.Classes.Where(AstUtils.IsAllowedClass))
        {
            if(!CppTypeUtils.CanBeGenerated(@class))
                continue;
            if(ExclusionList.IgnoreFromList.Contains(@class.Name))
                continue;
            ProcessClass(@class);
        }
    }

    private void ProcessClass(CppClass @class)
    {
        var grps = @class.Functions
            .Where(AstUtils.IsAllowedFunction)
            .GroupBy(x => x.Name)
            .ToArray();
        foreach (var grp in grps)
        {
            var isVariantCall = grp.Count() > 1;

            for (var funcIdx = 0; funcIdx < grp.Count(); funcIdx++)
            {
                var func = grp.ElementAt(funcIdx);
                if(AstUtils.IsOperatorFunction(func))
                    continue;
                
                var methodName = isVariantCall
                    ? CppTypeUtils.GetFunctionVariantName(@class, func, funcIdx)
                    : CppTypeUtils.GetFunctionName(@class, func);
                
                _functionsList.Add(methodName);
            }
        }
    }
}