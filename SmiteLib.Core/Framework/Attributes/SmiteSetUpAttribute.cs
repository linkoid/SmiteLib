﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmiteLib.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class SmiteSetUpAttribute : Attribute
{
}
