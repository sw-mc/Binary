namespace SkyWing.Binary;

public class BaseBinaryStream {
	
	public Stream Stream { get; protected set; }

	public StreamReader? Reader { get; protected set; }
	public StreamWriter? Writer { get; protected set; }
	
	public BaseBinaryStream(Stream stream) {
		Stream = stream;
	}
}