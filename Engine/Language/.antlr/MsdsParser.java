// Generated from d:\github\Validation\Engine\Language\MsdsParser.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class MsdsParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		AFTER=1, ALL=2, AND=3, AS=4, BEFORE=5, BY=6, COLLECTION=7, DO=8, DEFINE=9, 
		ELSE=10, EXIST=11, EXPECT=12, FALSE=13, FOR=14, INCLUDES=15, IN=16, IS=17, 
		MATCH=18, NOT=19, NULL=20, OF=21, OR=22, REQUIRE=23, RULE=24, RULESET=25, 
		SINCE=26, TRUE=27, THAT=28, THE=29, THEN=30, TODAY=31, UNIQUE=32, WHEN=33, 
		LT=34, LE=35, EQ=36, GE=37, GT=38, NE=39, L_PAREN=40, R_PAREN=41, L_BRACKET=42, 
		R_BRACKET=43, L_BRACE=44, R_BRACE=45, COMMA=46, DOT=47, NEG=48, RULEID1=49, 
		RULEID2=50, INT=51, NUMBER=52, DATE1=53, DATE2=54, AGGREGATE=55, COUNT=56, 
		CARDINAL=57, DATEOP=58, MONTH=59, TIMEUNIT=60, WEEKDAY=61, ID=62, STRING=63, 
		COMMENT=64, WS=65;
	public static final int
		RULE_file = 0, RULE_collection = 1, RULE_collectionid = 2, RULE_aliasId = 3, 
		RULE_alias = 4, RULE_aliasDefinitions = 5, RULE_ruleset = 6, RULE_rulesetid = 7, 
		RULE_ruleDefinition = 8, RULE_ruleid = 9, RULE_error = 10, RULE_pattern = 11, 
		RULE_component = 12, RULE_components = 13, RULE_lookups = 14, RULE_componentid = 15, 
		RULE_characteristicid = 16, RULE_characteristicids = 17, RULE_functionid = 18, 
		RULE_function = 19, RULE_tuple = 20, RULE_tuples = 21, RULE_filter = 22, 
		RULE_collection_filter = 23, RULE_condition = 24, RULE_expr = 25, RULE_intrinsic = 26, 
		RULE_constant = 27, RULE_date = 28, RULE_bool = 29, RULE_int = 30, RULE_num = 31, 
		RULE_str = 32, RULE_constants = 33, RULE_comparison = 34, RULE_operation = 35;
	public static final String[] ruleNames = {
		"file", "collection", "collectionid", "aliasId", "alias", "aliasDefinitions", 
		"ruleset", "rulesetid", "ruleDefinition", "ruleid", "error", "pattern", 
		"component", "components", "lookups", "componentid", "characteristicid", 
		"characteristicids", "functionid", "function", "tuple", "tuples", "filter", 
		"collection_filter", "condition", "expr", "intrinsic", "constant", "date", 
		"bool", "int", "num", "str", "constants", "comparison", "operation"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'after'", "'all'", "'and'", "'as'", "'before'", "'by'", "'collection'", 
		null, null, "'else'", null, "'expect'", "'false'", "'for'", "'includes'", 
		"'in'", "'is'", null, null, "'null'", "'of'", "'or'", "'require'", "'rule'", 
		"'ruleset'", "'since'", "'true'", "'that'", "'the'", "'then'", "'today'", 
		"'unique'", "'when'", "'<'", null, "'='", null, "'>'", null, "'('", "')'", 
		"'['", "']'", "'{'", "'}'", "','", "'.'", "'-'", null, null, null, null, 
		null, null, null, "'count'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "AFTER", "ALL", "AND", "AS", "BEFORE", "BY", "COLLECTION", "DO", 
		"DEFINE", "ELSE", "EXIST", "EXPECT", "FALSE", "FOR", "INCLUDES", "IN", 
		"IS", "MATCH", "NOT", "NULL", "OF", "OR", "REQUIRE", "RULE", "RULESET", 
		"SINCE", "TRUE", "THAT", "THE", "THEN", "TODAY", "UNIQUE", "WHEN", "LT", 
		"LE", "EQ", "GE", "GT", "NE", "L_PAREN", "R_PAREN", "L_BRACKET", "R_BRACKET", 
		"L_BRACE", "R_BRACE", "COMMA", "DOT", "NEG", "RULEID1", "RULEID2", "INT", 
		"NUMBER", "DATE1", "DATE2", "AGGREGATE", "COUNT", "CARDINAL", "DATEOP", 
		"MONTH", "TIMEUNIT", "WEEKDAY", "ID", "STRING", "COMMENT", "WS"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "MsdsParser.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public MsdsParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class FileContext extends ParserRuleContext {
		public List<AliasDefinitionsContext> aliasDefinitions() {
			return getRuleContexts(AliasDefinitionsContext.class);
		}
		public AliasDefinitionsContext aliasDefinitions(int i) {
			return getRuleContext(AliasDefinitionsContext.class,i);
		}
		public List<CollectionContext> collection() {
			return getRuleContexts(CollectionContext.class);
		}
		public CollectionContext collection(int i) {
			return getRuleContext(CollectionContext.class,i);
		}
		public List<RulesetContext> ruleset() {
			return getRuleContexts(RulesetContext.class);
		}
		public RulesetContext ruleset(int i) {
			return getRuleContext(RulesetContext.class,i);
		}
		public FileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_file; }
	}

	public final FileContext file() throws RecognitionException {
		FileContext _localctx = new FileContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_file);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(77);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << COLLECTION) | (1L << DEFINE) | (1L << RULESET))) != 0)) {
				{
				setState(75);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case DEFINE:
					{
					setState(72);
					aliasDefinitions();
					}
					break;
				case COLLECTION:
					{
					setState(73);
					collection();
					}
					break;
				case RULESET:
					{
					setState(74);
					ruleset();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				setState(79);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CollectionContext extends ParserRuleContext {
		public TerminalNode COLLECTION() { return getToken(MsdsParser.COLLECTION, 0); }
		public CollectionidContext collectionid() {
			return getRuleContext(CollectionidContext.class,0);
		}
		public TerminalNode INCLUDES() { return getToken(MsdsParser.INCLUDES, 0); }
		public List<RulesetidContext> rulesetid() {
			return getRuleContexts(RulesetidContext.class);
		}
		public RulesetidContext rulesetid(int i) {
			return getRuleContext(RulesetidContext.class,i);
		}
		public List<RuleidContext> ruleid() {
			return getRuleContexts(RuleidContext.class);
		}
		public RuleidContext ruleid(int i) {
			return getRuleContext(RuleidContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public AliasDefinitionsContext aliasDefinitions() {
			return getRuleContext(AliasDefinitionsContext.class,0);
		}
		public CollectionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_collection; }
	}

	public final CollectionContext collection() throws RecognitionException {
		CollectionContext _localctx = new CollectionContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_collection);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(80);
			match(COLLECTION);
			setState(81);
			collectionid();
			setState(82);
			match(INCLUDES);
			setState(85);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ID:
				{
				setState(83);
				rulesetid();
				}
				break;
			case RULEID1:
			case RULEID2:
				{
				setState(84);
				ruleid();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			setState(94);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(87);
				match(COMMA);
				setState(90);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case ID:
					{
					setState(88);
					rulesetid();
					}
					break;
				case RULEID1:
				case RULEID2:
					{
					setState(89);
					ruleid();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				}
				setState(96);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(98);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,5,_ctx) ) {
			case 1:
				{
				setState(97);
				aliasDefinitions();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CollectionidContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public CollectionidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_collectionid; }
	}

	public final CollectionidContext collectionid() throws RecognitionException {
		CollectionidContext _localctx = new CollectionidContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_collectionid);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(100);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AliasIdContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public AliasIdContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_aliasId; }
	}

	public final AliasIdContext aliasId() throws RecognitionException {
		AliasIdContext _localctx = new AliasIdContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_aliasId);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(102);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AliasContext extends ParserRuleContext {
		public AliasIdContext aliasId() {
			return getRuleContext(AliasIdContext.class,0);
		}
		public TerminalNode EQ() { return getToken(MsdsParser.EQ, 0); }
		public ConstantContext constant() {
			return getRuleContext(ConstantContext.class,0);
		}
		public AliasContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_alias; }
	}

	public final AliasContext alias() throws RecognitionException {
		AliasContext _localctx = new AliasContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_alias);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(104);
			aliasId();
			setState(105);
			match(EQ);
			setState(106);
			constant();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AliasDefinitionsContext extends ParserRuleContext {
		public TerminalNode DEFINE() { return getToken(MsdsParser.DEFINE, 0); }
		public List<AliasContext> alias() {
			return getRuleContexts(AliasContext.class);
		}
		public AliasContext alias(int i) {
			return getRuleContext(AliasContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public AliasDefinitionsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_aliasDefinitions; }
	}

	public final AliasDefinitionsContext aliasDefinitions() throws RecognitionException {
		AliasDefinitionsContext _localctx = new AliasDefinitionsContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_aliasDefinitions);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(108);
			match(DEFINE);
			setState(109);
			alias();
			setState(114);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(110);
				match(COMMA);
				setState(111);
				alias();
				}
				}
				setState(116);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class RulesetContext extends ParserRuleContext {
		public TerminalNode RULESET() { return getToken(MsdsParser.RULESET, 0); }
		public RulesetidContext rulesetid() {
			return getRuleContext(RulesetidContext.class,0);
		}
		public List<RuleDefinitionContext> ruleDefinition() {
			return getRuleContexts(RuleDefinitionContext.class);
		}
		public RuleDefinitionContext ruleDefinition(int i) {
			return getRuleContext(RuleDefinitionContext.class,i);
		}
		public RulesetContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ruleset; }
	}

	public final RulesetContext ruleset() throws RecognitionException {
		RulesetContext _localctx = new RulesetContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_ruleset);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(117);
			match(RULESET);
			setState(118);
			rulesetid();
			setState(120); 
			_errHandler.sync(this);
			_la = _input.LA(1);
			do {
				{
				{
				setState(119);
				ruleDefinition();
				}
				}
				setState(122); 
				_errHandler.sync(this);
				_la = _input.LA(1);
			} while ( _la==RULE );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class RulesetidContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public RulesetidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_rulesetid; }
	}

	public final RulesetidContext rulesetid() throws RecognitionException {
		RulesetidContext _localctx = new RulesetidContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_rulesetid);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(124);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class RuleDefinitionContext extends ParserRuleContext {
		public TerminalNode RULE() { return getToken(MsdsParser.RULE, 0); }
		public RuleidContext ruleid() {
			return getRuleContext(RuleidContext.class,0);
		}
		public ConditionContext condition() {
			return getRuleContext(ConditionContext.class,0);
		}
		public TerminalNode ELSE() { return getToken(MsdsParser.ELSE, 0); }
		public ErrorContext error() {
			return getRuleContext(ErrorContext.class,0);
		}
		public TerminalNode REQUIRE() { return getToken(MsdsParser.REQUIRE, 0); }
		public TerminalNode EXPECT() { return getToken(MsdsParser.EXPECT, 0); }
		public TerminalNode WHEN() { return getToken(MsdsParser.WHEN, 0); }
		public FilterContext filter() {
			return getRuleContext(FilterContext.class,0);
		}
		public TerminalNode THEN() { return getToken(MsdsParser.THEN, 0); }
		public TerminalNode THAT() { return getToken(MsdsParser.THAT, 0); }
		public RuleDefinitionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ruleDefinition; }
	}

	public final RuleDefinitionContext ruleDefinition() throws RecognitionException {
		RuleDefinitionContext _localctx = new RuleDefinitionContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_ruleDefinition);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(126);
			match(RULE);
			setState(127);
			ruleid();
			setState(132);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==WHEN) {
				{
				setState(128);
				match(WHEN);
				setState(129);
				filter(0);
				setState(130);
				match(THEN);
				}
			}

			setState(134);
			_la = _input.LA(1);
			if ( !(_la==EXPECT || _la==REQUIRE) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			setState(136);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==THAT) {
				{
				setState(135);
				match(THAT);
				}
			}

			setState(138);
			condition(0);
			setState(139);
			match(ELSE);
			setState(140);
			error();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class RuleidContext extends ParserRuleContext {
		public TerminalNode RULEID1() { return getToken(MsdsParser.RULEID1, 0); }
		public TerminalNode RULEID2() { return getToken(MsdsParser.RULEID2, 0); }
		public RuleidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ruleid; }
	}

	public final RuleidContext ruleid() throws RecognitionException {
		RuleidContext _localctx = new RuleidContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_ruleid);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(142);
			_la = _input.LA(1);
			if ( !(_la==RULEID1 || _la==RULEID2) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ErrorContext extends ParserRuleContext {
		public TerminalNode STRING() { return getToken(MsdsParser.STRING, 0); }
		public ErrorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_error; }
	}

	public final ErrorContext error() throws RecognitionException {
		ErrorContext _localctx = new ErrorContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_error);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(144);
			match(STRING);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PatternContext extends ParserRuleContext {
		public TerminalNode STRING() { return getToken(MsdsParser.STRING, 0); }
		public PatternContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pattern; }
	}

	public final PatternContext pattern() throws RecognitionException {
		PatternContext _localctx = new PatternContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_pattern);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(146);
			match(STRING);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ComponentContext extends ParserRuleContext {
		public ComponentidContext componentid() {
			return getRuleContext(ComponentidContext.class,0);
		}
		public TerminalNode DOT() { return getToken(MsdsParser.DOT, 0); }
		public CharacteristicidContext characteristicid() {
			return getRuleContext(CharacteristicidContext.class,0);
		}
		public ComponentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_component; }
	}

	public final ComponentContext component() throws RecognitionException {
		ComponentContext _localctx = new ComponentContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_component);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(148);
			componentid();
			setState(149);
			match(DOT);
			setState(150);
			characteristicid();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ComponentsContext extends ParserRuleContext {
		public ComponentidContext componentid() {
			return getRuleContext(ComponentidContext.class,0);
		}
		public TerminalNode DOT() { return getToken(MsdsParser.DOT, 0); }
		public CharacteristicidsContext characteristicids() {
			return getRuleContext(CharacteristicidsContext.class,0);
		}
		public ComponentsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_components; }
	}

	public final ComponentsContext components() throws RecognitionException {
		ComponentsContext _localctx = new ComponentsContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_components);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(152);
			componentid();
			setState(153);
			match(DOT);
			setState(154);
			characteristicids();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LookupsContext extends ParserRuleContext {
		public ComponentidContext componentid() {
			return getRuleContext(ComponentidContext.class,0);
		}
		public TerminalNode DOT() { return getToken(MsdsParser.DOT, 0); }
		public CharacteristicidsContext characteristicids() {
			return getRuleContext(CharacteristicidsContext.class,0);
		}
		public LookupsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_lookups; }
	}

	public final LookupsContext lookups() throws RecognitionException {
		LookupsContext _localctx = new LookupsContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_lookups);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(156);
			componentid();
			setState(157);
			match(DOT);
			setState(158);
			characteristicids();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ComponentidContext extends ParserRuleContext {
		public TerminalNode L_BRACE() { return getToken(MsdsParser.L_BRACE, 0); }
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public TerminalNode R_BRACE() { return getToken(MsdsParser.R_BRACE, 0); }
		public ComponentidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_componentid; }
	}

	public final ComponentidContext componentid() throws RecognitionException {
		ComponentidContext _localctx = new ComponentidContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_componentid);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(160);
			match(L_BRACE);
			setState(161);
			match(ID);
			setState(162);
			match(R_BRACE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CharacteristicidContext extends ParserRuleContext {
		public TerminalNode L_BRACKET() { return getToken(MsdsParser.L_BRACKET, 0); }
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public TerminalNode R_BRACKET() { return getToken(MsdsParser.R_BRACKET, 0); }
		public CharacteristicidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_characteristicid; }
	}

	public final CharacteristicidContext characteristicid() throws RecognitionException {
		CharacteristicidContext _localctx = new CharacteristicidContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_characteristicid);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(164);
			match(L_BRACKET);
			setState(165);
			match(ID);
			setState(166);
			match(R_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CharacteristicidsContext extends ParserRuleContext {
		public TerminalNode L_BRACKET() { return getToken(MsdsParser.L_BRACKET, 0); }
		public TerminalNode R_BRACKET() { return getToken(MsdsParser.R_BRACKET, 0); }
		public List<TerminalNode> ID() { return getTokens(MsdsParser.ID); }
		public TerminalNode ID(int i) {
			return getToken(MsdsParser.ID, i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public CharacteristicidsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_characteristicids; }
	}

	public final CharacteristicidsContext characteristicids() throws RecognitionException {
		CharacteristicidsContext _localctx = new CharacteristicidsContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_characteristicids);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(168);
			match(L_BRACKET);
			setState(170);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ID) {
				{
				setState(169);
				match(ID);
				}
			}

			setState(176);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(172);
				match(COMMA);
				setState(173);
				match(ID);
				}
				}
				setState(178);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(179);
			match(R_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctionidContext extends ParserRuleContext {
		public TerminalNode ID() { return getToken(MsdsParser.ID, 0); }
		public FunctionidContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_functionid; }
	}

	public final FunctionidContext functionid() throws RecognitionException {
		FunctionidContext _localctx = new FunctionidContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_functionid);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(181);
			match(ID);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctionContext extends ParserRuleContext {
		public FunctionidContext functionid() {
			return getRuleContext(FunctionidContext.class,0);
		}
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public FunctionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_function; }
	}

	public final FunctionContext function() throws RecognitionException {
		FunctionContext _localctx = new FunctionContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_function);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(183);
			functionid();
			setState(184);
			match(L_PAREN);
			setState(186);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << FALSE) | (1L << TRUE) | (1L << THE) | (1L << TODAY) | (1L << L_BRACE) | (1L << RULEID1) | (1L << INT) | (1L << NUMBER) | (1L << DATE1) | (1L << DATE2) | (1L << AGGREGATE) | (1L << COUNT) | (1L << CARDINAL) | (1L << DATEOP) | (1L << TIMEUNIT) | (1L << ID) | (1L << STRING))) != 0)) {
				{
				setState(185);
				expr();
				}
			}

			setState(192);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(188);
				match(COMMA);
				setState(189);
				expr();
				}
				}
				setState(194);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(195);
			match(R_PAREN);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TupleContext extends ParserRuleContext {
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public List<ConstantContext> constant() {
			return getRuleContexts(ConstantContext.class);
		}
		public ConstantContext constant(int i) {
			return getRuleContext(ConstantContext.class,i);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public TupleContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tuple; }
	}

	public final TupleContext tuple() throws RecognitionException {
		TupleContext _localctx = new TupleContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_tuple);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(197);
			match(L_PAREN);
			setState(198);
			constant();
			setState(203);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(199);
				match(COMMA);
				setState(200);
				constant();
				}
				}
				setState(205);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(206);
			match(R_PAREN);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TuplesContext extends ParserRuleContext {
		public TerminalNode L_BRACKET() { return getToken(MsdsParser.L_BRACKET, 0); }
		public List<TupleContext> tuple() {
			return getRuleContexts(TupleContext.class);
		}
		public TupleContext tuple(int i) {
			return getRuleContext(TupleContext.class,i);
		}
		public TerminalNode R_BRACKET() { return getToken(MsdsParser.R_BRACKET, 0); }
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public TuplesContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tuples; }
	}

	public final TuplesContext tuples() throws RecognitionException {
		TuplesContext _localctx = new TuplesContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_tuples);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(208);
			match(L_BRACKET);
			setState(209);
			tuple();
			setState(214);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(210);
				match(COMMA);
				setState(211);
				tuple();
				}
				}
				setState(216);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(217);
			match(R_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FilterContext extends ParserRuleContext {
		public FilterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_filter; }
	 
		public FilterContext() { }
		public void copyFrom(FilterContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Filter_operationContext extends FilterContext {
		public List<FilterContext> filter() {
			return getRuleContexts(FilterContext.class);
		}
		public FilterContext filter(int i) {
			return getRuleContext(FilterContext.class,i);
		}
		public OperationContext operation() {
			return getRuleContext(OperationContext.class,0);
		}
		public Filter_operationContext(FilterContext ctx) { copyFrom(ctx); }
	}
	public static class Filter_0Context extends FilterContext {
		public Collection_filterContext collection_filter() {
			return getRuleContext(Collection_filterContext.class,0);
		}
		public ConditionContext condition() {
			return getRuleContext(ConditionContext.class,0);
		}
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public FilterContext filter() {
			return getRuleContext(FilterContext.class,0);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public Filter_0Context(FilterContext ctx) { copyFrom(ctx); }
	}

	public final FilterContext filter() throws RecognitionException {
		return filter(0);
	}

	private FilterContext filter(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		FilterContext _localctx = new FilterContext(_ctx, _parentState);
		FilterContext _prevctx = _localctx;
		int _startState = 44;
		enterRecursionRule(_localctx, 44, RULE_filter, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(226);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,16,_ctx) ) {
			case 1:
				{
				_localctx = new Filter_0Context(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(220);
				collection_filter();
				}
				break;
			case 2:
				{
				_localctx = new Filter_0Context(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(221);
				condition(0);
				}
				break;
			case 3:
				{
				_localctx = new Filter_0Context(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(222);
				match(L_PAREN);
				setState(223);
				filter(0);
				setState(224);
				match(R_PAREN);
				}
				break;
			}
			_ctx.stop = _input.LT(-1);
			setState(234);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,17,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					{
					_localctx = new Filter_operationContext(new FilterContext(_parentctx, _parentState));
					pushNewRecursionContext(_localctx, _startState, RULE_filter);
					setState(228);
					if (!(precpred(_ctx, 1))) throw new FailedPredicateException(this, "precpred(_ctx, 1)");
					setState(229);
					operation();
					setState(230);
					filter(2);
					}
					} 
				}
				setState(236);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,17,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class Collection_filterContext extends ParserRuleContext {
		public Collection_filterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_collection_filter; }
	 
		public Collection_filterContext() { }
		public void copyFrom(Collection_filterContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Filter_collectionContext extends Collection_filterContext {
		public TerminalNode COLLECTION() { return getToken(MsdsParser.COLLECTION, 0); }
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public CollectionidContext collectionid() {
			return getRuleContext(CollectionidContext.class,0);
		}
		public Filter_collectionContext(Collection_filterContext ctx) { copyFrom(ctx); }
	}
	public static class Filter_collectionsContext extends Collection_filterContext {
		public TerminalNode COLLECTION() { return getToken(MsdsParser.COLLECTION, 0); }
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public TerminalNode L_BRACKET() { return getToken(MsdsParser.L_BRACKET, 0); }
		public TerminalNode R_BRACKET() { return getToken(MsdsParser.R_BRACKET, 0); }
		public List<CollectionidContext> collectionid() {
			return getRuleContexts(CollectionidContext.class);
		}
		public CollectionidContext collectionid(int i) {
			return getRuleContext(CollectionidContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public Filter_collectionsContext(Collection_filterContext ctx) { copyFrom(ctx); }
	}

	public final Collection_filterContext collection_filter() throws RecognitionException {
		Collection_filterContext _localctx = new Collection_filterContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_collection_filter);
		int _la;
		try {
			setState(255);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,20,_ctx) ) {
			case 1:
				_localctx = new Filter_collectionContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(237);
				match(COLLECTION);
				setState(238);
				match(IS);
				setState(239);
				collectionid();
				}
				break;
			case 2:
				_localctx = new Filter_collectionsContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(240);
				match(COLLECTION);
				setState(241);
				match(IS);
				setState(242);
				match(IN);
				setState(243);
				match(L_BRACKET);
				setState(245);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==ID) {
					{
					setState(244);
					collectionid();
					}
				}

				setState(251);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(247);
					match(COMMA);
					setState(248);
					collectionid();
					}
					}
					setState(253);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(254);
				match(R_BRACKET);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConditionContext extends ParserRuleContext {
		public ConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_condition; }
	 
		public ConditionContext() { }
		public void copyFrom(ConditionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Condition_patternContext extends ConditionContext {
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public TerminalNode MATCH() { return getToken(MsdsParser.MATCH, 0); }
		public PatternContext pattern() {
			return getRuleContext(PatternContext.class,0);
		}
		public TerminalNode DO() { return getToken(MsdsParser.DO, 0); }
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_patternContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_intuplesContext extends ConditionContext {
		public ComponentsContext components() {
			return getRuleContext(ComponentsContext.class,0);
		}
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public TuplesContext tuples() {
			return getRuleContext(TuplesContext.class,0);
		}
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_intuplesContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_inlookupsContext extends ConditionContext {
		public ComponentsContext components() {
			return getRuleContext(ComponentsContext.class,0);
		}
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public LookupsContext lookups() {
			return getRuleContext(LookupsContext.class,0);
		}
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_inlookupsContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_compoundContext extends ConditionContext {
		public List<ConditionContext> condition() {
			return getRuleContexts(ConditionContext.class);
		}
		public ConditionContext condition(int i) {
			return getRuleContext(ConditionContext.class,i);
		}
		public List<OperationContext> operation() {
			return getRuleContexts(OperationContext.class);
		}
		public OperationContext operation(int i) {
			return getRuleContext(OperationContext.class,i);
		}
		public Condition_compoundContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_parenthesisContext extends ConditionContext {
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public ConditionContext condition() {
			return getRuleContext(ConditionContext.class,0);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public Condition_parenthesisContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_inconstsContext extends ConditionContext {
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public ConstantsContext constants() {
			return getRuleContext(ConstantsContext.class,0);
		}
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_inconstsContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_comparisonContext extends ConditionContext {
		public List<ExprContext> expr() {
			return getRuleContexts(ExprContext.class);
		}
		public ExprContext expr(int i) {
			return getRuleContext(ExprContext.class,i);
		}
		public ComparisonContext comparison() {
			return getRuleContext(ComparisonContext.class,0);
		}
		public Condition_comparisonContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_exists1Context extends ConditionContext {
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public TerminalNode EXIST() { return getToken(MsdsParser.EXIST, 0); }
		public TerminalNode DO() { return getToken(MsdsParser.DO, 0); }
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_exists1Context(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_uniqueContext extends ConditionContext {
		public ComponentsContext components() {
			return getRuleContext(ComponentsContext.class,0);
		}
		public TerminalNode IS() { return getToken(MsdsParser.IS, 0); }
		public TerminalNode UNIQUE() { return getToken(MsdsParser.UNIQUE, 0); }
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_uniqueContext(ConditionContext ctx) { copyFrom(ctx); }
	}
	public static class Condition_exists2Context extends ConditionContext {
		public ComponentidContext componentid() {
			return getRuleContext(ComponentidContext.class,0);
		}
		public TerminalNode EXIST() { return getToken(MsdsParser.EXIST, 0); }
		public TerminalNode DO() { return getToken(MsdsParser.DO, 0); }
		public TerminalNode NOT() { return getToken(MsdsParser.NOT, 0); }
		public Condition_exists2Context(ConditionContext ctx) { copyFrom(ctx); }
	}

	public final ConditionContext condition() throws RecognitionException {
		return condition(0);
	}

	private ConditionContext condition(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ConditionContext _localctx = new ConditionContext(_ctx, _parentState);
		ConditionContext _prevctx = _localctx;
		int _startState = 48;
		enterRecursionRule(_localctx, 48, RULE_condition, _p);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(319);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,28,_ctx) ) {
			case 1:
				{
				_localctx = new Condition_parenthesisContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(258);
				match(L_PAREN);
				setState(259);
				condition(0);
				setState(260);
				match(R_PAREN);
				}
				break;
			case 2:
				{
				_localctx = new Condition_patternContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(262);
				component();
				setState(265);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==DO) {
					{
					setState(263);
					match(DO);
					setState(264);
					match(NOT);
					}
				}

				setState(267);
				match(MATCH);
				setState(268);
				pattern();
				}
				break;
			case 3:
				{
				_localctx = new Condition_exists1Context(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(270);
				component();
				setState(273);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==DO) {
					{
					setState(271);
					match(DO);
					setState(272);
					match(NOT);
					}
				}

				setState(275);
				match(EXIST);
				}
				break;
			case 4:
				{
				_localctx = new Condition_exists2Context(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(277);
				componentid();
				setState(280);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==DO) {
					{
					setState(278);
					match(DO);
					setState(279);
					match(NOT);
					}
				}

				setState(282);
				match(EXIST);
				}
				break;
			case 5:
				{
				_localctx = new Condition_uniqueContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(284);
				components();
				setState(285);
				match(IS);
				setState(287);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NOT) {
					{
					setState(286);
					match(NOT);
					}
				}

				setState(289);
				match(UNIQUE);
				}
				break;
			case 6:
				{
				_localctx = new Condition_intuplesContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(291);
				components();
				setState(292);
				match(IS);
				setState(294);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NOT) {
					{
					setState(293);
					match(NOT);
					}
				}

				setState(296);
				match(IN);
				setState(297);
				tuples();
				}
				break;
			case 7:
				{
				_localctx = new Condition_inconstsContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(299);
				component();
				setState(300);
				match(IS);
				setState(302);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NOT) {
					{
					setState(301);
					match(NOT);
					}
				}

				setState(304);
				match(IN);
				setState(305);
				constants();
				}
				break;
			case 8:
				{
				_localctx = new Condition_inlookupsContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(307);
				components();
				setState(308);
				match(IS);
				setState(310);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==NOT) {
					{
					setState(309);
					match(NOT);
					}
				}

				setState(312);
				match(IN);
				setState(313);
				lookups();
				}
				break;
			case 9:
				{
				_localctx = new Condition_comparisonContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(315);
				expr();
				setState(316);
				comparison();
				setState(317);
				expr();
				}
				break;
			}
			_ctx.stop = _input.LT(-1);
			setState(331);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					{
					_localctx = new Condition_compoundContext(new ConditionContext(_parentctx, _parentState));
					pushNewRecursionContext(_localctx, _startState, RULE_condition);
					setState(321);
					if (!(precpred(_ctx, 9))) throw new FailedPredicateException(this, "precpred(_ctx, 9)");
					setState(325); 
					_errHandler.sync(this);
					_alt = 1;
					do {
						switch (_alt) {
						case 1:
							{
							{
							setState(322);
							operation();
							setState(323);
							condition(0);
							}
							}
							break;
						default:
							throw new NoViableAltException(this);
						}
						setState(327); 
						_errHandler.sync(this);
						_alt = getInterpreter().adaptivePredict(_input,29,_ctx);
					} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
					}
					} 
				}
				setState(333);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class ExprContext extends ParserRuleContext {
		public ExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expr; }
	 
		public ExprContext() { }
		public void copyFrom(ExprContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Expr_componentContext extends ExprContext {
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public Expr_componentContext(ExprContext ctx) { copyFrom(ctx); }
	}
	public static class Expr_constantContext extends ExprContext {
		public ConstantContext constant() {
			return getRuleContext(ConstantContext.class,0);
		}
		public Expr_constantContext(ExprContext ctx) { copyFrom(ctx); }
	}
	public static class Expr_functionContext extends ExprContext {
		public FunctionContext function() {
			return getRuleContext(FunctionContext.class,0);
		}
		public Expr_functionContext(ExprContext ctx) { copyFrom(ctx); }
	}
	public static class Expr_intrinsicContext extends ExprContext {
		public IntrinsicContext intrinsic() {
			return getRuleContext(IntrinsicContext.class,0);
		}
		public Expr_intrinsicContext(ExprContext ctx) { copyFrom(ctx); }
	}

	public final ExprContext expr() throws RecognitionException {
		ExprContext _localctx = new ExprContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_expr);
		try {
			setState(338);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,31,_ctx) ) {
			case 1:
				_localctx = new Expr_componentContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(334);
				component();
				}
				break;
			case 2:
				_localctx = new Expr_functionContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(335);
				function();
				}
				break;
			case 3:
				_localctx = new Expr_constantContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(336);
				constant();
				}
				break;
			case 4:
				_localctx = new Expr_intrinsicContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(337);
				intrinsic();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IntrinsicContext extends ParserRuleContext {
		public IntrinsicContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_intrinsic; }
	 
		public IntrinsicContext() { }
		public void copyFrom(IntrinsicContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Intrinsic_aggregateContext extends IntrinsicContext {
		public TerminalNode AGGREGATE() { return getToken(MsdsParser.AGGREGATE, 0); }
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public TerminalNode BY() { return getToken(MsdsParser.BY, 0); }
		public CharacteristicidsContext characteristicids() {
			return getRuleContext(CharacteristicidsContext.class,0);
		}
		public TerminalNode WHEN() { return getToken(MsdsParser.WHEN, 0); }
		public ConditionContext condition() {
			return getRuleContext(ConditionContext.class,0);
		}
		public Intrinsic_aggregateContext(IntrinsicContext ctx) { copyFrom(ctx); }
	}
	public static class Intrinsic_arithmeticContext extends IntrinsicContext {
		public TerminalNode AGGREGATE() { return getToken(MsdsParser.AGGREGATE, 0); }
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public List<ComponentsContext> components() {
			return getRuleContexts(ComponentsContext.class);
		}
		public ComponentsContext components(int i) {
			return getRuleContext(ComponentsContext.class,i);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public Intrinsic_arithmeticContext(IntrinsicContext ctx) { copyFrom(ctx); }
	}
	public static class DifferentialDateContext extends IntrinsicContext {
		public IntContext int() {
			return getRuleContext(IntContext.class,0);
		}
		public TerminalNode TIMEUNIT() { return getToken(MsdsParser.TIMEUNIT, 0); }
		public DateContext date() {
			return getRuleContext(DateContext.class,0);
		}
		public TerminalNode BEFORE() { return getToken(MsdsParser.BEFORE, 0); }
		public TerminalNode AFTER() { return getToken(MsdsParser.AFTER, 0); }
		public DifferentialDateContext(IntrinsicContext ctx) { copyFrom(ctx); }
	}
	public static class Intrinsic_aggregatecountContext extends IntrinsicContext {
		public TerminalNode COUNT() { return getToken(MsdsParser.COUNT, 0); }
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public ComponentidContext componentid() {
			return getRuleContext(ComponentidContext.class,0);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public TerminalNode BY() { return getToken(MsdsParser.BY, 0); }
		public CharacteristicidsContext characteristicids() {
			return getRuleContext(CharacteristicidsContext.class,0);
		}
		public TerminalNode WHEN() { return getToken(MsdsParser.WHEN, 0); }
		public ConditionContext condition() {
			return getRuleContext(ConditionContext.class,0);
		}
		public Intrinsic_aggregatecountContext(IntrinsicContext ctx) { copyFrom(ctx); }
	}

	public final IntrinsicContext intrinsic() throws RecognitionException {
		IntrinsicContext _localctx = new IntrinsicContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_intrinsic);
		int _la;
		try {
			setState(383);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,37,_ctx) ) {
			case 1:
				_localctx = new Intrinsic_aggregatecountContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(340);
				match(COUNT);
				setState(341);
				match(L_PAREN);
				setState(342);
				componentid();
				setState(345);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==BY) {
					{
					setState(343);
					match(BY);
					setState(344);
					characteristicids();
					}
				}

				setState(349);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==WHEN) {
					{
					setState(347);
					match(WHEN);
					setState(348);
					condition(0);
					}
				}

				setState(351);
				match(R_PAREN);
				}
				break;
			case 2:
				_localctx = new Intrinsic_aggregateContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(353);
				match(AGGREGATE);
				setState(354);
				match(L_PAREN);
				setState(355);
				component();
				setState(358);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==BY) {
					{
					setState(356);
					match(BY);
					setState(357);
					characteristicids();
					}
				}

				setState(362);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==WHEN) {
					{
					setState(360);
					match(WHEN);
					setState(361);
					condition(0);
					}
				}

				setState(364);
				match(R_PAREN);
				}
				break;
			case 3:
				_localctx = new Intrinsic_arithmeticContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(366);
				match(AGGREGATE);
				setState(367);
				match(L_PAREN);
				setState(368);
				components();
				setState(373);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==COMMA) {
					{
					{
					setState(369);
					match(COMMA);
					setState(370);
					components();
					}
					}
					setState(375);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(376);
				match(R_PAREN);
				}
				break;
			case 4:
				_localctx = new DifferentialDateContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(378);
				int();
				setState(379);
				match(TIMEUNIT);
				setState(380);
				_la = _input.LA(1);
				if ( !(_la==AFTER || _la==BEFORE) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(381);
				date();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstantContext extends ParserRuleContext {
		public BoolContext bool() {
			return getRuleContext(BoolContext.class,0);
		}
		public DateContext date() {
			return getRuleContext(DateContext.class,0);
		}
		public NumContext num() {
			return getRuleContext(NumContext.class,0);
		}
		public StrContext str() {
			return getRuleContext(StrContext.class,0);
		}
		public AliasIdContext aliasId() {
			return getRuleContext(AliasIdContext.class,0);
		}
		public ConstantContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constant; }
	}

	public final ConstantContext constant() throws RecognitionException {
		ConstantContext _localctx = new ConstantContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_constant);
		try {
			setState(390);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,38,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(385);
				bool();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(386);
				date();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(387);
				num();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(388);
				str();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(389);
				aliasId();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DateContext extends ParserRuleContext {
		public DateContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_date; }
	 
		public DateContext() { }
		public void copyFrom(DateContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Intrinsic_timePeriodContext extends DateContext {
		public TerminalNode TIMEUNIT() { return getToken(MsdsParser.TIMEUNIT, 0); }
		public TerminalNode SINCE() { return getToken(MsdsParser.SINCE, 0); }
		public List<DateContext> date() {
			return getRuleContexts(DateContext.class);
		}
		public DateContext date(int i) {
			return getRuleContext(DateContext.class,i);
		}
		public TerminalNode AS() { return getToken(MsdsParser.AS, 0); }
		public TerminalNode OF() { return getToken(MsdsParser.OF, 0); }
		public Intrinsic_timePeriodContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class CardinalDateContext extends DateContext {
		public TerminalNode CARDINAL() { return getToken(MsdsParser.CARDINAL, 0); }
		public TerminalNode WEEKDAY() { return getToken(MsdsParser.WEEKDAY, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public TerminalNode MONTH() { return getToken(MsdsParser.MONTH, 0); }
		public CardinalDateContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class ComponentDateContext extends DateContext {
		public ComponentContext component() {
			return getRuleContext(ComponentContext.class,0);
		}
		public ComponentDateContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class TodayContext extends DateContext {
		public TerminalNode TODAY() { return getToken(MsdsParser.TODAY, 0); }
		public TodayContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class DayMonthContext extends DateContext {
		public TerminalNode DATE1() { return getToken(MsdsParser.DATE1, 0); }
		public DayMonthContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class DateoperationContext extends DateContext {
		public TerminalNode DATEOP() { return getToken(MsdsParser.DATEOP, 0); }
		public TerminalNode L_PAREN() { return getToken(MsdsParser.L_PAREN, 0); }
		public List<DateContext> date() {
			return getRuleContexts(DateContext.class);
		}
		public DateContext date(int i) {
			return getRuleContext(DateContext.class,i);
		}
		public TerminalNode R_PAREN() { return getToken(MsdsParser.R_PAREN, 0); }
		public TerminalNode THE() { return getToken(MsdsParser.THE, 0); }
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public TerminalNode OF() { return getToken(MsdsParser.OF, 0); }
		public TerminalNode IN() { return getToken(MsdsParser.IN, 0); }
		public DateoperationContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class DayMonthYearContext extends DateContext {
		public TerminalNode DATE2() { return getToken(MsdsParser.DATE2, 0); }
		public DayMonthYearContext(DateContext ctx) { copyFrom(ctx); }
	}
	public static class AliasIdDateContext extends DateContext {
		public AliasIdContext aliasId() {
			return getRuleContext(AliasIdContext.class,0);
		}
		public AliasIdDateContext(DateContext ctx) { copyFrom(ctx); }
	}

	public final DateContext date() throws RecognitionException {
		DateContext _localctx = new DateContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_date);
		int _la;
		try {
			setState(426);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case TODAY:
				_localctx = new TodayContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(392);
				match(TODAY);
				}
				break;
			case DATE1:
				_localctx = new DayMonthContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(393);
				match(DATE1);
				}
				break;
			case DATE2:
				_localctx = new DayMonthYearContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(394);
				match(DATE2);
				}
				break;
			case CARDINAL:
				_localctx = new CardinalDateContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(395);
				match(CARDINAL);
				setState(396);
				match(WEEKDAY);
				setState(397);
				match(IN);
				setState(398);
				match(MONTH);
				}
				break;
			case L_BRACE:
				_localctx = new ComponentDateContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(399);
				component();
				}
				break;
			case ID:
				_localctx = new AliasIdDateContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(400);
				aliasId();
				}
				break;
			case TIMEUNIT:
				_localctx = new Intrinsic_timePeriodContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(401);
				match(TIMEUNIT);
				setState(402);
				match(SINCE);
				setState(403);
				date();
				setState(407);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,39,_ctx) ) {
				case 1:
					{
					setState(404);
					match(AS);
					setState(405);
					match(OF);
					setState(406);
					date();
					}
					break;
				}
				}
				break;
			case THE:
			case DATEOP:
				_localctx = new DateoperationContext(_localctx);
				enterOuterAlt(_localctx, 8);
				{
				setState(410);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==THE) {
					{
					setState(409);
					match(THE);
					}
				}

				setState(412);
				match(DATEOP);
				setState(414);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==IN || _la==OF) {
					{
					setState(413);
					_la = _input.LA(1);
					if ( !(_la==IN || _la==OF) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					}
				}

				setState(416);
				match(L_PAREN);
				setState(417);
				date();
				setState(420); 
				_errHandler.sync(this);
				_la = _input.LA(1);
				do {
					{
					{
					setState(418);
					match(COMMA);
					setState(419);
					date();
					}
					}
					setState(422); 
					_errHandler.sync(this);
					_la = _input.LA(1);
				} while ( _la==COMMA );
				setState(424);
				match(R_PAREN);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BoolContext extends ParserRuleContext {
		public TerminalNode TRUE() { return getToken(MsdsParser.TRUE, 0); }
		public TerminalNode FALSE() { return getToken(MsdsParser.FALSE, 0); }
		public BoolContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_bool; }
	}

	public final BoolContext bool() throws RecognitionException {
		BoolContext _localctx = new BoolContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_bool);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(428);
			_la = _input.LA(1);
			if ( !(_la==FALSE || _la==TRUE) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IntContext extends ParserRuleContext {
		public TerminalNode INT() { return getToken(MsdsParser.INT, 0); }
		public IntContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_int; }
	}

	public final IntContext int() throws RecognitionException {
		IntContext _localctx = new IntContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_int);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(430);
			match(INT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NumContext extends ParserRuleContext {
		public TerminalNode RULEID1() { return getToken(MsdsParser.RULEID1, 0); }
		public TerminalNode NUMBER() { return getToken(MsdsParser.NUMBER, 0); }
		public TerminalNode INT() { return getToken(MsdsParser.INT, 0); }
		public NumContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_num; }
	}

	public final NumContext num() throws RecognitionException {
		NumContext _localctx = new NumContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_num);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(432);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << RULEID1) | (1L << INT) | (1L << NUMBER))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StrContext extends ParserRuleContext {
		public TerminalNode STRING() { return getToken(MsdsParser.STRING, 0); }
		public StrContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_str; }
	}

	public final StrContext str() throws RecognitionException {
		StrContext _localctx = new StrContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_str);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(434);
			match(STRING);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstantsContext extends ParserRuleContext {
		public TerminalNode L_BRACKET() { return getToken(MsdsParser.L_BRACKET, 0); }
		public TerminalNode R_BRACKET() { return getToken(MsdsParser.R_BRACKET, 0); }
		public List<ConstantContext> constant() {
			return getRuleContexts(ConstantContext.class);
		}
		public ConstantContext constant(int i) {
			return getRuleContext(ConstantContext.class,i);
		}
		public List<TerminalNode> COMMA() { return getTokens(MsdsParser.COMMA); }
		public TerminalNode COMMA(int i) {
			return getToken(MsdsParser.COMMA, i);
		}
		public ConstantsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constants; }
	}

	public final ConstantsContext constants() throws RecognitionException {
		ConstantsContext _localctx = new ConstantsContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_constants);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(436);
			match(L_BRACKET);
			setState(438);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << FALSE) | (1L << TRUE) | (1L << THE) | (1L << TODAY) | (1L << L_BRACE) | (1L << RULEID1) | (1L << INT) | (1L << NUMBER) | (1L << DATE1) | (1L << DATE2) | (1L << CARDINAL) | (1L << DATEOP) | (1L << TIMEUNIT) | (1L << ID) | (1L << STRING))) != 0)) {
				{
				setState(437);
				constant();
				}
			}

			setState(444);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==COMMA) {
				{
				{
				setState(440);
				match(COMMA);
				setState(441);
				constant();
				}
				}
				setState(446);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(447);
			match(R_BRACKET);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ComparisonContext extends ParserRuleContext {
		public TerminalNode LT() { return getToken(MsdsParser.LT, 0); }
		public TerminalNode LE() { return getToken(MsdsParser.LE, 0); }
		public TerminalNode EQ() { return getToken(MsdsParser.EQ, 0); }
		public TerminalNode GE() { return getToken(MsdsParser.GE, 0); }
		public TerminalNode GT() { return getToken(MsdsParser.GT, 0); }
		public TerminalNode NE() { return getToken(MsdsParser.NE, 0); }
		public ComparisonContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_comparison; }
	}

	public final ComparisonContext comparison() throws RecognitionException {
		ComparisonContext _localctx = new ComparisonContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_comparison);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(449);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << LT) | (1L << LE) | (1L << EQ) | (1L << GE) | (1L << GT) | (1L << NE))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperationContext extends ParserRuleContext {
		public TerminalNode AND() { return getToken(MsdsParser.AND, 0); }
		public TerminalNode OR() { return getToken(MsdsParser.OR, 0); }
		public OperationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operation; }
	}

	public final OperationContext operation() throws RecognitionException {
		OperationContext _localctx = new OperationContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_operation);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(451);
			_la = _input.LA(1);
			if ( !(_la==AND || _la==OR) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 22:
			return filter_sempred((FilterContext)_localctx, predIndex);
		case 24:
			return condition_sempred((ConditionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean filter_sempred(FilterContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 1);
		}
		return true;
	}
	private boolean condition_sempred(ConditionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 1:
			return precpred(_ctx, 9);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3C\u01c8\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\3\2\3\2\3\2\7\2N\n\2\f\2\16\2Q\13\2\3\3"+
		"\3\3\3\3\3\3\3\3\5\3X\n\3\3\3\3\3\3\3\5\3]\n\3\7\3_\n\3\f\3\16\3b\13\3"+
		"\3\3\5\3e\n\3\3\4\3\4\3\5\3\5\3\6\3\6\3\6\3\6\3\7\3\7\3\7\3\7\7\7s\n\7"+
		"\f\7\16\7v\13\7\3\b\3\b\3\b\6\b{\n\b\r\b\16\b|\3\t\3\t\3\n\3\n\3\n\3\n"+
		"\3\n\3\n\5\n\u0087\n\n\3\n\3\n\5\n\u008b\n\n\3\n\3\n\3\n\3\n\3\13\3\13"+
		"\3\f\3\f\3\r\3\r\3\16\3\16\3\16\3\16\3\17\3\17\3\17\3\17\3\20\3\20\3\20"+
		"\3\20\3\21\3\21\3\21\3\21\3\22\3\22\3\22\3\22\3\23\3\23\5\23\u00ad\n\23"+
		"\3\23\3\23\7\23\u00b1\n\23\f\23\16\23\u00b4\13\23\3\23\3\23\3\24\3\24"+
		"\3\25\3\25\3\25\5\25\u00bd\n\25\3\25\3\25\7\25\u00c1\n\25\f\25\16\25\u00c4"+
		"\13\25\3\25\3\25\3\26\3\26\3\26\3\26\7\26\u00cc\n\26\f\26\16\26\u00cf"+
		"\13\26\3\26\3\26\3\27\3\27\3\27\3\27\7\27\u00d7\n\27\f\27\16\27\u00da"+
		"\13\27\3\27\3\27\3\30\3\30\3\30\3\30\3\30\3\30\3\30\5\30\u00e5\n\30\3"+
		"\30\3\30\3\30\3\30\7\30\u00eb\n\30\f\30\16\30\u00ee\13\30\3\31\3\31\3"+
		"\31\3\31\3\31\3\31\3\31\3\31\5\31\u00f8\n\31\3\31\3\31\7\31\u00fc\n\31"+
		"\f\31\16\31\u00ff\13\31\3\31\5\31\u0102\n\31\3\32\3\32\3\32\3\32\3\32"+
		"\3\32\3\32\3\32\5\32\u010c\n\32\3\32\3\32\3\32\3\32\3\32\3\32\5\32\u0114"+
		"\n\32\3\32\3\32\3\32\3\32\3\32\5\32\u011b\n\32\3\32\3\32\3\32\3\32\3\32"+
		"\5\32\u0122\n\32\3\32\3\32\3\32\3\32\3\32\5\32\u0129\n\32\3\32\3\32\3"+
		"\32\3\32\3\32\3\32\5\32\u0131\n\32\3\32\3\32\3\32\3\32\3\32\3\32\5\32"+
		"\u0139\n\32\3\32\3\32\3\32\3\32\3\32\3\32\3\32\5\32\u0142\n\32\3\32\3"+
		"\32\3\32\3\32\6\32\u0148\n\32\r\32\16\32\u0149\7\32\u014c\n\32\f\32\16"+
		"\32\u014f\13\32\3\33\3\33\3\33\3\33\5\33\u0155\n\33\3\34\3\34\3\34\3\34"+
		"\3\34\5\34\u015c\n\34\3\34\3\34\5\34\u0160\n\34\3\34\3\34\3\34\3\34\3"+
		"\34\3\34\3\34\5\34\u0169\n\34\3\34\3\34\5\34\u016d\n\34\3\34\3\34\3\34"+
		"\3\34\3\34\3\34\3\34\7\34\u0176\n\34\f\34\16\34\u0179\13\34\3\34\3\34"+
		"\3\34\3\34\3\34\3\34\3\34\5\34\u0182\n\34\3\35\3\35\3\35\3\35\3\35\5\35"+
		"\u0189\n\35\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\5\36\u019a\n\36\3\36\5\36\u019d\n\36\3\36\3\36\5\36\u01a1"+
		"\n\36\3\36\3\36\3\36\3\36\6\36\u01a7\n\36\r\36\16\36\u01a8\3\36\3\36\5"+
		"\36\u01ad\n\36\3\37\3\37\3 \3 \3!\3!\3\"\3\"\3#\3#\5#\u01b9\n#\3#\3#\7"+
		"#\u01bd\n#\f#\16#\u01c0\13#\3#\3#\3$\3$\3%\3%\3%\2\4.\62&\2\4\6\b\n\f"+
		"\16\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>@BDFH\2\n\4\2\16\16"+
		"\31\31\3\2\63\64\4\2\3\3\7\7\4\2\22\22\27\27\4\2\17\17\35\35\4\2\63\63"+
		"\65\66\3\2$)\4\2\5\5\30\30\2\u01e7\2O\3\2\2\2\4R\3\2\2\2\6f\3\2\2\2\b"+
		"h\3\2\2\2\nj\3\2\2\2\fn\3\2\2\2\16w\3\2\2\2\20~\3\2\2\2\22\u0080\3\2\2"+
		"\2\24\u0090\3\2\2\2\26\u0092\3\2\2\2\30\u0094\3\2\2\2\32\u0096\3\2\2\2"+
		"\34\u009a\3\2\2\2\36\u009e\3\2\2\2 \u00a2\3\2\2\2\"\u00a6\3\2\2\2$\u00aa"+
		"\3\2\2\2&\u00b7\3\2\2\2(\u00b9\3\2\2\2*\u00c7\3\2\2\2,\u00d2\3\2\2\2."+
		"\u00e4\3\2\2\2\60\u0101\3\2\2\2\62\u0141\3\2\2\2\64\u0154\3\2\2\2\66\u0181"+
		"\3\2\2\28\u0188\3\2\2\2:\u01ac\3\2\2\2<\u01ae\3\2\2\2>\u01b0\3\2\2\2@"+
		"\u01b2\3\2\2\2B\u01b4\3\2\2\2D\u01b6\3\2\2\2F\u01c3\3\2\2\2H\u01c5\3\2"+
		"\2\2JN\5\f\7\2KN\5\4\3\2LN\5\16\b\2MJ\3\2\2\2MK\3\2\2\2ML\3\2\2\2NQ\3"+
		"\2\2\2OM\3\2\2\2OP\3\2\2\2P\3\3\2\2\2QO\3\2\2\2RS\7\t\2\2ST\5\6\4\2TW"+
		"\7\21\2\2UX\5\20\t\2VX\5\24\13\2WU\3\2\2\2WV\3\2\2\2X`\3\2\2\2Y\\\7\60"+
		"\2\2Z]\5\20\t\2[]\5\24\13\2\\Z\3\2\2\2\\[\3\2\2\2]_\3\2\2\2^Y\3\2\2\2"+
		"_b\3\2\2\2`^\3\2\2\2`a\3\2\2\2ad\3\2\2\2b`\3\2\2\2ce\5\f\7\2dc\3\2\2\2"+
		"de\3\2\2\2e\5\3\2\2\2fg\7@\2\2g\7\3\2\2\2hi\7@\2\2i\t\3\2\2\2jk\5\b\5"+
		"\2kl\7&\2\2lm\58\35\2m\13\3\2\2\2no\7\13\2\2ot\5\n\6\2pq\7\60\2\2qs\5"+
		"\n\6\2rp\3\2\2\2sv\3\2\2\2tr\3\2\2\2tu\3\2\2\2u\r\3\2\2\2vt\3\2\2\2wx"+
		"\7\33\2\2xz\5\20\t\2y{\5\22\n\2zy\3\2\2\2{|\3\2\2\2|z\3\2\2\2|}\3\2\2"+
		"\2}\17\3\2\2\2~\177\7@\2\2\177\21\3\2\2\2\u0080\u0081\7\32\2\2\u0081\u0086"+
		"\5\24\13\2\u0082\u0083\7#\2\2\u0083\u0084\5.\30\2\u0084\u0085\7 \2\2\u0085"+
		"\u0087\3\2\2\2\u0086\u0082\3\2\2\2\u0086\u0087\3\2\2\2\u0087\u0088\3\2"+
		"\2\2\u0088\u008a\t\2\2\2\u0089\u008b\7\36\2\2\u008a\u0089\3\2\2\2\u008a"+
		"\u008b\3\2\2\2\u008b\u008c\3\2\2\2\u008c\u008d\5\62\32\2\u008d\u008e\7"+
		"\f\2\2\u008e\u008f\5\26\f\2\u008f\23\3\2\2\2\u0090\u0091\t\3\2\2\u0091"+
		"\25\3\2\2\2\u0092\u0093\7A\2\2\u0093\27\3\2\2\2\u0094\u0095\7A\2\2\u0095"+
		"\31\3\2\2\2\u0096\u0097\5 \21\2\u0097\u0098\7\61\2\2\u0098\u0099\5\"\22"+
		"\2\u0099\33\3\2\2\2\u009a\u009b\5 \21\2\u009b\u009c\7\61\2\2\u009c\u009d"+
		"\5$\23\2\u009d\35\3\2\2\2\u009e\u009f\5 \21\2\u009f\u00a0\7\61\2\2\u00a0"+
		"\u00a1\5$\23\2\u00a1\37\3\2\2\2\u00a2\u00a3\7.\2\2\u00a3\u00a4\7@\2\2"+
		"\u00a4\u00a5\7/\2\2\u00a5!\3\2\2\2\u00a6\u00a7\7,\2\2\u00a7\u00a8\7@\2"+
		"\2\u00a8\u00a9\7-\2\2\u00a9#\3\2\2\2\u00aa\u00ac\7,\2\2\u00ab\u00ad\7"+
		"@\2\2\u00ac\u00ab\3\2\2\2\u00ac\u00ad\3\2\2\2\u00ad\u00b2\3\2\2\2\u00ae"+
		"\u00af\7\60\2\2\u00af\u00b1\7@\2\2\u00b0\u00ae\3\2\2\2\u00b1\u00b4\3\2"+
		"\2\2\u00b2\u00b0\3\2\2\2\u00b2\u00b3\3\2\2\2\u00b3\u00b5\3\2\2\2\u00b4"+
		"\u00b2\3\2\2\2\u00b5\u00b6\7-\2\2\u00b6%\3\2\2\2\u00b7\u00b8\7@\2\2\u00b8"+
		"\'\3\2\2\2\u00b9\u00ba\5&\24\2\u00ba\u00bc\7*\2\2\u00bb\u00bd\5\64\33"+
		"\2\u00bc\u00bb\3\2\2\2\u00bc\u00bd\3\2\2\2\u00bd\u00c2\3\2\2\2\u00be\u00bf"+
		"\7\60\2\2\u00bf\u00c1\5\64\33\2\u00c0\u00be\3\2\2\2\u00c1\u00c4\3\2\2"+
		"\2\u00c2\u00c0\3\2\2\2\u00c2\u00c3\3\2\2\2\u00c3\u00c5\3\2\2\2\u00c4\u00c2"+
		"\3\2\2\2\u00c5\u00c6\7+\2\2\u00c6)\3\2\2\2\u00c7\u00c8\7*\2\2\u00c8\u00cd"+
		"\58\35\2\u00c9\u00ca\7\60\2\2\u00ca\u00cc\58\35\2\u00cb\u00c9\3\2\2\2"+
		"\u00cc\u00cf\3\2\2\2\u00cd\u00cb\3\2\2\2\u00cd\u00ce\3\2\2\2\u00ce\u00d0"+
		"\3\2\2\2\u00cf\u00cd\3\2\2\2\u00d0\u00d1\7+\2\2\u00d1+\3\2\2\2\u00d2\u00d3"+
		"\7,\2\2\u00d3\u00d8\5*\26\2\u00d4\u00d5\7\60\2\2\u00d5\u00d7\5*\26\2\u00d6"+
		"\u00d4\3\2\2\2\u00d7\u00da\3\2\2\2\u00d8\u00d6\3\2\2\2\u00d8\u00d9\3\2"+
		"\2\2\u00d9\u00db\3\2\2\2\u00da\u00d8\3\2\2\2\u00db\u00dc\7-\2\2\u00dc"+
		"-\3\2\2\2\u00dd\u00de\b\30\1\2\u00de\u00e5\5\60\31\2\u00df\u00e5\5\62"+
		"\32\2\u00e0\u00e1\7*\2\2\u00e1\u00e2\5.\30\2\u00e2\u00e3\7+\2\2\u00e3"+
		"\u00e5\3\2\2\2\u00e4\u00dd\3\2\2\2\u00e4\u00df\3\2\2\2\u00e4\u00e0\3\2"+
		"\2\2\u00e5\u00ec\3\2\2\2\u00e6\u00e7\f\3\2\2\u00e7\u00e8\5H%\2\u00e8\u00e9"+
		"\5.\30\4\u00e9\u00eb\3\2\2\2\u00ea\u00e6\3\2\2\2\u00eb\u00ee\3\2\2\2\u00ec"+
		"\u00ea\3\2\2\2\u00ec\u00ed\3\2\2\2\u00ed/\3\2\2\2\u00ee\u00ec\3\2\2\2"+
		"\u00ef\u00f0\7\t\2\2\u00f0\u00f1\7\23\2\2\u00f1\u0102\5\6\4\2\u00f2\u00f3"+
		"\7\t\2\2\u00f3\u00f4\7\23\2\2\u00f4\u00f5\7\22\2\2\u00f5\u00f7\7,\2\2"+
		"\u00f6\u00f8\5\6\4\2\u00f7\u00f6\3\2\2\2\u00f7\u00f8\3\2\2\2\u00f8\u00fd"+
		"\3\2\2\2\u00f9\u00fa\7\60\2\2\u00fa\u00fc\5\6\4\2\u00fb\u00f9\3\2\2\2"+
		"\u00fc\u00ff\3\2\2\2\u00fd\u00fb\3\2\2\2\u00fd\u00fe\3\2\2\2\u00fe\u0100"+
		"\3\2\2\2\u00ff\u00fd\3\2\2\2\u0100\u0102\7-\2\2\u0101\u00ef\3\2\2\2\u0101"+
		"\u00f2\3\2\2\2\u0102\61\3\2\2\2\u0103\u0104\b\32\1\2\u0104\u0105\7*\2"+
		"\2\u0105\u0106\5\62\32\2\u0106\u0107\7+\2\2\u0107\u0142\3\2\2\2\u0108"+
		"\u010b\5\32\16\2\u0109\u010a\7\n\2\2\u010a\u010c\7\25\2\2\u010b\u0109"+
		"\3\2\2\2\u010b\u010c\3\2\2\2\u010c\u010d\3\2\2\2\u010d\u010e\7\24\2\2"+
		"\u010e\u010f\5\30\r\2\u010f\u0142\3\2\2\2\u0110\u0113\5\32\16\2\u0111"+
		"\u0112\7\n\2\2\u0112\u0114\7\25\2\2\u0113\u0111\3\2\2\2\u0113\u0114\3"+
		"\2\2\2\u0114\u0115\3\2\2\2\u0115\u0116\7\r\2\2\u0116\u0142\3\2\2\2\u0117"+
		"\u011a\5 \21\2\u0118\u0119\7\n\2\2\u0119\u011b\7\25\2\2\u011a\u0118\3"+
		"\2\2\2\u011a\u011b\3\2\2\2\u011b\u011c\3\2\2\2\u011c\u011d\7\r\2\2\u011d"+
		"\u0142\3\2\2\2\u011e\u011f\5\34\17\2\u011f\u0121\7\23\2\2\u0120\u0122"+
		"\7\25\2\2\u0121\u0120\3\2\2\2\u0121\u0122\3\2\2\2\u0122\u0123\3\2\2\2"+
		"\u0123\u0124\7\"\2\2\u0124\u0142\3\2\2\2\u0125\u0126\5\34\17\2\u0126\u0128"+
		"\7\23\2\2\u0127\u0129\7\25\2\2\u0128\u0127\3\2\2\2\u0128\u0129\3\2\2\2"+
		"\u0129\u012a\3\2\2\2\u012a\u012b\7\22\2\2\u012b\u012c\5,\27\2\u012c\u0142"+
		"\3\2\2\2\u012d\u012e\5\32\16\2\u012e\u0130\7\23\2\2\u012f\u0131\7\25\2"+
		"\2\u0130\u012f\3\2\2\2\u0130\u0131\3\2\2\2\u0131\u0132\3\2\2\2\u0132\u0133"+
		"\7\22\2\2\u0133\u0134\5D#\2\u0134\u0142\3\2\2\2\u0135\u0136\5\34\17\2"+
		"\u0136\u0138\7\23\2\2\u0137\u0139\7\25\2\2\u0138\u0137\3\2\2\2\u0138\u0139"+
		"\3\2\2\2\u0139\u013a\3\2\2\2\u013a\u013b\7\22\2\2\u013b\u013c\5\36\20"+
		"\2\u013c\u0142\3\2\2\2\u013d\u013e\5\64\33\2\u013e\u013f\5F$\2\u013f\u0140"+
		"\5\64\33\2\u0140\u0142\3\2\2\2\u0141\u0103\3\2\2\2\u0141\u0108\3\2\2\2"+
		"\u0141\u0110\3\2\2\2\u0141\u0117\3\2\2\2\u0141\u011e\3\2\2\2\u0141\u0125"+
		"\3\2\2\2\u0141\u012d\3\2\2\2\u0141\u0135\3\2\2\2\u0141\u013d\3\2\2\2\u0142"+
		"\u014d\3\2\2\2\u0143\u0147\f\13\2\2\u0144\u0145\5H%\2\u0145\u0146\5\62"+
		"\32\2\u0146\u0148\3\2\2\2\u0147\u0144\3\2\2\2\u0148\u0149\3\2\2\2\u0149"+
		"\u0147\3\2\2\2\u0149\u014a\3\2\2\2\u014a\u014c\3\2\2\2\u014b\u0143\3\2"+
		"\2\2\u014c\u014f\3\2\2\2\u014d\u014b\3\2\2\2\u014d\u014e\3\2\2\2\u014e"+
		"\63\3\2\2\2\u014f\u014d\3\2\2\2\u0150\u0155\5\32\16\2\u0151\u0155\5(\25"+
		"\2\u0152\u0155\58\35\2\u0153\u0155\5\66\34\2\u0154\u0150\3\2\2\2\u0154"+
		"\u0151\3\2\2\2\u0154\u0152\3\2\2\2\u0154\u0153\3\2\2\2\u0155\65\3\2\2"+
		"\2\u0156\u0157\7:\2\2\u0157\u0158\7*\2\2\u0158\u015b\5 \21\2\u0159\u015a"+
		"\7\b\2\2\u015a\u015c\5$\23\2\u015b\u0159\3\2\2\2\u015b\u015c\3\2\2\2\u015c"+
		"\u015f\3\2\2\2\u015d\u015e\7#\2\2\u015e\u0160\5\62\32\2\u015f\u015d\3"+
		"\2\2\2\u015f\u0160\3\2\2\2\u0160\u0161\3\2\2\2\u0161\u0162\7+\2\2\u0162"+
		"\u0182\3\2\2\2\u0163\u0164\79\2\2\u0164\u0165\7*\2\2\u0165\u0168\5\32"+
		"\16\2\u0166\u0167\7\b\2\2\u0167\u0169\5$\23\2\u0168\u0166\3\2\2\2\u0168"+
		"\u0169\3\2\2\2\u0169\u016c\3\2\2\2\u016a\u016b\7#\2\2\u016b\u016d\5\62"+
		"\32\2\u016c\u016a\3\2\2\2\u016c\u016d\3\2\2\2\u016d\u016e\3\2\2\2\u016e"+
		"\u016f\7+\2\2\u016f\u0182\3\2\2\2\u0170\u0171\79\2\2\u0171\u0172\7*\2"+
		"\2\u0172\u0177\5\34\17\2\u0173\u0174\7\60\2\2\u0174\u0176\5\34\17\2\u0175"+
		"\u0173\3\2\2\2\u0176\u0179\3\2\2\2\u0177\u0175\3\2\2\2\u0177\u0178\3\2"+
		"\2\2\u0178\u017a\3\2\2\2\u0179\u0177\3\2\2\2\u017a\u017b\7+\2\2\u017b"+
		"\u0182\3\2\2\2\u017c\u017d\5> \2\u017d\u017e\7>\2\2\u017e\u017f\t\4\2"+
		"\2\u017f\u0180\5:\36\2\u0180\u0182\3\2\2\2\u0181\u0156\3\2\2\2\u0181\u0163"+
		"\3\2\2\2\u0181\u0170\3\2\2\2\u0181\u017c\3\2\2\2\u0182\67\3\2\2\2\u0183"+
		"\u0189\5<\37\2\u0184\u0189\5:\36\2\u0185\u0189\5@!\2\u0186\u0189\5B\""+
		"\2\u0187\u0189\5\b\5\2\u0188\u0183\3\2\2\2\u0188\u0184\3\2\2\2\u0188\u0185"+
		"\3\2\2\2\u0188\u0186\3\2\2\2\u0188\u0187\3\2\2\2\u01899\3\2\2\2\u018a"+
		"\u01ad\7!\2\2\u018b\u01ad\7\67\2\2\u018c\u01ad\78\2\2\u018d\u018e\7;\2"+
		"\2\u018e\u018f\7?\2\2\u018f\u0190\7\22\2\2\u0190\u01ad\7=\2\2\u0191\u01ad"+
		"\5\32\16\2\u0192\u01ad\5\b\5\2\u0193\u0194\7>\2\2\u0194\u0195\7\34\2\2"+
		"\u0195\u0199\5:\36\2\u0196\u0197\7\6\2\2\u0197\u0198\7\27\2\2\u0198\u019a"+
		"\5:\36\2\u0199\u0196\3\2\2\2\u0199\u019a\3\2\2\2\u019a\u01ad\3\2\2\2\u019b"+
		"\u019d\7\37\2\2\u019c\u019b\3\2\2\2\u019c\u019d\3\2\2\2\u019d\u019e\3"+
		"\2\2\2\u019e\u01a0\7<\2\2\u019f\u01a1\t\5\2\2\u01a0\u019f\3\2\2\2\u01a0"+
		"\u01a1\3\2\2\2\u01a1\u01a2\3\2\2\2\u01a2\u01a3\7*\2\2\u01a3\u01a6\5:\36"+
		"\2\u01a4\u01a5\7\60\2\2\u01a5\u01a7\5:\36\2\u01a6\u01a4\3\2\2\2\u01a7"+
		"\u01a8\3\2\2\2\u01a8\u01a6\3\2\2\2\u01a8\u01a9\3\2\2\2\u01a9\u01aa\3\2"+
		"\2\2\u01aa\u01ab\7+\2\2\u01ab\u01ad\3\2\2\2\u01ac\u018a\3\2\2\2\u01ac"+
		"\u018b\3\2\2\2\u01ac\u018c\3\2\2\2\u01ac\u018d\3\2\2\2\u01ac\u0191\3\2"+
		"\2\2\u01ac\u0192\3\2\2\2\u01ac\u0193\3\2\2\2\u01ac\u019c\3\2\2\2\u01ad"+
		";\3\2\2\2\u01ae\u01af\t\6\2\2\u01af=\3\2\2\2\u01b0\u01b1\7\65\2\2\u01b1"+
		"?\3\2\2\2\u01b2\u01b3\t\7\2\2\u01b3A\3\2\2\2\u01b4\u01b5\7A\2\2\u01b5"+
		"C\3\2\2\2\u01b6\u01b8\7,\2\2\u01b7\u01b9\58\35\2\u01b8\u01b7\3\2\2\2\u01b8"+
		"\u01b9\3\2\2\2\u01b9\u01be\3\2\2\2\u01ba\u01bb\7\60\2\2\u01bb\u01bd\5"+
		"8\35\2\u01bc\u01ba\3\2\2\2\u01bd\u01c0\3\2\2\2\u01be\u01bc\3\2\2\2\u01be"+
		"\u01bf\3\2\2\2\u01bf\u01c1\3\2\2\2\u01c0\u01be\3\2\2\2\u01c1\u01c2\7-"+
		"\2\2\u01c2E\3\2\2\2\u01c3\u01c4\t\b\2\2\u01c4G\3\2\2\2\u01c5\u01c6\t\t"+
		"\2\2\u01c6I\3\2\2\2\60MOW\\`dt|\u0086\u008a\u00ac\u00b2\u00bc\u00c2\u00cd"+
		"\u00d8\u00e4\u00ec\u00f7\u00fd\u0101\u010b\u0113\u011a\u0121\u0128\u0130"+
		"\u0138\u0141\u0149\u014d\u0154\u015b\u015f\u0168\u016c\u0177\u0181\u0188"+
		"\u0199\u019c\u01a0\u01a8\u01ac\u01b8\u01be";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}