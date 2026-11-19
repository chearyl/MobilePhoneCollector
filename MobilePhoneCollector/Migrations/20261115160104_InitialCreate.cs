using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MobilePhoneCollector.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    hash_code = table.Column<string>(type: "TEXT", nullable: false),
                    data = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_data", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "phone",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    model = table.Column<string>(type: "TEXT", nullable: false),
                    brand = table.Column<string>(type: "TEXT", nullable: false),
                    announced = table.Column<string>(type: "TEXT", nullable: false),
                    info_url = table.Column<string>(type: "TEXT", nullable: false),
                    notes = table.Column<string>(type: "TEXT", nullable: false),
                    created_date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    origin = table.Column<string>(type: "TEXT", nullable: false),
                    logo_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                    table.ForeignKey(
                        name: "FK_brands_file_data_logo_id",
                        column: x => x.logo_id,
                        principalTable: "file_data",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "phone_image",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    phone_id = table.Column<int>(type: "INTEGER", nullable: false),
                    data_id = table.Column<int>(type: "INTEGER", nullable: false),
                    width = table.Column<int>(type: "INTEGER", nullable: false),
                    height = table.Column<int>(type: "INTEGER", nullable: false),
                    file_type = table.Column<string>(type: "TEXT", nullable: false),
                    file_name = table.Column<string>(type: "TEXT", nullable: false),
                    is_cover = table.Column<bool>(type: "INTEGER", nullable: false),
                    created_date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_image", x => x.id);
                    table.ForeignKey(
                        name: "FK_phone_image_file_data_data_id",
                        column: x => x.data_id,
                        principalTable: "file_data",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_phone_image_phone_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "phone_review",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    phone_id = table.Column<int>(type: "INTEGER", nullable: false),
                    data_id = table.Column<int>(type: "INTEGER", nullable: false),
                    file_type = table.Column<string>(type: "TEXT", nullable: false),
                    file_name = table.Column<string>(type: "TEXT", nullable: false),
                    created_date = table.Column<string>(type: "TEXT", nullable: false),
                    file_size = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_review", x => x.id);
                    table.ForeignKey(
                        name: "FK_phone_review_file_data_data_id",
                        column: x => x.data_id,
                        principalTable: "file_data",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_phone_review_phone_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "specs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    phone_id = table.Column<int>(type: "INTEGER", nullable: false),
                    category = table.Column<string>(type: "TEXT", nullable: false),
                    key = table.Column<string>(type: "TEXT", nullable: false),
                    value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specs", x => x.id);
                    table.ForeignKey(
                        name: "FK_specs_phone_phone_id",
                        column: x => x.phone_id,
                        principalTable: "phone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_brands_logo_id",
                table: "brands",
                column: "logo_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_image_data_id",
                table: "phone_image",
                column: "data_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_image_phone_id",
                table: "phone_image",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_review_data_id",
                table: "phone_review",
                column: "data_id");

            migrationBuilder.CreateIndex(
                name: "IX_phone_review_phone_id",
                table: "phone_review",
                column: "phone_id");

            migrationBuilder.CreateIndex(
                name: "IX_specs_phone_id",
                table: "specs",
                column: "phone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brands");

            migrationBuilder.DropTable(
                name: "phone_image");

            migrationBuilder.DropTable(
                name: "phone_review");

            migrationBuilder.DropTable(
                name: "specs");

            migrationBuilder.DropTable(
                name: "file_data");

            migrationBuilder.DropTable(
                name: "phone");
        }
    }
}
