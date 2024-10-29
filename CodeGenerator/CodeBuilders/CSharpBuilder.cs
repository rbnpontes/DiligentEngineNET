namespace CodeGenerator.CodeBuilders;

public class CSharpBuilder(int indentation = 0)
{
    private CodeBlockChunk _chunk = new(indentation);

    public CSharpBuilder Closure(Action<CSharpBuilder> closureBody)
    {
        var closure = new CSharpBuilder(_chunk.Indentation + 1);
        closureBody(closure);
        
        _chunk.Add("{");
        _chunk.Add(closure._chunk);
        _chunk.Add("}");
        return this;
    }
    
    public CSharpBuilder Namespace(Action<CSharpBuilder> namespaceBody, string ns)
    {
        _chunk.Add($"namespace {ns}");
        return Closure(namespaceBody);
    }

    public CSharpBuilder Namespace(string ns)
    {
        _chunk.Add($"namespace {ns};");
        return this;
    }

    public CSharpBuilder Class(Action<CSharpBuilder> body, string className, string qualifiers)
    {
        Line($"{qualifiers} class {className}");
        return Closure(body);
    }

    public CSharpBuilder Class(Action<CSharpBuilder> body, string className, string baseName, string qualifiers)
    {
        Line($"{qualifiers} class {className} : {baseName}");
        return Closure(body);
    }

    public CSharpBuilder Struct(Action<CSharpBuilder> body, string structName, string qualifiers)
    {
        Line($"{qualifiers} struct {structName}");
        return Closure(body);
    }

    public CSharpBuilder Line()
    {
        return Line(string.Empty);
    }
    public CSharpBuilder Line(string line)
    {
        _chunk.Add(line);
        return this;
    }

    public CSharpBuilder BeginRegion(string regionName)
    {
        return Line($"#region {regionName}");
    }

    public CSharpBuilder EndRegion()
    {
        return Line("#endregion");
    }
    
    public CSharpBuilder Using(string importName)
    {
        return Line($"using {importName};");
    }
    
    public CSharpBuilder DllImport(string libNameAccessor)
    {
        //return Line($"[DllImport({libNameAccessor}, CallingConvention = CallingConvention.Cdecl)]");
        Line($"[LibraryImport({libNameAccessor})]");
        Line("[SuppressUnmanagedCodeSecurity]");
        Line("[UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]");
        return this;
    }

    public CSharpBuilder StructLayout(int packSize, int size)
    {
        Line($"[StructLayout(LayoutKind.Explicit, Pack = {packSize}, Size = {size})]");
        return this;
    }

    public CSharpBuilder FieldOffset(int size)
    {
        return Line($"[FieldOffset({size})]");
    }
    
    public override string ToString()
    {
        return _chunk.GetString();
    }
}