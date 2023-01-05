using System.Collections.Generic;

namespace XD.SDK.Account.Internal
{
    public interface IXDGUser
    {
        // The _userStandalone's _userStandalone ID.
        string userId { get; }

        // The _userStandalone's _userStandalone name.
        string name { get; }

        // The _userStandalone's current loginType.
        long loginType //App传来的是字符串，如 TapTap。 通过 GetLoginType() 方法获取枚举
        {
            get;
        }

        string avatar { get; }

        string nickName { get; }

        List<string> boundAccounts { get; }

        // The _userStandalone's token.
        IXDGAccessToken token { get; }

        LoginType getLoginType();
    }
}