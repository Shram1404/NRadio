namespace NRadio.Models
{
    public class LoginEnum
    {
        public enum LoginResultType
        {
            Success,
            Unauthorized,
            CancelledByUser,
            NoNetworkAvailable,
            UnknownError
        }
    }
}
