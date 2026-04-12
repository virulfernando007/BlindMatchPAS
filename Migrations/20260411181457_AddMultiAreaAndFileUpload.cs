using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlindMatchPAS.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiAreaAndFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ResearchAreas_ResearchAreaId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "TechnicalStack",
                table: "Projects",
                newName: "ResearchAreaIds");

            migrationBuilder.AlterColumn<int>(
                name: "ResearchAreaId",
                table: "Projects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "Projects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectType",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Projects",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "ResearchAreas",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Distributed ledgers, smart contracts, and decentralised applications.", true, "Blockchain & Web3" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Image recognition, object detection, and visual AI systems.", true, "Computer Vision" },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Text analysis, language models, and conversational AI.", true, "Natural Language Processing" },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CI/CD pipelines, infrastructure as code, and deployment automation.", true, "DevOps & Automation" },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "UX research, accessibility, and user-centred design.", true, "Human-Computer Interaction" },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Relational and NoSQL databases, query optimisation, and data modelling.", true, "Database Systems" },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Game engines, graphics programming, and interactive media.", true, "Game Development" },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "AR/VR systems, spatial computing, and immersive experiences.", true, "Augmented & Virtual Reality" },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Microcontrollers, autonomous systems, and hardware-software integration.", true, "Robotics & Embedded Systems" },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Digital health systems, medical data, and clinical decision support.", true, "Health Informatics" },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Online payment systems, digital banking, and financial technology.", true, "E-Commerce & FinTech" },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Network protocols, wireless systems, and infrastructure design.", true, "Network Engineering" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ResearchAreas_ResearchAreaId",
                table: "Projects",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ResearchAreas_ResearchAreaId",
                table: "Projects");

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ResearchAreas",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectType",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ResearchAreaIds",
                table: "Projects",
                newName: "TechnicalStack");

            migrationBuilder.AlterColumn<int>(
                name: "ResearchAreaId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ResearchAreas_ResearchAreaId",
                table: "Projects",
                column: "ResearchAreaId",
                principalTable: "ResearchAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
