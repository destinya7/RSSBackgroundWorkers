﻿namespace RSSCollectorService.Services
{
    public interface ILoggerService
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);
    }
}
