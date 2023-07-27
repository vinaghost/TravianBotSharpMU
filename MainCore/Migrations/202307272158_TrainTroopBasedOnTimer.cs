using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202307272158)]
    public class TrainTroopBasedOnTimer : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsTrainTroopBasedOnTimer';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsBarrack';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsStable';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsWorkshop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'GreatBarrackTroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'GreatBarrackTroopTimeMax';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'GreatStableTroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'GreatStableTroopTimeMax';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsTrainTroopBasedOnTimer").AsBoolean().WithDefaultValue(false)
                .AddColumn("IsBarrack").AsBoolean().WithDefaultValue(false)
                .AddColumn("IsStable").AsBoolean().WithDefaultValue(false)
                .AddColumn("IsWorkshop").AsBoolean().WithDefaultValue(false)
                .AddColumn("GreatBarrackTroopTimeMin").AsInt32().WithDefaultValue(50)
                .AddColumn("GreatBarrackTroopTimeMax").AsInt32().WithDefaultValue(70)
                .AddColumn("GreatStableTroopTimeMin").AsInt32().WithDefaultValue(50)
                .AddColumn("GreatStableTroopTimeMax").AsInt32().WithDefaultValue(70);
        }
    }
}