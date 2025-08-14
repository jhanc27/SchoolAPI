using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Estudiante ===
            modelBuilder.Entity<Estudiante>(entity =>
            {
                entity.ToTable("Estudiantes");
                entity.HasKey(e => e.EstudianteID);
                entity.HasIndex(e => e.Matricula).IsUnique();
                entity.Property(e => e.Activo).HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Profesor ===
            modelBuilder.Entity<Profesor>(entity =>
            {
                entity.ToTable("Profesores");
                entity.HasKey(e => e.ProfesorID);
                entity.HasIndex(e => e.Correo).IsUnique();
                entity.Property(e => e.Activo).HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Periodo ===
            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.ToTable("Periodos");
                entity.HasKey(e => e.PeriodoID);
                entity.Property(e => e.FechaInicio).HasColumnType("date");
                entity.Property(e => e.FechaFin).HasColumnType("date");
                entity.Property(e => e.Activo).HasDefaultValue(true);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Materia ===
            modelBuilder.Entity<Materia>(entity =>
            {
                entity.ToTable("Materias");
                entity.HasKey(e => e.MateriaID);
                entity.Property(e => e.Activo).HasDefaultValue(true);

                entity.HasOne(m => m.Profesor)
                      .WithMany(p => p.Materias)
                      .HasForeignKey(m => m.ProfesorID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Inscripción ===
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.ToTable("Inscripciones");
                entity.HasKey(e => e.InscripcionID);

                entity.HasIndex(i => new { i.EstudianteID, i.MateriaID, i.PeriodoID })
                      .IsUnique()
                      .HasDatabaseName("IX_Inscripcion_Estudiante_Materia_Periodo");

                entity.HasIndex(i => new { i.EstudianteID, i.PeriodoID })
                      .HasDatabaseName("IX_Inscripcion_Estudiante_Periodo");

                entity.HasIndex(i => new { i.PeriodoID, i.MateriaID })
                      .HasDatabaseName("IX_Inscripcion_Periodo_Materia");

                entity.HasOne(i => i.Estudiante)
                      .WithMany(e => e.Inscripciones)
                      .HasForeignKey(i => i.EstudianteID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Materia)
                      .WithMany(m => m.Inscripciones)
                      .HasForeignKey(i => i.MateriaID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Periodo)
                      .WithMany(p => p.Inscripciones)
                      .HasForeignKey(i => i.PeriodoID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Calificación ===
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.ToTable("Calificaciones", t =>
                {
                    t.HasCheckConstraint("CK_Calificacion_Literal", "Literal IN ('A', 'B', 'C', 'F')");
                    t.HasCheckConstraint("CK_Calificacion_Nota", "Nota BETWEEN 0 AND 100");
                });

                entity.HasKey(e => e.CalificacionID);
                entity.Property(e => e.Nota).HasColumnType("decimal(5,2)");

                entity.Property(e => e.Literal)
                      .HasConversion<string>()
                      .HasMaxLength(1);

                entity.HasOne(c => c.Inscripcion)
                      .WithMany(i => i.Calificaciones)
                      .HasForeignKey(c => c.InscripcionID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(c => c.InscripcionID)
                      .HasDatabaseName("IX_Calificacion_InscripcionID");

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Asistencia ===
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.ToTable("Asistencias");
                entity.HasKey(e => e.AsistenciaID);
                entity.Property(e => e.Fecha).HasColumnType("date");

                entity.HasOne(a => a.Inscripcion)
                      .WithMany(i => i.Asistencias)
                      .HasForeignKey(a => a.InscripcionID)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índice compuesto InscripcionID + Fecha
                entity.HasIndex(a => new { a.InscripcionID, a.Fecha })
                      .HasDatabaseName("IX_Asistencia_Inscripcion_Fecha");

                entity.Property(e => e.FechaCreacion)
                      .HasDefaultValueSql("GETUTCDATE()");
            });
        }

        // ==== Manejo automático de FechaCreacion robusto ====
        public override int SaveChanges()
        {
            SetFechaCreacion();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetFechaCreacion();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetFechaCreacion()
        {
            var entries = ChangeTracker.Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.Property(p => p.FechaCreacion).CurrentValue == default)
                {
                    entry.Property(p => p.FechaCreacion).CurrentValue = DateTime.UtcNow;
                }
            }
        }

    }
}
