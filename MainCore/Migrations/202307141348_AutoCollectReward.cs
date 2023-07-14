using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307141348)]
    public class AutoCollectReward : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsAutoCollectReward';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsAutoCollectReward").AsBoolean().WithDefaultValue(false);
        }
    }
}