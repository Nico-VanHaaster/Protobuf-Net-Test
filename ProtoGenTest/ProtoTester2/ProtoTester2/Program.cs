// file:	Program.cs
//
// summary:	Implements the program class
extern alias gr;
extern alias pbr;

using gr::Google.Protobuf;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProtoTester2
{
    /// <summary>
    /// A program.
    /// </summary>
    static class Program
    {
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
    }
}