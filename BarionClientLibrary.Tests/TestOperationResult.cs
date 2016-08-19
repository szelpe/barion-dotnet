using BarionClientLibrary.Operations;
using System;
using System.Globalization;

namespace BarionClientLibrary.Tests
{
    public class TestOperationResult : BarionOperationResult
    {
        public int IntProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public double DoubleProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public string StringProperty { get; set; }
        public ConsoleColor EnumProperty { get; set; }
        public CultureInfo CultureInfoProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public TimeSpan TimeSpanProtperty { get; set; }
    }
}