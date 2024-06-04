using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Migrations
{
    /// <inheritdoc />
    public partial class mig_v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "OrderStates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessingId",
                table: "OrderStates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "OrderStates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseAddress",
                table: "OrderStates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderStates");

            migrationBuilder.DropColumn(
                name: "ProcessingId",
                table: "OrderStates");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "OrderStates");

            migrationBuilder.DropColumn(
                name: "ResponseAddress",
                table: "OrderStates");
        }
    }
}
