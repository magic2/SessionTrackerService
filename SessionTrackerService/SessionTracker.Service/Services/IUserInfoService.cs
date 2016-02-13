namespace SessionTracker.Service.Services
{
    using SessionTracker.Service.Entities;

    public interface IUserInfoService
    {
        UserInfo GetUserInformationbySessionId(int sessionId);
    }
}