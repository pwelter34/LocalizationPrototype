CREATE TABLE [dbo].[Theme]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,

    [Name] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(1000) NULL,

    -- ie www.example.com
    [DomainName] NVARCHAR(256) NOT NULL,

    -- ie Example Company
    [SiteName] NVARCHAR(100) NULL,

    [SiteLogo] NVARCHAR(MAX) NULL,
    [SiteStyle] NVARCHAR(MAX) NULL,

    -- ie Example Company LLC
    [LegalName] NVARCHAR(255) NULL,

    [SupportPhoneNumber] NVARCHAR(1000) NULL,
    [SupportEmail] NVARCHAR (1000) NULL,

    [Created] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Theme_Created] DEFAULT (SYSUTCDATETIME()),
    [CreatedBy] NVARCHAR(100) NULL,
    [Updated] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_Theme_Updated] DEFAULT (SYSUTCDATETIME()),
    [UpdatedBy] NVARCHAR(100) NULL,
    [RowVersion] ROWVERSION NOT NULL,

    CONSTRAINT [PK_Theme] PRIMARY KEY CLUSTERED ([Id] ASC),

    INDEX [IX_Theme_Name] NONCLUSTERED ([Name] ASC),
    INDEX [UX_Theme_DomainName] UNIQUE NONCLUSTERED ([DomainName] ASC),
)
