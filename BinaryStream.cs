using System.Buffers.Binary;

namespace SkyWing.Binary; 

public class BinaryStream {

	public MemoryStream Buffer;
	
	public BinaryReader Reader { get; }
	public BinaryWriter Writer { get; }

	public BinaryStream() {
		Buffer = new MemoryStream();
		Reader = new BinaryReader(Buffer);
		Writer = new BinaryWriter(Buffer);
	}
	
	public BinaryStream(byte[] buffer) {
		Buffer = new MemoryStream(buffer);
		Reader = new BinaryReader(Buffer);
		Writer = new BinaryWriter(Buffer);
	}
	
	public BinaryStream(int size) {
		Buffer = new MemoryStream(size);
		Reader = new BinaryReader(Buffer);
		Writer = new BinaryWriter(Buffer);
	}

	public BinaryStream(MemoryStream stream) {
		Buffer = stream;
		Reader = new BinaryReader(stream);
		Writer = new BinaryWriter(stream);
	}

	#region Signing and Unsigning

	public static sbyte SignByte(byte value) {
		return (sbyte) (value << 56 >> 56);
	}
	
	public static byte UnSignByte(sbyte value) {
		return (byte) (value & 0xff);
	}
	
	public static short SignShort(ushort value) {
		return (short) (value << 48 >> 48);
	}
	
	public static ushort UnSignShort(short value) {
		return (ushort) (value & 0xffff);
	}
	
	public static int SignInt(uint value) {
		return (int) (value << 32 >> 32);
	}
	
	public static uint UnSignInt(int value) {
		return (uint) (value & 0xffffffff);
	}

	#endregion

	#region Buffer Pointer
	
	public long Position {
		get => Buffer.Position;
		set => Buffer.Position = value;
	}
	
	public long Length => Buffer.Length;
	
	public void Rewind() {
		Buffer.Position = 0;
	}
	
	#endregion

	public byte ReadByte() {
		return Reader.ReadByte();
	}

	public void WriteByte(byte value) {
		Writer.Write(value);
	}

	public byte[] ReadBytes(int count) {
		return Reader.ReadBytes(count);
	}
	
	public byte[] GetRemainingBytes() {
		return Reader.ReadBytes((int) (Length - Position));
	}

	public byte[] GetBuffer() {
		return Buffer.GetBuffer();
	}
	
	public void WriteBytes(byte[] value) {
		Writer.Write(value);
	}

	public bool GetBool() {
		return Reader.ReadBoolean();
	}
	
	public void WriteBool(bool value) {
		Writer.Write(value);
	}

	// Reads a signed int16 from 2 bytes of big endian
	public short ReadShort() {
		return BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
	}
	
