using BarionClientLibrary.Operations.Common;

namespace BarionClientLibrary.Operations
{
    public class BarionOperationResult
    {
        public Error[] Errors { get; set; }
        public bool IsOperationSuccessful { get; internal set; }
    }
}