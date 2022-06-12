using Gyldendal.Jini.Services.Common.ConfigurationManager;
using LoggingManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public class LoggingManagerConfig : BaseConfigurationManager, ILoggingManagerConfig
    {
        public string RabbitMqLogUser => GetConfigValue("RabbitMqLogUser");

        public string RabbitMqLogUserPassword => GetConfigValue("RabbitMqLogUserPassword");

        public string RabbitMqLogHost => GetConfigValue("RabbitMqLogHost");

        public string RabbitMqLogVirtualHost => GetConfigValue("RabbitMqLogVirtualHost");

        public string RabbitMqLogExchange => GetConfigValue("RabbitMqLogExchange");

        public int LogEventHrs => GetConfigValueAsInt("LogEventHrs", 3);

        public bool LogInfo => GetConfigValueAsBool("LogInfo");

        public string LogName => GetConfigValue("LogName");

        public bool MailExToInfra => GetConfigValueAsBool("MailExToInfra");

        public string SourceName => GetConfigValue("SourceName");

        public string LogDirectoryPath => GetConfigValue("RabbitMqErrorFallbackLogDirectoryPath");

        public bool EnableEventLog => true;

        public bool EnableDebugLog => GetConfigValueAsBool("EnableDebugLog");
    }
}
