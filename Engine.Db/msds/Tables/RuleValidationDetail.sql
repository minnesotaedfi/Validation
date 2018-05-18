CREATE TABLE [msds].[RuleValidationDetail] (
    [RuleValidationId] BIGINT         NOT NULL,
    [Id]               BIGINT         NOT NULL,
    [RuleId]           NVARCHAR (50)  NOT NULL,
    [IsError]          BIT            NOT NULL,
    [Message]          NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [FK_RuleValidationDetail_RuleValidation] FOREIGN KEY ([RuleValidationId]) REFERENCES [msds].[RuleValidation] ([RuleValidationId]) ON DELETE CASCADE ON UPDATE CASCADE
);

