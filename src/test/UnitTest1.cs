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

            var b = p.ParseBlock();
        }
    }
}
