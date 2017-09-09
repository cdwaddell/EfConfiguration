using Microsoft.EntityFrameworkCore.Migrations;

namespace Titanosoft.EfConfiguration
{
    public static class EfSqlMigrationBuilderExtensions
    {
        public static void ExecSql(this MigrationBuilder migrationBuilder, string script)
        {
            migrationBuilder.Sql(GetSql(script));
        }

        /// <summary>
        /// Convert a SQL script into something that is not prone to break scripted endoints if the schema every changes
        /// </summary>
        /// <param name="script">The SQL script to run</param>
        /// <returns>The SQL script escaped using an EXEC statement. This new script is not prone to breaking when future schema changes</returns>
        private static string GetSql(string script)
        {
            return "EXEC('" + script.Replace("'", "''") + "')";
        }
    }
}