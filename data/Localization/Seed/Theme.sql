/* Table [dbo].[Theme] data */
SET IDENTITY_INSERT [dbo].[Theme] ON;
GO


MERGE INTO [dbo].[Theme] AS t
USING
(
    VALUES
    (1, N'Ready Credit', N'Ready Credit', N'readycredit.dev.localhost', N'Ready Credit', N'/logo/ReadyCredit.png', NULL, N'Ready Credit Corp', NULL, NULL),
    (2, N'Bravura', N'Bravura', N'bravura.dev.localhost', N'Bravura', N'/logo/Bravura.png', NULL, N'Bravura LLC', NULL, NULL),
    (3, N'Polymind', N'Polymind', N'polymind.dev.localhost', N'Polymind', N'/logo/Polymind.png', NULL, N'Polymind LLC', NULL, NULL)
)
AS s
([Id], [Name], [Description], [DomainName], [SiteName], [SiteLogo], [SiteStyle], [LegalName], [SupportPhoneNumber], [SupportEmail])
ON (t.[Id] = s.[Id])
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Name], [Description], [DomainName], [SiteName], [SiteLogo], [SiteStyle], [LegalName], [SupportPhoneNumber], [SupportEmail])
    VALUES (s.[Id], s.[Name], s.[Description], s.[DomainName], s.[SiteName], s.[SiteLogo], s.[SiteStyle], s.[LegalName], s.[SupportPhoneNumber], s.[SupportEmail])
WHEN MATCHED THEN
    UPDATE SET t.[Name] = s.[Name], t.[Description] = s.[Description], t.[DomainName] = s.[DomainName], t.[SiteName] = s.[SiteName], t.[SiteLogo] = s.[SiteLogo], t.[SiteStyle] = s.[SiteStyle], t.[LegalName] = s.[LegalName], t.[SupportPhoneNumber] = s.[SupportPhoneNumber], t.[SupportEmail] = s.[SupportEmail]
OUTPUT $action as MergeAction;

SET IDENTITY_INSERT [dbo].[Theme] OFF;
GO

