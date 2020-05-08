using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BryanPorter.Parcel
{
    public class PCap
        : System.Collections.Generic.IReadOnlyCollection<Block>
    {
        readonly FileStream _stream;

        public int Count => throw new NotImplementedException();

        public PCap(FileStream stream)
        {
            _stream = stream;
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return new PCapEnumerator(this._stream);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        private class PCapEnumerator
            : IEnumerator<Block>
        {
            readonly FileStream _stream;
            Block _current = null;

            public PCapEnumerator(FileStream stream)
            {
                _stream = stream;
            }

            private Block ParseBlock()
            {
                var type = (BlockType)readInt32(_stream, false);
                var totalLength = readInt32(_stream, false); 

                Block returnValue = null;

                switch (type)
                {
                    case BlockType.SectionHeader:
                        returnValue = new SectionHeader(
                            totalLength,
                            readUInt32(_stream, false), // BOM,
                            readUInt16(_stream, false), // major
                            readUInt16(_stream, false), // minor
                            readInt64(_stream, false) // section length
                        );

                        _stream.Seek(totalLength - 24, SeekOrigin.Current);
                        break;
                    default:
                        returnValue = new GenericBlock(
                            type,
                            totalLength
                        );

                        _stream.Seek(totalLength - 8, SeekOrigin.Current);
                        break;
                }

                return returnValue;
            }

            public Block Current {
                get {
                    return _current;
                }
            }

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            { }

            public bool MoveNext()
            {
                if (_stream.Position != _stream.Length)
                {
                    _current = ParseBlock();
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _stream.Seek(0, SeekOrigin.Begin);
            }

            private static short readInt16(FileStream stream, bool noSeek)
            {
                byte[] b = new byte[2];
                stream.Read(b, 0, 2);

                if (noSeek)
                    stream.Seek(-2, SeekOrigin.Current);

                return BitConverter.ToInt16(b, 0);
            }
            
            private static ushort readUInt16(FileStream stream, bool noSeek)
            {
                byte[] b = new byte[2];
                stream.Read(b, 0, 2);

                if (noSeek)
                    stream.Seek(-2, SeekOrigin.Current);

                return BitConverter.ToUInt16(b, 0);
            }

            private static int readInt32(FileStream stream, bool noSeek)
            {
                byte[] b = new byte[4];
                stream.Read(b, 0, 4);

                if (noSeek)
                    stream.Seek(-4, SeekOrigin.Current);

                return BitConverter.ToInt32(b, 0);
            }
            
            private static uint readUInt32(FileStream stream, bool noSeek)
            {
                byte[] b = new byte[4];
                stream.Read(b, 0, 4);

                if (noSeek)
                    stream.Seek(-4, SeekOrigin.Current);

                return BitConverter.ToUInt32(b, 0);
            }
            
            private static long readInt64(FileStream stream, bool noSeek)
            {
                byte[] b = new byte[8];
                stream.Read(b, 0, 8);

                if (noSeek)
                    stream.Seek(-8, SeekOrigin.Current);

                return BitConverter.ToInt64(b, 0);
            }
        }
    }
}