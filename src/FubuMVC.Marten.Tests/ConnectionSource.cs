using System;
using System.Diagnostics;
using FubuCore;

namespace FubuMVC.Marten.Tests
{
    public class ConnectionSource
    {
        public static readonly string EnvironmentKey = "marten-testing-database";

        public static void SetConnectionString(string connectionString)
        {
            Environment.SetEnvironmentVariable(EnvironmentKey, connectionString);
        }

        public static string GetConnectionString()
        {
            var connectionString = Environment.GetEnvironmentVariable(EnvironmentKey);

            if (connectionString.IsEmpty())
            {
                throw new Exception($"You need to set the Environment variable {EnvironmentKey} to the connection string for your Postgresql schema");
            }

            return connectionString;
        }

    }
}