CREATE ROLE [Role_Jini]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [Role_Jini] ADD MEMBER [GYLDENDAL\sa-jinideploy-d];


GO
ALTER ROLE [Role_Jini] ADD MEMBER [GYLDENDAL\sa-jini-d];

