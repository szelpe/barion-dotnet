using BarionClientLibrary.Operations.Common;
using System.Collections.Generic;

namespace BarionClientLibrary.Operations
{
    /// <summary>
    /// Defines a base class for barion operation results.
    /// </summary>
    public class BarionOperationResult
    {
        /// <summary>
        /// Array of errors returned from the Barion API.
        /// </summary>
        public IReadOnlyList<Error> Errors { get; set; }

        /// <summary>
        /// Returns true if the operation was successful.
        /// </summary>
        public bool IsOperationSuccessful { get; internal set; }

        public BarionOperationResult()
        {
            Errors = new List<Error>();
        }
    }
}