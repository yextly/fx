// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

namespace Yextly.Scripting.Abstractions
{
    /// <summary>
    /// Represents an exception thrown when the source text cannot be compiled.
    /// </summary>
    [Serializable]
    public sealed class InvalidSourceTextException : ScriptingEngineException
    {
        private const string DefaultExceptionMessage = "The provided text cannot be compiled.";

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public InvalidSourceTextException() : base(DefaultExceptionMessage)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidSourceTextException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidSourceTextException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
