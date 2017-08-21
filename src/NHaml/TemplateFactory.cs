using System;
using System.Reflection.Emit;
using NHaml.TemplateBase;

namespace NHaml
{
    /// <summary>
    /// Represents errors that occur while creating Haml templates.
    /// </summary>
    [Serializable]
    public class InvalidTemplateTypeException : Exception
    {
        public InvalidTemplateTypeException(Type t) : this(t, string.Empty)
        {
        }

        public InvalidTemplateTypeException(Type t, string message)
            : base($"Attempted to create a template factory using an invalid type {t.FullName}. {message}".Trim())
        { }
    }

    /// <summary>
    /// This class enables the fast creation of Haml templates at runtime.
    /// </summary>
    public class TemplateFactory
    {
        /// <summary>
        /// This is the placeholder for the delegate that contains the instantiation logic of
        /// the <see cref="Template"/> subtype.
        /// </summary>
        private readonly Func<Template> _fastActivator;

        /// <summary>
        /// Constructor for <see cref="TemplateFactory"/> that takes a single <paramref name="templateType"/> parameter.
        /// </summary>
        /// <param name="templateType"></param>
        public TemplateFactory( Type templateType )
        {
            _fastActivator = CreateFastActivator( templateType );
        }

        /// <summary>
        /// Creates a template from the type specified in this instance of <see cref="TemplateFactory"/>.
        /// </summary>
        /// <returns>A Haml template</returns>
        public Template CreateTemplate()
        {
            return _fastActivator();
        }

        /// <summary>
        /// Creates dynamic method "activatefast__" in the <paramref name="type"/> class. "activatefast__"
        /// returns a new instance of <paramref name="type"/> via the default constructor. 
        /// </summary>
        /// <remarks>
        /// Since the "activatefast__" method is dynamic, it only exists at the time <see cref="CreateFastActivator"/>
        /// is run.
        /// </remarks>
        /// <param name="type">The type that is to have the dynamic method appended.</param>
        /// <returns>A delegate of the dynamic method modified to return the <see cref="Template"/> type.</returns>
        private static Func<Template> CreateFastActivator(Type type)
        {
            var constructor = type.GetConstructor( new Type[] { } );
            if (constructor == null)
                throw new InvalidTemplateTypeException(type, $"The type {type} must be a class with a default constructor.");

            var dynamicMethod = new DynamicMethod( "activatefast__", type, null, type );
            var ilGenerator = dynamicMethod.GetILGenerator();

            ilGenerator.Emit( OpCodes.Newobj, constructor );
            ilGenerator.Emit( OpCodes.Ret );

            return (Func<Template>)dynamicMethod.CreateDelegate(typeof(Func<Template>));
        }
    }
}