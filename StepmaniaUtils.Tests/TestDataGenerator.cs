using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace StepmaniaUtils.Tests
{
    internal static class TestDataGeneratorExtensions
    {
        public static string NameWithoutExt(this FileInfo f) => Path.GetFileNameWithoutExtension(f.Name);
        
        public static List<FieldInfo> GetConstants(this Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public static string AsVariable(this string str)
        {
            return Regex.Replace(str, @"^[^A-Za-z_]+|\W+", String.Empty);
        }
    }

    
    public class TestDataGenerator
    {

        private readonly ITestOutputHelper output;

        public TestDataGenerator(ITestOutputHelper output)
        {
            this.output = output;
        }


        [Fact]
        public void GenerateITGOfficialsTestDataConstants()
        {
            var result = Directory.EnumerateFiles("TestData/ITGOfficial/")
                .Select(f => new FileInfo(f))
                .Select(f =>
                    $"public const string {f.NameWithoutExt().Replace(' ', '_').ToUpper().AsVariable()} = \"TestData/ITGOfficial/{f.Name}\";")
                .ToList();

            string declarations = string.Join('\n', result);

            output.WriteLine(declarations);
        }

        [Fact]
        public void GenerateSscTestDataConstants()
        {
            var result = Directory.EnumerateFiles("TestData/SSC/", "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".ssc"))
                .Select(f => new FileInfo(f))
                .Select(f =>
                    $"public const string {f.NameWithoutExt().Replace(' ', '_').ToUpper().AsVariable()} = \"TestData/SSC/{f.Directory.Name}/{f.Name}\";")
                .ToList();

            string declarations = string.Join('\n', result);

            output.WriteLine(declarations);
        }
    }
}