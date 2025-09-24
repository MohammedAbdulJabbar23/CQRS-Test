using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Infrastructure.Persistence.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("TodoItems");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(450);

        builder.Property(t => t.DeletedBy)
            .HasMaxLength(450);

        builder.HasOne(t => t.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.IsDeleted);
    }
}