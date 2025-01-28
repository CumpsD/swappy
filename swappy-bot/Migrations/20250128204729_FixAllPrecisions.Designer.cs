﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SwappyBot.EntityFramework;

#nullable disable

namespace SwappyBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20250128204729_FixAllPrecisions")]
    partial class FixAllPrecisions
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SwappyBot.EntityFramework.SwapState", b =>
                {
                    b.Property<string>("StateId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("stateid");

                    b.Property<decimal?>("Amount")
                        .HasPrecision(27, 18)
                        .HasColumnType("numeric(27,18)")
                        .HasColumnName("amount");

                    b.Property<string>("AnnouncementIds")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("announcementids");

                    b.Property<string>("AssetFrom")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("assetfrom");

                    b.Property<string>("AssetTo")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("assetto");

                    b.Property<string>("DepositAddress")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("depositaddress");

                    b.Property<string>("DepositChannel")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("depositchannel");

                    b.Property<DateTimeOffset?>("DepositGenerated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("depositgenerated");

                    b.Property<string>("DestinationAddress")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("destinationaddress");

                    b.Property<decimal?>("QuoteChainflipFee")
                        .HasPrecision(27, 18)
                        .HasColumnType("numeric(27,18)")
                        .HasColumnName("quotechainflipfee");

                    b.Property<decimal?>("QuoteDeposit")
                        .HasPrecision(27, 18)
                        .HasColumnType("numeric(27,18)")
                        .HasColumnName("quotedeposit");

                    b.Property<decimal?>("QuotePlatformFee")
                        .HasPrecision(27, 18)
                        .HasColumnType("numeric(27,18)")
                        .HasColumnName("quoteplatformfee");

                    b.Property<string>("QuoteRate")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("quoterate");

                    b.Property<decimal?>("QuoteReceive")
                        .HasPrecision(27, 18)
                        .HasColumnType("numeric(27,18)")
                        .HasColumnName("quotereceive");

                    b.Property<DateTimeOffset?>("QuoteTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("quotetime");

                    b.Property<bool?>("Replied")
                        .HasColumnType("boolean")
                        .HasColumnName("replied");

                    b.Property<DateTimeOffset?>("SwapAccepted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("swapaccepted");

                    b.Property<DateTimeOffset?>("SwapCancelled")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("swapcancelled");

                    b.Property<DateTimeOffset>("SwapStarted")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("swapstarted");

                    b.Property<string>("SwapStatus")
                        .HasMaxLength(10000)
                        .HasColumnType("character varying(10000)")
                        .HasColumnName("swapstatus");

                    b.HasKey("StateId")
                        .HasName("pk_swap_state");

                    b.HasIndex("DepositGenerated")
                        .HasDatabaseName("ix_swap_state_depositgenerated");

                    b.HasIndex("QuoteTime")
                        .HasDatabaseName("ix_swap_state_quotetime");

                    b.HasIndex("SwapAccepted")
                        .HasDatabaseName("ix_swap_state_swapaccepted");

                    b.HasIndex("SwapCancelled")
                        .HasDatabaseName("ix_swap_state_swapcancelled");

                    b.HasIndex("SwapStarted")
                        .HasDatabaseName("ix_swap_state_swapstarted");

                    b.ToTable("swap_state", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
