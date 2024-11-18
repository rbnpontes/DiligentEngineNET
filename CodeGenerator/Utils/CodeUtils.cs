using System.Text;
using CodeGenerator.CodeBuilders;
using Humanizer;

namespace CodeGenerator;

public class CodeUtils
{
    public static string ToCamelCase(string input)
    {
        if (input.Length == 0)
            return string.Empty;
        char[] chars = input.ToCharArray();
        chars[0] = char.ToLower(chars[0]);
        return new string(chars);
    }
    public static string ToSnakeCase(string input)
    {
        var output = new StringBuilder();

        var isUpper = false;
        for(var i = 0; i < input.Length; ++i)
        {
            if ((char.IsUpper(input[i]) ||char.IsNumber(input[i])) && !isUpper)
            {
                if(i > 0)
                    output.Append('_');
                isUpper = true;
            }
            else if (char.IsLower(input[i]))
            {
                isUpper = false;
            }

            output.Append(char.ToLowerInvariant(input[i]));
        }

        return output.ToString();
    }

    public static string ToPlural(string input)
    {
        return input.Pluralize();
    }

    public static string ConvertScreamingToPascalCase(string input)
    {
        var parts = input.Split('_');
        var result = new StringBuilder();
        foreach (var part in parts)
        {
            var currentPart = part;
            var firstChar = char.ToUpper(currentPart[0]);
            currentPart = part.Remove(0, 1).ToLowerInvariant();
            result.Append(firstChar);
            result.Append(currentPart);
        }

        return result.ToString();
    }

    public static bool IsScreamingCase(string input)
    {
        foreach (var c in input)
        {
            if(c == '_')
                continue;
            if (char.IsLower(c))
                return false;
        }

        return true;
    }
    
    public static void WriteCode(string filePath, string source)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir) && dir is not null)
            Directory.CreateDirectory(dir);
        if(File.Exists(filePath))
            File.Delete(filePath);
        
        
        File.WriteAllText(filePath, source);
    }
    
    public static void WriteCode(string path, CppBuilder builder)
    {
        WriteCode(path, builder.ToString());
    }

    public static void WriteCode(string path, CSharpBuilder builder)
    {
        WriteCode(path, builder.ToString());
    }
}