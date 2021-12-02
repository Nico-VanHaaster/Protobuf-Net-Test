Repository example for https://github.com/protobuf-net/protobuf-net/issues/864#issuecomment-984420257

The Generated Pb is generated using protobuf-net tools and then re-read by the Google Protobuf Toolsets. The read method will through an unexpected error.


```
 /// <summary>
/// Main entry-point for this application.
/// </summary>
/// <param name="args"> An array of command-line argument strings.</param>
public static void Main(string[] args)
{
    GeneratePbFile();
    ReadPbFile();
}

/// <summary>
/// Generates a pb file.
/// </summary>
/// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
private static void GeneratePbFile()
{
    var set = new pbr::Google.Protobuf.Reflection.FileDescriptorSet();
    var importPaths = new List<string>();
    set.AllowNameOnlyImport = true;
    importPaths.Add(@"D:\ProtoGenTest\src\Protos");
    importPaths.Add(@"D:\ProtoGenTest\src\Service\Protos\");
    foreach (var importPath in importPaths)
    {
        set.AddImportPath(importPath);
        var protoFiles = Directory.GetFiles(importPath, "*.proto", SearchOption.AllDirectories);
        foreach (var protoFile in protoFiles)
        {
            var file = Path.GetRelativePath(importPath, protoFile);
            if (!set.Add(file, true))
            {
                throw new Exception($"Unable to add file {file}");
            }
        }
    }

    set.Process();
    var errors = set.GetErrors();
    using (var fds = File.Create(".\\output.pb"))
    {
        Serializer.Serialize(fds, set);
    }
}

/// <summary>
/// Reads pb file.
/// </summary>
private static void ReadPbFile()
{
    using (var stream = new MemoryStream(File.ReadAllBytes(".\\output.pb")))
    {
        var descriptorSet = gr::Google.Protobuf.Reflection.FileDescriptorSet.Parser.ParseFrom(stream);

        var byteStrings = descriptorSet.File.Select(f => f.ToByteString()).ToList();
        System.Collections.Generic.IReadOnlyList<gr::Google.Protobuf.Reflection.FileDescriptor> descriptors = gr::Google.Protobuf.Reflection.FileDescriptor.BuildFromByteStrings(byteStrings);
    }
}

```
