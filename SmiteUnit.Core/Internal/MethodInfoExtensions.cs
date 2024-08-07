﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmiteUnit.Internal;

internal static class MethodInfoExtensions
{
	public static T CreateDelegate<T>(this MethodInfo method)
		where T : Delegate
	{
		return (T)(object)Delegate.CreateDelegate(typeof(T), method);
	}

	public static T CreateDelegate<T>(this MethodInfo method, object target)
		where T : Delegate
	{
		return (T)(object)Delegate.CreateDelegate(typeof(T), target, method);
	}
}
