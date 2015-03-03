CREATE TABLE [dbo].[dgmTypeAttributes] (
    [typeID]      INT        NOT NULL,
    [attributeID] SMALLINT   NOT NULL,
    [valueInt]    INT        NULL,
    [valueFloat]  FLOAT (53) NULL,
    CONSTRAINT [dgmTypeAttributes_PK] PRIMARY KEY CLUSTERED ([typeID] ASC, [attributeID] ASC)
);

