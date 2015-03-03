CREATE ROLE [aspnet_Membership_BasicAccess]
    AUTHORIZATION [evecruel_revamp];


GO
ALTER ROLE [aspnet_Membership_BasicAccess] ADD MEMBER [aspnet_Membership_FullAccess];

