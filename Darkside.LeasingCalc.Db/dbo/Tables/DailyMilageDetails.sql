CREATE TABLE [dbo].[DailyMilageDetails] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_DailyMilageDetails_DailyMileageId] DEFAULT (newid()) NOT NULL,
    [MilageDate]         DATETIME         NULL,
    [ExpectedMileDriven] INT              CONSTRAINT [DF_DailyMilageDetails_ExpectedMileDriven] DEFAULT ((0)) NULL,
    [MilesDriven]        INT              CONSTRAINT [DF_DailyMilageDetails_MilesDriven] DEFAULT ((0)) NULL,
    [ExpectedMilage]     INT              CONSTRAINT [DF_DailyMilageDetails_ActualMiles] DEFAULT ((0)) NULL,
    [ActualMileage]      INT              CONSTRAINT [DF_DailyMilageDetails_MileDriven] DEFAULT ((0)) NULL,
    [MilageDifference]   INT              CONSTRAINT [DF_DailyMilageDetails_MilageDifference] DEFAULT ((0)) NULL,
    [CarLeaseId]         UNIQUEIDENTIFIER NOT NULL,
    [IsDeleted]          BIT              CONSTRAINT [DF_DailyMilageDetails_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedBy]          VARCHAR (50)     NULL,
    [CreatedDate]        DATETIME         NOT NULL,
    [UpdatedBy]          VARCHAR (50)     NULL,
    [UpdatedDate]        DATETIME         NOT NULL,
    CONSTRAINT [PK_DailyMilageDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
);











