using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Migrations
{
    /// <inheritdoc />
    public partial class mig_v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderState",
                table: "OrderState");

            migrationBuilder.RenameTable(
                name: "OrderState",
                newName: "OrderStates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "OrderStates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStates",
                table: "OrderStates",
                column: "CorrelationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStates",
                table: "OrderStates");

            migrationBuilder.RenameTable(
                name: "OrderStates",
                newName: "OrderState");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "OrderState",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderState",
                table: "OrderState",
                column: "CorrelationId");
        }
    }
}
