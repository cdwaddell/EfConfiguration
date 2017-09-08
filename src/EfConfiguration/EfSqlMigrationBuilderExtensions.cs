using Microsoft.EntityFrameworkCore.Migrations;

namespace Titanosoft.EfConfiguration
{
    public static class EfSqlMigrationBuilderExtensions
    {
        public static void ExecSql(this MigrationBuilder migrationBuilder, string script)
        {
            migrationBuilder.Sql(GetSql(script));
        }

        private static string GetSql(string script)
        {
            return "EXEC('" + script.Replace("'", "''") + "')";
        }
    }
}