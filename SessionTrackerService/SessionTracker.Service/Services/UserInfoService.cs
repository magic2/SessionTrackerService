namespace SessionTracker.Service.Services
{
    using System;
    using System.Runtime.InteropServices;

    using SessionTracker.Service.Entities;

    public class UserInfoService : IUserInfoService
    {
        private enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo,
            WTSConfigInfo,
            WTSValidationInfo,
            WTSSessionAddressV4,
            WTSIsRemoteSession
        }

        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(System.IntPtr pServer, int iSessionID, WTS_INFO_CLASS oInfoClass, out System.IntPtr pBuffer, out uint iBytesReturned);

        [DllImport("wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pMemory);

        public UserInfo GetUserInformationbySessionId(int sessionId)
        {
            IntPtr buffer;
            uint length;

            var user = new UserInfo();

            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out length) && length > 1)
            {
                user.Name = Marshal.PtrToStringAnsi(buffer);

                WTSFreeMemory(buffer);
                if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSDomainName, out buffer, out length) && length > 1)
                {
                    user.Domain = Marshal.PtrToStringAnsi(buffer);
                    WTSFreeMemory(buffer);
                }
            }

            return string.IsNullOrEmpty(user.Name) ? null : user;
        }
    }
}
