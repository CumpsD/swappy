﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SwappyBot.EntityFramework;

#nullable disable

namespace SwappyBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20240316174507_AddMessageIds")]
    partial class AddMessageIds
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SwappyBot.EntityFramework.SwapState", b =>
                {
                    b.Property<string>("StateId")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("AnnouncementIds")
                        .HasMaxLength(1000)
                        .HasColumnType("varchar(1000)");

                    b.Property<string>("AssetFrom")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("AssetTo")
                        .HasMaxLength(5)
                        .HasColumnType("varchar(5)");

                    b.Property<string>("DepositAddress")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("DepositChannel")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTimeOffset?>("DepositGenerated")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal?>("QuoteChainflipFee")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("QuoteDeposit")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal?>("QuotePlatformFee")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("QuoteRate")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<decimal?>("QuoteReceive")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTimeOffset?>("QuoteTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool?>("Replied")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("SwapAccepted")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset?>("SwapCancelled")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTimeOffset>("SwapStarted")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SwapStatus")
                        .HasMaxLength(10000)
                        .HasColumnType("varchar(10000)");

                    b.HasKey("StateId");

                    b.HasIndex("DepositGenerated");

                    b.HasIndex("QuoteTime");

                    b.HasIndex("SwapAccepted");

                    b.HasIndex("SwapCancelled");

                    b.HasIndex("SwapStarted");

                    b.ToTable("swap_state", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
