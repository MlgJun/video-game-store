using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameStore.Migrations
{
    /// <inheritdoc />
    public partial class IdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. НАЙДИ И УДАЛИ constraints АВТОМАТИЧЕСКИ!
            migrationBuilder.Sql(@"
        DECLARE @sql NVARCHAR(MAX) = '';
        
        -- Customers constraint
        SELECT @sql = @sql + 'ALTER TABLE [Customers] DROP CONSTRAINT [' + name + ']; '
        FROM sys.default_constraints 
        WHERE parent_object_id = OBJECT_ID('Customers') 
        AND definition LIKE '%UserSequence%';
        
        -- Sellers constraint  
        SELECT @sql = @sql + 'ALTER TABLE [Sellers] DROP CONSTRAINT [' + name + ']; '
        FROM sys.default_constraints 
        WHERE parent_object_id = OBJECT_ID('Sellers') 
        AND definition LIKE '%UserSequence%';
        
        EXEC sp_executesql @sql;
        
        -- Удали sequence
        DROP SEQUENCE IF EXISTS UserSequence;
    ");

            // 2. Измени колонки
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Sellers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR [UserSequence]");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Customers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "NEXT VALUE FOR [UserSequence]");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "UserSequence");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Sellers",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [UserSequence]",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR [UserSequence]",
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
