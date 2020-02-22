using System.Collections.Generic;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public class ExecContext
    {
        public Line Line { get; set; }

        public IDictionary<string, ResultValue> ParamBag { get; set; }
    }
}