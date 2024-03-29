﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SwappyBot.EntityFramework;

#nullable disable

namespace SwappyBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20240127204038_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SwappyBot.EntityFramework.SwapState", b =>
                {
                    b.Property<string>("StateId")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<double?>("Amount")
                        .HasColumnType("double");

                    b.Property<string>("AssetFrom")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("AssetTo")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<double?>("QuoteChainflipFee")
                        .HasColumnType("double");

                    b.Property<double?>("QuoteDeposit")
                        .HasColumnType("double");

                    b.Property<double?>("QuotePlatformFee")
                        .HasColumnType("double");

                    b.Property<string>("QuoteRate")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<double?>("QuoteReceive")
                        .HasColumnType("double");

                    b.Property<DateTimeOffset?>("QuoteTime")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("ServerName")
                        .HasMaxLength(150)
                        .HasColumnType("varchar(150)");

                    b.Property<DateTimeOffset?>("SwapAccepted")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("SwapStarted")
                        .HasColumnType("datetime(6)");

                    b.Property<ulong>("UserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("UserName")
                        .HasMaxLength(40)
                        .HasColumnType("varchar(40)");

                    b.HasKey("StateId");

                    b.HasIndex("QuoteTime");

                    b.HasIndex("ServerId");

                    b.HasIndex("SwapAccepted");

                    b.HasIndex("SwapStarted");

                    b.HasIndex("UserId");

                    b.ToTable("swap_state", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
