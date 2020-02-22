using System.Collections.Generic;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public interface ICompiledRule
    {
        ResultValue Execute(ExecContext context, params Argument[] args);
    }
}