﻿// <auto-generated />
using System;
using API.ML.BOBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20241204140659_AddAddressForCustomer")]
    partial class AddAddressForCustomer
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("MySql:CharSet", "utf8mb4")
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("API.ML.BO.ChatbotConversation", b =>
                {
                    b.Property<Guid>("ConversationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("CustomerID")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsHelpful")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("ConversationID");

                    b.HasIndex("CustomerID");

                    b.ToTable("ChatbotConversation");
                });

            modelBuilder.Entity("API.ML.BO.ChatbotConversationDetail", b =>
                {
                    b.Property<Guid>("ConversationDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ConversationID")
                        .HasColumnType("char(36)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Sender")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ConversationDetailID");

                    b.HasIndex("ConversationID");

                    b.ToTable("ChatbotConversationDetail");
                });

            modelBuilder.Entity("API.ML.BO.Customer", b =>
                {
                    b.Property<Guid>("CustomerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("LoyaltyPoint")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Preferences")
                        .HasColumnType("longtext");

                    b.HasKey("CustomerID");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("API.ML.BO.Employee", b =>
                {
                    b.Property<Guid>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("EmployeeCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Schedule")
                        .HasColumnType("longtext");

                    b.HasKey("EmployeeID");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("API.ML.BO.InventoryItem", b =>
                {
                    b.Property<Guid>("InventoryItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ReorderLevel")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("InventoryItemID");

                    b.ToTable("InventoryItem");
                });

            modelBuilder.Entity("API.ML.BO.MenuItem", b =>
                {
                    b.Property<Guid>("MenuItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("OutOfStock")
                        .HasColumnType("tinyint(1)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MenuItemID");

                    b.ToTable("MenuItem");
                });

            modelBuilder.Entity("API.ML.BO.Order", b =>
                {
                    b.Property<Guid>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("CustomerID")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("OrderName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SpecialRequest")
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderID");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("API.ML.BO.OrderDetail", b =>
                {
                    b.Property<Guid>("OrderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("MenuItemID")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Note")
                        .HasColumnType("longtext");

                    b.Property<Guid>("OrderID")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("OrderDetailID");

                    b.HasIndex("MenuItemID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("API.ML.BO.Reservation", b =>
                {
                    b.Property<Guid>("ReservationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("CustomerID")
                        .HasColumnType("char(36)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CustomerPhoneNumber")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("GuestCount")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ReservationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ReservationInfo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ReservationID");

                    b.HasIndex("CustomerID");

                    b.ToTable("Reservation");
                });

            modelBuilder.Entity("API.ML.BO.UserLogin", b =>
                {
                    b.Property<Guid>("UserLoginID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("CustomerID")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("EmployeeID")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("UserLoginID");

                    b.HasIndex("CustomerID")
                        .IsUnique();

                    b.HasIndex("EmployeeID")
                        .IsUnique();

                    b.ToTable("UserLogin");
                });

            modelBuilder.Entity("API.ML.BO.ChatbotConversation", b =>
                {
                    b.HasOne("API.ML.BO.Customer", "Customer")
                        .WithMany("ChatbotConversations")
                        .HasForeignKey("CustomerID");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("API.ML.BO.ChatbotConversationDetail", b =>
                {
                    b.HasOne("API.ML.BO.ChatbotConversation", "ChatbotConversation")
                        .WithMany("ChatbotConversationDetails")
                        .HasForeignKey("ConversationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChatbotConversation");
                });

            modelBuilder.Entity("API.ML.BO.OrderDetail", b =>
                {
                    b.HasOne("API.ML.BO.MenuItem", "MenuItem")
                        .WithMany("OrderDetails")
                        .HasForeignKey("MenuItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.ML.BO.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MenuItem");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("API.ML.BO.Reservation", b =>
                {
                    b.HasOne("API.ML.BO.Customer", "Customer")
                        .WithMany("Reservations")
                        .HasForeignKey("CustomerID");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("API.ML.BO.UserLogin", b =>
                {
                    b.HasOne("API.ML.BO.Customer", "Customer")
                        .WithOne("UserLogin")
                        .HasForeignKey("API.ML.BO.UserLogin", "CustomerID");

                    b.HasOne("API.ML.BO.Employee", "Employee")
                        .WithOne("UserLogin")
                        .HasForeignKey("API.ML.BO.UserLogin", "EmployeeID");

                    b.Navigation("Customer");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("API.ML.BO.ChatbotConversation", b =>
                {
                    b.Navigation("ChatbotConversationDetails");
                });

            modelBuilder.Entity("API.ML.BO.Customer", b =>
                {
                    b.Navigation("ChatbotConversations");

                    b.Navigation("Reservations");

                    b.Navigation("UserLogin");
                });

            modelBuilder.Entity("API.ML.BO.Employee", b =>
                {
                    b.Navigation("UserLogin");
                });

            modelBuilder.Entity("API.ML.BO.MenuItem", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("API.ML.BO.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
