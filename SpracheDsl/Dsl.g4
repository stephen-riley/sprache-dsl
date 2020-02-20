grammar Dsl;

@header {
#pragma warning disable 1591   #pragma warning disable 1574   #pragma warning disable 0108   #pragma warning disable 3021   
	}

@parser::members {
    protected const int EOF = Eof;
}

@lexer::members {
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}

ruleDefinition: ( a += attribute)* ruleForm | ruleForm;

attribute:
	'[' id = ID '=' expr = expression ']' # AttributeRule;

ruleForm:
	'rate' args = expressionList				# RateRule
	| 'fee' args = expressionList				# FeeRule
	| 'notax' args = expressionList				# NotaxRule
	| 'sumtax' args = expressionList			# SumtaxRule
	| 'exclusive' args = expressionList			# ExclusiveRule
	| 'unitband' args = expressionList			# UnitbandRule
	| 'band' args = expressionList				# BandRule
	| 'unitbasis' args = expressionList			# UnitBasisRule
	| 'costbasis' args = expressionList			# CostBasisRule
	| 'defaultrate' args = expressionList		# DefaultRateRule
	| 'defaultfee' args = expressionList		# DefaultFeeRule
	| 'bandedamount' args = expressionList		# BandedAmountRule
	| 'amountinband' args = expressionList		# AmountInBandRule
	| 'juristypematch' args = expressionList	# JurisTypeMatchRule
	| 'discount' args = expressionList			# DiscountRule
	| 'setcostbasis' args = expressionList		# SetCostBasisRule
	| 'dollars' args = expressionList			# DollarsRule
	| 'cents' args = expressionList				# CentsRule
	| 'bandedfee' args = expressionList			# BandedFeeRule
	| 'roundmoney' args = expressionList		# RoundMoneyRule;

expressionList:
	'(' e += expression (',' e += expression)* ')'
	| '(' ')';

symbolList: '[' s += SYMBOL ( ',' s += SYMBOL)* ']' | '[' ']';

expression:
	PERCENT			# PercentAtom
	| MONEY			# MoneyAtom
	| SYMBOL		# SymbolAtom
	| VAR			# VarAtom
	| FLOAT			# NumberAtom
	| INF			# InfinityAtom
	| symbolList	# SymbolListAtom
	| ruleForm		# RuleAtom;

INF: 'INF';

ID: ('a' ..'z' | 'A' ..'Z' | '_') (
		'a' ..'z'
		| 'A' ..'Z'
		| '0' ..'9'
		| '_'
		| '-'
	)*;

SYMBOL: ':' ID;

VAR: '@' ('a' ..'z' | 'A' ..'Z' | '0' ..'9' | '_' | '-')*;

PERCENT: FLOAT '%';

MONEY: '$' FLOAT;

FLOAT: ('0' ..'9')+ '.' ('0' ..'9')*
	| '.' ('0' ..'9')+
	| ('0' ..'9')+;

COMMENT: ('//' ~('\n' | '\r')* '\r'? '\n' | '/*' .*? '*/') -> skip;

WS: ( ' ' | '\t' | '\r' | '\n') -> skip;
