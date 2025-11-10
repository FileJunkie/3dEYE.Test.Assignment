namespace _3dEYE.Test.Assignment.Sorter;

public class LimitedStream : Stream
{
    private readonly Stream _baseStream;
    private readonly long _start;
    private readonly long _end;
    private long _bytesRead;

    public LimitedStream(Stream baseStream, long start, long end)
    {
        _baseStream = baseStream;
        _baseStream.Position = start;
        _start = start;
        _end = end;
    }

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _end - _start;
    public override long Position
    {
        get => _bytesRead;
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_bytesRead >= Length)
            return 0;

        var remaining = Length - _bytesRead;
        if (count > remaining)
            count = (int)remaining;

        var read = _baseStream.Read(buffer, offset, count);
        _bytesRead += read;
        return read;
    }

    public override void Flush() => _baseStream.Flush();
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}