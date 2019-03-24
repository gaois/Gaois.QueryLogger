using Ansa.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Gaois.QueryLogger
{
    /// <summary>
    /// Alerts designated users via e-mail in case of possible issues with the query logger service
    /// </summary>
    public class EmailAlertService : EmailAlertServiceBase, IAlertService
    {
        private readonly QueryLoggerSettings _settings;
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Alerts designated users via e-mail in case of possible issues with the query logger service
        /// </summary>
        public EmailAlertService(IOptionsMonitor<QueryLoggerSettings> settings)
        {
            _settings = settings.CurrentValue;
            _emailSettings = settings.CurrentValue.Email;
        }

        /// <summary>
        /// Sends an e-mail alert
        /// </summary>
        /// <param name="alert">The alert to be sent</param>
        public void Alert(Alert alert)
        {
            _ = alert ?? throw new ArgumentNullException(nameof(alert));

            var address = _emailSettings.ToAddress;
            var subject = "QueryLogger Alert";

            if (_settings.ApplicationName.HasValue())
                subject += $": {_settings.ApplicationName}";

            var body = GetAlertBody(subject, alert);

            SendEmail(_emailSettings, address, subject, body);
        }
    }
}