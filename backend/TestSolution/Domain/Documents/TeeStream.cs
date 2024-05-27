namespace DocumentStore.Domain.Documents;

public class TeeStream(Stream stream1, Stream stream2) : Stream
{
	private readonly Stream _stream1 = stream1 ?? throw new ArgumentNullException(nameof(stream1));
	private readonly Stream _stream2 = stream2 ?? throw new ArgumentNullException(nameof(stream2));

	public TeeStream(Stream stream1, Stream stream2, long position) : this(stream1, stream2)
	{
		Position = position;
	}

	public override bool CanRead => _stream1.CanRead && _stream2.CanRead;
	public override bool CanSeek => false;
	public override bool CanWrite => _stream1.CanWrite && _stream2.CanWrite;
	public override long Length => throw new NotSupportedException();
	public override long Position
	{
		get => throw new NotSupportedException();
		set => throw new NotSupportedException();
	}

	public override void Flush()
	{
		_stream1.Flush();
		_stream2.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException("");
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_stream1.Write(buffer, offset, count);
		_stream2.Write(buffer, offset, count);
	}

	public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		await Task.WhenAll(
			_stream1.WriteAsync(buffer, offset, count, cancellationToken),
			_stream2.WriteAsync(buffer, offset, count, cancellationToken));
	}

	public override void WriteByte(byte value)
	{
		_stream1.WriteByte(value);
		_stream2.WriteByte(value);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_stream1.Dispose();
			_stream2.Dispose();
		}
		base.Dispose(disposing);
	}
}
