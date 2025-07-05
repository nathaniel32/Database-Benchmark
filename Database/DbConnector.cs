using NHibernate;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using csdb.Database.Models;

namespace csdb.Database {
    public static class DbConnector {
        //private static readonly ISessionFactory sessionFactory;
        private static ISessionFactory? sessionFactory;
        private static string? connectionString;

        //static DbConnector()
        public static void Initialize() {
            var config = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("config.json").Build();
            connectionString = config.GetConnectionString("DbConnection");
            sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString)
                    .ShowSql()
                )
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Mitarbeiter>())
                .BuildSessionFactory();
        }

        public static string GetConnectionString() {
            if (connectionString == null)
                throw new InvalidOperationException("Connection string not initialized!");
            return connectionString;
        }

        public static ISessionFactory? GetSessionFactory() {
            return sessionFactory;
        }

        public static ISession OpenSession() {
            if (sessionFactory == null)
                throw new InvalidOperationException("SessionFactory Error!");
            return sessionFactory.OpenSession();
        }

        public static void ExecuteSqlScript(string sqlScript)
        {
            if (connectionString == null)
                throw new InvalidOperationException("Connection string not initialized!");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Split the script into individual commands using regex to handle GO statements
                // GO must be on its own line, optionally with whitespace before/after
                string[] commands = System.Text.RegularExpressions.Regex.Split(sqlScript, @"^\s*GO\s*$", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (string commandText in commands)
                {
                    string trimmedCommandText = commandText.Trim();
                    // Remove leading comments and whitespace
                    trimmedCommandText = System.Text.RegularExpressions.Regex.Replace(trimmedCommandText, @"^\s*(?:--.*|/\*.*?\*/)\s*", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.Multiline);
                    if (string.IsNullOrWhiteSpace(trimmedCommandText)) continue;

                    using (SqlCommand command = new SqlCommand(trimmedCommandText, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}