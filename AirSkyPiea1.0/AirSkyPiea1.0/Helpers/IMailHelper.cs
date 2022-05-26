using AirSkyPiea1._0.Common;

namespace AirSkyPiea1._0.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}
