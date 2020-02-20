using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public static class DslGrammar
    {
        public static Parser<Argument> ID =
            from id in Parse.AnyChar.AtLeastOnce().Token().Text()
            select Argument.AsIdentifier(id);

        public static Parser<Argument> SYMBOL =
            from colon in Parse.Char(':')
            from id in ID
            select id;

        public static Parser<Argument> VAR =
            from at in Parse.Char('@')
            from id in ID
            select id;

        public static Parser<Argument> MONEY =
            from dollar in Parse.Char('$')
            from value in Parse.DecimalInvariant
            select Argument.AsMoney(value);

        public static Parser<Argument> Expression =
            (ID
            .Or(VAR)
            .Or(MONEY)
            .Or(SYMBOL)).Token();

        public static Parser<char> Comma =
            Parse.Char(',').Token();

        public static Parser<IEnumerable<Argument>> ArgumentList =
            from args in Expression.DelimitedBy(Comma)
            select new List<Argument>(args);

        public static Parser<IEnumerable<Argument>> ParameterList =
            from openParen in Parse.Char('(')
            from args in ArgumentList
            from closeParen in Parse.Char(')')
            select args;

        public static Parser<FunctionInvocation> FunctionCall =
            from id in ID
            from args in ParameterList
            select new FunctionInvocation { Name = id.Id, Args = args.ToList() };

        public static readonly Parser<FunctionInvocation> Rule =
            FunctionCall;
    }
}