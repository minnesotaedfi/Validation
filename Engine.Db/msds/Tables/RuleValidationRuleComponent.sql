CREATE TABLE [msds].[RuleValidationRuleComponent] (
    [RuleValidationId] BIGINT        NOT NULL,
    [RulesetId]        NVARCHAR (50) NOT NULL,
    [RuleId]           NVARCHAR (50) NOT NULL,
    [Component]        NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RuleValidationRuleComponent] PRIMARY KEY CLUSTERED ([RuleValidationId] ASC, [RulesetId] ASC, [RuleId] ASC, [Component] ASC),
    CONSTRAINT [FK_RuleValidationRuleComponent_RuleValidation] FOREIGN KEY ([RuleValidationId]) REFERENCES [msds].[RuleValidation] ([RuleValidationId]) ON DELETE CASCADE ON UPDATE CASCADE
);



