using System;
using System.Collections.Generic;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public class DslEvaluator
    {
        public Line Line { get; set; }

        public IDictionary<string, ResultValue> ParamBag { get; set; }

        public ResultValue Eval(string dsl)
        {
            var invocation = DslGrammar.Rule.Parse(dsl);
            throw new NotImplementedException();
        }
    }
}
