﻿
using System;

namespace SpracheDsl.Types
{
    [Flags]
    public enum ArgumentTypes : int
    {
        Undefined = 1,

        Symbol = 2,

        Variable = 4,

        Identifier = 8,

        Money = 16,

        Number = 32,

        Percent = 64,

        FunctionCall = 128,

        Bool = 256,

        Dsl = 512,

        Dimensionless = 1024,

        Set = 2048,

        Str = 4096,
    }
}
