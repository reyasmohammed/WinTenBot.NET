using System;
using FluentMigrator.Builders;
using FluentMigrator.Infrastructure;

namespace WinTenBot.Helpers
{
    public static class FluentMigrationHelper
    {
        public static TNext AsMySqlText<TNext>(this IColumnTypeSyntax<TNext> createTableColumnAsTypeSyntax)
            where TNext : IFluentSyntax
        {
            return createTableColumnAsTypeSyntax.AsCustom("TEXT");
        }

        public static TNext AsMySqlMediumText<TNext>(this IColumnTypeSyntax<TNext> createTableColumnAsTypeSyntax)
            where TNext : IFluentSyntax
        {
            return createTableColumnAsTypeSyntax.AsCustom("MEDIUMTEXT");
        }

        public static TNext AsMySqlVarchar<TNext>(this IColumnTypeSyntax<TNext> columnTypeSyntax, Int16 max)
            where TNext : IFluentSyntax
        {
            string varcharType = $"VARCHAR({max})";
            return columnTypeSyntax.AsCustom(varcharType);
        }

        public static TNext AsMySqlTimestamp<TNext>(this IColumnTypeSyntax<TNext> createTableColumnAsTypeSyntax)
            where TNext : IFluentSyntax
        {
            return createTableColumnAsTypeSyntax.AsCustom("TIMESTAMP");
        }
    }
}