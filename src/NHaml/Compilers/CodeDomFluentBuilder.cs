using System;
using System.CodeDom;

namespace NHaml.Compilers
{
    /// <summary>
    /// This is a wrapper class for fluently building 
    /// </summary>
    internal static class CodeDomFluentBuilder
    {
        /// <summary>
        /// Creates a method invocation expression that calls the <paramref name="methodName"/> of the <paramref name="targetObject"/>.
        /// </summary>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="targetObject">The variable name of the object to call its <paramref name="methodName"/> on.</param>
        /// <returns>The CodeDom representation of the method invocation expression.</returns>
        public static CodeMethodInvokeExpression GetCodeMethodInvokeExpression(string methodName, string targetObject)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = methodName,
                    TargetObject =
                        new CodeVariableReferenceExpression { VariableName = targetObject }
                }
            };
            return result;
        }

        /// <summary>
        /// Creates a method invocation expression that implicitly calls <paramref name="methodName"/>.
        /// </summary>
        /// <param name="methodName">The name of the method to call.</param>
        /// <returns>The CodeDom representation of the method invocation expression.</returns>
        public static CodeMethodInvokeExpression GetCodeMethodInvokeExpression(string methodName)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = methodName
                }
            };
            return result;
        }

        /// <summary>
        /// Creates a variable declaration statement.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the variable.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="valueExpression">The expression whose results are assigned to the variable.</param>
        /// <returns>The CodeDom representation of the variable declaration statement.</returns>
        public static CodeVariableDeclarationStatement GetDeclaration(Type type, string name, CodeExpression valueExpression)
        {
            return new CodeVariableDeclarationStatement(type, name, valueExpression);
        }

        /// <summary>
        /// Adds parameter to a <see cref="CodeMethodInvokeExpression"/> fluently.
        /// </summary>
        /// <param name="expression">The method invocation expression.</param>
        /// <param name="parameter">The parameter to add to the method invocation.</param>
        /// <returns>The CodeDom representation of the method invocation expression.</returns>
        public static CodeMethodInvokeExpression WithParameter(this CodeMethodInvokeExpression expression, CodeExpression parameter)
        {
            expression.Parameters.Add(parameter);
            return expression;
        }

        /// <summary>
        /// Adds a primitive parameter to a <see cref="CodeMethodInvokeExpression"/> fluently.
        /// </summary>
        /// <remarks>
        /// TODO: Describe what exception gets thrown if <paramref name="parameter"/> isn't primitive.
        /// </remarks>
        /// <param name="expression">The method invocation expression.</param>
        /// <param name="parameter">The primitive parameter to add to the method invocation.</param>
        /// <returns>The CodeDom representation of the method invocation expression.</returns>
        public static CodeMethodInvokeExpression WithInvokePrimitiveParameter(this CodeMethodInvokeExpression expression, object parameter)
        {
            //TODO: Will the default throw be sufficient if the parameter isn't a primitive? Maybe we should catch and throw with a descriptive exception here?
            return WithParameter(expression, new CodePrimitiveExpression { Value = parameter });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="codeSnippet"></param>
        /// <returns></returns>
        public static CodeMethodInvokeExpression WithInvokeCodeSnippetToStringParameter(this CodeMethodInvokeExpression expression,
            string codeSnippet)
        {
            return WithParameter(expression, 
                GetCodeMethodInvokeExpression("ToString", "Convert").WithCodeSnippetParameter(codeSnippet));
        }

        public static CodeMethodInvokeExpression WithCodeSnippetParameter(this CodeMethodInvokeExpression expression,
            string codeSnippet)
        {
            return WithParameter(expression, new CodeSnippetExpression(codeSnippet));
        }

        public static CodeMethodInvokeExpression WithInvokeCodeParameter(this CodeMethodInvokeExpression expression,
            CodeMethodInvokeExpression expressionToInvoke)
        {
            return WithParameter(expression, expressionToInvoke);
        }

        private static CodeExpressionStatement GetExpressionStatement(CodeMethodInvokeExpression writeInvoke)
        {
            return new CodeExpressionStatement { Expression = writeInvoke };
        }

        public static CodeMemberMethod WithParameter(this CodeMemberMethod method, Type type, string name)
        {
            method.Parameters.Add(new CodeParameterDeclarationExpression(type, name));
            return method;
        }

        public static void AddExpressionStatement(this CodeMemberMethod method, CodeMethodInvokeExpression expression)
        {
            method.Statements.Add(GetExpressionStatement(expression));
        }

        public static void AddStatement(this CodeMemberMethod method, CodeStatement statement)
        {
            method.Statements.Add(statement);
        }
    }
}
