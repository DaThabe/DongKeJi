using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DongKeJi.Work.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Json = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Area = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Describe = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Describe = table.Column<string>(type: "TEXT", nullable: true),
                    SubscribeTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    State = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    StaffId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consume_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkStaffCustomer",
                columns: table => new
                {
                    CustomersId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkStaffCustomer", x => new { x.CustomersId, x.StaffsId });
                    table.ForeignKey(
                        name: "FK_LinkStaffCustomer_Customer_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkStaffCustomer_Staff_StaffsId",
                        column: x => x.StaffsId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkStaffPosition",
                columns: table => new
                {
                    PositionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkStaffPosition", x => new { x.PositionsId, x.StaffsId });
                    table.ForeignKey(
                        name: "FK_LinkStaffPosition_Position_PositionsId",
                        column: x => x.PositionsId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkStaffPosition_Staff_StaffsId",
                        column: x => x.StaffsId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkStaffOrder",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StaffsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkStaffOrder", x => new { x.OrdersId, x.StaffsId });
                    table.ForeignKey(
                        name: "FK_LinkStaffOrder_Order_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkStaffOrder_Staff_StaffsId",
                        column: x => x.StaffsId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderCounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalCounts = table.Column<double>(type: "REAL", nullable: false),
                    InitCounts = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCounting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCounting_Order_Id",
                        column: x => x.Id,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderMixing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalCounts = table.Column<double>(type: "REAL", nullable: false),
                    InitCounts = table.Column<double>(type: "REAL", nullable: false),
                    TotalDays = table.Column<double>(type: "REAL", nullable: false),
                    InitDays = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMixing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMixing_Order_Id",
                        column: x => x.Id,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTiming",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalDays = table.Column<double>(type: "REAL", nullable: false),
                    InitDays = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTiming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTiming_Order_Id",
                        column: x => x.Id,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumeCounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    consume_counts = table.Column<double>(type: "REAL", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumeCounting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumeCounting_Consume_Id",
                        column: x => x.Id,
                        principalTable: "Consume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumeCounting_OrderCounting_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderCounting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumeMixing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsumeDays = table.Column<double>(type: "REAL", nullable: false),
                    ConsumeCounts = table.Column<double>(type: "REAL", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumeMixing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumeMixing_Consume_Id",
                        column: x => x.Id,
                        principalTable: "Consume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumeMixing_OrderMixing_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderMixing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumeTiming",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConsumeDays = table.Column<double>(type: "REAL", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumeTiming", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumeTiming_Consume_Id",
                        column: x => x.Id,
                        principalTable: "Consume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumeTiming_OrderTiming_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderTiming",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consume_StaffId",
                table: "Consume",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumeCounting_OrderId",
                table: "ConsumeCounting",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumeMixing_OrderId",
                table: "ConsumeMixing",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumeTiming_OrderId",
                table: "ConsumeTiming",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkStaffCustomer_StaffsId",
                table: "LinkStaffCustomer",
                column: "StaffsId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkStaffOrder_StaffsId",
                table: "LinkStaffOrder",
                column: "StaffsId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkStaffPosition_StaffsId",
                table: "LinkStaffPosition",
                column: "StaffsId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "ConsumeCounting");

            migrationBuilder.DropTable(
                name: "ConsumeMixing");

            migrationBuilder.DropTable(
                name: "ConsumeTiming");

            migrationBuilder.DropTable(
                name: "LinkStaffCustomer");

            migrationBuilder.DropTable(
                name: "LinkStaffOrder");

            migrationBuilder.DropTable(
                name: "LinkStaffPosition");

            migrationBuilder.DropTable(
                name: "OrderCounting");

            migrationBuilder.DropTable(
                name: "OrderMixing");

            migrationBuilder.DropTable(
                name: "Consume");

            migrationBuilder.DropTable(
                name: "OrderTiming");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
