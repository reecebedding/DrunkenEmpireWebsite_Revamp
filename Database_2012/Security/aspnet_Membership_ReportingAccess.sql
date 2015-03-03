CREATE ROLE [aspnet_Membership_ReportingAccess]
    AUTHORIZATION [evecruel_revamp];


GO
ALTER ROLE [aspnet_Membership_ReportingAccess] ADD MEMBER [aspnet_Membership_FullAccess];

