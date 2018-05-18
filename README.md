# MSDS-RulesEngine
The MSDS Rules Engine is a tool for validating higher-level rules in a database. It uses an english-like syntax to describe the conditions that should apply to a group of data.

## Development Environment
Microsoft Visual Studio 2015 and Microsoft SQL Server 2012 are required for this project.

## Projects
1. Engine - this project contains the core logic for the rules engine.
1. Engine.Db - this is a database project that documents the structure of a sample views/tables, and more importantly, the exact schema of the RuleValidation tables.
1. EngineTest - This project contains unit tests for the engine
1. EngineTest.Db - This project performs full integration tests across all aspects of the rule language against a sample database without logging the results.
1. Rules - this *partial* project does not compile, but contains the current MSDS rules definitions
1. Runner - This is a sample wrapper application for the rules engine. It is a stand-alone console application that may be used as a batch tool, or as an interactive web server for testing or running rules and data. It may be an example for how to integrate the rules engine into another website or tool.

## Architecture
The engine uses an Antlr grammar to define rules. Rules are parsed using an Antlr listener. As rules files are parsed, a model is created. The model is used to evaluate the rules against a database. The grammar files are locaed in the *Language* folder of the Engine project. Also located in this folder are the listener methods that build the rules model. The rules model is located in the *Models* folder. Appropriate SQL statements for each rule are generated using simple in-line string concatenation as well as Handlebars templates (located in the *Templates* directory. _The handlebars template files must be embedded resources in the rules engine._

As the .rules files in a directory (no directory recursion) are being parsed, pieces of SQL are constructed, and stored for later use. When a rule has been completely parsed, a pair of complete SQL statements for that rule has been built: one for evaluating the rule and returning the results, another for placing the results into the RuleValidation tables. SQL parameters are also provided to SQL server when the SQL is run. These parameters are run-time constants that are identified in the rules file as well as the name of the collection being run; the latter being important to distinguish whether or not some rules are applied.

The rules engine expects tables or views in the database that have an integer field named Id, and additional data. Conceptually, all the tables referenced by the rules engine are joined across the Id field. The Id field is not necessarily the primary key for the table or view, and need not be unique, nor need there be a record in every table for every Id. SQL is generated for each rule that joins together all the tables referenced in the rule, as well as the filtering and conditions specified by the rule. Non-conforming records (Id values not matching the rule criteria) are identified by each rule. These non-conforming values are either returned by the rules engine for review, or may be saved directly to the database in the *RuleValidation* and *RuleValidationDetail* tables for later reporting.

The wrapper tool uses *Log4Net* to provide feedback on the progress of the rules. The *app.config* file in the wrapper is configured to provide summary console output, a detailed log file, and SignalR (web sockets) information to the web browser. These may be altered for other purposes as desired.

The compilation process requires a properly configured Java installation. This is required because the Antlr grammar files are "compiled" into listener classes using the Antlr toolkit, which is java based.

A valuable, if optional, tool is the IntelliJ IDE. This ide has a plug-in that supports Antlr v4 grammars. It is useful for verifying the grammary of rules before running them. Rules may also be validated by testing them using the command-line tool or web tool. See the integration tests for examples of how to use the grammar, and the actual grammar files for a complete specifiction.
