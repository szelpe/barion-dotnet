using BarionClientLibrary.Operations.Common;

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
        public Error[] Errors { get; set; }

        /// <summary>
        /// Returns true if the operation was successful.
        /// </summary>
        public bool IsOperationSuccessful { get; internal set; }
    }
}