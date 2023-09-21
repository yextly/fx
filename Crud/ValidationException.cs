// ==++==
//
//   Copyright (c) Shadowsoft Corporation.  All rights reserved.
//
// ==--==

using System.Runtime.Serialization;

namespace Yextly.ServiceFabric.Mvc.Crud
{
    /// <summary>
    /// Represents an exception throw entity validation fails.
    /// </summary>
    [Serializable]
    public sealed class ValidationException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="ValidationException"/> with the default message.
        /// </summary>
        public ValidationException()
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ValidationException"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ValidationException(string message, Exception inner) : base(message, inner)
        {
        }

        private ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
