using System.Collections.Generic;
using System.Linq;

namespace SpracheDsl.Types
{
    public class DslAttribute
    {
        public string Name { get; private set; }

        public IList<string> Values { get; private set; }

        public DslAttribute(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values.ToList();
        }

        public DslAttribute(string name, IEnumerable<Argument> args)
        {
            Name = name;
            Values = args.Select(a => a.ToString()).ToList();
        }
    }
}