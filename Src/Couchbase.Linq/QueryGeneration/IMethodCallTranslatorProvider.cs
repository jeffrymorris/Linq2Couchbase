﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Couchbase.Linq.QueryGeneration
{
    internal interface IMethodCallTranslatorProvider
    {
        IMethodCallTranslator GetTranslator(MethodCallExpression methodCallExpression);
    }
}
