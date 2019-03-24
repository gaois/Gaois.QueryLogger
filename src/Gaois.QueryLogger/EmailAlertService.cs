using Ansa.Extensions;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Alerts designated users via e-mail in case of possible issues with the query logger service
    /// </summary>
    public class EmailAlertService : EmailAlertServiceBase, IAlertService
    {
        private static readonly QueryLoggerSettings _settings = ConfigurationSettings.Settings;
        private static readonly EmailSettings _emailSettings = _settings.Email;

        /// <summary>
        /// Sends an e-mail alert
        /// </summary>
        /// <param name="alert">The alert to be sent</param>
        public void Alert(Alert alert)
        {
            var address = _emailSettings.ToAddress;
            var subject = "QueryLogger Alert";

            if (_settings.ApplicationName.HasValue())
                subject += $": {_settings.ApplicationName}";

            var body = GetAlertBody(subject, alert);

            SendEmail(_emailSettings, address, subject, body);
        }
    }
}