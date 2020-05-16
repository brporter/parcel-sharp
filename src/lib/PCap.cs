using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
                var returnValue = default(Block);
                var type = (BlockType)readInt32(_stream, true);

                switch (type)
                {
                    case BlockType.SectionHeader:
                        returnValue = ParseSectionHeader(_stream);
                        break;
                    default:
                        returnValue = ParseGenericBlock(_stream);
                        break;
                }

                return returnValue;
            }

            private static Block ParseGenericBlock(Stream stream)
            {
                var type = (BlockType)readInt32(stream, false);
                var totalLength = readInt32(stream, false);

                stream.Seek(totalLength - 8, SeekOrigin.Current);

                return new GenericBlock(
                    type,
                    totalLength
                );
            }

            private static Block ParseSectionHeader(Stream stream)
            {
                var type = (BlockType)readInt32(stream, false);
                var totalLength = readInt32(stream, false);
                var bom = readUInt32(stream, false);
                var major = readUInt16(stream, false);
                var minor = readUInt16(stream, false);
                var sectionLength = readInt64(stream, false);
                var options = ParseOptions<SectionHeader>(stream);

                stream.Seek(4, SeekOrigin.Current);

                return new SectionHeader(totalLength, options, bom, major, minor, sectionLength);
            }

            private static Option[] ParseOptions<T>(Stream stream)
            {
                List<Option> retVal = new List<Option>();

                ushort optionCode = 0;
                ushort optionLength = 0;
                byte[] optionValue = null;

                do 
                {
                    optionCode = readUInt16(stream, false);
                    optionLength = readUInt16(stream, false);
                    optionValue = new byte[optionLength];

                    stream.Read(optionValue, 0, optionLength);

                    Option opt = null;

                    if (typeof(T) == typeof(SectionHeader))
                    {
                        switch (optionCode)
                        {
                            case 2:
                                opt = new Option<string>(System.Text.Encoding.UTF8.GetString(optionValue), OptionType.Hardware, optionValue, optionLength);
                                break;
                            case 3:
                                opt = new Option<string>(System.Text.Encoding.UTF8.GetString(optionValue), OptionType.OperatingSystem, optionValue, optionLength);
                                break;
                            case 4:
                                opt = new Option<string>(System.Text.Encoding.UTF8.GetString(optionValue), OptionType.UserApplication, optionValue, optionLength);
                                break;
                        }
                    }

                    // switch (optionCode)
                    // {
                    //     case 2:
                    //         opt = new Option<string>(InterfaceOptionType.InterfaceName, optionLength, System.Text.Encoding.UTF8.GetString(optionValue));
                    //         break;
                    //     case 3: 
                    //         opt = new Option<string>(InterfaceOptionType.InterfaceDescription, optionLength, System.Text.Encoding.UTF8.GetString(optionValue));
                    //         break;
                    //     case 4:
                    //         var buf = new byte[4];
                    //         int parsedAddressIndex = 0;
                    //         IPAddress[] parsedAddresses = new IPAddress[optionLength / 4];
                    //         for (int i = 0; i < optionLength && parsedAddressIndex < parsedAddresses.Length; i += 4, parsedAddressIndex++)
                    //         {
                    //             Array.Copy(optionValue, i, buf, 0, 4);
                    //             parsedAddresses[parsedAddressIndex] = new IPAddress(buf);
                    //         }
                    //         opt = new Option<IPAddress[]>(InterfaceOptionType.IPv4Address, optionLength, parsedAddresses);
                    //         break;
                    //     default:
                    //         opt = new Option<byte[]>(InterfaceOptionType.Unknown, optionLength, optionValue);
                    //         break;
                    // }

                    if (opt != null)
                    {
                        retVal.Add(opt);
                        stream.Seek(opt.PaddedLength - optionValue.Length, SeekOrigin.Current);
                    }
                } while (optionCode != 0);

                return retVal.ToArray();
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

            private static short readInt16(Stream stream, bool noSeek)
            {
                byte[] b = new byte[2];
                stream.Read(b, 0, 2);

                if (noSeek)
                    stream.Seek(-2, SeekOrigin.Current);

                return BitConverter.ToInt16(b, 0);
            }
            
            private static ushort readUInt16(Stream stream, bool noSeek)
            {
                byte[] b = new byte[2];
                stream.Read(b, 0, 2);

                if (noSeek)
                    stream.Seek(-2, SeekOrigin.Current);

                return BitConverter.ToUInt16(b, 0);
            }

            private static int readInt32(Stream stream, bool noSeek)
            {
                byte[] b = new byte[4];
                stream.Read(b, 0, 4);

                if (noSeek)
                    stream.Seek(-4, SeekOrigin.Current);

                return BitConverter.ToInt32(b, 0);
            }
            
            private static uint readUInt32(Stream stream, bool noSeek)
            {
                byte[] b = new byte[4];
                stream.Read(b, 0, 4);

                if (noSeek)
                    stream.Seek(-4, SeekOrigin.Current);

                return BitConverter.ToUInt32(b, 0);
            }
            
            private static long readInt64(Stream stream, bool noSeek)
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