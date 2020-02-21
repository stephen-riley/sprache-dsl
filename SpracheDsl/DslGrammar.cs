using System.Collections.Generic;
using System.Linq;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public static class DslGrammar
    {
        public static Parser<Argument> ID =
            from id in Parse.Letter.AtLeastOnce().Token().Text()
            select Argument.AsIdentifier(id);

        public static Parser<Argument> SYMBOL =
            from colon in Parse.Char(':')
            from id in ID
            select Argument.AsSymbol(id.Id);

        public static Parser<Argument> VAR =
            from at in Parse.Char('@')
            from id in ID
            select id;

        public static Parser<Argument> MONEY =
            from dollar in Parse.Char('$')
            from value in Parse.DecimalInvariant
            select Argument.AsMoney(value);

        public static Parser<Argument> PERCENT =
            from value in Parse.DecimalInvariant
            from percent in Parse.Char('%')
            select Argument.AsPercent(value);

        public static Parser<Argument> Expression =
            VAR
            .Or(MONEY)
            .Or(SYMBOL)
            .Or(PERCENT)
            .Or(
                from id in ID
                from openParen in Parse.Char('(')
                from args in ArgumentList.Optional()
                from closeParen in Parse.Char(')')
                select Argument.AsFunctionCall(new FunctionInvocation { Name = id.Id, Args = args.IsDefined ? args.Get().ToList() : new List<Argument>() })
            );
        // .Or(FunctionCall);

        public static Parser<char> Comma =
            Parse.Char(',').Token();

        public static Parser<IEnumerable<Argument>> ArgumentList =
            from args in Expression.DelimitedBy(Comma)
            select new List<Argument>(args);

        public static Parser<IEnumerable<Argument>> ParameterList =
            from openParen in Parse.Char('(')
            from args in ArgumentList.Optional()
            from closeParen in Parse.Char(')')
            select args.IsDefined ? args.Get() : new List<Argument>();

        public static Parser<Argument> FunctionCall =
            from id in ID
            from args in ParameterList
            select Argument.AsFunctionCall(new FunctionInvocation { Name = id.Id, Args = args.ToList() });

        public static readonly Parser<FunctionInvocation> Rule =
            (from invocation in FunctionCall
             select invocation.FuncInvocation).End();
    }
}