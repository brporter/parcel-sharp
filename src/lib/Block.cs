namespace BryanPorter.Parcel
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public enum BlockType
    {
        SectionHeader = 0x0A0D0D0A,
        InterfaceDescription = 0x00000001,
        EnhancedPacket = 0x00000006,
        SimplePacket = 0x00000003,
        NameResolution = 0x00000004,
        InterfaceStatistics = 0x00000005,
        Custom = 0x00000BAD,
        CustomNoCopy = 0x40000BAD
    }

    public interface IBlock
    {
        BlockType Type { get; }
        int BlockLength { get; }
        Option[] Options { get; }
    }

    public abstract class Block
        : IBlock
    {
        public Block(BlockType type, int length, Option[] options)
        {
            Type = type;
            BlockLength = length;
            Options = options;
        }

        public BlockType Type { get; private set; }
        public int BlockLength { get; private set; }
        public Option[] Options { get; private set;}
    }

    public sealed class GenericBlock
        : Block
    {
        public GenericBlock(BlockType type, int length)
            : base(type, length, null)
        { }
    }

    public sealed class SectionHeader
        : Block
    {
        public SectionHeader(int length, Option[] options, uint bom, ushort major, ushort minor, long sectionLength)
            : base(BlockType.SectionHeader, length, options)
        {
            ByteOrderMagic = bom;
            MajorVersion = major;
            MinorVersion = minor;
            SectionLength = sectionLength;
        }

        public uint ByteOrderMagic { get; private set; }
        public ushort MajorVersion { get; private set; }
        public ushort MinorVersion { get; private set; }
        public long SectionLength { get; private set; }
    }

    public enum OptionType
    {
        Unknown,
        Hardware,
        OperatingSystem,
        UserApplication,
        InterfaceName,
        InterfaceDescription,
        IPv4Address,
        IPv6Address,
        MACAddress,
        EUIAddress,
        LinkSpeed,
        TimestampResolution,
        TimeZone,
        Filter,
        FrameCheckSequenceLength,
        TimestampOffset
    }

    public interface IOption
    {
        int PaddedLength { get; }
        byte[] ValueBytes { get; }
        OptionType Type { get; }

        string ToString();
    }

    public interface IOption<T>
        : IOption
    {
        T Value { get; }
    }

    public abstract class Option 
        : IOption
    {
        readonly int _valueLength;

        public Option(OptionType type, int valueLength, byte[] valueBytes)
        {
            Type = type;
            _valueLength = valueLength;
            ValueBytes = valueBytes;
        }

        public int PaddedLength
        {
            get
            {
                if (_valueLength > 0)
                    return _valueLength + (4 - (_valueLength % 4));

                return 0;
            }
        }

        public byte[] ValueBytes { get; }

        public OptionType Type { get; }

        public override abstract string ToString();
    }

    public class Option<T>
        : Option, IOption<T>
    {
        public Option(T value, OptionType type, byte[] valueBytes, int valueLength)
            : base(type, valueLength, valueBytes)
        {
            Value = value;
        }

        public T Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
