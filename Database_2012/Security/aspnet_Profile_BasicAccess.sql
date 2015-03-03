CREATE ROLE [aspnet_Profile_BasicAccess]
    AUTHORIZATION [evecruel_revamp];


GO
ALTER ROLE [aspnet_Profile_BasicAccess] ADD MEMBER [aspnet_Profile_FullAccess];

