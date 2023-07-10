using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307102106)]
    public class HeroRevive : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'IsAutoHeroRevive';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'HeroReviveVillageId';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'IsUseHeroResToRevive';");
        }

        public override void Up()
        {
            Alter.Table("AccountsSettings")
                .AddColumn("IsAutoHeroRevive").AsBoolean().WithDefaultValue(false)
                .AddColumn("HeroReviveVillageId").AsInt32().WithDefaultValue(-1)
                .AddColumn("IsUseHeroResToRevive").AsBoolean().WithDefaultValue(false);
        }
    }
}