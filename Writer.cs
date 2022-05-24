using System.Text;

namespace SkyWing.Binary; 

public unsafe class Writer : StreamWriter {
    
	// Write at most 4 MiB at a time.
    private const int MaxWriteChunk = 4 * 1024 * 1024;

    // Encoding can be shared among all instances of NbtBinaryWriter, because it is stateless.
    private static readonly UTF8Encoding encoding = new UTF8Encoding(false, true);

    // Each instance has to have its own encoder, because it does maintain state.
    readonly Encoder _encoder = encoding.GetEncoder();

    public Stream BaseStream {
        get {
            _stream.Flush();
            return _stream;
        }
    }

    private readonly Stream _stream;

    // Buffer used for temporary conversion
    private const int BufferSize = 256;

    // UTF8 characters use at most 4 bytes each.
    private const int MaxBufferedStringLength = BufferSize / 4;

    // Each NbtBinaryWriter needs to have its own instance of the buffer.
    private readonly byte[] _buffer = new byte[BufferSize];

    // Swap is only needed if endianness of the runtime differs from desired NBT stream
    private readonly bool _swapNeeded;

    public Writer(Stream input, bool bigEndian) {
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (!input.CanWrite) throw new ArgumentException("Given stream must be writable", nameof(input));
        _stream = input;
        _swapNeeded = (BitConverter.IsLittleEndian == bigEndian);
    }

    public void Write(byte value) {
        _stream.WriteByte(value);
    }

    public void Write(short value) {
        unchecked {
            if (_swapNeeded) {
                _buffer[0] = (byte) (value >> 8);
                _buffer[1] = (byte) value;
            }
            else {
                _buffer[0] = (byte) value;
                _buffer[1] = (byte) (value >> 8);
            }
        }

        _stream.Write(_buffer, 0, 2);
    }

    public void Write(int value) {
        unchecked {
            if (_swapNeeded) {
                _buffer[0] = (byte) (value >> 24);
                _buffer[1] = (byte) (value >> 16);
                _buffer[2] = (byte) (value >> 8);
                _buffer[3] = (byte) value;
            }
            else {
                _buffer[0] = (byte) value;
                _buffer[1] = (byte) (value >> 8);
                _buffer[2] = (byte) (value >> 16);
                _buffer[3] = (byte) (value >> 24);
            }
        }

        _stream.Write(_buffer, 0, 4);
    }

    public void Write(long value) {
        unchecked {
            if (_swapNeeded) {
                _buffer[0] = (byte) (value >> 56);
                _buffer[1] = (byte) (value >> 48);
                _buffer[2] = (byte) (value >> 40);
                _buffer[3] = (byte) (value >> 32);
                _buffer[4] = (byte) (value >> 24);
                _buffer[5] = (byte) (value >> 16);
                _buffer[6] = (byte) (value >> 8);
                _buffer[7] = (byte) value;
            }
            else {
                _buffer[0] = (byte) value;
                _buffer[1] = (byte) (value >> 8);
                _buffer[2] = (byte) (value >> 16);
                _buffer[3] = (byte) (value >> 24);
                _buffer[4] = (byte) (value >> 32);
                _buffer[5] = (byte) (value >> 40);
                _buffer[6] = (byte) (value >> 48);
                _buffer[7] = (byte) (value >> 56);
            }
        }

        _stream.Write(_buffer, 0, 8);
    }

    public unsafe void Write(float value) {
        ulong tmpValue = *(uint*) &value;
        unchecked {
            if (_swapNeeded) {
                _buffer[0] = (byte) (tmpValue >> 24);
                _buffer[1] = (byte) (tmpValue >> 16);
                _buffer[2] = (byte) (tmpValue >> 8);
                _buffer[3] = (byte) tmpValue;
            }
            else {
                _buffer[0] = (byte) tmpValue;
                _buffer[1] = (byte) (tmpValue >> 8);
                _buffer[2] = (byte) (tmpValue >> 16);
                _buffer[3] = (byte) (tmpValue >> 24);
            }
        }

        _stream.Write(_buffer, 0, 4);
    }

