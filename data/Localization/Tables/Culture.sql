CREATE TABLE [dbo].[Culture]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,

    -- e.g. en-US, fr-FR
    [CultureCode] NVARCHAR(10) NOT NULL,

    [DisplayName] NVARCHAR(100) NOT NULL,
    [NativeName] NVARCHAR(100) NULL,

    -- css class name to apply for flag icons 
    [FlagClass] NVARCHAR(100) NULL,

    [IsActive] BIT NOT NULL CONSTRAINT [DF_Culture_IsActive] DEFAULT (1),
    [IsDefault] BIT NOT NULL CONSTRAINT [DF_Culture_IsDefault] DEFAULT (0),
    [DisplayOrder] INT NOT NULL CONSTRAINT [DF_Culture_DisplayOrder] DEFAULT (0),

    [Created] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Culture_Created] DEFAULT (SYSUTCDATETIME()),
    [CreatedBy] NVARCHAR(100) NULL,
    [Updated] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Culture_Updated] DEFAULT (SYSUTCDATETIME()),
    [UpdatedBy] NVARCHAR(100) NULL,
    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT [PK_Culture] PRIMARY KEY CLUSTERED ([Id] ASC),

    INDEX [UX_Culture_CultureCode] UNIQUE NONCLUSTERED ([CultureCode] ASC),
    INDEX [IX_Culture_IsActive] NONCLUSTERED ([IsActive] ASC),
)
