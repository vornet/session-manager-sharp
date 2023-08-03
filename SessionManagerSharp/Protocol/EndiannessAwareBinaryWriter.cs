using System.Buffers.Binary;
using System.Text;

namespace VorNet.SessionManagerSharp.Protocol
{
    internal class EndiannessAwareBinaryWriter : BinaryWriter
    {
        public enum Endianness
        {
            Little,
            Big,
        }

        private readonly Endianness _endianness = Endianness.Little;

        public EndiannessAwareBinaryWriter(Stream input) : base(input)
        {
        }

        public EndiannessAwareBinaryWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public EndiannessAwareBinaryWriter(Stream input, Encoding encoding, bool leaveOpen) : base(
            input, encoding, leaveOpen)
        {
        }

        public EndiannessAwareBinaryWriter(Stream input, Endianness endianness) : base(input)
        {
            _endianness = endianness;
        }

        public EndiannessAwareBinaryWriter(Stream input, Encoding encoding, Endianness endianness) :
            base(input, encoding)
        {
            _endianness = endianness;
        }

        public EndiannessAwareBinaryWriter(Stream input, Encoding encoding, bool leaveOpen,
            Endianness endianness) : base(input, encoding, leaveOpen)
        {
            _endianness = endianness;
        }

        public override void Write(short value)
        {
            byte[] buffer = new byte[2];

            if (_endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt16LittleEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt16BigEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
        }

        public override void Write(int value)
        {
            byte[] buffer = new byte[4];

            if (_endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt32LittleEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
        }

        public override void Write(uint value)
        {
            byte[] buffer = new byte[4];

            if (_endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
        }

        public override void Write(long value)
        {
            byte[] buffer = new byte[8];

            if (_endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteInt64LittleEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
        }

        public override void Write(ulong value)
        {
            byte[] buffer = new byte[8];

            if (_endianness == Endianness.Little)
            {
                BinaryPrimitives.WriteUInt64LittleEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(new Span<byte>(buffer), value);
                base.Write(buffer);
            }
        }
    }
}
