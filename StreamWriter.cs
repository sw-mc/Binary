namespace SkyWing.Binary; 

public interface StreamWriter {
	
	public void WriteByte(byte v);
	
	public void WriteSignedByte(sbyte v);

	public void WriteShort(ushort v);

	public void WriteSignedShort(short v);

	public void WriteInt(int v);

	public void WriteLong(long v);

	public void WriteFloat(float v);

	public void WriteDouble(double v);

	public void WriteByteArray(byte[] v);

	public void WriteString(string v);
	
	public void WriteIntArray(int[] v);
	
	public void WriteLongArray(long[] v);

}