CREATE TABLE [dbo].[Content]
(
    [Id] INT IDENTITY (1000, 1) NOT NULL,

    [CultureCode] NVARCHAR(10) NOT NULL,
    [LocalizeKey] NVARCHAR(256) NOT NULL,

    [LocalizeText] NVARCHAR(MAX) NULL,

    [Created] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Content_Created] DEFAULT (SYSUTCDATETIME()),
    [CreatedBy] NVARCHAR(100) NULL,
    [Updated] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Content_Updated] DEFAULT (SYSUTCDATETIME()),
    [UpdatedBy] NVARCHAR(100) NULL,
    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Content_Culture_CultureCode] FOREIGN KEY ([CultureCode]) REFERENCES [dbo].[Culture] ([CultureCode]),

    INDEX [UX_Content_Culture_Key] UNIQUE NONCLUSTERED ([CultureCode] ASC, [LocalizeKey] ASC),
)
