using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Migrations;

/// <inheritdoc />
public partial class AddSnakeCaseColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_activities_clients_ClientId",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_activities_deals_DealId",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_activities_users_UserId",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_deal_items_deals_DealId",
            table: "deal_items");

        migrationBuilder.DropForeignKey(
            name: "FK_deal_items_products_ProductId",
            table: "deal_items");

        migrationBuilder.DropForeignKey(
            name: "FK_deals_clients_ClientId",
            table: "deals");

        migrationBuilder.DropForeignKey(
            name: "FK_deals_users_UserId",
            table: "deals");

        migrationBuilder.DropForeignKey(
            name: "FK_users_roles_RoleId",
            table: "users");

        migrationBuilder.RenameColumn(
            name: "Status",
            table: "users",
            newName: "status");

        migrationBuilder.RenameColumn(
            name: "Phone",
            table: "users",
            newName: "phone");

        migrationBuilder.RenameColumn(
            name: "Email",
            table: "users",
            newName: "email");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "users",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "users",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "RoleId",
            table: "users",
            newName: "role_id");

        migrationBuilder.RenameColumn(
            name: "LastName",
            table: "users",
            newName: "last_name");

        migrationBuilder.RenameColumn(
            name: "FirstName",
            table: "users",
            newName: "first_name");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "users",
            newName: "created_at_utc");

        migrationBuilder.RenameIndex(
            name: "IX_users_Email",
            table: "users",
            newName: "IX_users_email");

        migrationBuilder.RenameIndex(
            name: "IX_users_RoleId",
            table: "users",
            newName: "IX_users_role_id");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "task_items",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "task_items",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "task_items",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "sales_forecasts",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "sales_forecasts",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "sales_forecasts",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "roles",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "roles",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "roles",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "recommendations",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "recommendations",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "recommendations",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "Size",
            table: "products",
            newName: "size");

        migrationBuilder.RenameColumn(
            name: "Name",
            table: "products",
            newName: "name");

        migrationBuilder.RenameColumn(
            name: "Color",
            table: "products",
            newName: "color");

        migrationBuilder.RenameColumn(
            name: "Category",
            table: "products",
            newName: "category");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "products",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "products",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "IsActive",
            table: "products",
            newName: "is_active");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "products",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "BasePrice",
            table: "products",
            newName: "base_price");

        migrationBuilder.RenameIndex(
            name: "IX_products_Name_Category_Color_Size",
            table: "products",
            newName: "IX_products_name_category_color_size");

        migrationBuilder.RenameColumn(
            name: "Season",
            table: "deals",
            newName: "season");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "deals",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "deals",
            newName: "user_id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "deals",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "ShippingType",
            table: "deals",
            newName: "shipping_type");

        migrationBuilder.RenameColumn(
            name: "ReviewRating",
            table: "deals",
            newName: "review_rating");

        migrationBuilder.RenameColumn(
            name: "PurchaseDateUtc",
            table: "deals",
            newName: "purchase_date_utc");

        migrationBuilder.RenameColumn(
            name: "PurchaseAmount",
            table: "deals",
            newName: "purchase_amount");

        migrationBuilder.RenameColumn(
            name: "PromoCodeUsed",
            table: "deals",
            newName: "promo_code_used");

        migrationBuilder.RenameColumn(
            name: "PaymentMethod",
            table: "deals",
            newName: "payment_method");

        migrationBuilder.RenameColumn(
            name: "DiscountApplied",
            table: "deals",
            newName: "discount_applied");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "deals",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "ClientId",
            table: "deals",
            newName: "client_id");

        migrationBuilder.RenameIndex(
            name: "IX_deals_UserId",
            table: "deals",
            newName: "IX_deals_user_id");

        migrationBuilder.RenameIndex(
            name: "IX_deals_PurchaseDateUtc",
            table: "deals",
            newName: "IX_deals_purchase_date_utc");

        migrationBuilder.RenameIndex(
            name: "IX_deals_ClientId",
            table: "deals",
            newName: "IX_deals_client_id");

        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "deal_items",
            newName: "quantity");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "deal_items",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "deal_items",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "UnitPrice",
            table: "deal_items",
            newName: "unit_price");

        migrationBuilder.RenameColumn(
            name: "TotalPrice",
            table: "deal_items",
            newName: "total_price");

        migrationBuilder.RenameColumn(
            name: "ProductId",
            table: "deal_items",
            newName: "product_id");

        migrationBuilder.RenameColumn(
            name: "DealId",
            table: "deal_items",
            newName: "deal_id");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "deal_items",
            newName: "created_at_utc");

        migrationBuilder.RenameIndex(
            name: "IX_deal_items_ProductId",
            table: "deal_items",
            newName: "IX_deal_items_product_id");

        migrationBuilder.RenameIndex(
            name: "IX_deal_items_DealId_ProductId",
            table: "deal_items",
            newName: "IX_deal_items_deal_id_product_id");

        migrationBuilder.RenameColumn(
            name: "Name",
            table: "clients",
            newName: "name");

        migrationBuilder.RenameColumn(
            name: "Location",
            table: "clients",
            newName: "location");

        migrationBuilder.RenameColumn(
            name: "Gender",
            table: "clients",
            newName: "gender");

        migrationBuilder.RenameColumn(
            name: "Age",
            table: "clients",
            newName: "age");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "clients",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "clients",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "PreviousPurchases",
            table: "clients",
            newName: "previous_purchases");

        migrationBuilder.RenameColumn(
            name: "FrequencyOfPurchases",
            table: "clients",
            newName: "frequency_of_purchases");

        migrationBuilder.RenameColumn(
            name: "ExternalId",
            table: "clients",
            newName: "external_id");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "clients",
            newName: "created_at_utc");

        migrationBuilder.RenameIndex(
            name: "IX_clients_ExternalId",
            table: "clients",
            newName: "IX_clients_external_id");

        migrationBuilder.RenameColumn(
            name: "Type",
            table: "activities",
            newName: "type");

        migrationBuilder.RenameColumn(
            name: "Subject",
            table: "activities",
            newName: "subject");

        migrationBuilder.RenameColumn(
            name: "Description",
            table: "activities",
            newName: "description");

        migrationBuilder.RenameColumn(
            name: "Id",
            table: "activities",
            newName: "id");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "activities",
            newName: "user_id");

        migrationBuilder.RenameColumn(
            name: "UpdatedAtUtc",
            table: "activities",
            newName: "updated_at_utc");

        migrationBuilder.RenameColumn(
            name: "DealId",
            table: "activities",
            newName: "deal_id");

        migrationBuilder.RenameColumn(
            name: "CreatedAtUtc",
            table: "activities",
            newName: "created_at_utc");

        migrationBuilder.RenameColumn(
            name: "ClientId",
            table: "activities",
            newName: "client_id");

        migrationBuilder.RenameColumn(
            name: "ActivityDateUtc",
            table: "activities",
            newName: "activity_date_utc");

        migrationBuilder.RenameIndex(
            name: "IX_activities_UserId",
            table: "activities",
            newName: "IX_activities_user_id");

        migrationBuilder.RenameIndex(
            name: "IX_activities_DealId",
            table: "activities",
            newName: "IX_activities_deal_id");

        migrationBuilder.RenameIndex(
            name: "IX_activities_ClientId",
            table: "activities",
            newName: "IX_activities_client_id");

        migrationBuilder.AddForeignKey(
            name: "FK_activities_clients_client_id",
            table: "activities",
            column: "client_id",
            principalTable: "clients",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_activities_deals_deal_id",
            table: "activities",
            column: "deal_id",
            principalTable: "deals",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_activities_users_user_id",
            table: "activities",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_deal_items_deals_deal_id",
            table: "deal_items",
            column: "deal_id",
            principalTable: "deals",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_deal_items_products_product_id",
            table: "deal_items",
            column: "product_id",
            principalTable: "products",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_deals_clients_client_id",
            table: "deals",
            column: "client_id",
            principalTable: "clients",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_deals_users_user_id",
            table: "deals",
            column: "user_id",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_users_roles_role_id",
            table: "users",
            column: "role_id",
            principalTable: "roles",
            principalColumn: "id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_activities_clients_client_id",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_activities_deals_deal_id",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_activities_users_user_id",
            table: "activities");

        migrationBuilder.DropForeignKey(
            name: "FK_deal_items_deals_deal_id",
            table: "deal_items");

        migrationBuilder.DropForeignKey(
            name: "FK_deal_items_products_product_id",
            table: "deal_items");

        migrationBuilder.DropForeignKey(
            name: "FK_deals_clients_client_id",
            table: "deals");

        migrationBuilder.DropForeignKey(
            name: "FK_deals_users_user_id",
            table: "deals");

        migrationBuilder.DropForeignKey(
            name: "FK_users_roles_role_id",
            table: "users");

        migrationBuilder.RenameColumn(
            name: "status",
            table: "users",
            newName: "Status");

        migrationBuilder.RenameColumn(
            name: "phone",
            table: "users",
            newName: "Phone");

        migrationBuilder.RenameColumn(
            name: "email",
            table: "users",
            newName: "Email");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "users",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "users",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "role_id",
            table: "users",
            newName: "RoleId");

        migrationBuilder.RenameColumn(
            name: "last_name",
            table: "users",
            newName: "LastName");

        migrationBuilder.RenameColumn(
            name: "first_name",
            table: "users",
            newName: "FirstName");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "users",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameIndex(
            name: "IX_users_email",
            table: "users",
            newName: "IX_users_Email");

        migrationBuilder.RenameIndex(
            name: "IX_users_role_id",
            table: "users",
            newName: "IX_users_RoleId");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "task_items",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "task_items",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "task_items",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "sales_forecasts",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "sales_forecasts",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "sales_forecasts",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "roles",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "roles",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "roles",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "recommendations",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "recommendations",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "recommendations",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "size",
            table: "products",
            newName: "Size");

        migrationBuilder.RenameColumn(
            name: "name",
            table: "products",
            newName: "Name");

        migrationBuilder.RenameColumn(
            name: "color",
            table: "products",
            newName: "Color");

        migrationBuilder.RenameColumn(
            name: "category",
            table: "products",
            newName: "Category");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "products",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "products",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "is_active",
            table: "products",
            newName: "IsActive");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "products",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "base_price",
            table: "products",
            newName: "BasePrice");

        migrationBuilder.RenameIndex(
            name: "IX_products_name_category_color_size",
            table: "products",
            newName: "IX_products_Name_Category_Color_Size");

        migrationBuilder.RenameColumn(
            name: "season",
            table: "deals",
            newName: "Season");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "deals",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "user_id",
            table: "deals",
            newName: "UserId");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "deals",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "shipping_type",
            table: "deals",
            newName: "ShippingType");

        migrationBuilder.RenameColumn(
            name: "review_rating",
            table: "deals",
            newName: "ReviewRating");

        migrationBuilder.RenameColumn(
            name: "purchase_date_utc",
            table: "deals",
            newName: "PurchaseDateUtc");

        migrationBuilder.RenameColumn(
            name: "purchase_amount",
            table: "deals",
            newName: "PurchaseAmount");

        migrationBuilder.RenameColumn(
            name: "promo_code_used",
            table: "deals",
            newName: "PromoCodeUsed");

        migrationBuilder.RenameColumn(
            name: "payment_method",
            table: "deals",
            newName: "PaymentMethod");

        migrationBuilder.RenameColumn(
            name: "discount_applied",
            table: "deals",
            newName: "DiscountApplied");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "deals",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "client_id",
            table: "deals",
            newName: "ClientId");

        migrationBuilder.RenameIndex(
            name: "IX_deals_user_id",
            table: "deals",
            newName: "IX_deals_UserId");

        migrationBuilder.RenameIndex(
            name: "IX_deals_purchase_date_utc",
            table: "deals",
            newName: "IX_deals_PurchaseDateUtc");

        migrationBuilder.RenameIndex(
            name: "IX_deals_client_id",
            table: "deals",
            newName: "IX_deals_ClientId");

        migrationBuilder.RenameColumn(
            name: "quantity",
            table: "deal_items",
            newName: "Quantity");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "deal_items",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "deal_items",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "unit_price",
            table: "deal_items",
            newName: "UnitPrice");

        migrationBuilder.RenameColumn(
            name: "total_price",
            table: "deal_items",
            newName: "TotalPrice");

        migrationBuilder.RenameColumn(
            name: "product_id",
            table: "deal_items",
            newName: "ProductId");

        migrationBuilder.RenameColumn(
            name: "deal_id",
            table: "deal_items",
            newName: "DealId");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "deal_items",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameIndex(
            name: "IX_deal_items_product_id",
            table: "deal_items",
            newName: "IX_deal_items_ProductId");

        migrationBuilder.RenameIndex(
            name: "IX_deal_items_deal_id_product_id",
            table: "deal_items",
            newName: "IX_deal_items_DealId_ProductId");

        migrationBuilder.RenameColumn(
            name: "name",
            table: "clients",
            newName: "Name");

        migrationBuilder.RenameColumn(
            name: "location",
            table: "clients",
            newName: "Location");

        migrationBuilder.RenameColumn(
            name: "gender",
            table: "clients",
            newName: "Gender");

        migrationBuilder.RenameColumn(
            name: "age",
            table: "clients",
            newName: "Age");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "clients",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "clients",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "previous_purchases",
            table: "clients",
            newName: "PreviousPurchases");

        migrationBuilder.RenameColumn(
            name: "frequency_of_purchases",
            table: "clients",
            newName: "FrequencyOfPurchases");

        migrationBuilder.RenameColumn(
            name: "external_id",
            table: "clients",
            newName: "ExternalId");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "clients",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameIndex(
            name: "IX_clients_external_id",
            table: "clients",
            newName: "IX_clients_ExternalId");

        migrationBuilder.RenameColumn(
            name: "type",
            table: "activities",
            newName: "Type");

        migrationBuilder.RenameColumn(
            name: "subject",
            table: "activities",
            newName: "Subject");

        migrationBuilder.RenameColumn(
            name: "description",
            table: "activities",
            newName: "Description");

        migrationBuilder.RenameColumn(
            name: "id",
            table: "activities",
            newName: "Id");

        migrationBuilder.RenameColumn(
            name: "user_id",
            table: "activities",
            newName: "UserId");

        migrationBuilder.RenameColumn(
            name: "updated_at_utc",
            table: "activities",
            newName: "UpdatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "deal_id",
            table: "activities",
            newName: "DealId");

        migrationBuilder.RenameColumn(
            name: "created_at_utc",
            table: "activities",
            newName: "CreatedAtUtc");

        migrationBuilder.RenameColumn(
            name: "client_id",
            table: "activities",
            newName: "ClientId");

        migrationBuilder.RenameColumn(
            name: "activity_date_utc",
            table: "activities",
            newName: "ActivityDateUtc");

        migrationBuilder.RenameIndex(
            name: "IX_activities_user_id",
            table: "activities",
            newName: "IX_activities_UserId");

        migrationBuilder.RenameIndex(
            name: "IX_activities_deal_id",
            table: "activities",
            newName: "IX_activities_DealId");

        migrationBuilder.RenameIndex(
            name: "IX_activities_client_id",
            table: "activities",
            newName: "IX_activities_ClientId");

        migrationBuilder.AddForeignKey(
            name: "FK_activities_clients_ClientId",
            table: "activities",
            column: "ClientId",
            principalTable: "clients",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_activities_deals_DealId",
            table: "activities",
            column: "DealId",
            principalTable: "deals",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_activities_users_UserId",
            table: "activities",
            column: "UserId",
            principalTable: "users",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_deal_items_deals_DealId",
            table: "deal_items",
            column: "DealId",
            principalTable: "deals",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_deal_items_products_ProductId",
            table: "deal_items",
            column: "ProductId",
            principalTable: "products",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_deals_clients_ClientId",
            table: "deals",
            column: "ClientId",
            principalTable: "clients",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_deals_users_UserId",
            table: "deals",
            column: "UserId",
            principalTable: "users",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.AddForeignKey(
            name: "FK_users_roles_RoleId",
            table: "users",
            column: "RoleId",
            principalTable: "roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
