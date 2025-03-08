using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkflowVisualizer.Models;

namespace WorkflowVisualizer.Data;

public partial class WkfDbContext : DbContext
{
    public WkfDbContext()
    {
    }

    public WkfDbContext(DbContextOptions<WkfDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WkfActn> WkfActns { get; set; }

    public virtual DbSet<WkfActnCode> WkfActnCodes { get; set; }

    public virtual DbSet<WkfModl> WkfModls { get; set; }

    public virtual DbSet<WkfRule> WkfRules { get; set; }

    public virtual DbSet<WkfRuleCrtum> WkfRuleCrta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<WkfDbContext>()
                .Build();

            var connectionString = configuration.GetConnectionString("WorkflowAppConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WkfActn>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__WKF_ACTN__A7EC83712DF1BF10");

            entity.ToTable("WKF_ACTN");

            entity.Property(e => e.ActionId).HasColumnName("ACTION_ID");
            entity.Property(e => e.ActionCde).HasColumnName("ACTION_CDE");
            entity.Property(e => e.ExeSequence).HasColumnName("EXE_SEQUENCE");
            entity.Property(e => e.ExecutionDte)
                .HasColumnType("datetime")
                .HasColumnName("EXECUTION_DTE");
            entity.Property(e => e.ExecutionOffset).HasColumnName("EXECUTION_OFFSET");
            entity.Property(e => e.ModelCde).HasColumnName("MODEL_CDE");
            entity.Property(e => e.Params)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("PARAMS");
            entity.Property(e => e.RuleId).HasColumnName("RULE_ID");
            entity.Property(e => e.SessionCde)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SESSION_CDE");
            entity.Property(e => e.SessionId).HasColumnName("SESSION_ID");
            entity.Property(e => e.StateCde).HasColumnName("STATE_CDE");

            entity.HasOne(d => d.ActionCdeNavigation).WithMany(p => p.WkfActns)
                .HasForeignKey(d => d.ActionCde)
                .HasConstraintName("FK__WKF_ACTN__ACTION__54D818B5");

            entity.HasOne(d => d.ModelCdeNavigation).WithMany(p => p.WkfActns)
                .HasForeignKey(d => d.ModelCde)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_wa__modcde");

            entity.HasOne(d => d.Rule).WithMany(p => p.WkfActns)
                .HasForeignKey(d => d.RuleId)
                .HasConstraintName("FK__WKF_ACTN__RULE_I__55CC3CEE");
        });

        modelBuilder.Entity<WkfActnCode>(entity =>
        {
            entity.HasKey(e => e.ActionCde).HasName("PK__WKF_ACTN__2DFBAD0F03FB8544");

            entity.ToTable("WKF_ACTN_CODE");

            entity.Property(e => e.ActionCde).HasColumnName("ACTION_CDE");
            entity.Property(e => e.ActionDsc)
                .HasMaxLength(100)
                .HasColumnName("ACTION_DSC");
            entity.Property(e => e.ActiveInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("T")
                .IsFixedLength()
                .HasColumnName("ACTIVE_IND");
            entity.Property(e => e.ExecutionDte)
                .HasColumnType("datetime")
                .HasColumnName("EXECUTION_DTE");
            entity.Property(e => e.ExecutionOffset).HasColumnName("EXECUTION_OFFSET");
            entity.Property(e => e.LanguageCde)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("LANGUAGE_CDE");
            entity.Property(e => e.Params)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("PARAMS");
            entity.Property(e => e.RecordVer).HasColumnName("RECORD_VER");
            entity.Property(e => e.SessionCde)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SESSION_CDE");
            entity.Property(e => e.SessionId).HasColumnName("SESSION_ID");
            entity.Property(e => e.SysInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("F")
                .IsFixedLength()
                .HasColumnName("SYS_IND");
        });

        modelBuilder.Entity<WkfModl>(entity =>
        {
            entity.HasKey(e => e.ModelCde).HasName("PK__WKF_MODL__C3ADE9573138400F");

            entity.ToTable("WKF_MODL");

            entity.Property(e => e.ModelCde).HasColumnName("MODEL_CDE");
            entity.Property(e => e.ActiveInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("T")
                .IsFixedLength()
                .HasColumnName("ACTIVE_IND");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedDte)
                .HasColumnType("datetime")
                .HasColumnName("CREATED_DTE");
            entity.Property(e => e.ExecPriority).HasColumnName("EXEC_PRIORITY");
            entity.Property(e => e.ExecutionDte)
                .HasColumnType("datetime")
                .HasColumnName("EXECUTION_DTE");
            entity.Property(e => e.ExecutionOffset).HasColumnName("EXECUTION_OFFSET");
            entity.Property(e => e.Formula)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("FORMULA");
            entity.Property(e => e.FormulaDsc)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("FORMULA_DSC");
            entity.Property(e => e.ModelDsc)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("MODEL_DSC");
            entity.Property(e => e.RequestTypeCode)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("request_type_code");
            entity.Property(e => e.SessionCde)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SESSION_CDE");
            entity.Property(e => e.SessionId).HasColumnName("SESSION_ID");
            entity.Property(e => e.SysInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("F")
                .IsFixedLength()
                .HasColumnName("SYS_IND");
        });

        modelBuilder.Entity<WkfRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__WKF_RULE__E103520C23DE44F1");

            entity.ToTable("WKF_RULE");

            entity.Property(e => e.RuleId).HasColumnName("RULE_ID");
            entity.Property(e => e.ActiveInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("T")
                .IsFixedLength()
                .HasColumnName("ACTIVE_IND");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreatedDte)
                .HasColumnType("datetime")
                .HasColumnName("CREATED_DTE");
            entity.Property(e => e.ExecutionDte)
                .HasColumnType("datetime")
                .HasColumnName("EXECUTION_DTE");
            entity.Property(e => e.ExecutionOffset).HasColumnName("EXECUTION_OFFSET");
            entity.Property(e => e.Formula)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("FORMULA");
            entity.Property(e => e.FormulaDsc)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("FORMULA_DSC");
            entity.Property(e => e.RuleDsc)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("RULE_DSC");
            entity.Property(e => e.SessionCde)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SESSION_CDE");
            entity.Property(e => e.SessionId).HasColumnName("SESSION_ID");
            entity.Property(e => e.SysInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("F")
                .IsFixedLength()
                .HasColumnName("SYS_IND");
        });

        modelBuilder.Entity<WkfRuleCrtum>(entity =>
        {
            entity.HasKey(e => new { e.ModelCde, e.StateCde, e.RuleCde, e.RuleCdeSeq }).HasName("PK_WKF_RULE_CRITERIA");

            entity.ToTable("WKF_RULE_CRTA");

            entity.Property(e => e.ModelCde).HasColumnName("MODEL_CDE");
            entity.Property(e => e.StateCde).HasColumnName("STATE_CDE");
            entity.Property(e => e.RuleCde).HasColumnName("RULE_CDE");
            entity.Property(e => e.RuleCdeSeq)
                .ValueGeneratedOnAdd()
                .HasColumnName("RULE_CDE_SEQ");
            entity.Property(e => e.ActiveInd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("active_ind");
            entity.Property(e => e.AssociationTyp)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("association_typ");
            entity.Property(e => e.BusinessRuleCde)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("business_rule_cde");
            entity.Property(e => e.ExecutionDte)
                .HasColumnType("datetime")
                .HasColumnName("execution_dte");
            entity.Property(e => e.ExecutionOffset).HasColumnName("EXECUTION_OFFSET");
            entity.Property(e => e.ExpressionSeq).HasColumnName("expression_seq");
            entity.Property(e => e.PostfixExpression)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("postfix_expression");
            entity.Property(e => e.PrefixExpression)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("prefix_expression");
            entity.Property(e => e.SessionCde)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("SESSION_CDE");
            entity.Property(e => e.SessionId).HasColumnName("SESSION_ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
