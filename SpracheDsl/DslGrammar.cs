using System.Collections.Generic;
using System.Linq;
using Sprache;
using SpracheDsl.Types;

namespace SpracheDsl
{
    public class DslGrammar
    {
        public static Parser<Argument> TRUE =
            from trueLiteral in Parse.String("true").Token()
            select Argument.AsBoolean(true);

        public static Parser<Argument> FALSE =
            from trueLiteral in Parse.String("false").Token()
            select Argument.AsBoolean(false);

        public static Parser<Argument> STRING =
            from openQuote in Parse.Char('\'')
            from str in Parse.Regex(@"[^']*")
            from closeQuote in Parse.Char('\'')
            select Argument.AsString(str);

        public static Parser<Argument> ID =
            from id in Parse.Regex(@"[A-Za-z_\-0-9\.]+").Token().Text()
            select Argument.AsIdentifier(id);

        public static Parser<Argument> SYMBOL =
            from colon in Parse.Char(':')
            from id in ID
            select Argument.AsSymbol(id.Id);

        public static Parser<Argument> VAR =
            from at in Parse.Char('@')
            from id in ID
            select Argument.AsVariable(id.Id);

        public static Parser<Argument> MONEY =
            from dollar in Parse.Char('$')
            from value in Parse.DecimalInvariant
            select Argument.AsMoney(value);

        public static Parser<Argument> PERCENT =
            from value in Parse.DecimalInvariant
            from percent in Parse.Char('%')
            select Argument.AsPercent(value);

        public static Parser<Argument> NUMBER =
            from value in Parse.DecimalInvariant
            select Argument.AsNumber(value);

        public static Parser<Argument> INF =
            from value in Parse.String("INF").Token()
            select Argument.AsNumber(decimal.MaxValue);

        public static Parser<Argument> SET =
            from openBracket in Parse.Char('[').Token()
            from args in ArgumentList.Optional()
            from closeBracket in Parse.Char(']').Token()
            select Argument.AsSet(args.IsDefined ? args.Get().ToList() : new List<Argument>());

        public static Parser<Argument> Atom =
            TRUE
            .Or(FALSE)
            .Or(VAR)
            .Or(MONEY)
            .Or(SYMBOL)
            .Or(PERCENT)
            .Or(NUMBER)
            .Or(STRING)
            .Or(INF)
            .Or(SET);

        public static Parser<Argument> Expression =
            Atom
            .Or(
                from id in ID
                from openParen in Parse.Char('(').Token()
                from args in ArgumentList.Optional()
                from closeParen in Parse.Char(')').Token()
                select Argument.AsFunctionCall(new FunctionInvocation { Name = id.Id, Args = args.IsDefined ? args.Get().ToList() : new List<Argument>() })
            );

        public static Parser<char> Comma =
            Parse.Char(',').Token();

        public static Parser<IEnumerable<Argument>> ArgumentList =
            from args in Expression.DelimitedBy(Comma)
            select new List<Argument>(args);

        public static readonly Parser<FunctionInvocation> Rule =
            from invocation in Expression
            select invocation.FuncInvocation;

        public static Parser<DslAttribute> Attr =
            from openBracket in Parse.Char('[').Token()
            from id in ID
            from eq in Parse.Char('=').Token()
            from atoms in Atom.DelimitedBy(Comma)
            from closeBracket in Parse.Char(']').Token()
            select new DslAttribute(id.Id, atoms);

        public static Parser<IEnumerable<DslAttribute>> AttrList =
            from attrs in Attr.Token().Many().Optional()
            select attrs.IsDefined ? attrs.Get() : new List<DslAttribute>();

        public static Parser<FullRule> FullRule =
            from attrs in AttrList
            from rule in Rule
            select new FullRule(rule, attrs);

        public static Parser<IEnumerable<FullRule>> CompoundRule =
            FullRule.AtLeastOnce();
    }
}