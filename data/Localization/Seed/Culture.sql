/* Table [dbo].[Culture] data */
SET IDENTITY_INSERT [dbo].[Culture] ON;
GO


MERGE INTO [dbo].[Culture] AS t
USING
(
    VALUES
    (1, N'en-US', N'English', N'English (United States)', N'fi fi-us', 1, 1, 1),
    (2, N'es-ES', N'Español', N'Español (España)', N'fi fi-es', 1, 0, 2),
    (3, N'fr-FR', N'Français', N'Français (France)', N'fi fi-fr', 1, 0, 3)
)
AS s
([Id], [CultureCode], [DisplayName], [NativeName], [FlagClass], [IsActive], [IsDefault], [DisplayOrder])
ON (t.[Id] = s.[Id])
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [CultureCode], [DisplayName], [NativeName], [FlagClass], [IsActive], [IsDefault], [DisplayOrder])
    VALUES (s.[Id], s.[CultureCode], s.[DisplayName], s.[NativeName], s.[FlagClass], s.[IsActive], s.[IsDefault], s.[DisplayOrder])
WHEN MATCHED THEN
    UPDATE SET t.[CultureCode] = s.[CultureCode], t.[DisplayName] = s.[DisplayName], t.[NativeName] = s.[NativeName], t.[FlagClass] = s.[FlagClass], t.[IsActive] = s.[IsActive], t.[IsDefault] = s.[IsDefault], t.[DisplayOrder] = s.[DisplayOrder]
OUTPUT $action as MergeAction;

SET IDENTITY_INSERT [dbo].[Culture] OFF;
GO

