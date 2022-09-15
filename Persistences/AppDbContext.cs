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

namespace backend.Persistences
{
    public interface IAppDbContext
    {
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Land> Lands { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Data> Datas { get; set; }
        public DbSet<IotStatus> IotStatus { get; set; }
        public DbSet<Mikrokontroller> Mikrokontrollers { get; set; }
        public DbSet<Sensor> Sensors {get;set;}
        // public DbSet<DataResult> DataResults { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<IdIoT> IdentityIoTs { get; set; }
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
        public DbSet<Plant> Plants { get ; set ; }
        // public DbSet<DataResult> DataResults { get ; set ; }
        public DbSet<Land> Lands { get ; set ; }
        public DbSet<Region> Regions { get ; set ; }
        public DbSet<IotStatus> IotStatus { get ; set ; }
        public DbSet<Parameter> Parameters { get ; set ; }
        public DbSet<Data> Datas { get ; set ; }
        public DbSet<Mikrokontroller> Mikrokontrollers{ get; set; }
        public DbSet<Sensor> Sensors {get;set;}
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<ApplicationUser> Users { get; set;}
        public DbSet<IdIoT> IdentityIoTs { get; set; }
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
                        entry.Entity.CreatedBy = (int)_currentUserService.UserId;
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
            //         .Property(x => x.RServoTime)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RWaterPumpTime)
            //         .HasColumnType("decimal(6,3)");
            // builder.Entity<DataResult>()
            //         .Property(x => x.RLampState)
            //         .HasColumnType("decimal(6,3)");
            builder.Entity<Data>()
                    .Property(x => x.ValueParameter)
                    .HasColumnType("decimal(7,3)");

            base.OnModelCreating(builder);
        }
    }

}
