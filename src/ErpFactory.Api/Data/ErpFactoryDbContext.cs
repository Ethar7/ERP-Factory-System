using Microsoft.EntityFrameworkCore;
using ErpFactory.Api.Models;
using ErpFactory.Api.Contracts;

namespace ErpFactory.Api.Data;

public sealed class ErpFactoryDbContext(DbContextOptions<ErpFactoryDbContext> options) : DbContext(options)
{
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectItem> ProjectItems => Set<ProjectItem>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Mold> Molds => Set<Mold>();
    public DbSet<ProjectMold> ProjectMolds => Set<ProjectMold>();
    public DbSet<MixDesign> MixDesigns => Set<MixDesign>();
    public DbSet<MixIngredient> MixIngredients => Set<MixIngredient>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<ProductionMaterialConsumption> ProductionMaterialConsumption => Set<ProductionMaterialConsumption>();
    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();
    public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();
    public DbSet<SiteOperation> SiteOperations => Set<SiteOperation>();
    public DbSet<SiteMaterialConsumption> SiteMaterialConsumption => Set<SiteMaterialConsumption>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<ProjectCostSummary> ProjectCostSummary => Set<ProjectCostSummary>();
    public DbSet<JournalEntryBalance> JournalEntryBalance => Set<JournalEntryBalance>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureAccounting(modelBuilder);
        ConfigureProjects(modelBuilder);
        ConfigureInventory(modelBuilder);
        ConfigureMolds(modelBuilder);
        ConfigureMixDesigns(modelBuilder);
        ConfigureProduction(modelBuilder);
        ConfigureDelivery(modelBuilder);
        ConfigureSite(modelBuilder);
        ConfigureReports(modelBuilder);
        ConfigureUsers(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(2);
                }
            }
        }

        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, Name = "Admin" },
            new Role { RoleId = 2, Name = "ProjectManager" },
            new Role { RoleId = 3, Name = "InventoryUser" },
            new Role { RoleId = 4, Name = "Accountant" },
            new Role { RoleId = 5, Name = "Viewer" }
        );
    }

    private static void ConfigureAccounting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.HasKey(x => x.AccountId);
            entity.HasIndex(x => x.AccountCode).IsUnique();
            entity.Property(x => x.AccountCode).HasMaxLength(50);
            entity.Property(x => x.AccountName).HasMaxLength(150);
            entity.Property(x => x.AccountType).HasMaxLength(50);
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(x => x.JournalEntryId);
            entity.Property(x => x.ReferenceType).HasMaxLength(50);
            entity.Property(x => x.Narration).HasMaxLength(500);
            entity.Property(x => x.TransactionDate).HasDefaultValueSql("sysutcdatetime()");
        });

        modelBuilder.Entity<JournalEntryLine>(entity =>
        {
            entity.HasKey(x => x.JournalLineId);
            entity.HasOne(x => x.JournalEntry).WithMany(x => x.Lines).HasForeignKey(x => x.JournalEntryId);
            entity.HasOne(x => x.Account).WithMany(x => x.JournalEntryLines).HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureProjects(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.CustomerId);
            entity.HasIndex(x => x.CustomerName).IsUnique();
            entity.Property(x => x.CustomerName).HasMaxLength(150);
            entity.Property(x => x.ContactPerson).HasMaxLength(100);
            entity.Property(x => x.Phone).HasMaxLength(50);
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.Address).HasMaxLength(250);
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("sysutcdatetime()");
            entity.HasOne(x => x.Account).WithMany(x => x.Customers).HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(x => x.ProjectId);
            entity.Property(x => x.ProjectName).HasMaxLength(150);
            entity.Property(x => x.ProjectStatus).HasMaxLength(50).HasDefaultValue("Draft");
            entity.Property(x => x.StartDate).HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.CreatedAt).HasDefaultValueSql("sysutcdatetime()");
            entity.HasOne(x => x.Customer).WithMany(x => x.Projects).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ProjectItem>(entity =>
        {
            entity.HasKey(x => x.ProjectItemId);
            entity.HasIndex(x => new { x.ProjectId, x.ItemCode }).IsUnique();
            entity.Property(x => x.ItemCode).HasMaxLength(50);
            entity.Property(x => x.ItemName).HasMaxLength(200);
            entity.Property(x => x.Unit).HasMaxLength(50);
            entity.Property(x => x.TaxRate).HasPrecision(5, 2);
            entity.Property(x => x.TaxAmount).HasComputedColumnSql("[RequiredQuantity] * [EstimatedUnitPrice] * ([TaxRate] / 100.0)");
            entity.Property(x => x.TotalPrice).HasComputedColumnSql("[RequiredQuantity] * [EstimatedUnitPrice]");
            entity.Property(x => x.TotalPriceWithTax).HasComputedColumnSql("([RequiredQuantity] * [EstimatedUnitPrice]) + ([RequiredQuantity] * [EstimatedUnitPrice] * ([TaxRate] / 100.0))");
            entity.HasOne(x => x.Project).WithMany(x => x.Items).HasForeignKey(x => x.ProjectId);
        });
    }

    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(x => x.ItemId);
            entity.Property(x => x.ItemName).HasMaxLength(150);
            entity.Property(x => x.ItemType).HasMaxLength(50);
            entity.Property(x => x.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(x => x.TransactionId);
            entity.Property(x => x.TransactionType).HasMaxLength(50);
            entity.Property(x => x.TransactionDate).HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.ReferenceType).HasMaxLength(50);
            entity.Property(x => x.Notes).HasMaxLength(250);
            entity.HasOne(x => x.Item).WithMany(x => x.Transactions).HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureMolds(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mold>(entity =>
        {
            entity.HasKey(x => x.MoldId);
            entity.Property(x => x.MoldName).HasMaxLength(100);
            entity.Property(x => x.MoldStatus).HasMaxLength(50).HasDefaultValue("Available");
        });

        modelBuilder.Entity<ProjectMold>(entity =>
        {
            entity.HasKey(x => x.ProjectMoldId);
            entity.HasIndex(x => new { x.ProjectId, x.MoldId }).IsUnique();
            entity.Property(x => x.AllocatedAt).HasDefaultValueSql("sysutcdatetime()");
            entity.HasOne(x => x.Project).WithMany(x => x.ProjectMolds).HasForeignKey(x => x.ProjectId);
            entity.HasOne(x => x.Mold).WithMany(x => x.ProjectMolds).HasForeignKey(x => x.MoldId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureMixDesigns(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MixDesign>(entity =>
        {
            entity.HasKey(x => x.MixDesignId);
            entity.HasIndex(x => x.MixName).IsUnique();
            entity.Property(x => x.MixName).HasMaxLength(100);
            entity.Property(x => x.TargetStrength).HasMaxLength(50);
        });

        modelBuilder.Entity<MixIngredient>(entity =>
        {
            entity.HasKey(x => x.IngredientId);
            entity.HasIndex(x => new { x.MixDesignId, x.RawMaterialId }).IsUnique();
            entity.HasOne(x => x.MixDesign).WithMany(x => x.Ingredients).HasForeignKey(x => x.MixDesignId);
            entity.HasOne(x => x.RawMaterial).WithMany().HasForeignKey(x => x.RawMaterialId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureProduction(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.HasKey(x => x.ProductionOrderId);
            entity.HasIndex(x => x.BatchNumber).IsUnique();
            entity.Property(x => x.BatchNumber).HasMaxLength(50);
            entity.Property(x => x.OrderDate).HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.ProductionStatus).HasMaxLength(50).HasDefaultValue("Setup");
            entity.HasOne(x => x.Project).WithMany(x => x.ProductionOrders).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.ProjectItem).WithMany().HasForeignKey(x => x.ProjectItemId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.MixDesign).WithMany().HasForeignKey(x => x.MixDesignId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.Mold).WithMany().HasForeignKey(x => x.MoldId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ProductionMaterialConsumption>(entity =>
        {
            entity.HasKey(x => x.ConsumptionId);
            entity.HasIndex(x => new { x.ProductionOrderId, x.MaterialId }).IsUnique();
            entity.Property(x => x.WastageQty).HasComputedColumnSql("[ActualQtyConsumed] - [StandardQtyExpected]");
            entity.HasOne(x => x.ProductionOrder).WithMany(x => x.MaterialConsumption).HasForeignKey(x => x.ProductionOrderId);
            entity.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureDelivery(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryOrder>(entity =>
        {
            entity.HasKey(x => x.DeliveryOrderId);
            entity.Property(x => x.DeliveryDate).HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.DriverName).HasMaxLength(100);
            entity.Property(x => x.VehicleNumber).HasMaxLength(50);
            entity.Property(x => x.LoadingTicketNumber).HasMaxLength(50);
            entity.Property(x => x.DeliveryTicketNumber).HasMaxLength(50);
            entity.Property(x => x.DeliveryStatus).HasMaxLength(50).HasDefaultValue("InTransit");
            entity.HasOne(x => x.Project).WithMany(x => x.DeliveryOrders).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<DeliveryItem>(entity =>
        {
            entity.HasKey(x => x.DeliveryItemId);
            entity.HasIndex(x => new { x.DeliveryOrderId, x.ProjectItemId }).IsUnique();
            entity.HasOne(x => x.DeliveryOrder).WithMany(x => x.Items).HasForeignKey(x => x.DeliveryOrderId);
            entity.HasOne(x => x.ProjectItem).WithMany().HasForeignKey(x => x.ProjectItemId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureSite(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SiteOperation>(entity =>
        {
            entity.HasKey(x => x.SiteOperationId);
            entity.Property(x => x.OperationDate).HasDefaultValueSql("sysutcdatetime()");
            entity.HasOne(x => x.Project).WithMany(x => x.SiteOperations).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.ProjectItem).WithMany().HasForeignKey(x => x.ProjectItemId).OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<SiteMaterialConsumption>(entity =>
        {
            entity.HasKey(x => x.SiteConsumptionId);
            entity.HasIndex(x => new { x.SiteOperationId, x.MaterialId }).IsUnique();
            entity.HasOne(x => x.SiteOperation).WithMany(x => x.Consumption).HasForeignKey(x => x.SiteOperationId);
            entity.HasOne(x => x.Material).WithMany().HasForeignKey(x => x.MaterialId).OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureReports(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectCostSummary>().HasNoKey().ToView("ProjectCostSummary");
        modelBuilder.Entity<JournalEntryBalance>().HasNoKey().ToView("JournalEntryBalance");
    }

    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.UserId);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100);
            entity.Property(x => x.Email).HasMaxLength(150);
            entity.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.RoleId);
            entity.HasIndex(x => x.Name).IsUnique();
        });
    }
}
