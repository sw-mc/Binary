namespace SkyWing.Binary; 

public class MemoryBinaryStream : BaseBinaryStream {

	public MemoryBinaryStream(Stream stream) : base(stream) {
		Reader = new Reader(stream, false);
		Writer = new Writer(stream, false);
	}

	public MemoryBinaryStream(byte[] data) : base(new MemoryStream(data)) {
		Reader = new Reader(Stream, false);
		Writer = new Writer(Stream, false);
	}
}