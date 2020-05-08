using System;
using Xunit;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var f = System.IO.File.OpenRead("/home/brporter/projects/parcel-sharp/samples/sample.pcapng");
            var p = new BryanPorter.Parcel.PCap(f);

            foreach (var b in p)
            {
                Console.WriteLine($"Block is of type {b.Type} with length {b.BlockLength}");
            }
        }
    }
}
