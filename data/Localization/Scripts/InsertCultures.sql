-- Insert Culture records for English (US), Spanish (Spain), and French (France)
INSERT INTO [dbo].[Culture] ([CultureCode], [DisplayName], [NativeName], [IsActive], [IsDefault], [DisplayOrder], [CreatedBy], [UpdatedBy])
VALUES
    ('en-US', 'English (US)', 'English (United States)', 1, 1, 1, 'System', 'System'),
    ('es-ES', 'Spanish (Spain)', 'Español (España)', 1, 0, 2, 'System', 'System'),
    ('fr-FR', 'French (France)', 'Français (France)', 1, 0, 3, 'System', 'System');
