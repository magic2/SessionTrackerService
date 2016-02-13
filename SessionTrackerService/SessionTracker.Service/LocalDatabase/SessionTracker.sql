CREATE TABLE TrackerInstance 
(
	Id uniqueidentifier NOT NULL,
	StartAt datetime NOT NULL,
	LastUpdateAt datetime NOT NULL,
	MachineName nvarchar(50) NOT NULL,
	CONSTRAINT PK_TrackerInstance PRIMARY KEY (Id)
);
CREATE TABLE SessionLog (
	Id INTEGER NOT NULL,
	SessionId int NOT NULL,
	UserName nvarchar(50) NOT NULL,
	UserDomain nvarchar(50) NOT NULL,
	SessionChangeReason nvarchar(50) NOT NULL,
	CreatedAt datetime NOT NULL,
	TrackerInstanceId uniqueidentifier NOT NULL,
	CONSTRAINT PK_SessionLog PRIMARY KEY (Id),
	FOREIGN KEY (TrackerInstanceId) REFERENCES TrackerInstance (Id) ON DELETE NO ACTION ON UPDATE NO ACTION
);
CREATE INDEX SessionLog_CreatedAt_SessionLog ON SessionLog (CreatedAt ASC);
