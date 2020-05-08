using System;
using System.IO;

namespace BryanPorter.Parcel
{
    public class PCap
    {
        readonly FileStream _stream;

        public PCap(FileStream stream)
        {
            _stream = stream;
        }

        public Block ParseBlock()
        {
            var type = (BlockType)readInt32(_stream);
            var totalLength = readInt32(_stream); 
            
            switch (type)
            {
                case BlockType.SectionHeader:
                    return new SectionHeader(
                        totalLength,
                        readUInt32(_stream), // BOM,
                        readUInt16(_stream), // major
                        readUInt16(_stream), // minor
                        readInt64(_stream) // section length
                    );
                default:
                    return null;
            }
        }
        
        private static short readInt16(FileStream stream)
        {
            byte[] b = new byte[2];
            stream.Read(b, 0, 2);

            return BitConverter.ToInt16(b, 0);
        }
        
        private static ushort readUInt16(FileStream stream)
        {
            byte[] b = new byte[2];
            stream.Read(b, 0, 2);

            return BitConverter.ToUInt16(b, 0);
        }

        private static int readInt32(FileStream stream)
        {
            byte[] b = new byte[4];
            stream.Read(b, 0, 4);

            return BitConverter.ToInt32(b, 0);
        }
        
        private static uint readUInt32(FileStream stream)
        {
            byte[] b = new byte[4];
            stream.Read(b, 0, 4);

            return BitConverter.ToUInt32(b, 0);
        }
        
        private static long readInt64(FileStream stream)
        {
            byte[] b = new byte[8];
            stream.Read(b, 0, 8);

            return BitConverter.ToInt64(b, 0);
        }
    }
}