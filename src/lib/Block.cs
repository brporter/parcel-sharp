using System;

namespace BryanPorter.Parcel
{
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
    }

    public abstract class Block
        : IBlock
    {
        public Block(BlockType type, int length)
        {
            Type = type;
            BlockLength = length;
        }

        public BlockType Type { get; private set; }
        public int BlockLength { get; private set; }
    }

    public sealed class SectionHeader
        : Block
    {
        public SectionHeader(int length, uint bom, ushort major, ushort minor, long sectionLength)
            : base(BlockType.SectionHeader, length)
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

        // todo: options
    }
}
