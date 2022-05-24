namespace SkyWing.Binary;

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

	private static uint ReadRawVarInt32(Reader buf, int maxSize) {
		uint result = 0;
		int j = 0;
		int b0;

		do {
			b0 = buf.ReadByte();
			result |= (uint) (b0 & 0x7f) << j++ * 7;
			if (j > maxSize)
				throw new OverflowException("VarInt too big");
		} while ((b0 & 0x80) == 0x80);

		return result;
	}

	private static ulong ReadRawVarInt64(Reader buf, int maxSize) {
		ulong result = 0;
		int j = 0;
		int b0;

		do {
			b0 = buf.ReadByte();
			result |= (ulong) (b0 & 0x7f) << j++ * 7;
			if (j > maxSize)
				throw new OverflowException("VarInt too big");
		} while ((b0 & 0x80) == 0x80);

		return result;
	}

	private static void WriteRawVarInt32(Writer buf, uint value) {
		while ((value & -128) != 0) {
			buf.Write((byte) ((value & 0x7F) | 0x80));
			value >>= 7;
		}

		buf.WriteByte((byte) value);
	}

	private static void WriteRawVarInt64(Writer buf, ulong value) {
		while ((value & 0xFFFFFFFFFFFFFF80) != 0) {
			buf.WriteByte((byte) ((value & 0x7F) | 0x80));
			value >>= 7;
		}

		buf.WriteByte((byte) value);
	}

	// Int

	public static void WriteInt32(Writer stream, int value) {
		WriteRawVarInt32(stream, (uint) value);
	}

	public static int ReadInt32(Reader stream) {
		return (int) ReadRawVarInt32(stream, 5);
	}

	public static void WriteSInt32(Writer stream, int value) {
		WriteRawVarInt32(stream, EncodeZigZag32(value));
	}

	public static int ReadSInt32(Reader stream) {
		return DecodeZigZag32(ReadRawVarInt32(stream, 5));
	}

	public static void WriteUInt32(Writer stream, uint value) {
		WriteRawVarInt32(stream, value);
	}

	public static uint ReadUInt32(Reader stream) {
		return ReadRawVarInt32(stream, 5);
	}

	// Long

	public static void WriteInt64(Writer stream, long value) {
		WriteRawVarInt64(stream, (ulong) value);
	}

	public static long ReadInt64(Reader stream) {
		return (long) ReadRawVarInt64(stream, 10);
	}

	public static void WriteSInt64(Writer stream, long value) {
		WriteRawVarInt64(stream, EncodeZigZag64(value));
	}

	public static long ReadSInt64(Reader stream) {
		return DecodeZigZag64(ReadRawVarInt64(stream, 10));
	}

	public static void WriteUInt64(Writer stream, ulong value) {
		WriteRawVarInt64(stream, value);
	}

	public static ulong ReadUInt64(Reader stream) {
		return ReadRawVarInt64(stream, 10);
	}
}