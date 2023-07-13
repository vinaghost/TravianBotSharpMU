using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307131527)]
    public class HeroEquip : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'IsAutoEquipBeforeAdventure';");
        }

        public override void Up()
        {
            Alter.Table("AccountsSettings")
                .AddColumn("IsAutoEquipBeforeAdventure").AsBoolean().WithDefaultValue(false);
        }
    }
}