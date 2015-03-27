CREATE ROLE [aspnet_Personalization_BasicAccess]
    AUTHORIZATION [evecruel_revamp];


GO
ALTER ROLE [aspnet_Personalization_BasicAccess] ADD MEMBER [aspnet_Personalization_FullAccess];

