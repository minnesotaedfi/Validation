// Generated from d:\github\Validation\Engine\Language\MsdsParser.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link MsdsParser}.
 */
public interface MsdsParserListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link MsdsParser#file}.
	 * @param ctx the parse tree
	 */
	void enterFile(MsdsParser.FileContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#file}.
	 * @param ctx the parse tree
	 */
	void exitFile(MsdsParser.FileContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#collection}.
	 * @param ctx the parse tree
	 */
	void enterCollection(MsdsParser.CollectionContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#collection}.
	 * @param ctx the parse tree
	 */
	void exitCollection(MsdsParser.CollectionContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#collectionid}.
	 * @param ctx the parse tree
	 */
	void enterCollectionid(MsdsParser.CollectionidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#collectionid}.
	 * @param ctx the parse tree
	 */
	void exitCollectionid(MsdsParser.CollectionidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#aliasId}.
	 * @param ctx the parse tree
	 */
	void enterAliasId(MsdsParser.AliasIdContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#aliasId}.
	 * @param ctx the parse tree
	 */
	void exitAliasId(MsdsParser.AliasIdContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#alias}.
	 * @param ctx the parse tree
	 */
	void enterAlias(MsdsParser.AliasContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#alias}.
	 * @param ctx the parse tree
	 */
	void exitAlias(MsdsParser.AliasContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#aliasDefinitions}.
	 * @param ctx the parse tree
	 */
	void enterAliasDefinitions(MsdsParser.AliasDefinitionsContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#aliasDefinitions}.
	 * @param ctx the parse tree
	 */
	void exitAliasDefinitions(MsdsParser.AliasDefinitionsContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#ruleset}.
	 * @param ctx the parse tree
	 */
	void enterRuleset(MsdsParser.RulesetContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#ruleset}.
	 * @param ctx the parse tree
	 */
	void exitRuleset(MsdsParser.RulesetContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#rulesetid}.
	 * @param ctx the parse tree
	 */
	void enterRulesetid(MsdsParser.RulesetidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#rulesetid}.
	 * @param ctx the parse tree
	 */
	void exitRulesetid(MsdsParser.RulesetidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#ruleDefinition}.
	 * @param ctx the parse tree
	 */
	void enterRuleDefinition(MsdsParser.RuleDefinitionContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#ruleDefinition}.
	 * @param ctx the parse tree
	 */
	void exitRuleDefinition(MsdsParser.RuleDefinitionContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#ruleid}.
	 * @param ctx the parse tree
	 */
	void enterRuleid(MsdsParser.RuleidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#ruleid}.
	 * @param ctx the parse tree
	 */
	void exitRuleid(MsdsParser.RuleidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#error}.
	 * @param ctx the parse tree
	 */
	void enterError(MsdsParser.ErrorContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#error}.
	 * @param ctx the parse tree
	 */
	void exitError(MsdsParser.ErrorContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#pattern}.
	 * @param ctx the parse tree
	 */
	void enterPattern(MsdsParser.PatternContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#pattern}.
	 * @param ctx the parse tree
	 */
	void exitPattern(MsdsParser.PatternContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#component}.
	 * @param ctx the parse tree
	 */
	void enterComponent(MsdsParser.ComponentContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#component}.
	 * @param ctx the parse tree
	 */
	void exitComponent(MsdsParser.ComponentContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#components}.
	 * @param ctx the parse tree
	 */
	void enterComponents(MsdsParser.ComponentsContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#components}.
	 * @param ctx the parse tree
	 */
	void exitComponents(MsdsParser.ComponentsContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#lookups}.
	 * @param ctx the parse tree
	 */
	void enterLookups(MsdsParser.LookupsContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#lookups}.
	 * @param ctx the parse tree
	 */
	void exitLookups(MsdsParser.LookupsContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#componentid}.
	 * @param ctx the parse tree
	 */
	void enterComponentid(MsdsParser.ComponentidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#componentid}.
	 * @param ctx the parse tree
	 */
	void exitComponentid(MsdsParser.ComponentidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#characteristicid}.
	 * @param ctx the parse tree
	 */
	void enterCharacteristicid(MsdsParser.CharacteristicidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#characteristicid}.
	 * @param ctx the parse tree
	 */
	void exitCharacteristicid(MsdsParser.CharacteristicidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#characteristicids}.
	 * @param ctx the parse tree
	 */
	void enterCharacteristicids(MsdsParser.CharacteristicidsContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#characteristicids}.
	 * @param ctx the parse tree
	 */
	void exitCharacteristicids(MsdsParser.CharacteristicidsContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#functionid}.
	 * @param ctx the parse tree
	 */
	void enterFunctionid(MsdsParser.FunctionidContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#functionid}.
	 * @param ctx the parse tree
	 */
	void exitFunctionid(MsdsParser.FunctionidContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#function}.
	 * @param ctx the parse tree
	 */
	void enterFunction(MsdsParser.FunctionContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#function}.
	 * @param ctx the parse tree
	 */
	void exitFunction(MsdsParser.FunctionContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#tuple}.
	 * @param ctx the parse tree
	 */
	void enterTuple(MsdsParser.TupleContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#tuple}.
	 * @param ctx the parse tree
	 */
	void exitTuple(MsdsParser.TupleContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#tuples}.
	 * @param ctx the parse tree
	 */
	void enterTuples(MsdsParser.TuplesContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#tuples}.
	 * @param ctx the parse tree
	 */
	void exitTuples(MsdsParser.TuplesContext ctx);
	/**
	 * Enter a parse tree produced by the {@code filter_operation}
	 * labeled alternative in {@link MsdsParser#filter}.
	 * @param ctx the parse tree
	 */
	void enterFilter_operation(MsdsParser.Filter_operationContext ctx);
	/**
	 * Exit a parse tree produced by the {@code filter_operation}
	 * labeled alternative in {@link MsdsParser#filter}.
	 * @param ctx the parse tree
	 */
	void exitFilter_operation(MsdsParser.Filter_operationContext ctx);
	/**
	 * Enter a parse tree produced by the {@code filter_0}
	 * labeled alternative in {@link MsdsParser#filter}.
	 * @param ctx the parse tree
	 */
	void enterFilter_0(MsdsParser.Filter_0Context ctx);
	/**
	 * Exit a parse tree produced by the {@code filter_0}
	 * labeled alternative in {@link MsdsParser#filter}.
	 * @param ctx the parse tree
	 */
	void exitFilter_0(MsdsParser.Filter_0Context ctx);
	/**
	 * Enter a parse tree produced by the {@code filter_collection}
	 * labeled alternative in {@link MsdsParser#collection_filter}.
	 * @param ctx the parse tree
	 */
	void enterFilter_collection(MsdsParser.Filter_collectionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code filter_collection}
	 * labeled alternative in {@link MsdsParser#collection_filter}.
	 * @param ctx the parse tree
	 */
	void exitFilter_collection(MsdsParser.Filter_collectionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code filter_collections}
	 * labeled alternative in {@link MsdsParser#collection_filter}.
	 * @param ctx the parse tree
	 */
	void enterFilter_collections(MsdsParser.Filter_collectionsContext ctx);
	/**
	 * Exit a parse tree produced by the {@code filter_collections}
	 * labeled alternative in {@link MsdsParser#collection_filter}.
	 * @param ctx the parse tree
	 */
	void exitFilter_collections(MsdsParser.Filter_collectionsContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_pattern}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_pattern(MsdsParser.Condition_patternContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_pattern}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_pattern(MsdsParser.Condition_patternContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_intuples}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_intuples(MsdsParser.Condition_intuplesContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_intuples}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_intuples(MsdsParser.Condition_intuplesContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_inlookups}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_inlookups(MsdsParser.Condition_inlookupsContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_inlookups}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_inlookups(MsdsParser.Condition_inlookupsContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_compound}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_compound(MsdsParser.Condition_compoundContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_compound}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_compound(MsdsParser.Condition_compoundContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_parenthesis}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_parenthesis(MsdsParser.Condition_parenthesisContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_parenthesis}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_parenthesis(MsdsParser.Condition_parenthesisContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_inconsts}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_inconsts(MsdsParser.Condition_inconstsContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_inconsts}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_inconsts(MsdsParser.Condition_inconstsContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_comparison}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_comparison(MsdsParser.Condition_comparisonContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_comparison}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_comparison(MsdsParser.Condition_comparisonContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_exists1}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_exists1(MsdsParser.Condition_exists1Context ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_exists1}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_exists1(MsdsParser.Condition_exists1Context ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_unique}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_unique(MsdsParser.Condition_uniqueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_unique}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_unique(MsdsParser.Condition_uniqueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code condition_exists2}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void enterCondition_exists2(MsdsParser.Condition_exists2Context ctx);
	/**
	 * Exit a parse tree produced by the {@code condition_exists2}
	 * labeled alternative in {@link MsdsParser#condition}.
	 * @param ctx the parse tree
	 */
	void exitCondition_exists2(MsdsParser.Condition_exists2Context ctx);
	/**
	 * Enter a parse tree produced by the {@code expr_component}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterExpr_component(MsdsParser.Expr_componentContext ctx);
	/**
	 * Exit a parse tree produced by the {@code expr_component}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitExpr_component(MsdsParser.Expr_componentContext ctx);
	/**
	 * Enter a parse tree produced by the {@code expr_function}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterExpr_function(MsdsParser.Expr_functionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code expr_function}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitExpr_function(MsdsParser.Expr_functionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code expr_constant}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterExpr_constant(MsdsParser.Expr_constantContext ctx);
	/**
	 * Exit a parse tree produced by the {@code expr_constant}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitExpr_constant(MsdsParser.Expr_constantContext ctx);
	/**
	 * Enter a parse tree produced by the {@code expr_intrinsic}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void enterExpr_intrinsic(MsdsParser.Expr_intrinsicContext ctx);
	/**
	 * Exit a parse tree produced by the {@code expr_intrinsic}
	 * labeled alternative in {@link MsdsParser#expr}.
	 * @param ctx the parse tree
	 */
	void exitExpr_intrinsic(MsdsParser.Expr_intrinsicContext ctx);
	/**
	 * Enter a parse tree produced by the {@code intrinsic_aggregatecount}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void enterIntrinsic_aggregatecount(MsdsParser.Intrinsic_aggregatecountContext ctx);
	/**
	 * Exit a parse tree produced by the {@code intrinsic_aggregatecount}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void exitIntrinsic_aggregatecount(MsdsParser.Intrinsic_aggregatecountContext ctx);
	/**
	 * Enter a parse tree produced by the {@code intrinsic_aggregate}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void enterIntrinsic_aggregate(MsdsParser.Intrinsic_aggregateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code intrinsic_aggregate}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void exitIntrinsic_aggregate(MsdsParser.Intrinsic_aggregateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code intrinsic_arithmetic}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void enterIntrinsic_arithmetic(MsdsParser.Intrinsic_arithmeticContext ctx);
	/**
	 * Exit a parse tree produced by the {@code intrinsic_arithmetic}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void exitIntrinsic_arithmetic(MsdsParser.Intrinsic_arithmeticContext ctx);
	/**
	 * Enter a parse tree produced by the {@code differentialDate}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void enterDifferentialDate(MsdsParser.DifferentialDateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code differentialDate}
	 * labeled alternative in {@link MsdsParser#intrinsic}.
	 * @param ctx the parse tree
	 */
	void exitDifferentialDate(MsdsParser.DifferentialDateContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#constant}.
	 * @param ctx the parse tree
	 */
	void enterConstant(MsdsParser.ConstantContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#constant}.
	 * @param ctx the parse tree
	 */
	void exitConstant(MsdsParser.ConstantContext ctx);
	/**
	 * Enter a parse tree produced by the {@code today}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterToday(MsdsParser.TodayContext ctx);
	/**
	 * Exit a parse tree produced by the {@code today}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitToday(MsdsParser.TodayContext ctx);
	/**
	 * Enter a parse tree produced by the {@code dayMonth}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterDayMonth(MsdsParser.DayMonthContext ctx);
	/**
	 * Exit a parse tree produced by the {@code dayMonth}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitDayMonth(MsdsParser.DayMonthContext ctx);
	/**
	 * Enter a parse tree produced by the {@code dayMonthYear}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterDayMonthYear(MsdsParser.DayMonthYearContext ctx);
	/**
	 * Exit a parse tree produced by the {@code dayMonthYear}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitDayMonthYear(MsdsParser.DayMonthYearContext ctx);
	/**
	 * Enter a parse tree produced by the {@code cardinalDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterCardinalDate(MsdsParser.CardinalDateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code cardinalDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitCardinalDate(MsdsParser.CardinalDateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code componentDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterComponentDate(MsdsParser.ComponentDateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code componentDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitComponentDate(MsdsParser.ComponentDateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code aliasIdDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterAliasIdDate(MsdsParser.AliasIdDateContext ctx);
	/**
	 * Exit a parse tree produced by the {@code aliasIdDate}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitAliasIdDate(MsdsParser.AliasIdDateContext ctx);
	/**
	 * Enter a parse tree produced by the {@code intrinsic_timePeriod}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterIntrinsic_timePeriod(MsdsParser.Intrinsic_timePeriodContext ctx);
	/**
	 * Exit a parse tree produced by the {@code intrinsic_timePeriod}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitIntrinsic_timePeriod(MsdsParser.Intrinsic_timePeriodContext ctx);
	/**
	 * Enter a parse tree produced by the {@code dateoperation}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void enterDateoperation(MsdsParser.DateoperationContext ctx);
	/**
	 * Exit a parse tree produced by the {@code dateoperation}
	 * labeled alternative in {@link MsdsParser#date}.
	 * @param ctx the parse tree
	 */
	void exitDateoperation(MsdsParser.DateoperationContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#bool}.
	 * @param ctx the parse tree
	 */
	void enterBool(MsdsParser.BoolContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#bool}.
	 * @param ctx the parse tree
	 */
	void exitBool(MsdsParser.BoolContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#xint}.
	 * @param ctx the parse tree
	 */
	void enterXint(MsdsParser.XintContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#xint}.
	 * @param ctx the parse tree
	 */
	void exitXint(MsdsParser.XintContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#num}.
	 * @param ctx the parse tree
	 */
	void enterNum(MsdsParser.NumContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#num}.
	 * @param ctx the parse tree
	 */
	void exitNum(MsdsParser.NumContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#str}.
	 * @param ctx the parse tree
	 */
	void enterStr(MsdsParser.StrContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#str}.
	 * @param ctx the parse tree
	 */
	void exitStr(MsdsParser.StrContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#constants}.
	 * @param ctx the parse tree
	 */
	void enterConstants(MsdsParser.ConstantsContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#constants}.
	 * @param ctx the parse tree
	 */
	void exitConstants(MsdsParser.ConstantsContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#comparison}.
	 * @param ctx the parse tree
	 */
	void enterComparison(MsdsParser.ComparisonContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#comparison}.
	 * @param ctx the parse tree
	 */
	void exitComparison(MsdsParser.ComparisonContext ctx);
	/**
	 * Enter a parse tree produced by {@link MsdsParser#operation}.
	 * @param ctx the parse tree
	 */
	void enterOperation(MsdsParser.OperationContext ctx);
	/**
	 * Exit a parse tree produced by {@link MsdsParser#operation}.
	 * @param ctx the parse tree
	 */
	void exitOperation(MsdsParser.OperationContext ctx);
}