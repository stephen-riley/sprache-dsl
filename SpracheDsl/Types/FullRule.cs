using System.Collections.Generic;

namespace SpracheDsl.Types
{
    public class FullRule
    {
        public IEnumerable<DslAttribute> Attributes { get; private set; }

        public FunctionInvocation Invocation { get; private set; }

        public FullRule(FunctionInvocation invocation, IEnumerable<DslAttribute> attributes)
        {
            Invocation = invocation;
            Attributes = attributes;
        }
    }
}