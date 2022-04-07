using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SilverScreen.Models.Tables
{
    public partial class SilverScreenContext : DbContext
    {
        public SilverScreenContext()
        {
        }

        public SilverScreenContext(DbContextOptions<SilverScreenContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountReport> AccountReports { get; set; }
        public virtual DbSet<BanConfig> BanConfigs { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentReport> CommentReports { get; set; }
        public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }
        public virtual DbSet<FriendList> FriendLists { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieGenre> MovieGenres { get; set; }
        public virtual DbSet<MovieNotification> MovieNotifications { get; set; }
        public virtual DbSet<MovieRating> MovieRatings { get; set; }
        public virtual DbSet<MovieStaff> MovieStaffs { get; set; }
        public virtual DbSet<MyList> MyLists { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserCommentReport> UserCommentReports { get; set; }
        public virtual DbSet<UserWarning> UserWarnings { get; set; }
        public virtual DbSet<staff> staff { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL($"datasource={Environment.GetEnvironmentVariable("MYSQL_DATABASE_IP") ?? "localhost"}; port=3306; database=SilverScreen; username=root; password=" + Environment.GetEnvironmentVariable("SSDbPass"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountReport>(entity =>
            {
                entity.HasIndex(e => e.UserId, "UserFKAR");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountReports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserFKAR");
            });

            modelBuilder.Entity<BanConfig>(entity =>
            {
                entity.ToTable("BanConfig");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.MovieId, "CMovieID");

                entity.HasIndex(e => e.UserId, "CUserID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CMovieID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CUserID");
            });

            modelBuilder.Entity<CommentReport>(entity =>
            {

                entity.HasIndex(e => e.UserId, "UserFKCR");

                entity.HasIndex(e => e.UnderReview, "UserFKRW");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Contents)
                    .IsRequired()
                    .HasMaxLength(500);


                entity.HasOne(d => d.UnderReviewNavigation)
                    .WithMany(p => p.CommentReportUnderReviewNavigations)
                    .HasForeignKey(d => d.UnderReview)
                    .HasConstraintName("UserFKRW");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CommentReportUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserFKCR");
            });

            modelBuilder.Entity<EfmigrationsHistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__EFMigrationsHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<FriendList>(entity =>
            {
                entity.ToTable("FriendList");

                entity.HasIndex(e => e.UserId, "UsersFriendFK1");

                entity.HasIndex(e => e.UserId1, "UsersFriendFK2");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserId1).HasColumnName("UserID1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FriendListUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UsersFriendFK1");

                entity.HasOne(d => d.UserId1Navigation)
                    .WithMany(p => p.FriendListUserId1Navigations)
                    .HasForeignKey(d => d.UserId1)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UsersFriendFK2");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Genre1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Genre");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Bgimage)
                    .HasMaxLength(200)
                    .HasColumnName("BGImage")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ContentType).HasMaxLength(10);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(800);

                entity.Property(e => e.ReleaseDate)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Thumbnail)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.TmdbId).HasColumnName("TMDB_ID");

                entity.Property(e => e.Trailer).HasMaxLength(100);
            });

            modelBuilder.Entity<MovieGenre>(entity =>
            {
                entity.HasKey(e => new { e.GenreId, e.MovieId })
                    .HasName("PRIMARY");

                entity.ToTable("MovieGenre");

                entity.HasIndex(e => e.MovieId, "MMovieFK");

                entity.Property(e => e.GenreId).HasColumnName("GenreID");

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.MovieGenres)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("GGenreFK");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.MovieGenres)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MMovieFK");
            });

            modelBuilder.Entity<MovieNotification>(entity =>
            {
                entity.HasIndex(e => e.MovieId, "MNMovieFK");

                entity.HasIndex(e => e.UserId, "MNUserFK");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.MovieNotifications)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MNMovieFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MovieNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MNUserFK");
            });

            modelBuilder.Entity<MovieRating>(entity =>
            {
                entity.ToTable("MovieRating");

                entity.HasIndex(e => e.MovieId, "MRMovieFK");

                entity.HasIndex(e => e.UserId, "MRUserFK");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.MovieRatings)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MRMovieFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MovieRatings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MRUserFK");
            });

            modelBuilder.Entity<MovieStaff>(entity =>
            {
                entity.HasKey(e => new { e.StaffId, e.MovieId })
                    .HasName("PRIMARY");

                entity.ToTable("MovieStaff");

                entity.HasIndex(e => e.MovieId, "SMovieFK");

                entity.Property(e => e.StaffId).HasColumnName("StaffID");

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.MovieStaffs)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SMovieFK");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.MovieStaffs)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MStaffFK");
            });

            modelBuilder.Entity<MyList>(entity =>
            {
                entity.ToTable("MyList");

                entity.HasIndex(e => e.MovieId, "MLMovieFK");

                entity.HasIndex(e => e.UserId, "MLUserFK");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.MyLists)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MLMovieFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MyLists)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MLUserFK");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.AuthorId, "NAuthorFK");

                entity.HasIndex(e => e.MovieId, "NMovieFK");

                entity.HasIndex(e => e.UserId, "NUserFK");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.AuthorId).HasColumnName("AuthorID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.MovieId).HasColumnName("MovieID");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("enum('TextOnly','FriendRequest')");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.NotificationAuthors)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("NAuthorFK");

                entity.HasOne(d => d.Movie)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.MovieId)
                    .HasConstraintName("NMovieFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NotificationUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("NUserFK");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "EmailUIndex")
                    .IsUnique();

                entity.HasIndex(e => e.Username, "UsernameUIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Avatar)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserCommentReport>(entity =>
            {
                entity.HasIndex(e => e.ReportId, "ReportIDFK");

                entity.HasIndex(e => e.UserId, "UserFKCR1");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.UserCommentReports)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ReportIDFK");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCommentReports)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserFKCR1");
            });

            modelBuilder.Entity<UserWarning>(entity =>
            {
                entity.HasIndex(e => e.UserId, "UserFKUW");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserWarnings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserFKUW");
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasColumnType("enum('Writer','Director','Actor','')");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
