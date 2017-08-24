using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHaml.Parser;
using NHaml.Parser.Rules;
using Microsoft.CSharp;

namespace NHaml.Compilers
{
    /// <summary>
    /// This class is responsible for building the <see cref="TemplateBase.Template"/> subclasses.
    /// </summary>
    /// <remarks>
    /// This class has implicit coupling to <see cref="TemplateBase.Template"/>.
    /// </remarks>
    public class CodeDomClassBuilder : ITemplateClassBuilder
    {
        /// <summary>
        /// The name of the <see cref="RenderMethod"/> method parameter.
        /// </summary>
        private const string TextWriterVariableName = "textWriter";

        /// <summary>
        /// The core rendering method to be built for the dynamic Haml template.
        /// </summary>
        private CodeMemberMethod RenderMethod { get; }

        public CodeDomClassBuilder()
        {
            RenderMethod = new CodeMemberMethod
                               {
                                   //TODO: Not a fan of the coupling to the string name "CoreRender". Leaving as is until there's a better solution.
                                   //TODO: This looks like a good candidate for an interface that contains the CoreRender method. Reflection of the method name makes this regression-safe.
                                   Name = "CoreRender",
                                   // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                                   Attributes = MemberAttributes.Override | MemberAttributes.Family,
                               }
                               .WithParameter(typeof(TextWriter), TextWriterVariableName);
        }

        /// <summary>
        /// Renders the end of a code block to the <see cref="RenderMethod"/>.
        /// </summary>
        public void RenderEndBlock()
        {
            AppendCodeSnippet("}//");
        }

        /// <summary>
        /// Renders the beginning of a code block to the <see cref="RenderMethod"/>.
        /// </summary>
        private void RenderBeginBlock()
        {
            AppendCodeSnippet("{//");
        }

        /// <summary>
        /// Merges a list of namespace imports to add
        /// </summary>
        /// <param name="imports"></param>
        /// <returns></returns>
        private static IEnumerable<string> MergeRequiredImports(IEnumerable<string> imports)
        {
            var result = new List<string>(imports);
            if (result.Contains("System") == false)
                result.Add("System");
            return result;
        }

