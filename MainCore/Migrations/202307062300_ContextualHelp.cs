using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307062300)]
    public class ContextualHelp : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsInfo' DROP COLUMN 'IsContextualHelpDisabled';");
        }

        public override void Up()
        {
            Alter.Table("AccountsInfo")
                .AddColumn("IsContextualHelpDisabled").AsBoolean().WithDefaultValue(false);
        }
    }
}