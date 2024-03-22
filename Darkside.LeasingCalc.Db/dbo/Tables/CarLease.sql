CREATE TABLE [dbo].[CarLease] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_CarLease_CarId] DEFAULT (newid()) NOT NULL,
    [CarNumber]       VARCHAR (10)     CONSTRAINT [DF_CarLease_CarNumber] DEFAULT ((0)) NULL,
    [CustomerName]    VARCHAR (40)     NULL,
    [YearlyMiles]     DECIMAL (18)     CONSTRAINT [DF_CarLease_YearlyMiles] DEFAULT ((0)) NULL,
    [StartDate]       DATETIME         NULL,
    [TotalYears]      INT              CONSTRAINT [DF_CarLease_TotalYears] DEFAULT ((0)) NULL,
    [StartingMileage] INT              CONSTRAINT [DF_CarLease_StartingMileage] DEFAULT ((0)) NULL,
    [CurrentMileage]  INT              CONSTRAINT [DF_CarLease_CurrentMileage] DEFAULT ((0)) NULL,
    [IsDeleted]       BIT              CONSTRAINT [DF_CarLease_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedBy]       VARCHAR (50)     NULL,
    [CreatedDate]     DATETIME         NOT NULL,
    [UpdatedBy]       VARCHAR (50)     NULL,
    [UpdatedDate]     DATETIME         NOT NULL,
    CONSTRAINT [PK_CarLease] PRIMARY KEY CLUSTERED ([Id] ASC)
);











