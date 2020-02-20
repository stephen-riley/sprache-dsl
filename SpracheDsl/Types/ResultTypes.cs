
using System;

namespace SpracheDsl.Types
{
    [Flags]
    public enum ResultTypes : int
    {
        Undefined = 1,

        Dimensionless = 2,

        Money = 4,

        Unit = 8,

        Percent = 16,

        Dsl = 32,
    }
}
