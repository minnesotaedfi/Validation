CREATE TABLE [msds].[RuleValidation] (
    [RuleValidationId] BIGINT IDENTITY (1, 1) NOT NULL,
    [CollectionId] NVARCHAR (50) NULL,
    [RunDateTime]  DATETIME CONSTRAINT [DF_RuleValidation_RunDateTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_RuleValidation] PRIMARY KEY CLUSTERED ([RuleValidationId] ASC)
);

