using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Minecraft
{
	public class CommandStream : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		private readonly CancellationTokenSource _linkedTokenSource;
		private MultiplexedStream _stream;
		private MemoryStream _stdin;
		private MemoryStream _stdout;
		private MemoryStream _stderr;
		private int _readPosition;

		public CommandStream(MultiplexedStream stream, CancellationToken cancellationToken)
		{
			_linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
			_stream = stream;
			_stdin = new MemoryStream();
			_stdout = new MemoryStream();
			_stderr = new MemoryStream();
			//Task.Run(async () =>
			//{
			//	await _stream.CopyOutputToAsync(_stdin, _stdout, _stderr, _cancellationTokenSource.Token);
			//});
		}


		public void Dispose()
		{
			_cancellationTokenSource.Cancel();
			_stream.Dispose();
		}

		internal async Task WriteLine(string line)
		{
			var buffer = Encoding.UTF8.GetBytes(line + MinecraftServer.LineSeparator);
			await _stream.WriteAsync(buffer, 0, buffer.Length, _linkedTokenSource.Token);
		}

		internal async Task<string> ReadLine(int timeout = 10000)
		{
			var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_linkedTokenSource.Token, new CancellationTokenSource(timeout).Token);
			var start = _readPosition;
			var buffer = new byte[1024];
			bool lineEndFound = false;
			while (!lineEndFound)
			{
				if (_readPosition >= _stdout.Length)
				{
					var readResult = await _stream.ReadOutputAsync(buffer, 0, buffer.Length, linkedToken.Token).ConfigureAwait(false);
					if (readResult.EOF)
					{
						break;
					}
					_stdout.Write(buffer, 0, readResult.Count);
				}
				while (_readPosition < _stdout.Length)
				{
					if (_stdout.GetBuffer()[_readPosition++] == '\n')
					{
						lineEndFound = true;
						break;
					}
				}
			}
			if (_readPosition == start)
			{
				return "";
			}
			var line = Encoding.UTF8.GetString(_stdout.GetBuffer().AsSpan().Slice(start, _readPosition - start)).TrimEnd();
			return line;
		}

		internal async Task WriteLineWithVerify(string cmd)
		{
			await WriteLine(cmd);
			var confirm = await ReadLine();
			if (confirm != cmd)
			{
				throw new Exception("Server does not echo command");
			}
		}
	}
}
