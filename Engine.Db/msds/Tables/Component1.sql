CREATE TABLE [msds].[Component1](
	[Component1Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Id] [bigint] NOT NULL,
	[StringCharacteristic1] [nvarchar](max) NULL,
	[DateCharacteristic1] [datetime] NULL,
	[NumberCharacteristic1] [decimal](18, 2) NULL,
	[BoolCharacteristic1] [bit] NULL,
	[StringCharacteristic2] [nvarchar](max) NULL,
	[DateCharacteristic2] [datetime] NULL,
	[NumberCharacteristic2] [decimal](18, 2) NULL,
	[BoolCharacteristic2] [bit] NULL,
 CONSTRAINT [PK_dbo.Component1] PRIMARY KEY CLUSTERED 
(
	[Component1Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];