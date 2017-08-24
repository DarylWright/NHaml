using System;

namespace NHaml.Crosscutting
{
    /// <summary>
    /// Provides guard statements for method parameters.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public static class Invariant
    {
        /// <summary>
        /// Checks an <paramref name="argument"/> for null and throws an exception if true.
        /// </summary>
        /// <param name="argument">The argument to null-check.</param>
        /// <param name="argumentName">The name of the argument in the calling scope.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        public static void ArgumentNotNull( object argument, string argumentName )
        {
            if( argument == null )
                throw new ArgumentNullException( argumentName );
        }

        /// <summary>
        /// Checks an <paramref name="argument"/> for null and zero-length; throws exceptions for either case being true.
        /// </summary>
        /// <param name="argument">The argument to null-check or empty-check.</param>
        /// <param name="argumentName">The name of the argument in the calling scope.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="argument"/> is zero-length.</exception>
        public static void ArgumentNotEmpty( string argument, string argumentName )
        {
            ArgumentNotNull(argument, argumentName);

            if( argument.Length == 0 )
                throw new ArgumentOutOfRangeException(
                    string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "The provided string argument '{0}' cannot be empty", argumentName));
        }
    }
}