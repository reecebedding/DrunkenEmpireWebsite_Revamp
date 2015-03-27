CREATE TABLE [dbo].[dgmAttributeTypes] (
    [attributeID]   SMALLINT       NOT NULL,
    [attributeName] VARCHAR (100)  NULL,
    [description]   VARCHAR (1000) NULL,
    [iconID]        INT            NULL,
    [defaultValue]  FLOAT (53)     NULL,
    [published]     BIT            NULL,
    [displayName]   VARCHAR (100)  NULL,
    [unitID]        TINYINT        NULL,
    [stackable]     BIT            NULL,
    [highIsGood]    BIT            NULL,
    [categoryID]    TINYINT        NULL,
    CONSTRAINT [dgmAttributeTypes_PK] PRIMARY KEY CLUSTERED ([attributeID] ASC)
);

