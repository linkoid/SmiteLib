﻿using SmiteLib.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmiteLib.Tests.TestProgram;

internal static class AttributeTestsEdge
{
	[SmiteSetUp]
	static void SetUp()
	{
		Console.WriteLine("SetUp");
	}


	[SmiteMethod]
	public static void Test()
	{
		Console.WriteLine("Test");
	}
}
