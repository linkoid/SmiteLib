﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmiteUnit.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class SmiteMethodAttribute : SmiteAttribute
{
}
