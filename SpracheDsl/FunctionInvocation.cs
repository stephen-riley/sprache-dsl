using System.Collections.Generic;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public class FunctionInvocation
    {
        public string Name { get; set; }

        public IList<Argument> Args { get; set; } = new List<Argument>();
    }
}