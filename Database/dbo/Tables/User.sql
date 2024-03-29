﻿CREATE TABLE [dbo].[User]
(
	[Id] VARCHAR(255) NOT NULL PRIMARY KEY UNIQUE,
	[Email] VARCHAR(255) NOT NULL UNIQUE,
	[Username] VARCHAR(16) NOT NULL,
	[Password] VARCHAR(255) NOT NULL,
	[Discriminator] INT NOT NULL,
	[Icon] VARCHAR(16) DEFAULT NULL,
	[Permissions] INT NOT NULL DEFAULT 0,
	[Verified] BIT NOT NULL DEFAULT 0
)
