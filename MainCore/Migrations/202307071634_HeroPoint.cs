using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307071634)]
    public class HeroPoint : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'IsAutoHeroPoint';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'HeroFightingPoint';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'HeroOffPoint';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'HeroDefPoint';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'HeroResourcePoint';");
        }

        public override void Up()
        {
            Alter.Table("AccountsSettings")
                .AddColumn("IsAutoHeroPoint").AsBoolean().WithDefaultValue(false)
                .AddColumn("HeroFightingPoint").AsInt32().WithDefaultValue(1)
                .AddColumn("HeroOffPoint").AsInt32().WithDefaultValue(0)
                .AddColumn("HeroDefPoint").AsInt32().WithDefaultValue(0)
                .AddColumn("HeroResourcePoint").AsInt32().WithDefaultValue(3);
        }
    }
}