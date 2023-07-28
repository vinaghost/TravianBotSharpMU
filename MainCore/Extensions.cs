﻿using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MainCore
{
    public static class Extensions
    {
        public static bool IsResourceField(this BuildingEnums building)
        {
            int buildingInt = (int)building;
            // If id between 1 and 4, it's resource field
            return buildingInt < 5 && buildingInt > 0;
        }

        public static bool IsNotAdsUpgrade(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.CommandCenter => true,
                BuildingEnums.Palace => true,
                BuildingEnums.Residence => true,
                _ => false,
            };
        }

        public static bool IsWall(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.EarthWall => true,
                BuildingEnums.CityWall => true,
                BuildingEnums.Palisade => true,
                BuildingEnums.StoneWall => true,
                BuildingEnums.MakeshiftWall => true,
                _ => false,
            };
        }

        public static BuildingEnums GetWall(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Teutons => BuildingEnums.EarthWall,
                TribeEnums.Romans => BuildingEnums.CityWall,
                TribeEnums.Gauls => BuildingEnums.Palisade,
                TribeEnums.Egyptians => BuildingEnums.StoneWall,
                TribeEnums.Huns => BuildingEnums.MakeshiftWall,
                _ => BuildingEnums.Site,
            };
        }

        public static bool IsMultipleAllow(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Warehouse => true,
                BuildingEnums.Granary => true,
                BuildingEnums.GreatWarehouse => true,
                BuildingEnums.GreatGranary => true,
                BuildingEnums.Trapper => true,
                BuildingEnums.Cranny => true,
                _ => false,
            };
        }

        public static int GetMaxLevel(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Brewery => 20,
                BuildingEnums.Bakery => 5,
                BuildingEnums.Brickyard => 5,
                BuildingEnums.IronFoundry => 5,
                BuildingEnums.GrainMill => 5,
                BuildingEnums.Sawmill => 5,

                BuildingEnums.Cranny => 10,
                _ => 20,
            };
        }

        public static Color GetColor(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Site => Color.White,
                BuildingEnums.Woodcutter => Color.Lime,
                BuildingEnums.ClayPit => Color.Orange,
                BuildingEnums.IronMine => Color.LightGray,
                BuildingEnums.Cropland => Color.Yellow,
                _ => Color.LightCyan,
            };
        }

        public static BuildingEnums GetTribesWall(this TribeEnums tribe) => tribe switch
        {
            TribeEnums.Teutons => BuildingEnums.EarthWall,
            TribeEnums.Romans => BuildingEnums.CityWall,
            TribeEnums.Gauls => BuildingEnums.Palisade,
            TribeEnums.Egyptians => BuildingEnums.StoneWall,
            TribeEnums.Huns => BuildingEnums.MakeshiftWall,
            _ => BuildingEnums.Site,
        };

        public static bool HasMultipleTabs(this BuildingEnums building) => building switch
        {
            BuildingEnums.RallyPoint => true,
            BuildingEnums.CommandCenter => true,
            BuildingEnums.Residence => true,
            BuildingEnums.Palace => true,
            BuildingEnums.Marketplace => true,
            BuildingEnums.Treasury => true,
            _ => false,
        };

        public static int GetBuildingsCategory(this BuildingEnums building) => building switch
        {
            BuildingEnums.GrainMill => 2,
            BuildingEnums.Sawmill => 2,
            BuildingEnums.Brickyard => 2,
            BuildingEnums.IronFoundry => 2,
            BuildingEnums.Bakery => 2,
            BuildingEnums.Barracks => 1,
            BuildingEnums.HerosMansion => 1,
            BuildingEnums.Academy => 1,
            BuildingEnums.Smithy => 1,
            BuildingEnums.Stable => 1,
            BuildingEnums.GreatBarracks => 1,
            BuildingEnums.GreatStable => 1,
            BuildingEnums.Workshop => 1,
            BuildingEnums.TournamentSquare => 1,
            BuildingEnums.Trapper => 1,
            _ => 0,
        };

        public static (TribeEnums, List<PrerequisiteBuilding>) GetPrerequisiteBuildings(this BuildingEnums building)
        {
            TribeEnums tribe = TribeEnums.Any;
            var ret = new List<PrerequisiteBuilding>();
            switch (building)
            {
                case BuildingEnums.Sawmill:
                    ret.Add(new() { Building = BuildingEnums.Woodcutter, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Brickyard:
                    ret.Add(new() { Building = BuildingEnums.ClayPit, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.IronFoundry:
                    ret.Add(new() { Building = BuildingEnums.IronMine, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.GrainMill:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Bakery:
                    ret.Add(new() { Building = BuildingEnums.Cropland, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    ret.Add(new() { Building = BuildingEnums.GrainMill, Level = 5 });
                    break;

                case BuildingEnums.Warehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Granary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Blacksmith:
                    //DOESN'T EXIST ANYMORE
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, he can't build Blacksmith
                    break;

                case BuildingEnums.Smithy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 1 });
                    break;

                case BuildingEnums.TournamentSquare:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 15 });
                    break;

                case BuildingEnums.MainBuilding:
                    break;

                case BuildingEnums.RallyPoint:
                    break;

                case BuildingEnums.Marketplace:
                    ret.Add(new() { Building = BuildingEnums.Warehouse, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 1 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    break;

                case BuildingEnums.Embassy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 1 });
                    break;

                case BuildingEnums.Barracks:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.Stable:
                    ret.Add(new() { Building = BuildingEnums.Smithy, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 5 });
                    break;

                case BuildingEnums.Workshop:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Academy:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 3 });
                    break;

                case BuildingEnums.Cranny:
                    break;

                case BuildingEnums.TownHall:
                    ret.Add(new() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.Residence:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no palace!
                    break;

                case BuildingEnums.Palace:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //no residence!
                    ret.Add(new() { Building = BuildingEnums.Embassy, Level = 1 });
                    break;

                case BuildingEnums.Treasury:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 });
                    break;

                case BuildingEnums.TradeOffice:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Marketplace, Level = 20 });
                    break;

                case BuildingEnums.GreatBarracks:
                    ret.Add(new() { Building = BuildingEnums.Barracks, Level = 20 }); //not capital!
                    break;

                case BuildingEnums.GreatStable:
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 }); //not capital
                    break;

                case BuildingEnums.CityWall:
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.EarthWall:
                    tribe = TribeEnums.Teutons;
                    break;

                case BuildingEnums.Palisade:
                    tribe = TribeEnums.Gauls;
                    break;

                case BuildingEnums.StonemasonsLodge:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 }); //capital
                    break;

                case BuildingEnums.Brewery:
                    tribe = TribeEnums.Teutons;
                    ret.Add(new() { Building = BuildingEnums.Granary, Level = 20 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    break;

                case BuildingEnums.Trapper:
                    tribe = TribeEnums.Gauls;
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.HerosMansion:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 3 });
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 1 });
                    break;

                case BuildingEnums.GreatWarehouse:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.GreatGranary:
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 10 }); //art/ww vill
                    break;

                case BuildingEnums.WW: //ww vill
                    tribe = TribeEnums.Nature; //Just a dirty hack, since user can't be Nature, bot can't construct WW.
                    break;

                case BuildingEnums.HorseDrinkingTrough:
                    ret.Add(new() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    ret.Add(new() { Building = BuildingEnums.Stable, Level = 20 });
                    tribe = TribeEnums.Romans;
                    break;

                case BuildingEnums.StoneWall:
                    tribe = TribeEnums.Egyptians;
                    break;

                case BuildingEnums.MakeshiftWall:
                    tribe = TribeEnums.Huns;
                    break;

                case BuildingEnums.CommandCenter: //no res/palace
                    tribe = TribeEnums.Huns;
                    ret.Add(new() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    break;

                case BuildingEnums.Waterworks:
                    tribe = TribeEnums.Egyptians;
                    ret.Add(new() { Building = BuildingEnums.HerosMansion, Level = 10 });
                    break;

                default:
                    break;
            }
            return (tribe, ret);
        }

        public static bool IsUsableWhenHeroAway(this HeroItemEnums item)
        {
            return item switch
            {
                HeroItemEnums.Ointment or HeroItemEnums.Scroll or HeroItemEnums.Bucket or HeroItemEnums.Tablets or HeroItemEnums.Book or HeroItemEnums.Artwork or HeroItemEnums.SmallBandage or HeroItemEnums.BigBandage or HeroItemEnums.Cage or HeroItemEnums.Wood or HeroItemEnums.Clay or HeroItemEnums.Iron or HeroItemEnums.Crop => true,
                _ => false,
            };
        }

        public static bool IsNumeric(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
        }

        public static int ToNumeric(this string value)
        {
            var valueStr = new string(value.Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueStr)) return 0;
            return int.Parse(valueStr);
        }

        public static string EnumStrToString(this string value)
        {
            var len = value.Length;
            for (int i = 1; i < len; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    value = value.Insert(i, " ");
                    i++;
                    len++;
                }
            }
            return value;
        }

        public static TimeSpan ToDuration(this string value)
        {
            //00:00:02 (+332 ms), TTWars, milliseconds matter
            int ms = 0;
            if (value.Contains("(+"))
            {
                var parts = value.Split('(');
                ms = parts[1].ToNumeric();
                value = parts[0];
            }
            // h:m:s
            var arr = value.Split(':');
            var h = arr[0].ToNumeric();
            var m = arr[1].ToNumeric();
            var s = arr[2].ToNumeric();
            return new TimeSpan(0, h, m, s, ms);
        }

        public static TribeEnums GetTribe(this TroopEnums troop)
        {
            return (int)troop switch
            {
                >= (int)TroopEnums.Legionnaire and <= (int)TroopEnums.RomanSettler => TribeEnums.Romans,
                >= (int)TroopEnums.Clubswinger and <= (int)TroopEnums.TeutonSettler => TribeEnums.Teutons,
                >= (int)TroopEnums.Phalanx and <= (int)TroopEnums.GaulSettler => TribeEnums.Gauls,
                >= (int)TroopEnums.Rat and <= (int)TroopEnums.Elephant => TribeEnums.Nature,
                >= (int)TroopEnums.Pikeman and <= (int)TroopEnums.Settler => TribeEnums.Natars,
                >= (int)TroopEnums.SlaveMilitia and <= (int)TroopEnums.EgyptianSettler => TribeEnums.Egyptians,
                >= (int)TroopEnums.Mercenary and <= (int)TroopEnums.HunSettler => TribeEnums.Huns,
                _ => TribeEnums.Any,
            };
        }

        public static List<TroopEnums> GetTroops(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Romans => new()
                {
                    TroopEnums.Legionnaire,
                    TroopEnums.Praetorian,
                    TroopEnums.Imperian,
                    TroopEnums.EquitesLegati,
                    TroopEnums.EquitesImperatoris,
                    TroopEnums.EquitesCaesaris,
                    TroopEnums.RomanRam,
                    TroopEnums.RomanCatapult,
                    TroopEnums.RomanChief,
                    TroopEnums.RomanSettler,
                },
                TribeEnums.Teutons => new()
                {
                    TroopEnums.Clubswinger,
                    TroopEnums.Spearman,
                    TroopEnums.Axeman,
                    TroopEnums.Scout,
                    TroopEnums.Paladin,
                    TroopEnums.TeutonicKnight,
                    TroopEnums.TeutonRam,
                    TroopEnums.TeutonCatapult,
                    TroopEnums.TeutonChief,
                    TroopEnums.TeutonSettler,
                },
                TribeEnums.Gauls => new()
                {
                   TroopEnums.Phalanx,
                    TroopEnums.Swordsman,
                    TroopEnums.Pathfinder,
                    TroopEnums.TheutatesThunder,
                    TroopEnums.Druidrider,
                    TroopEnums.Haeduan,
                    TroopEnums.GaulRam,
                    TroopEnums.GaulCatapult,
                    TroopEnums.GaulChief,
                    TroopEnums.GaulSettler,
                },
                TribeEnums.Nature => new()
                {
                    TroopEnums.Rat,
                    TroopEnums.Spider,
                    TroopEnums.Snake,
                    TroopEnums.Bat,
                    TroopEnums.WildBoar,
                    TroopEnums.Wolf,
                    TroopEnums.Bear,
                    TroopEnums.Crocodile,
                    TroopEnums.Tiger,
                    TroopEnums.Elephant,
                },
                TribeEnums.Natars => new()
                {
                    TroopEnums.Pikeman,
                    TroopEnums.ThornedWarrior,
                    TroopEnums.Guardsman,
                    TroopEnums.BirdsOfPrey,
                    TroopEnums.Axerider,
                    TroopEnums.NatarianKnight,
                    TroopEnums.Warelephant,
                    TroopEnums.Ballista,
                    TroopEnums.NatarianEmperor,
                    TroopEnums.Settler,
                },
                TribeEnums.Egyptians => new()
                {
                    TroopEnums.SlaveMilitia,
                    TroopEnums.AshWarden,
                    TroopEnums.KhopeshWarrior,
                    TroopEnums.SopduExplorer,
                    TroopEnums.AnhurGuard,
                    TroopEnums.ReshephChariot,
                    TroopEnums.EgyptianRam,
                    TroopEnums.EgyptianCatapult,
                    TroopEnums.EgyptianChief,
                    TroopEnums.EgyptianSettler,
                },
                TribeEnums.Huns => new()
                {
                    TroopEnums.Mercenary,
                    TroopEnums.Bowman,
                    TroopEnums.Spotter,
                    TroopEnums.SteppeRider,
                    TroopEnums.Marksman,
                    TroopEnums.Marauder,
                    TroopEnums.HunRam,
                    TroopEnums.HunCatapult,
                    TroopEnums.HunChief,
                    TroopEnums.HunSettler,
                },
                _ => null,
            };
        }

        public static List<TroopEnums> GetInfantryTroops(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Romans => new()
                {
                    TroopEnums.Legionnaire,
                    TroopEnums.Praetorian,
                    TroopEnums.Imperian,
                },
                TribeEnums.Teutons => new()
                {
                    TroopEnums.Clubswinger,
                    TroopEnums.Spearman,
                    TroopEnums.Axeman,
                    TroopEnums.Scout,
                },
                TribeEnums.Gauls => new()
                {
                   TroopEnums.Phalanx,
                    TroopEnums.Swordsman,
                },
                TribeEnums.Nature => new(),
                TribeEnums.Natars => new(),
                TribeEnums.Egyptians => new()
                {
                    TroopEnums.SlaveMilitia,
                    TroopEnums.AshWarden,
                    TroopEnums.KhopeshWarrior,
                },
                TribeEnums.Huns => new()
                {
                    TroopEnums.Mercenary,
                    TroopEnums.Bowman,
                },
                _ => new(),
            };
        }

        public static List<TroopEnums> GetCavalryTroops(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Romans => new()
                {
                    TroopEnums.EquitesLegati,
                    TroopEnums.EquitesImperatoris,
                    TroopEnums.EquitesCaesaris,
                },
                TribeEnums.Teutons => new()
                {
                    TroopEnums.Paladin,
                    TroopEnums.TeutonicKnight,
                },
                TribeEnums.Gauls => new()
                {
                    TroopEnums.Pathfinder,
                    TroopEnums.TheutatesThunder,
                    TroopEnums.Druidrider,
                    TroopEnums.Haeduan,
                },
                TribeEnums.Nature => new(),
                TribeEnums.Natars => new(),
                TribeEnums.Egyptians => new()
                {
                    TroopEnums.SopduExplorer,
                    TroopEnums.AnhurGuard,
                    TroopEnums.ReshephChariot,
                },
                TribeEnums.Huns => new()
                {
                    TroopEnums.Spotter,
                    TroopEnums.SteppeRider,
                    TroopEnums.Marksman,
                    TroopEnums.Marauder,
                },
                _ => new(),
            };
        }

        public static List<TroopEnums> GetSiegeTroops(this TribeEnums tribe)
        {
            return tribe switch
            {
                TribeEnums.Romans => new()
                {
                    TroopEnums.RomanRam,
                    TroopEnums.RomanCatapult,
                },
                TribeEnums.Teutons => new()
                {
                    TroopEnums.TeutonRam,
                    TroopEnums.TeutonCatapult,
                },
                TribeEnums.Gauls => new()
                {
                    TroopEnums.GaulRam,
                    TroopEnums.GaulCatapult,
                },
                TribeEnums.Nature => new(),
                TribeEnums.Natars => new(),
                TribeEnums.Egyptians => new()
                {
                    TroopEnums.EgyptianRam,
                    TroopEnums.EgyptianCatapult
                },
                TribeEnums.Huns => new()
                {
                    TroopEnums.HunRam,
                    TroopEnums.HunCatapult,
                },
                _ => new(),
            };
        }

        public static List<PrerequisiteBuilding> GetPrerequisiteBuilding(this TroopEnums troop)
        {
            var ret = new List<PrerequisiteBuilding>();
            switch (troop)
            {
                //romans
                case TroopEnums.Praetorian:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Imperian:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.EquitesLegati:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.EquitesImperatoris:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.EquitesCaesaris:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.RomanRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.RomanCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    return ret;

                case TroopEnums.RomanChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    return ret;
                //Teutons
                case TroopEnums.Spearman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    return ret;

                case TroopEnums.Axeman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Scout:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.MainBuilding, Level = 5 });
                    return ret;

                case TroopEnums.Paladin:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    return ret;

                case TroopEnums.TeutonicKnight:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.TeutonRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.TeutonCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.TeutonChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 5 });
                    return ret;
                //Gauls
                case TroopEnums.Swordsman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Pathfinder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.TheutatesThunder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    return ret;

                case TroopEnums.Druidrider:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.Haeduan:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.GaulRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.GaulCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.GaulChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;
                //Egyptians
                case TroopEnums.AshWarden:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Barracks, Level = 1 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.KhopeshWarrior:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.SopduExplorer:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.AnhurGuard:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.ReshephChariot:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.EgyptianRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 5 });
                    return ret;

                case TroopEnums.EgyptianCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.EgyptianChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;
                //Huns
                case TroopEnums.Bowman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 3 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Smithy, Level = 1 });
                    return ret;

                case TroopEnums.Spotter:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 1 });
                    return ret;

                case TroopEnums.SteppeRider:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 3 });
                    return ret;

                case TroopEnums.Marksman:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 5 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 5 });
                    return ret;

                case TroopEnums.Marauder:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Stable, Level = 10 });
                    return ret;

                case TroopEnums.HunRam:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 10 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 1 });
                    return ret;

                case TroopEnums.HunCatapult:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 15 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Workshop, Level = 10 });
                    return ret;

                case TroopEnums.HunChief:
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.Academy, Level = 20 });
                    ret.Add(new PrerequisiteBuilding() { Building = BuildingEnums.RallyPoint, Level = 10 });
                    return ret;

                default: return ret;
            }
        }

        public static DateTime GetTimeWhenEnough(this VillageProduction production, long[] resRequired)
        {
            var productionArr = new long[] { production.Wood, production.Clay, production.Iron, production.Crop };

            var now = DateTime.Now;
            var ret = now.AddMinutes(2);

            for (int i = 0; i < 4; i++)
            {
                DateTime toWaitForThisRes = DateTime.MinValue;
                if (resRequired[i] > 0)
                {
                    // In case of negative crop, we will never have enough crop
                    if (productionArr[i] <= 0) return DateTime.MaxValue;

                    float hoursToWait = resRequired[i] / (float)productionArr[i];
                    float secToWait = hoursToWait * 3600;
                    toWaitForThisRes = now.AddSeconds(secToWait);
                }

                if (ret < toWaitForThisRes) ret = toWaitForThisRes;
            }
            return ret;
        }

        // we use residence for chief and settler
        public static BuildingEnums GetTrainBuilding(this TroopEnums troop)
        {
            return troop switch
            {
                TroopEnums.Legionnaire or TroopEnums.Praetorian or TroopEnums.Imperian or TroopEnums.Clubswinger or TroopEnums.Spearman or TroopEnums.Axeman or TroopEnums.Scout or TroopEnums.Phalanx or TroopEnums.Swordsman or TroopEnums.SlaveMilitia or TroopEnums.AshWarden or TroopEnums.KhopeshWarrior or TroopEnums.Mercenary or TroopEnums.Bowman => BuildingEnums.Barracks,
                TroopEnums.EquitesLegati or TroopEnums.EquitesImperatoris or TroopEnums.EquitesCaesaris or TroopEnums.Paladin or TroopEnums.TeutonicKnight or TroopEnums.Pathfinder or TroopEnums.TheutatesThunder or TroopEnums.Druidrider or TroopEnums.Haeduan or TroopEnums.SopduExplorer or TroopEnums.AnhurGuard or TroopEnums.ReshephChariot or TroopEnums.Spotter or TroopEnums.SteppeRider or TroopEnums.Marksman or TroopEnums.Marauder => BuildingEnums.Stable,
                TroopEnums.RomanRam or TroopEnums.RomanCatapult or TroopEnums.TeutonRam or TroopEnums.TeutonCatapult or TroopEnums.GaulRam or TroopEnums.GaulCatapult or TroopEnums.EgyptianRam or TroopEnums.EgyptianCatapult or TroopEnums.HunRam or TroopEnums.HunCatapult => BuildingEnums.Workshop,
                TroopEnums.RomanChief or TroopEnums.RomanSettler or TroopEnums.TeutonChief or TroopEnums.TeutonSettler or TroopEnums.GaulChief or TroopEnums.GaulSettler or TroopEnums.EgyptianChief or TroopEnums.EgyptianSettler or TroopEnums.HunChief or TroopEnums.HunSettler => BuildingEnums.Residence,
                _ => BuildingEnums.Site,
            };
        }

        public static BuildingEnums GetGreatVersion(this BuildingEnums building)
        {
            return building switch
            {
                BuildingEnums.Barracks => BuildingEnums.GreatBarracks,
                BuildingEnums.Stable => BuildingEnums.GreatStable,
                BuildingEnums.Workshop => BuildingEnums.Workshop,
                _ => BuildingEnums.Site,
            };
        }

        public static bool IsAllowLogin(this AccountStatus status)
        {
            if (status == AccountStatus.Offline) return true;
            return false;
        }

        public static bool IsAllowLogout(this AccountStatus status)
        {
            if (status == AccountStatus.Online) return true;
            if (status == AccountStatus.Paused) return true;
            return false;
        }

        public static bool IsAllowPause(this AccountStatus status)
        {
            if (status == AccountStatus.Online) return true;
            if (status == AccountStatus.Paused) return true;
            return false;
        }

        public static bool IsAllowRestart(this AccountStatus status)
        {
            if (status == AccountStatus.Paused) return true;
            return false;
        }

        public static string GetPauseText(this AccountStatus status)
        {
            if (status == AccountStatus.Online) return "Pause";
            if (status == AccountStatus.Paused) return "Resume";
            return "[~!~]";
        }

        public static Color GetColor(this AccountStatus status)
        {
            return status switch
            {
                AccountStatus.Offline => Color.Black,
                AccountStatus.Starting => Color.Orange,
                AccountStatus.Online => Color.Green,
                AccountStatus.Pausing => Color.Orange,
                AccountStatus.Paused => Color.Red,
                AccountStatus.Stopping => Color.Orange,
                _ => Color.Black,
            };
        }

        public static Resources GetTrainCost(this TroopEnums troop)
        {
            return troop switch
            {
                TroopEnums.None => null,
                TroopEnums.Legionnaire => new Resources(120, 100, 150, 30),
                TroopEnums.Praetorian => new Resources(100, 130, 160, 70),
                TroopEnums.Imperian => new Resources(150, 160, 210, 80),
                TroopEnums.EquitesLegati => new Resources(140, 160, 20, 40),
                TroopEnums.EquitesImperatoris => new Resources(550, 440, 320, 100),
                TroopEnums.EquitesCaesaris => new Resources(550, 640, 800, 180),
                TroopEnums.RomanRam => new Resources(900, 360, 500, 70),
                TroopEnums.RomanCatapult => new Resources(950, 1350, 600, 90),
                TroopEnums.RomanChief => new Resources(30750, 27200, 45000, 37500),
                TroopEnums.RomanSettler => new Resources(4600, 4200, 5800, 4400),
                TroopEnums.Clubswinger => new Resources(95, 75, 40, 40),
                TroopEnums.Spearman => new Resources(145, 70, 85, 40),
                TroopEnums.Axeman => new Resources(130, 120, 170, 70),
                TroopEnums.Scout => new Resources(160, 100, 50, 50),
                TroopEnums.Paladin => new Resources(370, 270, 290, 75),
                TroopEnums.TeutonicKnight => new Resources(450, 515, 480, 80),
                TroopEnums.TeutonRam => new Resources(1000, 300, 350, 70),
                TroopEnums.TeutonCatapult => new Resources(900, 1200, 600, 60),
                TroopEnums.TeutonChief => new Resources(35500, 26600, 25000, 27200),
                TroopEnums.TeutonSettler => new Resources(5800, 4400, 4600, 5200),
                TroopEnums.Phalanx => new Resources(100, 130, 55, 30),
                TroopEnums.Swordsman => new Resources(140, 150, 185, 60),
                TroopEnums.Pathfinder => new Resources(170, 150, 20, 40),
                TroopEnums.TheutatesThunder => new Resources(350, 450, 230, 60),
                TroopEnums.Druidrider => new Resources(360, 330, 280, 120),
                TroopEnums.Haeduan => new Resources(500, 620, 675, 170),
                TroopEnums.GaulRam => new Resources(950, 555, 330, 75),
                TroopEnums.GaulCatapult => new Resources(960, 1450, 630, 90),
                TroopEnums.GaulChief => new Resources(30750, 45400, 31000, 37500),
                TroopEnums.GaulSettler => new Resources(4400, 5600, 4200, 3900),
                TroopEnums.Rat or TroopEnums.Spider or TroopEnums.Snake or TroopEnums.Bat or TroopEnums.WildBoar or TroopEnums.Wolf or TroopEnums.Bear or TroopEnums.Crocodile or TroopEnums.Tiger or TroopEnums.Elephant or TroopEnums.Pikeman or TroopEnums.ThornedWarrior or TroopEnums.Guardsman or TroopEnums.BirdsOfPrey or TroopEnums.Axerider or TroopEnums.NatarianKnight or TroopEnums.Warelephant or TroopEnums.Ballista or TroopEnums.NatarianEmperor or TroopEnums.Settler => null,
                TroopEnums.SlaveMilitia => new Resources(45, 60, 30, 15),
                TroopEnums.AshWarden => new Resources(115, 100, 145, 60),
                TroopEnums.KhopeshWarrior => new Resources(170, 180, 220, 80),
                TroopEnums.SopduExplorer => new Resources(170, 150, 20, 40),
                TroopEnums.AnhurGuard => new Resources(350, 450, 230, 60),
                TroopEnums.ReshephChariot => new Resources(450, 560, 610, 180),
                TroopEnums.EgyptianRam => new Resources(995, 575, 340, 80),
                TroopEnums.EgyptianCatapult => new Resources(980, 1510, 660, 100),
                TroopEnums.EgyptianChief => new Resources(34000, 50000, 34000, 42000),
                TroopEnums.EgyptianSettler => new Resources(5040, 6510, 4830, 4620),
                TroopEnums.Mercenary => new Resources(130, 80, 40, 40),
                TroopEnums.Bowman => new Resources(140, 110, 60, 60),
                TroopEnums.Spotter => new Resources(170, 150, 20, 40),
                TroopEnums.SteppeRider => new Resources(290, 370, 190, 45),
                TroopEnums.Marksman => new Resources(320, 350, 330, 50),
                TroopEnums.Marauder => new Resources(450, 560, 610, 140),
                TroopEnums.HunRam => new Resources(1060, 330, 360, 70),
                TroopEnums.HunCatapult => new Resources(950, 1280, 620, 60),
                TroopEnums.HunChief => new Resources(37200, 27600, 25200, 27600),
                TroopEnums.HunSettler => new Resources(6100, 4600, 4800, 5400),
                _ => null,
            };
        }
    }
}