    public unsafe void Write(double value) {
        var tmpValue = *(ulong*) &value;
        unchecked {
            if (_swapNeeded) {
                _buffer[0] = (byte) (tmpValue >> 56);
                _buffer[1] = (byte) (tmpValue >> 48);
                _buffer[2] = (byte) (tmpValue >> 40);
                _buffer[3] = (byte) (tmpValue >> 32);
                _buffer[4] = (byte) (tmpValue >> 24);
                _buffer[5] = (byte) (tmpValue >> 16);
                _buffer[6] = (byte) (tmpValue >> 8);
                _buffer[7] = (byte) tmpValue;
            }
            else {
                _buffer[0] = (byte) tmpValue;
                _buffer[1] = (byte) (tmpValue >> 8);
                _buffer[2] = (byte) (tmpValue >> 16);
                _buffer[3] = (byte) (tmpValue >> 24);
                _buffer[4] = (byte) (tmpValue >> 32);
                _buffer[5] = (byte) (tmpValue >> 40);
                _buffer[6] = (byte) (tmpValue >> 48);
                _buffer[7] = (byte) (tmpValue >> 56);
            }
        }

        _stream.Write(_buffer, 0, 8);
    }

    // Based on BinaryWriter.Write(String)
    public unsafe void Write(string value) {
        if (value == null) {
            throw new ArgumentNullException(nameof(value));
        }

        // Write out string length (as number of bytes)
        var numBytes = encoding.GetByteCount(value);
        Write((short) numBytes);

        if (numBytes <= BufferSize) {
            // If the string fits entirely in the buffer, encode and write it as one
            encoding.GetBytes(value, 0, value.Length, _buffer, 0);
            _stream.Write(_buffer, 0, numBytes);
        }
        else {
            // Aggressively try to not allocate memory in this loop for runtime performance reasons.
            // Use an Encoder to write out the string correctly (handling surrogates crossing buffer
            // boundaries properly).  
            var charStart = 0;
            var numLeft = value.Length;
            while (numLeft > 0) {
                // Figure out how many chars to process this round.
                var charCount = (numLeft > MaxBufferedStringLength) ? MaxBufferedStringLength : numLeft;
                int byteLen;
                fixed (char* pChars = value) {
                    fixed (byte* pBytes = _buffer) {
                        byteLen = _encoder.GetBytes(pChars + charStart, charCount, pBytes, BufferSize,
                            charCount == numLeft);
                    }
                }

                _stream.Write(_buffer, 0, byteLen);
                charStart += charCount;
                numLeft -= charCount;
            }
        }
    }

    public void Write(byte[] data, int offset, int count) {
        var written = 0;
        while (written < count) {
            var toWrite = Math.Min(MaxWriteChunk, count - written);
            _stream.Write(data, offset + written, toWrite);
            written += toWrite;
        }
    }

    public void WriteByte(byte v) {
        Write(v);
    }

    public void WriteSignedByte(sbyte v) {
        Write((byte) v);
    }

    public void WriteShort(ushort v) {
        Write(v);
    }

    public void WriteSignedShort(short v) {
        Write(v);
    }

    public void WriteInt(int v) {
        Write(v);
    }

    public void WriteLong(long v) {
        Write(v);
    }

    public void WriteFloat(float v) {
        Write(v);
    }

    public void WriteDouble(double v) {
        Write(v);
    }

    public void WriteByteArray(byte[] v) {
        Write(v.Length);
        foreach (var b in v) {
            Write(b);
        }
    }

    public void WriteString(string v) {
        Write(v);
    }

    public void WriteIntArray(int[] v) {
        Write(v.Length);
        foreach (var i in v) {
            Write(i);
        }
    }
    
    public void WriteLongArray(long[] v) {
        Write(v.Length);
        foreach (var l in v) {
            Write(l);
        }
    }
}