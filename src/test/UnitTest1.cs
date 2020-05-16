using System;
using Xunit;
using BryanPorter.Parcel;

namespace test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var f = System.IO.File.OpenRead("/home/brporter/projects/parcel-sharp/samples/sample2.pcapng");
            var p = new PCap(f);

            foreach (var b in p)
            {
                var shb = b as SectionHeader;
                if (shb != null)
                {
                    Console.WriteLine($"{shb.Type}: {shb.MajorVersion}.{shb.MinorVersion}, {shb.SectionLength} bytes of content, {shb.Options.Length} options specified.");
                    foreach (var opt in shb.Options)
                    {
                        Console.WriteLine(
                            $"{Enum.GetName(typeof(OptionType), opt.Type)}: {opt.ToString()}"
                        );
                    }
                }
                
                Console.WriteLine($"Block is of type {b.Type} with length {b.BlockLength}");
            }
        }
    }
}
