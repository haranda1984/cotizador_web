using Microsoft.EntityFrameworkCore;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UsersProfile { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<TradePolicy> TradePolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("users_profile");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Email).HasColumnName("email").IsRequired().HasMaxLength(256);
                entity.Property(p => p.Password).HasColumnName("secret").IsRequired().HasMaxLength(256);
                entity.Property(p => p.FirstName).HasColumnName("first_name").IsRequired();
                entity.Property(p => p.LastName).HasColumnName("last_name").IsRequired();
                entity.Property(p => p.Position).HasColumnName("position");
                entity.Property(p => p.Company).HasColumnName("company");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.ProfileId);
                entity.HasOne(p => p.Profile).WithMany().HasForeignKey(p => p.ProfileId);
                entity.Property(p => p.ProfileId).HasColumnName("profile_id").IsRequired();
                entity.Property(p => p.LastLoggedIn).HasColumnName("last_logged_in");
                entity.Property(p => p.FailedLoginAttempts).HasColumnName("failed_login_attempts").IsRequired();
                entity.Property(p => p.SuccessfulLoginAttempts).HasColumnName("successful_login_attempts").IsRequired();
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired();
                entity.Property(p => p.Description).HasColumnName("description");
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("users_roles");
                entity.HasKey(p => new { p.UserId, p.RoleId });
                entity.HasOne<User>(p => p.User).WithMany(p => p.UserRoles).HasForeignKey(p => p.UserId);
                entity.HasOne<Role>(p => p.Role).WithMany(p => p.UserRoles).HasForeignKey(p => p.RoleId);
                entity.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
                entity.Property(p => p.RoleId).HasColumnName("role_id").IsRequired();
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.ToTable("stages");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.Order).HasColumnName("order");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("projects");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
                entity.Property(p => p.DisplayName).HasColumnName("display_name").HasMaxLength(256);
                entity.Property(p => p.DeliveryDate).HasColumnName("delivery_date").IsRequired();
                entity.Property(p => p.Description).HasColumnName("description").IsRequired().HasMaxLength(1024);
                entity.Property(p => p.PhotoUrl).HasColumnName("photo_url").HasMaxLength(256);
                entity.Property(p => p.ThemeColor).HasColumnName("theme_color").IsRequired().HasMaxLength(7);
                entity.Property(p => p.Location).HasColumnName("location").IsRequired().HasMaxLength(256);
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();
                entity.Property(p => p.AllowsCondoHotel).HasColumnName("allows_condohotel").IsRequired();
                entity.Property(p => p.Appreciation).HasColumnName("appreciation").HasPrecision(20, 8).IsRequired();
                entity.Property(p => p.MinimumCondoHotelUnits).HasColumnName("min_condohotel_units");
                entity.Property(p => p.CurrentStageId).HasColumnName("current_stage").IsRequired();
                entity.Property(p => p.CondoHotelPreOperatingCost).HasColumnName("condohotel_preoperating_cost").HasPrecision(20, 8);
                entity.Property(p => p.CapRate).HasColumnName("cap_rate").HasPrecision(7, 4);
                entity.HasOne<Stage>(p => p.CurrentStage).WithMany().HasForeignKey(p => p.CurrentStageId);
                entity.HasMany<ProjectStage>(p => p.ProjectStages).WithOne(p => p.Project).HasForeignKey(p => p.ProjectId);
                entity.Ignore(p => p.Amenities);
            });

            modelBuilder.Entity<ProjectStage>(entity =>
            {
                entity.ToTable("project_stages");
                entity.HasKey(p => new { p.ProjectId, p.StageId });
                entity.HasOne<Project>(p => p.Project).WithMany(p => p.ProjectStages).HasForeignKey(p => p.ProjectId);
                entity.HasOne<Stage>(p => p.Stage).WithMany(p => p.ProjectStages).HasForeignKey(p => p.StageId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                entity.Property(p => p.StageId).HasColumnName("stage_id").IsRequired();
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.ToTable("models");
                entity.HasKey(p => p.Id);
                entity.HasOne<Project>(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(p => p.Type).HasColumnName("type").HasMaxLength(10);
                entity.Property(p => p.SubType).HasColumnName("subtype").HasMaxLength(50);
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("units");
                entity.HasKey(p => p.Id);
                entity.HasOne<Model>(p => p.Model).WithMany().HasForeignKey(p => p.ModelId);
                entity.Property(p => p.ModelId).HasColumnName("model_id").IsRequired();
                entity.Property(p => p.Number).HasColumnName("number").IsRequired().HasMaxLength(16);
                entity.Property(p => p.Building).HasColumnName("building").HasMaxLength(32);
                entity.Property(p => p.Level).HasColumnName("level");
                entity.Property(p => p.GrossArea).HasColumnName("gross_area").HasPrecision(20, 8).IsRequired();
                entity.Property(p => p.BuiltUpArea).HasColumnName("built_up_area").HasPrecision(20, 8).IsRequired();
                entity.Property(p => p.TerraceArea).HasColumnName("terrace_area").HasPrecision(20, 8);
                entity.Property(p => p.Status).HasColumnName("status").IsRequired();
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.IsCondoHotel).HasColumnName("is_condohotel").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();
            });

            modelBuilder.Entity<UnitPrice>(entity =>
            {
                entity.ToTable("units_price");
                entity.HasKey(p => new { p.UnitId, p.StageId });
                entity.HasOne<Unit>(p => p.Unit).WithMany(p => p.UnitPrices).HasForeignKey(p => p.UnitId);
                entity.HasOne<Stage>(p => p.Stage).WithMany(p => p.UnitPrices).HasForeignKey(p => p.StageId);
                entity.Property(p => p.UnitId).HasColumnName("unit_id").IsRequired();
                entity.Property(p => p.StageId).HasColumnName("stage_id").IsRequired();
                entity.Property(p => p.Price).HasColumnName("price").HasPrecision(20, 8).IsRequired();
                entity.Property(p => p.Currency).HasColumnName("currency").IsRequired().HasMaxLength(50);
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
            });

            modelBuilder.Entity<TradePolicy>(entity =>
            {
                entity.ToTable("trade_policies");
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.ProjectId);
                entity.HasOne<Project>(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
                entity.Property(p => p.Discount).HasColumnName("discount").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.Deposit).HasColumnName("deposit").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.AdditionalPayments).HasColumnName("additional_payments").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.MonthlyPayments).HasColumnName("monthly_payments").IsRequired();
                entity.Property(p => p.LastPayment).HasColumnName("last_payment").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.IsActive).HasColumnName("is_active").IsRequired();
                entity.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
                entity.Property(p => p.ModifiedAt).HasColumnName("modified_at").IsRequired();
            });

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.ToTable("rates");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(255);
            });

            modelBuilder.Entity<Temporality>(entity =>
            {
                entity.ToTable("temporalities");
                entity.HasKey(p => new { p.ProjectId, p.RateId, p.Month });
                entity.HasOne<Project>(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId);
                entity.HasOne<Rate>(p => p.Rate).WithMany().HasForeignKey(p => p.RateId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                entity.Property(p => p.RateId).HasColumnName("rate_id").IsRequired();
                entity.Property(p => p.Month).HasColumnName("month").IsRequired();
                entity.Property(p => p.OccupationInDays).HasColumnName("occupation_in_days").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.OccupationInDaysMax).HasColumnName("occupation_in_days_max").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.Percentage).HasColumnName("percentage").HasPrecision(12, 8);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("expenses");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(50).IsRequired();
                entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(128);
                entity.Property(p => p.DefaultValue).HasColumnName("default_value").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.Type).HasColumnName("type").IsRequired();
            });

            modelBuilder.Entity<CondoHotelExpense>(entity =>
            {
                entity.ToTable("condohotel_expenses");
                entity.HasKey(p => new { p.ProjectId, p.ExpenseId });
                entity.HasOne<Project>(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId);
                entity.HasOne<Expense>(p => p.Expense).WithMany().HasForeignKey(p => p.ExpenseId);
                entity.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
                entity.Property(p => p.ExpenseId).HasColumnName("expense_id").IsRequired();
                entity.Property(p => p.Cost).HasColumnName("cost").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.Percentage).HasColumnName("percentage").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.IsOptional).HasColumnName("is_optional").IsRequired();
            });

            modelBuilder.Entity<UnitEquipment>(entity =>
            {
                entity.ToTable("units_equipment");
                entity.HasKey(p => p.Id);
                entity.HasOne<Unit>(p => p.Unit).WithMany().HasForeignKey(p => p.UnitId);
                entity.Property(p => p.UnitId).HasColumnName("unit_id").IsRequired();
                entity.Property(p => p.Description).HasColumnName("description").HasMaxLength(50).IsRequired();
                entity.Property(p => p.Cost).HasColumnName("cost").HasPrecision(15, 9);
            });

            modelBuilder.Entity<UnitRate>(entity =>
            {
                entity.ToTable("units_rates");
                entity.HasKey(p => p.UnitId);
                entity.HasOne<Unit>(p => p.Unit).WithMany().HasForeignKey(p => p.UnitId);
                entity.Property(p => p.UnitId).HasColumnName("unit_id").IsRequired();
                entity.Property(p => p.CostPerNight).HasColumnName("cost_per_night").HasPrecision(12, 8);
                entity.Property(p => p.BuiltUpAreaCost).HasColumnName("built_up_area_cost").HasPrecision(12, 8).IsRequired();
                entity.Property(p => p.TerraceAreaCost).HasColumnName("terrace_area_cost").HasPrecision(12, 8);
            });
        }
    }
}
