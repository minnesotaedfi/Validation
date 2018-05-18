parser grammar MsdsParser;

options { tokenVocab=MsdsLexer; }

file
    : (aliasDefinitions | collection | ruleset)*
    ;

collection
    : COLLECTION collectionid INCLUDES (rulesetid | ruleid) (COMMA (rulesetid | ruleid))* aliasDefinitions?
    ;

collectionid : ID;

aliasId : ID;

alias
    : aliasId EQ constant
    ;

aliasDefinitions
    : DEFINE alias (COMMA alias)*
    ;

ruleset
    : RULESET rulesetid ruleDefinition+
    ;

rulesetid : ID;

ruleDefinition
    : RULE ruleid
            (WHEN filter THEN)?
            (REQUIRE | EXPECT) THAT? condition
            ELSE error
    ;

ruleid : RULEID1 | RULEID2;

error : STRING;

pattern : STRING;

component : componentid DOT characteristicid;

components : componentid DOT characteristicids;

lookups : componentid DOT characteristicids;

componentid : L_BRACE ID R_BRACE;

characteristicid: L_BRACKET ID R_BRACKET;

characteristicids: L_BRACKET ID? (COMMA ID)* R_BRACKET;

functionid : ID;

function
    : functionid L_PAREN expr? (COMMA expr)* R_PAREN
    ;

tuple
    : L_PAREN constant (COMMA constant)* R_PAREN
    ;

tuples
    : L_BRACKET tuple (COMMA tuple)* R_BRACKET
    ;

filter
    : collection_filter             #filter_0
    | condition                     #filter_0
    | L_PAREN filter R_PAREN        #filter_0
    | filter operation filter       #filter_operation
    ;

collection_filter
    : COLLECTION IS collectionid                                               #filter_collection
    | COLLECTION IS IN L_BRACKET collectionid? (COMMA collectionid)* R_BRACKET #filter_collections
    ;

condition
    : L_PAREN condition R_PAREN             # condition_parenthesis
    | condition (operation condition)+      # condition_compound
    | component (DO NOT)? MATCH pattern     # condition_pattern
    | component (DO NOT)? EXIST             # condition_exists1
    | componentid (DO NOT)? EXIST           # condition_exists2
    | components IS NOT? UNIQUE             # condition_unique
    | components IS NOT? IN tuples          # condition_intuples
    | component IS NOT? IN constants        # condition_inconsts
    | components IS NOT? IN lookups         # condition_inlookups
    | expr comparison expr                  # condition_comparison
    ;

expr
    : component     # expr_component
    | function      # expr_function
    | constant      # expr_constant
    | intrinsic     # expr_intrinsic
    ;

intrinsic
    : COUNT L_PAREN componentid (BY characteristicids)? (WHEN condition)? R_PAREN   # intrinsic_aggregatecount
    | AGGREGATE L_PAREN component (BY characteristicids)? (WHEN condition)? R_PAREN # intrinsic_aggregate
    | AGGREGATE L_PAREN components (COMMA components)* R_PAREN                      # intrinsic_arithmetic
    | int TIMEUNIT (BEFORE|AFTER) date                                              # differentialDate
    ;

constant
    : bool
    | date
    | num
    | str
    | aliasId
    ;

date
    : TODAY                                                         # today
    | DATE1                                                         # dayMonth
    | DATE2                                                         # dayMonthYear
    | CARDINAL WEEKDAY IN MONTH                                     # cardinalDate
    | component                                                     # componentDate
    | aliasId                                                       # aliasIdDate
    | TIMEUNIT SINCE date (AS OF date)?                             # intrinsic_timePeriod
    | THE? DATEOP (OF | IN)? L_PAREN date (COMMA date)+ R_PAREN     # dateoperation
    ;

bool : TRUE | FALSE ;
int : INT;
num : RULEID1 | NUMBER | INT;
str : STRING;

constants
    : L_BRACKET constant? (COMMA constant)* R_BRACKET ;

comparison
    : LT | LE | EQ | GE | GT | NE ;

operation
    : AND | OR ;
