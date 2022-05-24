namespace SkyWing.Binary; 

public interface StreamReader {
	
	public byte ReadByte();
	
	public sbyte ReadSignedByte();

	public short ReadInt16();
	
	public int ReadInt32();

	public long ReadInt64();

	public float ReadSingle();

	public double ReadDouble();

	public byte[] ReadByteArray();

	public string ReadString();
	
	public int[] ReadIntArray();
	
	public long[] ReadLongArray();

}