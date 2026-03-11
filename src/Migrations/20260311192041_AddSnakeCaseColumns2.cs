using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Migrations
{
    /// <inheritdoc />
    public partial class AddSnakeCaseColumns2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommendations_clients_ClientId",
                table: "recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_recommendations_products_ProductId",
                table: "recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_forecasts_clients_ClientId",
                table: "sales_forecasts");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_clients_ClientId",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_deals_DealId",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_users_AssignedToUserId",
                table: "task_items");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "task_items",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "task_items",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "task_items",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "DueDateUtc",
                table: "task_items",
                newName: "due_date_utc");

            migrationBuilder.RenameColumn(
                name: "DealId",
                table: "task_items",
                newName: "deal_id");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "task_items",
                newName: "client_id");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "task_items",
                newName: "assigned_to_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_DealId",
                table: "task_items",
                newName: "IX_task_items_deal_id");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_ClientId",
                table: "task_items",
                newName: "IX_task_items_client_id");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_AssignedToUserId",
                table: "task_items",
                newName: "IX_task_items_assigned_to_user_id");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "sales_forecasts",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "PredictedAmount",
                table: "sales_forecasts",
                newName: "predicted_amount");

            migrationBuilder.RenameColumn(
                name: "PeriodStartUtc",
                table: "sales_forecasts",
                newName: "period_start_utc");

            migrationBuilder.RenameColumn(
                name: "PeriodEndUtc",
                table: "sales_forecasts",
                newName: "period_end_utc");

            migrationBuilder.RenameColumn(
                name: "ModelVersion",
                table: "sales_forecasts",
                newName: "model_version");

            migrationBuilder.RenameColumn(
                name: "ForecastDateUtc",
                table: "sales_forecasts",
                newName: "forecast_date_utc");

            migrationBuilder.RenameColumn(
                name: "ConfidenceScore",
                table: "sales_forecasts",
                newName: "confidence_score");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "sales_forecasts",
                newName: "client_id");

            migrationBuilder.RenameIndex(
                name: "IX_sales_forecasts_ClientId_ForecastDateUtc",
                table: "sales_forecasts",
                newName: "IX_sales_forecasts_client_id_forecast_date_utc");

            migrationBuilder.RenameIndex(
                name: "IX_sales_forecasts_ClientId",
                table: "sales_forecasts",
                newName: "IX_sales_forecasts_client_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roles",
                newName: "description");

            migrationBuilder.RenameIndex(
                name: "IX_roles_Name",
                table: "roles",
                newName: "IX_roles_name");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "recommendations",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "recommendations",
                newName: "score");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "recommendations",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "RecommendationDateUtc",
                table: "recommendations",
                newName: "recommendation_date_utc");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "recommendations",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "recommendations",
                newName: "client_id");

            migrationBuilder.RenameIndex(
                name: "IX_recommendations_ProductId",
                table: "recommendations",
                newName: "IX_recommendations_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_recommendations_ClientId",
                table: "recommendations",
                newName: "IX_recommendations_client_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "assigned_to_user_id",
                table: "task_items",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_recommendations_clients_client_id",
                table: "recommendations",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recommendations_products_product_id",
                table: "recommendations",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_forecasts_clients_client_id",
                table: "sales_forecasts",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_clients_client_id",
                table: "task_items",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_deals_deal_id",
                table: "task_items",
                column: "deal_id",
                principalTable: "deals",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_users_assigned_to_user_id",
                table: "task_items",
                column: "assigned_to_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recommendations_clients_client_id",
                table: "recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_recommendations_products_product_id",
                table: "recommendations");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_forecasts_clients_client_id",
                table: "sales_forecasts");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_clients_client_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_deals_deal_id",
                table: "task_items");

            migrationBuilder.DropForeignKey(
                name: "FK_task_items_users_assigned_to_user_id",
                table: "task_items");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "task_items",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "task_items",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "task_items",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "due_date_utc",
                table: "task_items",
                newName: "DueDateUtc");

            migrationBuilder.RenameColumn(
                name: "deal_id",
                table: "task_items",
                newName: "DealId");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "task_items",
                newName: "ClientId");

            migrationBuilder.RenameColumn(
                name: "assigned_to_user_id",
                table: "task_items",
                newName: "AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_deal_id",
                table: "task_items",
                newName: "IX_task_items_DealId");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_client_id",
                table: "task_items",
                newName: "IX_task_items_ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_task_items_assigned_to_user_id",
                table: "task_items",
                newName: "IX_task_items_AssignedToUserId");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "sales_forecasts",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "predicted_amount",
                table: "sales_forecasts",
                newName: "PredictedAmount");

            migrationBuilder.RenameColumn(
                name: "period_start_utc",
                table: "sales_forecasts",
                newName: "PeriodStartUtc");

            migrationBuilder.RenameColumn(
                name: "period_end_utc",
                table: "sales_forecasts",
                newName: "PeriodEndUtc");

            migrationBuilder.RenameColumn(
                name: "model_version",
                table: "sales_forecasts",
                newName: "ModelVersion");

            migrationBuilder.RenameColumn(
                name: "forecast_date_utc",
                table: "sales_forecasts",
                newName: "ForecastDateUtc");

            migrationBuilder.RenameColumn(
                name: "confidence_score",
                table: "sales_forecasts",
                newName: "ConfidenceScore");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "sales_forecasts",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_sales_forecasts_client_id_forecast_date_utc",
                table: "sales_forecasts",
                newName: "IX_sales_forecasts_ClientId_ForecastDateUtc");

            migrationBuilder.RenameIndex(
                name: "IX_sales_forecasts_client_id",
                table: "sales_forecasts",
                newName: "IX_sales_forecasts_ClientId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "roles",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_roles_name",
                table: "roles",
                newName: "IX_roles_Name");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "recommendations",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "score",
                table: "recommendations",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "recommendations",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "recommendation_date_utc",
                table: "recommendations",
                newName: "RecommendationDateUtc");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "recommendations",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "recommendations",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_recommendations_product_id",
                table: "recommendations",
                newName: "IX_recommendations_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_recommendations_client_id",
                table: "recommendations",
                newName: "IX_recommendations_ClientId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedToUserId",
                table: "task_items",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_recommendations_clients_ClientId",
                table: "recommendations",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recommendations_products_ProductId",
                table: "recommendations",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_forecasts_clients_ClientId",
                table: "sales_forecasts",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_clients_ClientId",
                table: "task_items",
                column: "ClientId",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_deals_DealId",
                table: "task_items",
                column: "DealId",
                principalTable: "deals",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_task_items_users_AssignedToUserId",
                table: "task_items",
                column: "AssignedToUserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