        /// <summary>
        /// Adds an expression statement to the <see cref="RenderMethod"/> that writes to its TextWriter parameter.
        /// </summary>
        /// <param name="line">The <paramref name="line"/> that is to be written in the <see cref="RenderMethod"/>.</param>
        public void Append(string line)
        {
            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokePrimitiveParameter(line);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        /// <summary>
        /// Calls <see cref="Append"/> with a formatted string and parameters.
        /// </summary>
        /// <param name="content">The string format template.</param>
        /// <param name="args">Parameters for the string template.</param>
        public void AppendFormat(string content, params object[] args)
        {
            Append(string.Format(content, args));
        }

        /// <summary>
        /// Adds an expression statement to the <see cref="RenderMethod"/> that writes a new line to its TextWriter parameter.
        /// </summary>
        public void AppendNewLine()
        {
            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("WriteLine", TextWriterVariableName)
                .WithInvokePrimitiveParameter("");

            RenderMethod.AddExpressionStatement(writeInvoke);
        }


        public void AppendCodeToString(string code)
        {
            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeSnippetToStringParameter(code);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        /// <summary>
        /// Appends a <see cref="CodeSnippetExpression"/> to the <see cref="RenderMethod"/>.
        /// </summary>
        /// <param name="code">The code snippet as a string to append.</param>
        /// <param name="containsChildren"></param>
        public void AppendCodeSnippet(string code, bool containsChildren = false)
        {
            if (containsChildren)
            {
                if (IsCodeBlockBeginningElse(code) == false)
                    InitialiseCodeBlock();
                RenderMethod.Statements.Add(
                    new CodeSnippetExpression { Value = code + "//"});
                RenderBeginBlock();
                WriteNewLineIfRepeated();
            }
            else
            {
                RenderMethod.Statements.Add(
                    new CodeSnippetExpression { Value = code });
            }
        }

        private bool IsCodeBlockBeginningElse(string code)
        {
            return (code.ToLower().Trim() + " ").StartsWith("else ");
        }

        private void InitialiseCodeBlock()
        {
            AppendCodeSnippet("HasCodeBlockRepeated = false;");
        }


        private void WriteNewLineIfRepeated()
        {
            //TODO: This is an implicit coupling to the Template.WriteNewLineIfRepeated(TextWriter) method. Need to guard
            //      against refactorings and also allow extensibility for other base types if necessary.
            AppendCodeSnippet("WriteNewLineIfRepeated(textWriter)");
        }

        public void AppendDocType(string docTypeId)
        {
            var docType = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("GetDocType")
                .WithInvokePrimitiveParameter(docTypeId);

            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(docType);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public void AppendVariable(string variableName)
        {
            var renderValueOrKeyAsString = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("RenderValueOrKeyAsString")
                .WithInvokePrimitiveParameter(variableName);

            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(renderValueOrKeyAsString);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        //public void BeginCodeBlock()
        //{
        //    Depth++;
        //    RenderBeginBlock();
        //}

        //public void EndCodeBlock()
        //{
        //    RenderEndBlock();
        //    Depth--;
        //}

        public void AppendAttributeNameValuePair(string name, IList<HamlNode> valueFragments, char quoteToUse)
        {
            if (valueFragments.Any() == false)
                AppendAttributeWithoutValue(name);
            else
                AppenAttributeWithValue(name, valueFragments, quoteToUse);
        }

        private void AppendAttributeWithoutValue(string name)
        {
            RenderMethod.AddExpressionStatement(
                CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                    .WithInvokePrimitiveParameter(" " + name));
        }

        private void AppenAttributeWithValue(string name, IEnumerable<HamlNode> valueFragments, char quoteToUse)
        {
            string variableName = "value_" + RenderMethod.Statements.Count;
            RenderMethod.AddStatement(
                CodeVariableDeclarationFluentBuilder.GetDeclaration(typeof (StringBuilder), variableName,
                                                    new CodeObjectCreateExpression("System.Text.StringBuilder",
                                                                                   new CodeExpression[] {})));

            foreach (var fragment in valueFragments)
            {
                CodeExpression parameter;
                if (fragment is HamlNodeTextVariable)
                {
                    string nodeVariableName = ((HamlNodeTextVariable) fragment).VariableName;
                    if (nodeVariableName.All(ch => Char.IsLetterOrDigit(ch)))
                        parameter = CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression(
                            "base.RenderValueOrKeyAsString")
                            .WithInvokePrimitiveParameter(nodeVariableName);
                    else
                    {
                        parameter = CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression("ToString", "Convert")
                            .WithCodeSnippetParameter(nodeVariableName);
                    }
                }
                else
                {
                    parameter = new CodePrimitiveExpression {Value = fragment.Content};
                }

                RenderMethod.AddExpressionStatement(
                    CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression("Append", variableName)
                        .WithParameter(parameter));
            }

            var outputExpression = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("base.RenderAttributeNameValuePair")
                .WithInvokePrimitiveParameter(name)
                .WithParameter(CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression("ToString", variableName))
                .WithInvokePrimitiveParameter(quoteToUse);

            RenderMethod.AddExpressionStatement(
                CodeMethodInvokeFluentBuilder.GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                    .WithParameter(outputExpression));
        }

        public void AppendSelfClosingTagSuffix()
        {
            var renderValueOrKeyAsString = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("base.AppendSelfClosingTagSuffix");

            var writeInvoke = CodeMethodInvokeFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(renderValueOrKeyAsString);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public string Build(string className)
        {
            return Build(className, typeof(TemplateBase.Template), new List<string>());
        }

        /// <summary>
        /// Builds a source file for the <paramref name="className"/> class.
        /// </summary>
        /// <param name="className">The name of the class to create</param>
        /// <param name="baseType">The base type of <paramref name="className"/>.</param>
        /// <param name="imports">A list of namespace references to import in the returned source file.</param>
        /// <returns>A string representation of the <paramref name="className"/> source file.</returns>
        public string Build(string className, Type baseType, IEnumerable<string> imports)
        {
            imports = MergeRequiredImports(imports);

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {

                var compileUnit = new CodeCompileUnit();

                var testNamespace = new CodeNamespace();
                compileUnit.Namespaces.Add(testNamespace);

                testNamespace.Imports.AddRange(
                    imports.Select(x => new CodeNamespaceImport(x)).ToArray());

                var generator = new CSharpCodeProvider().CreateGenerator(writer);
                var options = new CodeGeneratorOptions();
                var declaration = new CodeTypeDeclaration
                {
                    Name = className,
                    IsClass = true
                };
                declaration.BaseTypes.Add(new CodeTypeReference(baseType));

                declaration.Members.Add(RenderMethod);

                testNamespace.Types.Add(declaration);
                generator.GenerateCodeFromNamespace(testNamespace, writer, options);

                //TODO: implement IDisposable
                writer.Close();
            }

            return builder.ToString();
        }

        public void Clear()
        {
            RenderMethod.Statements.Clear();
        }
    }
}