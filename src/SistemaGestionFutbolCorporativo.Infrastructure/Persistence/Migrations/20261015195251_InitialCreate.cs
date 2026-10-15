using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaGestionFutbolCorporativo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "idempotency_records",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Path = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_records", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "jugadores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    dorsal = table.Column<int>(type: "integer", nullable: false),
                    posicion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jugadores", x => x.id);
                    table.ForeignKey(
                        name: "FK_jugadores_equipos_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "partidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipo_local_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipo_visitante_id = table.Column<Guid>(type: "uuid", nullable: false),
                    jornada = table.Column<int>(type: "integer", nullable: false),
                    fecha_hora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    goles_local = table.Column<int>(type: "integer", nullable: true),
                    goles_visitante = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partidos", x => x.id);
                    table.ForeignKey(
                        name: "FK_partidos_equipos_equipo_local_id",
                        column: x => x.equipo_local_id,
                        principalTable: "equipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_partidos_equipos_equipo_visitante_id",
                        column: x => x.equipo_visitante_id,
                        principalTable: "equipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    partido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    jugador_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minuto = table.Column<int>(type: "integer", nullable: true),
                    es_autogol = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goles", x => x.id);
                    table.ForeignKey(
                        name: "FK_goles_equipos_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_goles_jugadores_jugador_id",
                        column: x => x.jugador_id,
                        principalTable: "jugadores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_goles_partidos_partido_id",
                        column: x => x.partido_id,
                        principalTable: "partidos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_equipos_nombre",
                table: "equipos",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_goles_equipo_id",
                table: "goles",
                column: "equipo_id");

            migrationBuilder.CreateIndex(
                name: "IX_goles_jugador_id",
                table: "goles",
                column: "jugador_id");

            migrationBuilder.CreateIndex(
                name: "IX_goles_partido_id",
                table: "goles",
                column: "partido_id");

            migrationBuilder.CreateIndex(
                name: "IX_jugadores_equipo_id_dorsal",
                table: "jugadores",
                columns: new[] { "equipo_id", "dorsal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_partidos_equipo_local_id",
                table: "partidos",
                column: "equipo_local_id");

            migrationBuilder.CreateIndex(
                name: "IX_partidos_equipo_visitante_id",
                table: "partidos",
                column: "equipo_visitante_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "goles");

            migrationBuilder.DropTable(
                name: "idempotency_records");

            migrationBuilder.DropTable(
                name: "jugadores");

            migrationBuilder.DropTable(
                name: "partidos");

            migrationBuilder.DropTable(
                name: "equipos");
        }
    }
}
