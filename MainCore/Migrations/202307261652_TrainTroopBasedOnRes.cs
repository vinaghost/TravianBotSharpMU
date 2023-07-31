using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307261652)]
    public class TrainTroopBasedOnRes : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsTrainTroopBasedOnRes';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentWarehouseTrainTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentResForBarrack';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentResForStable';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentResForWorkshop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentResForGreatBarrack';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'PercentResForGreatStable';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsTrainTroopBasedOnRes").AsBoolean().WithDefaultValue(false)
                .AddColumn("PercentWarehouseTrainTroop").AsInt32().WithDefaultValue(50)
                .AddColumn("PercentResForBarrack").AsInt32().WithDefaultValue(10)
                .AddColumn("PercentResForStable").AsInt32().WithDefaultValue(10)
                .AddColumn("PercentResForWorkshop").AsInt32().WithDefaultValue(10)
                .AddColumn("PercentResForGreatBarrack").AsInt32().WithDefaultValue(10)
                .AddColumn("PercentResForGreatStable").AsInt32().WithDefaultValue(10);
        }
    }
}