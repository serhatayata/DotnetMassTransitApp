using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Migrations
{
    /// <inheritdoc />
    public partial class mig_v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderCompletionTimeoutTokenId",
                table: "OrderStates",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCompletionTimeoutTokenId",
                table: "OrderStates");
        }
    }
}
