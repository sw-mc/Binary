using System.Text;

namespace SkyWing.Binary;

public class BinaryStream {

	private int _offset;
	private byte[] _buffer;
	private int _bufferMaxSize;
	
	public BinaryStream(int size, byte[]? buffer = null, int offset = 0) {
		_offset = offset;
		_buffer = buffer ?? new byte[size];
		_bufferMaxSize = size;
	}

	public void Rewind() {
		_offset = 0;
	}
	
	public int GetOffset() {
		return _offset;
	}
	
	public void SetOffset(int offset) {
		_offset = offset;
	}
	
	public int GetSize() {
		return _buffer.Length;
	}
	
	public byte[] GetBuffer() {
		return _buffer;
	}

	private void IncreaseSize(int size) {
		Array.Resize(ref _buffer, _bufferMaxSize += size);
	}

	public byte[] Get(int len = 1) {
		if (len == 0)
			return Array.Empty<byte>();
		if (len < 0)
			throw new ArgumentException("Length must be positive");

		var remaining = _buffer.Length - _offset;
		if (remaining < len)
			throw new BinaryDataException("Not enough bytes left in buffer: need "+len+", have "+remaining);

		var buffer = new byte[len];
		Array.Copy(_buffer, _offset, buffer, 0, len);
		_offset += len;
		return buffer;
	}

	public byte[] GetRemaining() {
		var length = _buffer.Length;
		if (_offset >= length)
			throw new BinaryDataException("No remaining bytes in buffer");

		_offset = length;
		return Copy(length - 1, _offset);
	}

	public byte[] Copy(int from, int to) {
		var buffer = new byte[to - from];
		Array.Copy(_buffer, from, buffer, 0, to);
		return buffer;
	}

	public void Put(byte[] bytes) {
		int lengthBefore = _buffer.Length;
		if (_buffer.Length + bytes.Length > _bufferMaxSize)
			IncreaseSize(_buffer.Length + bytes.Length - _bufferMaxSize);
		
		Array.Copy(bytes, 0, _buffer, lengthBefore, _buffer.Length);
	}

	public void Put(byte value) {
		if (_buffer.Length + 1 > _bufferMaxSize)
			IncreaseSize(_buffer.Length + 1 - _bufferMaxSize);
		
		_buffer[_buffer.Length] = value;
	}

	public bool GetBool() {
		return Get()[0] != 0;
	}
	
	public void PutBool(bool value) {
		Put(Convert.ToByte(value));
	}
	
	public byte GetByte() {
		return Get()[0];
	}
	
	public void PutByte(byte value) {
		Put(value);
	}
	
	public sbyte GetSByte() {
		return (sbyte)Get()[0];
	}
	
	public void PutSByte(sbyte value) {
		Put(Convert.ToByte(value));
	}
	
	public ushort GetShort() {
		return BitConverter.ToUInt16(Get(2), 0);
	}
	
	public short GetSignedShort() {
		return BitConverter.ToInt16(Get(2), 0);
	}
	
	public void PutShort(ushort value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public void PutSignedShort(short value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public uint GetInt() {
		return BitConverter.ToUInt32(Get(4), 0);
	}
	
	public int GetSignedInt() {
		return BitConverter.ToInt32(Get(4), 0);
	}
	
	public void PutInt(uint value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public void PutSignedInt(int value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public float GetFloat() {
		return BitConverter.ToSingle(Get(4), 0);
	}
	
	public void PutFloat(float value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public double GetDouble() {
		return BitConverter.ToDouble(Get(8), 0);
	}
	
	public void PutDouble(double value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public ulong GetLong() {
		return BitConverter.ToUInt64(Get(8), 0);
	}
	
	public long GetSignedLong() {
		return BitConverter.ToInt64(Get(8), 0);
	}
	
	public void PutLong(ulong value) {
		Put(BitConverter.GetBytes(value));
	}
	
	public void PutSignedLong(long value) {
		Put(BitConverter.GetBytes(value));
	}

	public uint ReadVarInt() {
		return VarInt.ReadUInt32(this);
	}
	
	public int ReadSignedVarInt() {
		return VarInt.ReadInt32(this);
	}
	
	public void WriteVarInt(uint value) {
		VarInt.WriteUInt32(this, value);
	}
	
	public void WriteSignedVarInt(int value) {
		VarInt.WriteInt32(this, value);
	}
	
	public ulong ReadVarLong() {
		return VarInt.ReadUInt64(this);
	}
	
	public long ReadSignedVarLong() {
		return VarInt.ReadInt64(this);
	}
	
	public void WriteVarLong(ulong value) {
		VarInt.WriteUInt64(this, value);
	}
	
	public void WriteSignedVarLong(long value) {
		VarInt.WriteInt64(this, value);
	}
	
	public string Encode(EncodeTypes type = EncodeTypes.Utf8) {
		return type switch {
			EncodeTypes.Utf8 => Encoding.UTF8.GetString(_buffer),
			EncodeTypes.Utf32 => Encoding.UTF32.GetString(_buffer),
			EncodeTypes.Unicode => Encoding.Unicode.GetString(_buffer),
			EncodeTypes.BigEndianUnicode => Encoding.BigEndianUnicode.GetString(_buffer),
			EncodeTypes.AscII => Encoding.ASCII.GetString(_buffer),
			EncodeTypes.Latin1 => Encoding.Latin1.GetString(_buffer),
			_ => Encoding.Default.GetString(_buffer)
		};
	}
	
	public enum EncodeTypes {
		Utf8,
		Utf32,
		Unicode,
		BigEndianUnicode,
		AscII,
		Latin1,
		Default 
	}
}