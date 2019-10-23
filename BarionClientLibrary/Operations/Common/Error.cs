using System;

namespace BarionClientLibrary.Operations.Common
{
    public class Error
    {
        public string ErrorCode { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public Error(string errorCode)
        {
            ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        }
    }
}
