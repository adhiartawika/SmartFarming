using backend.Commons;
using backend.Model.AppEntity;
using backend.Model.IdEntity;
using backend.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using  Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace backend.Persistences
{
    public interface IAppDbContext
    {
        public DbSet<Schedule> Schedules {get;set;}
        public DbSet<ScheduleLog> ScheduleLogs {get;set;}
        public DbSet<ScheduleLogImage> ScheduleLogImages {get;set;}
        public DbSet<ScheduleOccurrence> ScheduleOccurrences {get;set;}
        public DbSet<UserRole>UserRoles{get;set;}
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Land> Lands { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Data> Datas { get; set; }
        public DbSet<IotStatus> IotStatus { get; set; }
        
        public DbSet<Actuator> Actuators { get; set; }
        public DbSet<Mikrokontroller> Mikrokontrollers { get; set; }
        public DbSet<MiniPc> MiniPcs {get;set;}
        public DbSet<Sensor> Sensors {get;set;}
        // public DbSet<DataResult> DataResults { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<ActuatorStatus> ActuatorStatuses {get;set;}
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<IdIoT> IdentityIoTs { get; set; }
        public DbSet<TypeActuators> TypeActuators {get;set;}
        public DbSet<ParentParameter> ParentParameters {get;set;}
        public DbSet<ParentType> ParentTypes {get;set;}
        public DbSet<PlantVirus> PlantViruses {get;set;}
        public DbSet<VirusMonitor> VirusMonitors {get;set;}
        public DbSet<MonitorStatus> MonitorStatuses {get;set;}
        public DbSet<Instituted> Instituteds {get;set;}
        public DbSet<LanLatDiseases> LanLatDiseases {get;set;}
        public DbSet<DiseaseImage> DiseaseImages { get; set; }
        // public DbSet<NotificationType> NotificationTypes { get; set; }
        // public DbSet<NotificationContent> NotificationContents { get; set; }

        //TODO FuzzyInference Parameter
        //TODO FuzzyInference Output
        //TODO FuzzyCrisp Output

        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync();
        public void RollbackTransaction();
        public IExecutionStrategy GetExecStrat();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    }
    public class AppDbContext : DbContext, IAppDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentIoTService _currentIoTService;
        private readonly IDateTime _dateTime;
        private IDbContextTransaction _currentTransaction;
        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService,
            ICurrentIoTService currentIoTService,
            IDateTime dateTime) : base(options)
        {
            this._currentUserService = currentUserService;
            this._currentIoTService = currentIoTService;
            this._dateTime = dateTime;
        }

        public DbSet<PlantVirus> PlantViruses {get;set;}
        public DbSet<VirusMonitor> VirusMonitors {get;set;}
        public DbSet<MonitorStatus> MonitorStatuses {get;set;}
        public DbSet<UserRole>UserRoles{get;set;}
        public DbSet<Plant> Plants { get ; set ; }
        // public DbSet<DataResult> DataResults { get ; set ; }
        public DbSet<Land> Lands { get ; set ; }
        public DbSet<Instituted> Instituteds {get;set;}
        public DbSet<Region> Regions { get ; set ; }
        public DbSet<IotStatus> IotStatus { get ; set ; }
        public DbSet<LanLatDiseases> LanLatDiseases {get;set;}
        public DbSet<MiniPc> MiniPcs {get;set;}
        public DbSet<Parameter> Parameters { get ; set ; }
        public DbSet<TypeActuators> TypeActuators {get;set;}

        public DbSet<ActuatorStatus> ActuatorStatuses {get;set;}
        public DbSet<ParentType> ParentTypes {get;set;}
        public DbSet<ParentParameter> ParentParameters {get;set;}
        public DbSet<Data> Datas { get ; set ; }
        public DbSet<Actuator> Actuators { get; set; }
        public DbSet<Mikrokontroller> Mikrokontrollers{ get; set; }
        public DbSet<Sensor> Sensors {get;set;}
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<ApplicationUser> Users { get; set;}
        public DbSet<IdIoT> IdentityIoTs { get; set; }

        public DbSet<DiseaseImage> DiseaseImages { get; set; }
        public DbSet<Schedule> Schedules {get;set;}
        public DbSet<ScheduleLog> ScheduleLogs {get;set;}
        public DbSet<ScheduleLogImage> ScheduleLogImages {get;set;}
        public DbSet<ScheduleOccurrence> ScheduleOccurrences {get;set;}
///ubah get set
        // public DbSet<NotificationContent> NotificationContents { get; set; }
        // public DbSet<NotificationType> NotificationTypes { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<Auditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // entry.Entity.CreatedById = (int)_currentUserService.UserId;
                        entry.Entity.CreatedAt = _dateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedAt = _dateTime.Now;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.DeletedBy = _currentUserService.UserId;
                        entry.Entity.DeletedAt = _dateTime.Now;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<TimeStampDataAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = _dateTime.Now;
                        break;
                }
            }
            foreach (var entry in ChangeTracker.Entries<TimeStampDataIot>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.IdIoT = (int)this._currentIoTService.IoTId;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public IExecutionStrategy GetExecStrat()
        {
            return base.Database.CreateExecutionStrategy();
        }
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await base.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(false);
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync().ConfigureAwait(false);

                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder.Entity<DataResult>()
            //         .Property(x => x.RFanTime)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RFanMode)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RServoTime)s
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RWaterPumpTime)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RLampState)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<Sensor>().HasOne(x => x.ParentType).WithMany().HasForeignKey(x => x.ParentTypeId).OnDelete(DeleteBehavior.Restrict);
            // builder.Entity<Data>().HasOne(x => x.Sensor).WithMany().HasForeignKey(x => x.SensorId).OnDelete(DeleteBehavior.Restrict);
        // foreach (var entityType in builder.Model.GetEntityTypes())
        // {
        //     foreach (var property in entityType.GetProperties())
        //     {
        //         if (property.Name == "CreatedBy")
        //         {
        //             //fixTextDatas.Add(new Tuple<Type, Type, string>(entityType.ClrType, property.ClrType, property.Name));
        //             builder.Entity(entityType.ClrType).HasOne( ).WithMany();

        //         }
        //         else if (Type.GetTypeCode(property.ClrType) == TypeCode.String)
        //         {
        //             builder.Entity(entityType.ClrType).Property(property.ClrType, property.Name).HasColumnType("varchar(255)");

        //         }
        //     }
        // }
        // builder.Entity<Plant>()
        //     .HasOne( e => e.CreatedBy)
        //     .WithMany( e => e.instituted)
        //     .HasForeignKey(e => e.CreatedById);
        // builder.Entity<Plant>().HasOne(e => e.CreatedBy).WithMany().HasForeignKey(e => e.CreatedById);
        builder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.User)
                .UsingEntity<IdentityUserRole<int>>(
                    UserRole => UserRole.HasOne<UserRole>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired(),
                    UserRole => UserRole.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
                );
            // builder.Entity<ApplicationUser>()
            //         .HasOne(x => x.instituted)
            //         .WithMany(x => x.User)
            //         .HasForeignKey(x => x.institutedId);

            builder.Entity<Data>()
                    .Property(x => x.ValueParameter)
                    .HasColumnType("decimal(7,3)");
                    
            /* For Schedule Relation*/
            builder.Entity<Schedule>()
                .HasOne(l => l.Land);
            builder.Entity<Schedule>()
                .HasMany(g => g.ScheduleLogs);
            builder.Entity<ScheduleLog>()
                .HasMany(i => i.Images);
            // builder.Entity<ScheduleOccurrence>()
                // .HasOne(s => s.Schedule);
            builder.Entity<Schedule>()
                .HasOne(dm => dm.DiseaseMonitor);
            base.OnModelCreating(builder);
        }
    }

}
