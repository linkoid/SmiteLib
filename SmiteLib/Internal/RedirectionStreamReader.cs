﻿using SmiteLib.Internal.Streams;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SmiteLib.Internal;

public class RedirectionStreamReader : StreamReaderWrapper
{
	public bool Forward { get; set; } = true;
	//public bool Record { get; set; } = true;

	public Encoding Encoding
	{ 
		get => CurrentEncoding; 
		set => SetEncoding(value); 
	}

	protected override StreamReader WrappedStreamReader => _recordingReader;

	private readonly Process _process;
	private readonly StandardStream _target;

	private readonly MemoryStream _recordingStream;
	private readonly ReadOnlyMemoryStream _recordingStreamReadOnly;
	private StreamReader _recordingReader;
	private StreamWriter _recordingWriter;

	private bool _isListening = false;

	internal RedirectionStreamReader(Process process, StandardStream target)
	{
		_process = process;
		_target = target;

		_recordingStream = new MemoryStream();
		_recordingStreamReadOnly = new ReadOnlyMemoryStream(_recordingStream);

		var encoding = _process.StartInfo.GetEncoding(_target) ?? Encoding.UTF8;
		_recordingReader = new StreamReader(_recordingStreamReadOnly, encoding);
		_recordingWriter = new StreamWriter(_recordingStream, encoding);
	}

	internal void StartListening()
	{
		_recordingStream.SetLength(0);

		if (!_process.StartInfo.GetRedirect(_target)) return;

		switch (_target)
		{
			case StandardStream.Output:
				_process.OutputDataReceived += OnDataRecieved;
				_process.BeginOutputReadLine();
				break;
			case StandardStream.Error:
				_process.ErrorDataReceived += OnDataRecieved;
				_process.BeginErrorReadLine();
				break;
			default:
				throw new InvalidOperationException();
		}
		_isListening = true;
	}


	private bool _isCanceling = false;
	internal void StopListening()
	{
		if (!_isListening) return;

		_isCanceling = true;
		switch (_target)
		{
			case StandardStream.Output:
				_process.CancelOutputRead();
				_process.OutputDataReceived -= OnDataRecieved;
				break;
			case StandardStream.Error:
				_process.CancelErrorRead();
				_process.ErrorDataReceived -= OnDataRecieved;
				break;
			default:
				throw new InvalidOperationException();
		}
		_isListening = false;
		_isCanceling = false;
	}

	private void OnDataRecieved(object sender, DataReceivedEventArgs e)
	{
		try
		{
			OnDataRecieved(e.Data, !_isCanceling && e.Data != null);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
		}
	}

	private void OnDataRecieved(string? data, bool isLine)
	{
		if (true)//Record)
		{
			//string visual = data == null ? "null" : $"\"{data}\""
			//	.Replace("\r", @"\r")
			//	.Replace("\n", @"\n")
			//	.Replace("\0", @"\0");
			//Console.Error.WriteLine($"Write {_target}: {visual}");

			if (isLine)
			{
				_recordingWriter.WriteLine(data);
			}
			else
			{
				_recordingWriter.Write(data);
			}
			_recordingWriter.Flush();

			//Console.Error.WriteLine($"Write {_target} position={_recordingWriter.BaseStream.Position} length={_recordingWriter.BaseStream.Length}");
		}

		if (Forward && data != null)
		{
			var forwardWriter = _target.GetConsoleWriter();
			if (isLine)
			{
				forwardWriter.WriteLine(data);
			}
			else
			{
				forwardWriter.Write(data);
			}
		}
	}

	private void SetEncoding(Encoding encoding)
	{
		if (_isListening)
			throw new InvalidOperationException("Cannot change encoding while process is running");

		_process.StartInfo.SetEncoding(_target, encoding);
		_recordingReader = new StreamReader(_recordingStreamReadOnly, encoding);
		_recordingWriter = new StreamWriter(_recordingStream, encoding);
	}

	private static string ReinterpretWithEncoding(string data, Encoding fromEncoding, Encoding toEncoding)
	{
		byte[] bytes = new byte[fromEncoding.GetByteCount(data)];
		fromEncoding.GetBytes(data, bytes);
		return toEncoding.GetString(bytes);
	}

	protected override void Dispose(bool disposing)
	{
		throw new NotImplementedException();
	}
}
