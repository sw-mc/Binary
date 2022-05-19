using System.Text;

namespace SkyNet.Binary;

public class BinaryStream{
	
	private readonly MemoryStream _buffer;
	private readonly BinaryWriter _writer;
	private readonly BinaryReader _reader;
	
	public BinaryStream(byte[]? buffer = null) {
		_buffer = buffer != null ? new MemoryStream(buffer) : new MemoryStream();
		_writer = new BinaryWriter(_buffer);
		_reader = new BinaryReader(_buffer);
	}
	
	public MemoryStream GetBuffer() {
		return _buffer;
	}
	
	public string Encode(EncodeTypes type = EncodeTypes.Utf8) {
		return type switch {
			EncodeTypes.Utf8 => Encoding.UTF8.GetString(_buffer.ToArray()),
			EncodeTypes.Utf32 => Encoding.UTF32.GetString(_buffer.ToArray()),
			EncodeTypes.Unicode => Encoding.Unicode.GetString(_buffer.ToArray()),
			EncodeTypes.BigEndianUnicode => Encoding.BigEndianUnicode.GetString(_buffer.ToArray()),
			EncodeTypes.AscII => Encoding.ASCII.GetString(_buffer.ToArray()),
			EncodeTypes.Latin1 => Encoding.Latin1.GetString(_buffer.ToArray()),
			_ => Encoding.Default.GetString(_buffer.ToArray())
		};
	}
	
	public void WriteByte(byte value) {
		_writer.Write(value);
	}
	
	public void ReadByte() {
		_reader.ReadByte();
	}
	
	public void WriteShort(short value) {
		_writer.Write(value);
	}
	
	public short ReadShort() {
		return _reader.ReadInt16();
	}
	
	public void WriteUShort(ushort value) {
		_writer.Write(value);
	}
	
	public ushort ReadUShort() {
		return _reader.ReadUInt16();
	}
	
	public void WriteInt(int value) {
		_writer.Write(value);
	}
	
	public int ReadInt() {
		return _reader.ReadInt32();
	}
	
	public void WriteUInt(uint value) {
		_writer.Write(value);
	}
	
	public uint ReadUInt() {
		return _reader.ReadUInt32();
	}
	
	public void WriteLong(long value) {
		_writer.Write(value);
	}
	
	public long ReadLong() {
		return _reader.ReadInt64();
	}
	
	public void WriteULong(ulong value) {
		_writer.Write(value);
	}
	
	public ulong ReadULong() {
		return _reader.ReadUInt64();
	}
	
	public void WriteFloat(float value) {
		_writer.Write(value);
	}
	
	public float ReadFloat() {
		return _reader.ReadSingle();
	}
	
	public void WriteDouble(double value) {
		_writer.Write(value);
	}
	
	public double ReadDouble() {
		return _reader.ReadDouble();
	}
	
	public void WriteDecimal(decimal value) {
		_writer.Write(value);
	}
	
	public decimal ReadDecimal() {
		return _reader.ReadDecimal();
	}
	
	public void WriteChar(char value) {
		_writer.Write(value);
	}
	
	public char ReadChar() {
		return _reader.ReadChar();
	}
	
	public void WriteBool(bool value) {
		_writer.Write(value);
	}
	
	public bool ReadBool() {
		return _reader.ReadBoolean();
	}
	
	public void WriteString(string value) {
		_writer.Write(value);
	}
	
	public string ReadString() {
		return _reader.ReadString();
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