	// Reads a unsigned int16 from 2 bytes of big endian
	public short ReadSignedShort() {
		return SignShort(BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2)));
	}
	
	public void WriteShort(short value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt16BigEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a signed int16 from 2 bytes of little endian
	public short ReadLShort() {
		return BinaryPrimitives.ReadInt16LittleEndian(ReadBytes(2));
	}
	
	// Reads a unsigned int16 from 2 bytes of little endian
	public short ReadSignedLShort() {
		return SignShort(BinaryPrimitives.ReadUInt16LittleEndian(ReadBytes(2)));
	}
	
	public void WriteLShort(short value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	public Int24 ReadInt24() {
		return new Int24(ReadBytes(3).Reverse().ToArray());
	}
	
	public Int24 ReadLInt24() {
		return new Int24(ReadBytes(3));
	}
	
	public void WriteInt24(Int24 value) {
		WriteBytes(value.GetBytes().Reverse().ToArray());
	}
	
	public void WriteLInt24(Int24 value) {
		WriteBytes(value.GetBytes());
	}

	// Reads a signed int32 from 4 bytes of big endian
	public int ReadInt() {
		return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
	}

	public void WriteInt(int value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt32BigEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a signed int32 from 4 bytes of little endian
	public int ReadLInt() {
		return BinaryPrimitives.ReadInt32LittleEndian(ReadBytes(4));
	}

	public void WriteLInt(int value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a signed long from 8 bytes of big endian
	public long ReadLong() {
		return BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));
	}

	public void WriteLong(long value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt64BigEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a signed long from 8 bytes of little endian
	public long ReadLLong() {
		return BinaryPrimitives.ReadInt64LittleEndian(ReadBytes(8));
	}

	public void WriteLLong(long value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a float from 4 bytes of big endian
	public float ReadFloat() {
		return BinaryPrimitives.ReadSingleBigEndian(ReadBytes(4));
	}

	// Reads and rounds a float from 4 bytes of big endian
	public float ReadRoundedFloat(int accuracy) {
		return MathF.Round(ReadFloat(), accuracy);
	}

	// Reads a float from 4 bytes of little endian
	public float ReadLFloat() {
		return BinaryPrimitives.ReadSingleLittleEndian(ReadBytes(4));
	}
	
	// Reads and rounds a float from 4 bytes of little endian
	public float ReadRoundedLFloat(int accuracy) {
		return MathF.Round(ReadLFloat(), accuracy);
	}
	
	public void WriteFloat(float value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteSingleBigEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}
	
	public void WriteLFloat(float value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteSingleLittleEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a double from 8 bytes of big endian
	public double ReadDouble() {
		return BinaryPrimitives.ReadDoubleBigEndian(ReadBytes(8));
	}
	
	// Reads a double from 8 bytes of little endian
	public double ReadLDouble() {
		return BinaryPrimitives.ReadDoubleLittleEndian(ReadBytes(8));
	}
	
	public void WriteDouble(double value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteDoubleBigEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}
	
	public void WriteLDouble(double value) {
		var bytes = new Span<byte>();
		BinaryPrimitives.WriteDoubleLittleEndian(bytes, value);
		WriteBytes(bytes.ToArray());
	}

	// Reads a 32-bit variable-length signed integer.
	public int ReadVarInt() {
		return VarInt.ReadInt32(Reader);
	}
	
	// Reads a 32-bit variable-length unsigned integer.
	public uint ReadUnsignedVarInt() {
		return VarInt.ReadUInt32(Reader);
	}
	
	// Writes a 32-bit variable-length signed integer.
	public void WriteVarInt(int value) {
		VarInt.WriteInt32(Writer, value);
	}
	
	// Writes a 32-bit variable-length unsigned integer.
	public void WriteUnsignedVarInt(uint value) {
		VarInt.WriteUInt32(Writer, value);
	}
	
	// Reads a 64-bit variable-length signed integer.
	public long ReadVarLong() {
		return VarInt.ReadInt64(Reader);
	}
	
	// Reads a 64-bit variable-length unsigned integer.
	public ulong ReadUnsignedVarLong() {
		return VarInt.ReadUInt64(Reader);
	}
	
	// Writes a 64-bit variable-length signed integer.
	public void WriteVarLong(long value) {
		VarInt.WriteInt64(Writer, value);
	}
	
	// Writes a 64-bit variable-length unsigned integer.
	public void WriteUnsignedVarLong(ulong value) {
		VarInt.WriteUInt64(Writer, value);
	}

	public bool Feof() {
		return Buffer.Position >= Buffer.Length;
	}
}

public struct Int24 : IComparable // later , IConvertible
{
	private int _value;

	public Int24(ReadOnlySpan<byte> value) {
		_value = ToInt24(value).IntValue();
	}

	public Int24(int value) {
		_value = value;
	}

	private static Int24 ToInt24(ReadOnlySpan<byte> value) {
		if (value.Length > 3) throw new ArgumentOutOfRangeException();
		return new Int24(value[0] | value[1] << 8 | value[2] << 16);
	}

	public byte[] GetBytes() {
		return FromInt(_value);
	}

	public int IntValue() {
		return _value;
	}

	public static byte[] FromInt(int value) {
		var buffer = new byte[3];
		buffer[0] = (byte) value;
		buffer[1] = (byte) (value >> 8);
		buffer[2] = (byte) (value >> 16);
		return buffer;
	}

	public static byte[] FromInt24(Int24 value) {
		var buffer = new byte[3];
		buffer[0] = (byte) value.IntValue();
		buffer[1] = (byte) (value.IntValue() >> 8);
		buffer[2] = (byte) (value.IntValue() >> 16);
		return buffer;
	}

	public int CompareTo(object value) {
		return _value.CompareTo(value);
	}

	public static explicit operator Int24(byte[] values) {
		return new Int24(values);
	}

	public static implicit operator Int24(int value) {
		return new Int24(value);
	}

	public static explicit operator byte[](Int24 d) {
		return d.GetBytes();
	}

	public static implicit operator int(Int24 d) {
		return d.IntValue(); // implicit conversion
	}

	public override string ToString() {
		return _value.ToString();
	}
}

public static class VarInt {
	private static uint EncodeZigZag32(int n) {
		// Note:  the right-shift must be arithmetic
		return (uint) ((n << 1) ^ (n >> 31));
	}

	private static int DecodeZigZag32(uint n) {
		return (int) (n >> 1) ^ -(int) (n & 1);
	}

	private static ulong EncodeZigZag64(long n) {
		return (ulong) ((n << 1) ^ (n >> 63));
	}

	private static long DecodeZigZag64(ulong n) {
		return (long) (n >> 1) ^ -(long) (n & 1);
	}

	private static uint ReadRawVarInt32(BinaryReader buf, int maxSize) {
		uint result = 0;
		var j = 0;
		int b0;

		do {
			b0 = buf.ReadByte();
			result |= (uint) (b0 & 0x7f) << j++ * 7;
			if (j > maxSize)
				throw new OverflowException("VarInt too big");
		} while ((b0 & 0x80) == 0x80);

		return result;
	}

	private static ulong ReadRawVarInt64(BinaryReader buf, int maxSize) {
		ulong result = 0;
		var j = 0;
		int b0;

		do {
			b0 = buf.ReadByte();
			result |= (ulong) (b0 & 0x7f) << j++ * 7;
			if (j > maxSize)
				throw new OverflowException("VarInt too big");
		} while ((b0 & 0x80) == 0x80);

		return result;
	}

	private static void WriteRawVarInt32(BinaryWriter buf, uint value) {
		while ((value & -128) != 0) {
			buf.Write((byte) ((value & 0x7F) | 0x80));
			value >>= 7;
		}

		buf.Write((byte) value);
	}

	private static void WriteRawVarInt64(BinaryWriter buf, ulong value) {
		while ((value & 0xFFFFFFFFFFFFFF80) != 0) {
			buf.Write((byte) ((value & 0x7F) | 0x80));
			value >>= 7;
		}

		buf.Write((byte) value);
	}

	// Int

	public static void WriteInt32(BinaryWriter stream, int value) {
		WriteRawVarInt32(stream, (uint) value);
	}

	public static int ReadInt32(BinaryReader stream) {
		return (int) ReadRawVarInt32(stream, 5);
	}

	public static void WriteSInt32(BinaryWriter stream, int value) {
		WriteRawVarInt32(stream, EncodeZigZag32(value));
	}

	public static int ReadSInt32(BinaryReader stream) {
		return DecodeZigZag32(ReadRawVarInt32(stream, 5));
	}

	public static void WriteUInt32(BinaryWriter stream, uint value) {
		WriteRawVarInt32(stream, value);
	}

	public static uint ReadUInt32(BinaryReader stream) {
		return ReadRawVarInt32(stream, 5);
	}

	// Long

	public static void WriteInt64(BinaryWriter stream, long value) {
		WriteRawVarInt64(stream, (ulong) value);
	}

	public static long ReadInt64(BinaryReader stream) {
		return (long) ReadRawVarInt64(stream, 10);
	}

	public static void WriteSInt64(BinaryWriter stream, long value) {
		WriteRawVarInt64(stream, EncodeZigZag64(value));
	}

	public static long ReadSInt64(BinaryReader stream) {
		return DecodeZigZag64(ReadRawVarInt64(stream, 10));
	}

	public static void WriteUInt64(BinaryWriter stream, ulong value) {
		WriteRawVarInt64(stream, value);
	}

	public static ulong ReadUInt64(BinaryReader stream) {
		return ReadRawVarInt64(stream, 10);
	}
}