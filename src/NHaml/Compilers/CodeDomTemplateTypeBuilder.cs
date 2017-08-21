using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using NHaml.Compilers.Exceptions;

namespace NHaml.Compilers
{
    /// <summary>
    /// This class is responsible for providing the base functionality for building a <see cref="Type"/> representation
    /// for a Haml document.
    /// </summary>
    public abstract class CodeDomTemplateTypeBuilder : ITemplateTypeBuilder
    {
        private readonly CodeDomProvider _codeDomProvider;
        protected Dictionary<string, string> ProviderOptions { get; private set; }

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        protected CodeDomTemplateTypeBuilder(CodeDomProvider codeDomProvider)
        {
            _codeDomProvider = codeDomProvider;
            ProviderOptions = new Dictionary<string, string>();
        }

        [SuppressMessage("Microsoft.Security", "CA2122")]
        [SuppressMessage("Microsoft.Portability", "CA1903")]
        public Type Build(string source, string typeName, IEnumerable<string> referencedAssemblyLocations)
        {
            var compilerParams = new CompilerParameters();
            AddReferences(compilerParams, referencedAssemblyLocations);
            return SupportsDebug()
                ? BuildWithDebug(source, typeName, compilerParams)
                : BuildWithoutDebug(source, typeName, compilerParams);
        }

        private Type BuildWithDebug(string source, string typeName, CompilerParameters compilerParams)
        {
            compilerParams.GenerateInMemory = false;
            compilerParams.IncludeDebugInformation = true;
            var directoryInfo = GetNHamlTempDirectoryInfo();
            var classFileInfo = GetClassFileInfo(directoryInfo, typeName);
            using (var writer = classFileInfo.CreateText())
            {
                writer.Write(source);
            }

            //TODO: when we move to vs2010 fully this becomes redundant as it will load the debug info for an in memory assembly.
            var tempFileName = Path.GetTempFileName();
            var tempAssemblyName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".dll"));
            var tempSymbolsName = new FileInfo(Path.Combine(directoryInfo.FullName, tempFileName + ".pdb"));
            try
            {
                compilerParams.OutputAssembly = tempAssemblyName.FullName;
                var compilerResults = _codeDomProvider.CompileAssemblyFromFile(compilerParams, classFileInfo.FullName);
                ValidateCompilerResults(compilerResults, source);

                var assembly = Assembly.Load(File.ReadAllBytes(tempAssemblyName.FullName),
                                             File.ReadAllBytes(tempSymbolsName.FullName));
                return assembly.GetType(typeName);
            }
            finally
            {
                if (tempAssemblyName.Exists)
                {
                    tempAssemblyName.Delete();
                }
                if (tempSymbolsName.Exists)
                {
                    tempSymbolsName.Delete();
                }
            }
        }

        private Type BuildWithoutDebug(string source, string typeName, CompilerParameters compilerParams)
        {
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            var compilerResults = _codeDomProvider.CompileAssemblyFromSource(compilerParams, source);
            ValidateCompilerResults(compilerResults, source);
            var assembly = compilerResults.CompiledAssembly;
            return ExtractType(typeName, assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compilerResults"></param>
        /// <param name="source"></param>
        /// <exception cref="CompilerException">Thrown when the <paramref name="compilerResults"/> contains errors.</exception>
        private static void ValidateCompilerResults(CompilerResults compilerResults, string source)
        {
            if (ContainsErrors(compilerResults))
            {
                throw new CompilerException(compilerResults, source);
            }
        }

        /// <summary>
        /// Adds refrences to the <see cref="CompilerParameters"/> used to build the <see cref="Type"/> of the Haml template.
        /// </summary>
        /// <param name="parameters">The object representing the compiler parameters for the Haml template.</param>
        /// <param name="referencedAssemblyLocations">The location of the assemblies to reference in the Haml template class.</param>
        private static void AddReferences(CompilerParameters parameters, IEnumerable<string> referencedAssemblyLocations)
        {
            parameters.ReferencedAssemblies.Clear();

            foreach (var assemblyLocation in referencedAssemblyLocations)
            {
                parameters.ReferencedAssemblies.Add(assemblyLocation);
            }
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> for the Haml template source file while ensuring no existing file exists.
        /// </summary>
        /// <param name="directoryInfo">The representation of the location of the Haml template source file.</param>
        /// <param name="typeName">The name of the type whose <see cref="FileInfo"/> gets returned.</param>
        /// <returns></returns>
        private FileInfo GetClassFileInfo(FileSystemInfo directoryInfo, string typeName)
        {
            var fileInfo = new FileInfo($"{directoryInfo.FullName}\\{typeName}.{_codeDomProvider.FileExtension}");
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            return fileInfo;
        }

        /// <summary>
        /// Gets the temp folder for Haml template source files.
        /// </summary>
        /// <returns>The <see cref="DirectoryInfo"/> for the temp folder.</returns>
        private static DirectoryInfo GetNHamlTempDirectoryInfo()
        {
            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase.Remove(0, 8);
            var runningFolder = Path.GetDirectoryName(codeBase).Replace(@"\", "_").Replace(":","");
            var nhamlTempPath = Path.Combine(Path.GetTempPath(), "nhamlTemp");
            nhamlTempPath = Path.Combine(nhamlTempPath, runningFolder);
            var directoryInfo = new DirectoryInfo(nhamlTempPath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            Debug.WriteLine($"NHaml temp directory is '{directoryInfo.FullName}'.");
            return directoryInfo;
        }

        protected abstract bool SupportsDebug();

        /// <summary>
        /// Gets the <see cref="Type"/> representation of the class identifier <see cref="string"/> <paramref name="typeName"/> from
        /// the <see cref="Assembly"/> <paramref name="assembly"/>.
        /// </summary>
        /// <param name="typeName">The name of the type to extract from the <paramref name="assembly"/>.</param>
        /// <param name="assembly">The <see cref="Assembly"/> to extract the <param name="typeName"> class from.</param></param>
        /// <returns>The <see cref="Type"/> representation of the <paramref name="typeName"/> value.</returns>
        private static Type ExtractType(string typeName, Assembly assembly)
        {
            return assembly.GetType(typeName);
        }

        /// <summary>
        /// Determines if the results of compilation contains errors.
        /// </summary>
        /// <param name="results">The <see cref="CompilerResults"/> to examine.</param>
        /// <returns>True if <paramref name="results"/> contains a non-warning error, false otherwise.</returns>
        private static bool ContainsErrors(CompilerResults results)
        {
            return results.Errors.Cast<CompilerError>()
                .Any(error => error.IsWarning == false);
        }
    }
}