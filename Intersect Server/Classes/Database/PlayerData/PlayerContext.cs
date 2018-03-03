﻿using System;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Classes.Database.PlayerData
{
    public class PlayerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Friend> Character_Friends { get; set; }
        public DbSet<SpellSlot> Character_Spells { get; set; }
        public DbSet<Switch> Character_Switches { get; set; }
        public DbSet<Variable> Character_Variables { get; set; }
        public DbSet<HotbarSlot> Character_Hotbar { get; set; }
        public DbSet<Quest> Character_Quests { get; set; }
        public DbSet<Bag> Bags { get; set; }
        public DbSet<InventorySlot> Character_Items { get; set; }
        public DbSet<BankSlot> Character_Bank { get; set; }
        public DbSet<BagSlot> Bag_Items { get; set; }
        public DbSet<Mute> Mutes { get; set; }
        public DbSet<Ban> Bans { get; set; }


        public enum DbProvider
        {
            Sqlite,
            MySql,
        }

        private DbProvider mConnection = DbProvider.Sqlite;
        private string mConnectionString = @"Data Source=resources/playerdata.db";

        public PlayerContext()
        {
            
        }

        public PlayerContext(DbProvider connection,string connectionString)
        {
            mConnection = connection;
            mConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (mConnection)
            {
                case DbProvider.Sqlite:
                    optionsBuilder.UseSqlite(mConnectionString);
                    break;
                case DbProvider.MySql:
                    optionsBuilder.UseMySql(mConnectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(b => b.Characters)
                .WithOne(p => p.Account)
                .HasForeignKey(p => p.Id);

            modelBuilder.Entity<Character>().HasMany(b => b.Friends).WithOne(p => p.Owner);

            modelBuilder.Entity<Character>().HasMany(b => b.Spells).WithOne(p => p.Character);

            modelBuilder.Entity<Character>().HasMany(b => b.Items).WithOne(p => p.Character);

            modelBuilder.Entity<Character>().HasMany(b => b.Switches).WithOne(p => p.Character);
            modelBuilder.Entity<Switch>().HasIndex(p => new { p.SwitchId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Character>().HasMany(b => b.Variables).WithOne(p => p.Character);
            modelBuilder.Entity<Variable>().HasIndex(p => new { p.VariableId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Character>().HasMany(b => b.Hotbar).WithOne(p => p.Character);

            modelBuilder.Entity<Character>().HasMany(b => b.Quests).WithOne(p => p.Character);
            modelBuilder.Entity<Quest>().HasIndex(p => new { p.QuestId, p.CharacterId }).IsUnique();

            modelBuilder.Entity<Character>().HasMany(b => b.Bank).WithOne(p => p.Character);

            modelBuilder.Entity<Bag>().HasMany(b => b.Slots).WithOne(p => p.ParentBag).HasForeignKey(p => p.ParentBagId);

            modelBuilder.Entity<InventorySlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BagSlot>().HasOne(b => b.Bag);
            modelBuilder.Entity<BankSlot>().HasOne(b => b.Bag);

            modelBuilder.Entity<Ban>().HasOne(b => b.Player);
            modelBuilder.Entity<Mute>().HasOne(b => b.Player);

        }

    }
}
