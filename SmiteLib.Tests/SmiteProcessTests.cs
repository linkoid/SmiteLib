﻿using NUnit.Framework;
using SmiteLib.Internal;
using SmiteLib.Tests.TestProgram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmiteLib.Tests;

public class SmiteProcessTests
{
	[Test]
	public void StandardIO()
	{
		string expected;
		using (var stream = new MemoryStream())
		{
			SmiteProcessTestsEdge.PrintManyLinesUsing(new StreamWriter(stream));
			stream.Position = 0;
			expected = new StreamReader(stream).ReadToEnd();
		}
		var process = new SmiteProcess("SmiteLib.Tests.TestProgram.exe")
		{
			RunTimeout = 10000,
			UseSubprocess = true,
		};
		process.RunTest(SmiteId.Method(SmiteProcessTestsEdge.PrintManyLines));

		var output = process.Output.ReadToEnd();
		Assert.AreEqual(expected, output);

		var error = process.Error.ReadToEnd();
		Assert.AreEqual(expected, error);
	}
}
