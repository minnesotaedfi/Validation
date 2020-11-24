lexer grammar MsdsLexer;

AFTER : 'after';
ALL : 'all';
AND : 'and';
AS : 'as';
BEFORE : 'before';
BY : 'by';
COLLECTION : 'collection';
DO : 'do' | 'does';
DEFINE : 'define' | 'defines';
ELSE : 'else';
EXIST : 'exist' | 'exists';
EXPECT : 'expect';
FALSE : 'false';
FOR : 'for';
INCLUDES : 'includes';
IN : 'in';
IS : 'is';
MATCH : 'match' | 'matches';
NOT : 'does'? 'not';
NULL : 'null';
OF : 'of';
OR : 'or';
REQUIRE : 'require';
RULE : 'rule';
RULESET : 'ruleset';
SINCE : 'since';
TRUE : 'true';
THAT : 'that';
THE : 'the';
THEN : 'then';
TODAY : 'today';
UNIQUE : 'unique';
WHEN : 'when';

LT : '<';
LE : '<=' | '=<';
EQ : '=';
GE : '>=' | '=>';
GT : '>';
NE : '<>' | '!=';

L_PAREN : '(';
R_PAREN : ')';

L_BRACKET : '[';
R_BRACKET : ']';

L_BRACE : '{';
R_BRACE : '}';

COMMA : ',';
DOT : '.';
NEG : '-';

RULEID1 : INT DOT INT;                       // 100.1
RULEID2 : INT DOT INT DOT INT;              // 100.1.2
RULEID3 : INT DOT INT DOT INT DOT INT;     // 100.1.2.3

fragment DIGIT : [0-9] ;
INT :   DIGIT+ ;

NUMBER
    :   NEG? INT DOT INT    // 1.35, 0.3, -4.5
    |   NEG? DOT INT        // .034
    |   NEG? INT            // -3, 45
    ;

DATE1 : DAY DDEL MONTH;
DATE2 : DAY DDEL MONTH DDEL YEAR;

fragment DDEL : [\-/] ;
fragment DAY : [1-9] | [1-2] [0-9] | [3][0-1] ;
fragment YEAR : ('19' | '20') DIGIT DIGIT;

AGGREGATE : MAX | MIN | SUM;

COUNT : 'count';
fragment MAX : 'max';
fragment MIN : 'min';
fragment SUM : 'sum';

CARDINAL : FIRST | SECOND | THIRD | FOURTH | LAST ;

fragment FIRST  : 'first';
fragment SECOND : 'second';
fragment THIRD  : 'third';
fragment FOURTH : 'fourth';
fragment LAST   : 'last';

DATEOP : EARLIEST | LATEST;

fragment EARLIEST : 'earliest';
fragment LATEST : 'latest';

MONTH : JAN | FEB | MAR | APR | MAY | JUN | JUL | AUG | SEP | OCT | NOV | DEC ;

fragment JAN : 'Jan' | 'January' ;
fragment FEB : 'Feb' | 'February' ;
fragment MAR : 'Mar' | 'March' ;
fragment APR : 'Apr' | 'April' ;
fragment MAY : 'May' ;
fragment JUN : 'Jun' | 'June' ;
fragment JUL : 'Jul' | 'July' ;
fragment AUG : 'Aug' | 'August' ;
fragment SEP : 'Sep' | 'September' ;
fragment OCT : 'Oct' | 'October' ;
fragment NOV : 'Nov' | 'November' ;
fragment DEC : 'Dec' | 'December' ;

TIMEUNIT : DAYS | WEEKS | MONTHS | YEARS ;

fragment DAYS   : 'days' | 'day';
fragment WEEKS  : 'weeks' | 'week';
fragment MONTHS : 'months' | 'month';
fragment YEARS  : 'years' | 'year';

WEEKDAY : SUN | MON | TUE | WED | THU | FRI | SAT ;

fragment SUN : 'Sunday';
fragment MON : 'Monday';
fragment TUE : 'Tuesday';
fragment WED : 'Wednesday';
fragment THU : 'Thursday';
fragment FRI : 'Friday';
fragment SAT : 'Saturday';

ID : [A-Z_] [a-zA-Z0-9_]* ; // identifiers start with upper case or underscore

STRING : '\'' (~'\''|'\\\'')* '\''  ;

COMMENT : '/*' .*? '*/' -> skip ; // Match "/*" stuff "*/"

WS : [ \t\n\r]+ -> skip ;
