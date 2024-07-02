﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmiteLib.VisualStudio.TestAdapter;

internal static class TestReflection
{
	public static IEnumerable<string> GetLoadedAssemblyPaths()
	{
		return from assembly in AppDomain.CurrentDomain.GetAssemblies()
			   where !assembly.IsDynamic
			   let loadedPath = assembly.Location
			   where loadedPath != null
			   select loadedPath;
	}

	public static MetadataLoadContext LoadContext(string assemblyPath)
	{
		assemblyPath = System.IO.Path.GetFullPath(assemblyPath);
		var paths = GetLoadedAssemblyPaths().Concat(new[] { assemblyPath, SmiteTestEnumerator.SmiteTestAttributeType.Assembly.Location });
		var resolver = new PathAssemblyResolver(paths);
		return new MetadataLoadContext(resolver);
	}

	public static MetadataLoadContext LoadWithContext(string assemblyPath, out TestAssembly testAssembly)
	{
		assemblyPath = System.IO.Path.GetFullPath(assemblyPath);
		var loadContext = LoadContext(assemblyPath);
		var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);
		testAssembly = new TestAssembly(assembly);
		return loadContext;
	}
}
