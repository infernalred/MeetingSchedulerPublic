using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MeetingScheduler.Models
{
    public class MeetingContext : DbContext
    {
        public DbSet<Meeting> Meeting { get; set; }
        public DbSet<Room> Rooms { get; set; }
        //public DbSet<User> Users { get; set; }

        public MeetingContext(DbContextOptions<MeetingContext> options) : base(options)
        {
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<EventsUsers>()
        //        .HasKey(t => new { t.MeetingId, t.UserId });

        //    modelBuilder.Entity<EventsUsers>()
        //        .HasOne(sc => sc.Meeting)
        //        .WithMany(s => s.EventsUsers)
        //        .HasForeignKey(sc => sc.MeetingId);

        //    modelBuilder.Entity<EventsUsers>()
        //        .HasOne(sc => sc.User)
        //        .WithMany(c => c.EventsUsers)
        //        .HasForeignKey(sc => sc.UserId);

        //}
    }
}
