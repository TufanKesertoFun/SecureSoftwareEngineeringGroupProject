CREATE TABLE [dbo].[CustomerReview]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CustomerName] NVARCHAR(150) NOT NULL,
    [Rating] INT NOT NULL CHECK ([Rating] BETWEEN 1 AND 5),
    [ReviewText] NVARCHAR(2000) NOT NULL,
    [ImagePath] NVARCHAR(512) NULL,
    [CreatedAtUtc] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedAtUtc] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_CustomerReview_CreatedAtUtc
    ON [dbo].[CustomerReview] ([CreatedAtUtc] DESC);

