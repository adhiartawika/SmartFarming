using backend.Persistences;
using backend.Model.AppEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Model.IdEntity;
using backend.Identity;
using backend.Model.AppEntity;
using  Microsoft.AspNetCore.Identity;

namespace backend.Seeders
{
    public class AppDbContextSeeder
    {
        // public static async Task SeedNotificationType(AppDbContext ctx)
        // {
        //     // if (ctx.NotificationTypes.FirstOrDefault(x => x.Id == 1)==null)
        //     // {
        //     //     ctx.NotificationTypes.Add(new NotificationType
        //     //     {
        //     //         Id=1,
        //     //         Name="Fertilizer Notification"
        //     //     });
        //     //     await ctx.SaveChangesAsync();
        //     // }
        // }
    public static async Task SeedRoleAsync(AppDbContext ctx)
        {
            if (!(ctx.UserRoles.ToList().Count()>0))
            {

                ctx.UserRoles.AddRange(
                    new UserRole
                    {
                        Id = 1,
                        RoleName = "Super Admin",
                        Name = "Super Admin",
                        NormalizedName = "Super Admin"
                    },
                    new UserRole
                    {
                        Id = 2,
                        RoleName = "Admin",
                        Name = "Admin",
                        NormalizedName = "Admin"
                    },
                    new UserRole
                    {
                        Id = 3,
                        RoleName = "User",
                        Name = "User",
                        NormalizedName = "User"
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedParentTypeAsync(AppDbContext ctx)
        {
            if (!(ctx.ParentTypes.ToList().Count()>0))
            {

                ctx.ParentTypes.AddRange(
                    new ParentType
                    {
                        Id = 1,
                        Name = "Ph",
                        Description= "Senor Ph"
                    },
                    new ParentType
                    {
                        Id = 2,
                        Name = "Soil Moisture",
                        Description = "Sensor Kelembapan Tanah" //5 sensor berdasrkan jumlah enum pas itu
                    },
                    new ParentType
                    {
                        Id = 3,
                        Name = "Air Moisture",
                        Description = "Sensor Kelembapan Udara"
                    },
                    new ParentType
                    {
                        Id = 4,
                        Name = "Soil Temperature",
                        Description = "Sensor Temprature Tanah"
                    },
                    new ParentType
                    {
                        Id = 5,
                        Name = "Air Temperature",
                        Description = "Sensor Temprature Udara"
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedActuatorTypeAsync(AppDbContext ctx)
        {
            if (!(ctx.TypeActuators.ToList().Count()>0))
            {

                ctx.TypeActuators.AddRange(
                    new TypeActuators
                    {
                        Id = 1,
                        Name = "Servo",
                        Description= "Servo"
                    },
                    new TypeActuators
                    {
                        Id = 2,
                        Name = "Waterpump",
                        Description = "Waterpump" //5 sensor berdasrkan jumlah enum pas itu
                    },
                    new TypeActuators
                    {
                        Id = 3,
                        Name = "Camera",
                        Description = "Camera"
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedPlantAsync(AppDbContext ctx)
        {
            if (!(ctx.Plants.ToList().Count()>0))
            {
                var p = new List<Parameter>(); //ph tanah
                p.Add( new Parameter
                {
                    //3 parameter unique optimal low ~ high ph basa 
                    Id = 1,
                    Description="Asam",
                    MinValue=0,
                    MaxValue=7,
                    Color="0.5",
                });
                p.Add(new Parameter
                {
                    Id = 2,
                    Description ="Optimal",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                p.Add(new Parameter
                {
                    Id = 3,
                    Description ="Basa",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                var p2 = new List<Parameter>();//suhu tanah
                p2.Add( new Parameter
                {
                    //3 parameter unique optimal low ~ high ph basa 
                    Id = 4,
                    Description="Dingin",
                    MinValue=0,
                    MaxValue=7,
                    Color="0.5",
                });
                p2.Add(new Parameter
                {
                    Id = 5,
                    Description ="Optimal",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                p2.Add(new Parameter
                {
                    Id = 6,
                    Description ="Panas",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                var p3 = new List<Parameter>();//kelembapan tanah
                p3.Add( new Parameter
                {
                    //3 parameter unique optimal low ~ high ph basa 
                    Id = 7,
                    Description="Kering",
                    MinValue=0,
                    MaxValue=7,
                    Color="0.5",
                });
                p3.Add(new Parameter
                {
                    Id = 8,
                    Description ="Optimal",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                p3.Add(new Parameter
                {
                    Id = 9,
                    Description ="Basah",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                var p4 = new List<Parameter>();//suhu udara
                p4.Add( new Parameter
                {
                    //3 parameter unique optimal low ~ high ph basa 
                    Id = 10,
                    Description="Dingin",
                    MinValue=0,
                    MaxValue=7,
                    Color="0.5",
                });
                p4.Add(new Parameter
                {
                    Id = 11,
                    Description ="Optimal",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                p4.Add(new Parameter
                {
                    Id = 12,
                    Description ="Panas",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                var p5 = new List<Parameter>();//kelembapan udara
                p5.Add( new Parameter
                {
                    //3 parameter unique optimal low ~ high ph basa 
                    Id = 13,
                    Description="Kering",
                    MinValue=0,
                    MaxValue=7,
                    Color="0.5",
                });
                p5.Add(new Parameter
                {
                    Id = 14,
                    Description ="Optimal",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                p5.Add(new Parameter
                {
                    Id = 15,
                    Description ="Basah",
                    MinValue = 13,
                    MaxValue = 14,
                    Color = "0.5",
                });
                var pn = new List<ParentParameter>();
                pn.Add(
                    new ParentParameter
                    {
                        Id = 1,
                        ParentTypesId  = 1, //cocokin
                        Parameters = p,
                    }
                );
                var pn2 = new List <ParentParameter>();
                pn2.Add(
                    new ParentParameter
                    {
                        Id = 2,
                        ParentTypesId = 4,
                        Parameters = p2,
                    }
                );
                var pn3 = new List <ParentParameter>();
                pn3.Add(
                    new ParentParameter
                    {
                        Id = 3,
                        ParentTypesId = 2,
                        Parameters = p3,
                    }
                );
                var pn4 = new List <ParentParameter>();
                pn4.Add(
                    new ParentParameter
                    {
                        Id = 4,
                        ParentTypesId = 5,
                        Parameters = p4,
                    }
                );
                var pn5 = new List <ParentParameter>();
                pn5.Add(
                    new ParentParameter
                    {
                        Id = 5,
                        ParentTypesId = 3,
                        Parameters = p5,
                    }
                );
                ctx.Plants.AddRange(
                    new Plant
                    {
                        Id = 1,
                        Name ="Sorghum Vulgare",
                        LatinName ="Sorghum technicum",
                        Description ="is an annual herbaceous plant belonging to the family of grasses",
                        ParentParameters = pn
                    },
                    new Plant
                    {
                        Id = 2,
                        Name ="Sorghum Serena",
                        LatinName ="Sorghum technicum",
                        Description ="is an annual herbaceous plant belonging to the family of grasses",
                        ParentParameters = pn2
                    },
                    new Plant
                    {
                        Id = 3,
                        Name ="Sorghum bicolor",
                        LatinName ="Sorghum technicum",
                        Description ="is an annual herbaceous plant belonging to the family of grasses",
                        ParentParameters = pn3
                    },
                    new Plant
                    {
                        Id = 4,
                        Name ="Sorghum amplum",
                        LatinName ="Sorghum technicum",
                        Description ="is an annual herbaceous plant belonging to the family of grasses",
                        ParentParameters = pn4
                    },
                    new Plant
                    {
                        Id = 5,
                        Name ="Sorghum bulbosum",
                        LatinName ="Sorghum technicum",
                        Description ="is an annual herbaceous plant belonging to the family of grasses",
                        ParentParameters = pn5
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedLandAsync(AppDbContext ctx)
        {
            if (!(ctx.Lands.ToList().Count>0))
            {

                ctx.Lands.AddRange(
                    new Land
                    {
                        Id = 1,
                        Name ="ITS Smart Farm",
                        Code="Lahan 1",
                        Address="Dusun Klampisan RT 1 RW 1. Desa Wirobiting. Kecamatan Prambon. Kabupaten Sidoarjo. Jawa Timur. 61264",
                        CordinateLand = "x:1 y:2"
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        //region disini
        public static async Task SeedRegionAsync(AppDbContext ctx)
        {
            if (!(ctx.Regions.ToList().Count()>0))
            {

                ctx.Regions.AddRange(
                    new Region
                    {   
                        Id = 1,
                        Name = "GreenHouse keputih 1",
                        RegionDescription = "GreenHouse keputih",
                        CordinateRegion = "x:1 y:1",
                        LandId = 1,
                        PlantId = 1,
                    },
                    new Region
                    {   
                        Id = 2,
                        Name = "GreenHouse keputih 2",
                        RegionDescription = "GreenHouse keputih",
                        CordinateRegion = "x:1 y:1",
                        LandId = 1,
                        PlantId = 2,
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedIdIotAsync(AppDbContext ctx)
        {
            if (!(ctx.IdentityIoTs.ToList().Count()>0))
            {
                ctx.IdentityIoTs.AddRange(
                   new IdIoT {
                        Id = 1,
                        Name = "Rasberry PI 3 +",
                        Description = "mini Pc",
                        Code="A012DF",
                        Secret="onetoomany",
                    },
                   new IdIoT {
                        Id = 2,
                        Name = "Rasberry PI 4 +",
                        Description = "mini Pc",
                        Code="A013DF",
                        Secret="onetoomany",
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedMiniPcAsync(AppDbContext ctx)
        {
            if (!(ctx.MiniPcs.ToList().Count()>0))
            {
                ctx.MiniPcs.AddRange(
                    new MiniPc
                    {
                        Id = 1,
                        Name="Rasberry PI 3 +",
                        Description="Mini Pc Region",
                        RegionId=1,
                        Code="A012DF",
                        Secret="onetoomany",
                        IdentityId =1
                    },
                    new MiniPc
                    {
                        Id = 2,
                        Name="Rasberry PI 3 +",
                        Description="Mini Pc Region",
                        RegionId=2,
                        Code="A013DF",
                        Secret="onetoomany",
                        IdentityId =2
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedMicroAsync(AppDbContext ctx)
        {
            if (!(ctx.Mikrokontrollers.ToList().Count()>0))
            {

                ctx.Mikrokontrollers.AddRange(
                    new Mikrokontroller
                    {
                            Id = 1,
                            Name="ESP 1",
                            Description="Microkontroller",
                            MiniPcId = 1
                    },
                    new Mikrokontroller
                    {
                            Id = 2,
                            Name="ESP 2",
                            Description="Microkontroller",
                            MiniPcId = 2
                    }
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedSensorAsync(AppDbContext ctx)
        {
            if (!(ctx.Sensors.ToList().Count()>0))
            {

                ctx.Sensors.AddRange(
                    new Sensor
                    {
                        Id = 1,
                        Name="DHT22",
                        Description="Suhu Udara",
                        ParentTypeId=5,//cocokin parentype static dengan sensor
                        MikrocontrollerId =1,
                        ParentParamId=5
                    },
                    new Sensor
                    {
                        Id = 2,
                        Name="DHT22",
                        Description="Kelembapan Udara",
                        ParentTypeId=3,//cocokin parentype static dengan sensor
                        MikrocontrollerId=1,
                        ParentParamId=3
                    },
                    new Sensor
                    {
                        Id = 3,
                        Name="DS18B20",
                        Description="Suhu Tanah",
                        ParentTypeId=4,//cocokin parentype static dengan sensor
                        MikrocontrollerId =1,
                        ParentParamId=4
                    },
                    new Sensor
                    {
                        Id = 4,
                        Name="PH TanahSensor",
                        Description="Sensor Ph Tanah",
                        ParentTypeId=1,//cocokin parentype static dengan sensor
                        MikrocontrollerId =1,
                        ParentParamId=1
                    },
                    new Sensor
                    {
                        Id = 5,
                        Name="Kelembapan Tanah",
                        Description="Sensor Kelembapan Tanah",
                        ParentTypeId=2,//cocokin parentype static dengan sensor
                        MikrocontrollerId =1,
                        ParentParamId=2
                    }
                    //namasensor [DHT22(2x Suhu Udara Sama Kelembapan Udaraa),DS18B20(suhu tanah),PHTanahSensor,KelembapanTanah]
                ); ; ;
                await ctx.SaveChangesAsync();
            }
        }
        public static async Task SeedDataAsync(AppDbContext ctx)
        {
            Random gen = new Random();
            List<Data> temp = new List<Data>();
            DateTime start = new DateTime(2022, 1, 1);
            int range = (DateTime.Today - start).Days;
            var randomTest = new Random();
            if (!(ctx.Datas.ToList().Count()>0)){
                for(int i = 1; i< 100; i++){
                    TimeSpan timeSpan = DateTime.Today - start;
                    TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
                    DateTime newDate = start + newSpan;
                    ctx.Datas.AddRange(
                    new Data{
                        SensorId = 1,
                        ParentParamId = 1,
                        ValueParameter = gen.Next(5, 10),
                        CreatedAt = newDate
                    },
                    new Data{
                        SensorId = 2,
                        ParentParamId = 1,
                        ValueParameter = gen.Next(5, 10),
                        CreatedAt = newDate
                    },
                    new Data{
                        SensorId = 3,
                        ParentParamId = 1,
                        ValueParameter = gen.Next(5, 10),
                        CreatedAt = newDate
                    },
                    new Data{
                        SensorId = 4,
                        ParentParamId = 1,
                        ValueParameter = gen.Next(5, 10),
                        CreatedAt = newDate
                    },
                    new Data{
                        SensorId = 5,
                        ParentParamId = 1,
                        ValueParameter = gen.Next(5, 10),
                        CreatedAt = newDate
                    });
                    await ctx.SaveChangesAsync();
                } 
            }
        }
    public static async Task SeedActuatorAsync(AppDbContext ctx)
    {
    if (!(ctx.Actuators.ToList().Count()>0))
        {

            ctx.Actuators.AddRange(
                new Actuator
                {
                    Id = 1,
                    Name="DHT22",
                    Description="Suhu Udara",
                    MikrocontrollerId =1,
                }
                //namasensor [DHT22(2x Suhu Udara Sama Kelembapan Udaraa),DS18B20(suhu tanah),PHTanahSensor,KelembapanTanah]
            ); ; ;
            await ctx.SaveChangesAsync();
        }
    }

    public static async Task SeedScheduleTagAsync(AppDbContext ctx){
        if (!(ctx.ScheduleTags.ToList().Count>0))
        {

            ctx.ScheduleTags.AddRange(
                new ScheduleTag
                {
                    Id = 1,
                    Name ="Tag 1",
                }
            ); ; ;
            await ctx.SaveChangesAsync();
        }
    }
    public static async Task SeedDiseaseAsync(AppDbContext ctx){
        if (!(ctx.Disease.ToList().Count>0))
        {

            ctx.Disease.AddRange(
                new Disease
                {
                    Id = 1,
                    Name ="Penyakit 1",
                }
            ); ; ;
            await ctx.SaveChangesAsync();
        }
    }

    //TODO Seed FuzzyInference Parameter
    //TODO Seed FuzzyInference Output
    //TODO Seed FuzzyCrisp Output
    }
}
