using Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Provides
{
    public class DBLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public Func<string, LogLevel, bool> Filter { get; set; }
        public string ConnectionString { get; set; }
    }

    public class DBLoggerProvider : ILoggerProvider
    {
        private readonly DBLoggerConfiguration _config;

        public DBLoggerProvider(DBLoggerConfiguration config)
        {
            _config = config;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DBLogger(categoryName, _config.Filter, _config.ConnectionString);
        }
        public void Dispose()
        {
        }
    }

    public class DBLogger : ILogger
    {
        private string _categoryName;
        private Func<string, LogLevel, bool> _filter;
        private int MessageMaxLength = 4000;
        private string _connectionString;

        public DBLogger(string categoryName, Func<string, LogLevel, bool> filter, string connectionString)
        {
            _categoryName = categoryName;
            _filter = filter;
            _connectionString = connectionString;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            if (exception != null)
            {
                message += "\n" + exception.ToString();
            }
            message = message.Length > MessageMaxLength ? message.Substring(0, MessageMaxLength) : message;

            var eventLog = new EventLog
            {
                Message = message,
                EventId = eventId.Id,
                LogLevel = logLevel.ToString(),
                CreatedTime = DateTime.UtcNow
            };
            var tableName = "EventLogs_" + DateTime.UtcNow.ToString("yyyy_MM");

            var cmdText = $@"
CREATE TABLE IF NOT EXISTS {tableName}
(
	EventLogId INTEGER PRIMARY KEY AUTOINCREMENT,
	EventId INTEGER NOT NULL,
	LogLevel INTEGER NOT NULL,
	Message TEXT NOT NULL,
	CreatedTime TEXT NOT null
);

INSERT INTO {tableName}(EventId, LogLevel, Message, CreatedTime)
    VALUES  (@EventId, @LogLevel, @Message, @CreatedTime);
";
            try
            {
                using (var conn = new SqliteConnection(_connectionString))
                using (var cmd = new SqliteCommand(cmdText, conn))
                {
                    cmd.CommandTimeout = 60;

                    cmd.Parameters.Add("EventId", SqliteType.Integer).Value = eventId.Id;
                    cmd.Parameters.Add("LogLevel", SqliteType.Integer).Value = logLevel;
                    cmd.Parameters.Add("Message", SqliteType.Text).Value = message;
                    cmd.Parameters.Add("CreatedTime", SqliteType.Text).Value = DateTime.UtcNow;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel));
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
