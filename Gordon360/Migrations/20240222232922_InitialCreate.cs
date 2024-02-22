using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gordon360.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "RecIM");

        migrationBuilder.CreateSequence(
            name: "Information_Change_Request_Seq");

        migrationBuilder.CreateTable(
            name: "ACT_INFO",
            columns: table => new
            {
                ACT_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                ACT_DESC = table.Column<string>(type: "char(45)", unicode: false, fixedLength: true, maxLength: 45, nullable: false),
                ACT_BLURB = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                ACT_URL = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                ACT_IMG_PATH = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                ACT_TYPE = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                ACT_TYPE_DESC = table.Column<string>(type: "char(60)", unicode: false, fixedLength: true, maxLength: 60, nullable: true),
                PRIVACY = table.Column<bool>(type: "bit", nullable: true),
                ACT_JOIN_INFO = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Activity_Info", x => x.ACT_CDE);
            });

        migrationBuilder.CreateTable(
            name: "ActivityStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ActivityStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "ActivityType",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ActivityType", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "Affiliation",
            schema: "RecIM",
            columns: table => new
            {
                Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Logo = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Affiliations", x => x.Name);
            });

        migrationBuilder.CreateTable(
            name: "Clifton_Strengths",
            columns: table => new
            {
                ID_NUM = table.Column<int>(type: "int", nullable: false),
                ACCESS_CODE = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                EMAIL = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false),
                DTE_COMPLETED = table.Column<DateTime>(type: "datetime", nullable: true),
                THEME_1 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                THEME_2 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                THEME_3 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                THEME_4 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                THEME_5 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                Private = table.Column<bool>(type: "bit", nullable: false, comment: "Whether the user wants their strengths to be private (not shown to other users)")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CliftonStrengths", x => new { x.ID_NUM, x.ACCESS_CODE });
            });

        migrationBuilder.CreateTable(
            name: "Config",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Key = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Value = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Config", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "CUSTOM_PROFILE",
            columns: table => new
            {
                username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                facebook = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                twitter = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                instagram = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                linkedin = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                handshake = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                calendar = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                PlannedGradYear = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CUSTOM_PROFILE", x => x.username);
            });

        migrationBuilder.CreateTable(
            name: "ERROR_LOG",
            columns: table => new
            {
                LOG_ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LOG_TIME = table.Column<DateTime>(type: "datetime", nullable: false),
                LOG_MESSAGE = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ERROR_LOG", x => x.LOG_ID);
            });

        migrationBuilder.CreateTable(
            name: "Housing_Applications",
            columns: table => new
            {
                HousingAppID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DateSubmitted = table.Column<DateTime>(type: "datetime", nullable: true),
                DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                EditorUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Housing_Applications", x => x.HousingAppID);
            });

        migrationBuilder.CreateTable(
            name: "Housing_HallChoices",
            columns: table => new
            {
                HallChoiceID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                HousingAppID = table.Column<int>(type: "int", nullable: false),
                Ranking = table.Column<int>(type: "int", nullable: false),
                HallName = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Housing_HallChoices", x => x.HallChoiceID);
            });

        migrationBuilder.CreateTable(
            name: "Housing_Halls",
            columns: table => new
            {
                Name = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                Type = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Housing_Halls", x => x.Name);
            });

        migrationBuilder.CreateTable(
            name: "Information_Change_Request",
            columns: table => new
            {
                ID = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RequestNumber = table.Column<long>(type: "bigint", nullable: false),
                ID_Num = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: false),
                FieldName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                FieldValue = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                TimeStamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Information_Change_Request", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "MatchStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatchStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "MatchTeamStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatchTeamStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "MEMBERSHIP",
            columns: table => new
            {
                MEMBERSHIP_ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ACT_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                SESS_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                ID_NUM = table.Column<int>(type: "int", nullable: false),
                PART_CDE = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                BEGIN_DTE = table.Column<DateTime>(type: "datetime", nullable: false),
                END_DTE = table.Column<DateTime>(type: "datetime", nullable: true),
                COMMENT_TXT = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                USER_NAME = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                JOB_NAME = table.Column<string>(type: "char(30)", unicode: false, fixedLength: true, maxLength: 30, nullable: true),
                JOB_TIME = table.Column<DateTime>(type: "datetime", nullable: true),
                GRP_ADMIN = table.Column<bool>(type: "bit", nullable: true),
                PRIVACY = table.Column<bool>(type: "bit", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Membership", x => x.MEMBERSHIP_ID);
            });

        migrationBuilder.CreateTable(
            name: "Participant",
            schema: "RecIM",
            columns: table => new
            {
                Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SpecifiedGender = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: false),
                AllowEmails = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                IsCustom = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Participant", x => x.Username);
            });

        migrationBuilder.CreateTable(
            name: "ParticipantStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParticipantStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "PrivType",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PrivType", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "REQUEST",
            columns: table => new
            {
                REQUEST_ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SESS_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                ACT_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                ID_NUM = table.Column<int>(type: "int", nullable: false),
                PART_CDE = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                DATE_SENT = table.Column<DateTime>(type: "datetime", nullable: false),
                COMMENT_TXT = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                STATUS = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Request", x => x.REQUEST_ID);
            });

        migrationBuilder.CreateTable(
            name: "RoleType",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleType", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "SeriesSchedule",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Sun = table.Column<bool>(type: "bit", nullable: false),
                Mon = table.Column<bool>(type: "bit", nullable: false),
                Tue = table.Column<bool>(type: "bit", nullable: false),
                Wed = table.Column<bool>(type: "bit", nullable: false),
                Thu = table.Column<bool>(type: "bit", nullable: false),
                Fri = table.Column<bool>(type: "bit", nullable: false),
                Sat = table.Column<bool>(type: "bit", nullable: false),
                StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                EstMatchTime = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeriesSchedule", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "SeriesStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeriesStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "SeriesType",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                TypeCode = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__SeriesTy__3214EC270DA32B2F", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "Slider_Images",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Path = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                LinkURL = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                Width = table.Column<int>(type: "int", nullable: false),
                Height = table.Column<int>(type: "int", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Slider_Images", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "Sport",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                Rules = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: false),
                Logo = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Sport__3214EC276FB2D545", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "StudentNewsExpiration",
            columns: table => new
            {
                SNID = table.Column<int>(type: "int", nullable: false),
                ManualExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StudentNewsExpiration_SNID", x => x.SNID);
            });

        migrationBuilder.CreateTable(
            name: "Surface",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Surface__3214EC27629CF90A", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "TeamStatus",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Description = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TeamStatus", x => x.ID);
            });

        migrationBuilder.CreateTable(
            name: "Housing_Applicants",
            columns: table => new
            {
                HousingAppID = table.Column<int>(type: "int", nullable: false),
                Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                AprtProgram = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                AprtProgramCredit = table.Column<bool>(type: "bit", nullable: true),
                SESS_CDE = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Housing_Applicants", x => new { x.HousingAppID, x.Username });
                table.ForeignKey(
                    name: "FK_Applicants_HousingAppID",
                    column: x => x.HousingAppID,
                    principalTable: "Housing_Applications",
                    principalColumn: "HousingAppID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CustomParticipant",
            schema: "RecIM",
            columns: table => new
            {
                Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__CustomPa__536C85E5A0FDE2AE", x => x.Username);
                table.ForeignKey(
                    name: "FK__CustomPar__Usern__70D3A237",
                    column: x => x.Username,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
            });

        migrationBuilder.CreateTable(
            name: "ParticipantNotification",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ParticipantUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                Message = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                EndDate = table.Column<DateTime>(type: "date", nullable: false),
                DispatchDate = table.Column<DateTime>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Particip__3214EC2702C79C04", x => x.ID);
                table.ForeignKey(
                    name: "FK_ParticipantNotification_Participant",
                    column: x => x.ParticipantUsername,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
            });

        migrationBuilder.CreateTable(
            name: "ParticipantStatusHistory",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ParticipantUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                StatusID = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "date", nullable: false),
                EndDate = table.Column<DateTime>(type: "date", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Particip__3214EC2705A45A77", x => x.ID);
                table.ForeignKey(
                    name: "FK_ParticipantStatusHistory_Participant",
                    column: x => x.ParticipantUsername,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
                table.ForeignKey(
                    name: "FK_ParticipantStatusHistory_ParticipantStatus",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "ParticipantStatus",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "Activity",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                Logo = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                RegistrationStart = table.Column<DateTime>(type: "datetime", nullable: false),
                RegistrationEnd = table.Column<DateTime>(type: "datetime", nullable: false),
                SportID = table.Column<int>(type: "int", nullable: false),
                StatusID = table.Column<int>(type: "int", nullable: false),
                MinCapacity = table.Column<int>(type: "int", nullable: false),
                MaxCapacity = table.Column<int>(type: "int", nullable: true),
                SoloRegistration = table.Column<bool>(type: "bit", nullable: false),
                Completed = table.Column<bool>(type: "bit", nullable: false),
                TypeID = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                EndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                SeriesScheduleID = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Activity__3214EC2752162A10", x => x.ID);
                table.ForeignKey(
                    name: "FK_Activity_ActivityStatus",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "ActivityStatus",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Activity_ActivityType",
                    column: x => x.TypeID,
                    principalSchema: "RecIM",
                    principalTable: "ActivityType",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Activity_SeriesSchedule",
                    column: x => x.SeriesScheduleID,
                    principalSchema: "RecIM",
                    principalTable: "SeriesSchedule",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Activity_Sport",
                    column: x => x.SportID,
                    principalSchema: "RecIM",
                    principalTable: "Sport",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "ParticipantActivity",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ActivityID = table.Column<int>(type: "int", nullable: false),
                ParticipantUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                PrivTypeID = table.Column<int>(type: "int", nullable: false),
                IsFreeAgent = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Particip__3214EC27C3ECBD4A", x => x.ID);
                table.ForeignKey(
                    name: "FK_ParticipantActivity_Activity",
                    column: x => x.ActivityID,
                    principalSchema: "RecIM",
                    principalTable: "Activity",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ParticipantActivity_Participant",
                    column: x => x.ParticipantUsername,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
                table.ForeignKey(
                    name: "FK_PrivType_ParticipantActivity",
                    column: x => x.PrivTypeID,
                    principalSchema: "RecIM",
                    principalTable: "PrivType",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "Team",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: false),
                StatusID = table.Column<int>(type: "int", nullable: false),
                ActivityID = table.Column<int>(type: "int", nullable: false),
                Logo = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                Affiliation = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Team__3214EC27A55EA0D3", x => x.ID);
                table.ForeignKey(
                    name: "FK_Team_Activity",
                    column: x => x.ActivityID,
                    principalSchema: "RecIM",
                    principalTable: "Activity",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Team_Affiliations",
                    column: x => x.Affiliation,
                    principalSchema: "RecIM",
                    principalTable: "Affiliation",
                    principalColumn: "Name",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_Team_TeamStatus",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "TeamStatus",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "ParticipantTeam",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TeamID = table.Column<int>(type: "int", nullable: false),
                ParticipantUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                SignDate = table.Column<DateTime>(type: "datetime", nullable: false),
                RoleTypeID = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Particip__3214EC27CA13A740", x => x.ID);
                table.ForeignKey(
                    name: "FK_ParticipantTeam_Participant",
                    column: x => x.ParticipantUsername,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
                table.ForeignKey(
                    name: "FK_ParticipantTeam_Team",
                    column: x => x.TeamID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RoleType_ParticipantTeam",
                    column: x => x.RoleTypeID,
                    principalSchema: "RecIM",
                    principalTable: "RoleType",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "Series",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                ActivityID = table.Column<int>(type: "int", nullable: false),
                TypeID = table.Column<int>(type: "int", nullable: false),
                StatusID = table.Column<int>(type: "int", nullable: false),
                ScheduleID = table.Column<int>(type: "int", nullable: false),
                WinnerID = table.Column<int>(type: "int", nullable: true),
                Points = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Series__3214EC27544E6A66", x => x.ID);
                table.ForeignKey(
                    name: "FK_SeriesSchedule_Series",
                    column: x => x.ScheduleID,
                    principalSchema: "RecIM",
                    principalTable: "SeriesSchedule",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_SeriesStatus_Series",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "SeriesStatus",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Series_Activity",
                    column: x => x.ActivityID,
                    principalSchema: "RecIM",
                    principalTable: "Activity",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Series_SeriesType",
                    column: x => x.TypeID,
                    principalSchema: "RecIM",
                    principalTable: "SeriesType",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Series_Team",
                    column: x => x.WinnerID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "Statistic",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ParticipantTeamID = table.Column<int>(type: "int", nullable: false),
                Win = table.Column<int>(type: "int", nullable: false),
                Loss = table.Column<int>(type: "int", nullable: false),
                Sportsmanship = table.Column<double>(type: "float", nullable: false),
                TimesRated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Statisti__3214EC2707DA8F6B", x => x.ID);
                table.ForeignKey(
                    name: "FK_Statistic_ParticipantTeam",
                    column: x => x.ParticipantTeamID,
                    principalSchema: "RecIM",
                    principalTable: "ParticipantTeam",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "AffiliationPoints",
            schema: "RecIM",
            columns: table => new
            {
                AffiliationName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                TeamID = table.Column<int>(type: "int", nullable: false),
                SeriesID = table.Column<int>(type: "int", nullable: false),
                Points = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AffiliationPoints", x => new { x.AffiliationName, x.TeamID, x.SeriesID });
                table.ForeignKey(
                    name: "FK_AffiliationPoints_Affiliations",
                    column: x => x.AffiliationName,
                    principalSchema: "RecIM",
                    principalTable: "Affiliation",
                    principalColumn: "Name");
                table.ForeignKey(
                    name: "FK_AffiliationPoints_Series",
                    column: x => x.SeriesID,
                    principalSchema: "RecIM",
                    principalTable: "Series",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_AffiliationPoints_Team",
                    column: x => x.TeamID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "Match",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SeriesID = table.Column<int>(type: "int", nullable: false),
                StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                SurfaceID = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                StatusID = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__Match__3214EC27BB2F05C8", x => x.ID);
                table.ForeignKey(
                    name: "FK_Match_MatchStatus",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "MatchStatus",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_Match_Series",
                    column: x => x.SeriesID,
                    principalSchema: "RecIM",
                    principalTable: "Series",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Match_Surface",
                    column: x => x.SurfaceID,
                    principalSchema: "RecIM",
                    principalTable: "Surface",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "SeriesSurface",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SeriesID = table.Column<int>(type: "int", nullable: false),
                SurfaceID = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SeriesSurface", x => x.ID);
                table.ForeignKey(
                    name: "FK_SeriesSurface_Series",
                    column: x => x.SeriesID,
                    principalSchema: "RecIM",
                    principalTable: "Series",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SeriesSurface_Surface",
                    column: x => x.SurfaceID,
                    principalSchema: "RecIM",
                    principalTable: "Surface",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SeriesTeam",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TeamID = table.Column<int>(type: "int", nullable: false),
                SeriesID = table.Column<int>(type: "int", nullable: false),
                WinCount = table.Column<int>(type: "int", nullable: false),
                LossCount = table.Column<int>(type: "int", nullable: false),
                TieCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__SeriesTe__3214EC27F3AD6274", x => x.ID);
                table.ForeignKey(
                    name: "FK_SeriesTeam_Series",
                    column: x => x.SeriesID,
                    principalSchema: "RecIM",
                    principalTable: "Series",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SeriesTeam_Team",
                    column: x => x.TeamID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "MatchBracket",
            schema: "RecIM",
            columns: table => new
            {
                MatchID = table.Column<int>(type: "int", nullable: false),
                RoundNumber = table.Column<int>(type: "int", nullable: false),
                RoundOf = table.Column<int>(type: "int", nullable: false),
                SeedIndex = table.Column<int>(type: "int", nullable: false),
                IsLosers = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MatchBracket", x => x.MatchID);
                table.ForeignKey(
                    name: "FK_MatchBracket_Match",
                    column: x => x.MatchID,
                    principalSchema: "RecIM",
                    principalTable: "Match",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "MatchParticipant",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MatchID = table.Column<int>(type: "int", nullable: false),
                ParticipantUsername = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                TeamID = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__MatchPar__3214EC27DFD38F69", x => x.ID);
                table.ForeignKey(
                    name: "FK_MatchParticipant_Match",
                    column: x => x.MatchID,
                    principalSchema: "RecIM",
                    principalTable: "Match",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_MatchParticipant_Participant",
                    column: x => x.ParticipantUsername,
                    principalSchema: "RecIM",
                    principalTable: "Participant",
                    principalColumn: "Username");
                table.ForeignKey(
                    name: "FK_MatchParticipant_Team",
                    column: x => x.TeamID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID");
            });

        migrationBuilder.CreateTable(
            name: "MatchTeam",
            schema: "RecIM",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TeamID = table.Column<int>(type: "int", nullable: false),
                MatchID = table.Column<int>(type: "int", nullable: false),
                StatusID = table.Column<int>(type: "int", nullable: false),
                Score = table.Column<int>(type: "int", nullable: false),
                SportsmanshipScore = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK__MatchTea__3214EC2743BD85C3", x => x.ID);
                table.ForeignKey(
                    name: "FK_MatchTeam_Match",
                    column: x => x.MatchID,
                    principalSchema: "RecIM",
                    principalTable: "Match",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_MatchTeam_MatchTeamStatus",
                    column: x => x.StatusID,
                    principalSchema: "RecIM",
                    principalTable: "MatchTeamStatus",
                    principalColumn: "ID");
                table.ForeignKey(
                    name: "FK_MatchTeam_Team",
                    column: x => x.TeamID,
                    principalSchema: "RecIM",
                    principalTable: "Team",
                    principalColumn: "ID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Activity_SeriesScheduleID",
            schema: "RecIM",
            table: "Activity",
            column: "SeriesScheduleID");

        migrationBuilder.CreateIndex(
            name: "IX_Activity_SportID",
            schema: "RecIM",
            table: "Activity",
            column: "SportID");

        migrationBuilder.CreateIndex(
            name: "IX_Activity_StatusID",
            schema: "RecIM",
            table: "Activity",
            column: "StatusID");

        migrationBuilder.CreateIndex(
            name: "IX_Activity_TypeID",
            schema: "RecIM",
            table: "Activity",
            column: "TypeID");

        migrationBuilder.CreateIndex(
            name: "IX_AffiliationPoints_SeriesID",
            schema: "RecIM",
            table: "AffiliationPoints",
            column: "SeriesID");

        migrationBuilder.CreateIndex(
            name: "IX_AffiliationPoints_TeamID",
            schema: "RecIM",
            table: "AffiliationPoints",
            column: "TeamID");

        migrationBuilder.CreateIndex(
            name: "IX_Housing_HallChoices",
            table: "Housing_HallChoices",
            column: "HousingAppID");

        migrationBuilder.CreateIndex(
            name: "IX_Match_SeriesID",
            schema: "RecIM",
            table: "Match",
            column: "SeriesID");

        migrationBuilder.CreateIndex(
            name: "IX_Match_StatusID",
            schema: "RecIM",
            table: "Match",
            column: "StatusID");

        migrationBuilder.CreateIndex(
            name: "IX_Match_SurfaceID",
            schema: "RecIM",
            table: "Match",
            column: "SurfaceID");

        migrationBuilder.CreateIndex(
            name: "IX_MatchParticipant_MatchID",
            schema: "RecIM",
            table: "MatchParticipant",
            column: "MatchID");

        migrationBuilder.CreateIndex(
            name: "IX_MatchParticipant_ParticipantUsername",
            schema: "RecIM",
            table: "MatchParticipant",
            column: "ParticipantUsername");

        migrationBuilder.CreateIndex(
            name: "IX_MatchParticipant_TeamID",
            schema: "RecIM",
            table: "MatchParticipant",
            column: "TeamID");

        migrationBuilder.CreateIndex(
            name: "IX_MatchTeam_MatchID",
            schema: "RecIM",
            table: "MatchTeam",
            column: "MatchID");

        migrationBuilder.CreateIndex(
            name: "IX_MatchTeam_StatusID",
            schema: "RecIM",
            table: "MatchTeam",
            column: "StatusID");

        migrationBuilder.CreateIndex(
            name: "IX_MatchTeam_TeamID",
            schema: "RecIM",
            table: "MatchTeam",
            column: "TeamID");

        migrationBuilder.CreateIndex(
            name: "IX_MEMBERSHIP",
            table: "MEMBERSHIP",
            columns: new[] { "ACT_CDE", "SESS_CDE", "ID_NUM" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantActivity_ActivityID",
            schema: "RecIM",
            table: "ParticipantActivity",
            column: "ActivityID");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantActivity_ParticipantUsername",
            schema: "RecIM",
            table: "ParticipantActivity",
            column: "ParticipantUsername");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantActivity_PrivTypeID",
            schema: "RecIM",
            table: "ParticipantActivity",
            column: "PrivTypeID");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantNotification_ParticipantUsername",
            schema: "RecIM",
            table: "ParticipantNotification",
            column: "ParticipantUsername");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantStatusHistory_ParticipantUsername",
            schema: "RecIM",
            table: "ParticipantStatusHistory",
            column: "ParticipantUsername");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantStatusHistory_StatusID",
            schema: "RecIM",
            table: "ParticipantStatusHistory",
            column: "StatusID");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantTeam_ParticipantUsername",
            schema: "RecIM",
            table: "ParticipantTeam",
            column: "ParticipantUsername");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantTeam_RoleTypeID",
            schema: "RecIM",
            table: "ParticipantTeam",
            column: "RoleTypeID");

        migrationBuilder.CreateIndex(
            name: "IX_ParticipantTeam_TeamID",
            schema: "RecIM",
            table: "ParticipantTeam",
            column: "TeamID");

        migrationBuilder.CreateIndex(
            name: "IX_Series_ActivityID",
            schema: "RecIM",
            table: "Series",
            column: "ActivityID");

        migrationBuilder.CreateIndex(
            name: "IX_Series_ScheduleID",
            schema: "RecIM",
            table: "Series",
            column: "ScheduleID");

        migrationBuilder.CreateIndex(
            name: "IX_Series_StatusID",
            schema: "RecIM",
            table: "Series",
            column: "StatusID");

        migrationBuilder.CreateIndex(
            name: "IX_Series_TypeID",
            schema: "RecIM",
            table: "Series",
            column: "TypeID");

        migrationBuilder.CreateIndex(
            name: "IX_Series_WinnerID",
            schema: "RecIM",
            table: "Series",
            column: "WinnerID");

        migrationBuilder.CreateIndex(
            name: "IX_SeriesSurface_SeriesID",
            schema: "RecIM",
            table: "SeriesSurface",
            column: "SeriesID");

        migrationBuilder.CreateIndex(
            name: "IX_SeriesSurface_SurfaceID",
            schema: "RecIM",
            table: "SeriesSurface",
            column: "SurfaceID");

        migrationBuilder.CreateIndex(
            name: "IX_SeriesTeam_SeriesID",
            schema: "RecIM",
            table: "SeriesTeam",
            column: "SeriesID");

        migrationBuilder.CreateIndex(
            name: "IX_SeriesTeam_TeamID",
            schema: "RecIM",
            table: "SeriesTeam",
            column: "TeamID");

        migrationBuilder.CreateIndex(
            name: "IX_Statistic_ParticipantTeamID",
            schema: "RecIM",
            table: "Statistic",
            column: "ParticipantTeamID");

        migrationBuilder.CreateIndex(
            name: "IX_Team_ActivityID",
            schema: "RecIM",
            table: "Team",
            column: "ActivityID");

        migrationBuilder.CreateIndex(
            name: "IX_Team_Affiliation",
            schema: "RecIM",
            table: "Team",
            column: "Affiliation");

        migrationBuilder.CreateIndex(
            name: "IX_Team_StatusID",
            schema: "RecIM",
            table: "Team",
            column: "StatusID");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ACCOUNT] AS
SELECT 
    acct.account_id
    , ISNULL(acct.gordon_id, - 1) AS gordon_id
    , acct.barcode, acct.firstname
    , acct.lastname, acct.email
    , acct.AD_Username
    , acct.account_type
    , acct.office_hours
    , acct.Primary_Photo
    , acct.Preferred_Photo
    , acct.show_pic
    , acct.Private
    , acct.ReadOnly
    , acct.Chapel_Required
    , acct.Mail_Location
    , acct.Chapel_Attended
    , CASE
        WHEN acct.gordon_id IN (999999097,8330171, 999999099) THEN GETDATE() 
        ELSE acct.Birth_Date 
    END AS Birth_Date
FROM webSQL.dbo.view_cct AS acct
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[AccountPhotoURL] AS
SELECT P.Gordonid,
CASE WHEN P.File_name IS NOT NULL THEN 'https://photos.gordon.edu/' + AH.hash_key + '?type=thmb' END as PhotoURL,
CASE WHEN P.Pref_file_name IS NOT NULL THEN 'https://photos.gordon.edu/' + AH.hash_key + '?type=pref' END as PrefPhotoURL
FROM websql.dbo.Photos P
LEFT JOIN webSQL.dbo.account_hashkeys AH on P.Gordonid = AH.gordon_id
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Alumni] AS
SELECT
    CONVERT(VARCHAR(9), ID) as ID
    , NULL as WebUpdate
    , [Title]
    , [FirstName]
    , [MiddleName]
    , [LastName]
    , [Suffix]
    , [MaidenName]
    , [NickName]
    , [HomeStreet1]
    , [HomeStreet2]
    , [HomeCity]
    , [HomeState]
    , [HomePostalCode]
    , [HomeCountry]
    , [HomePhone]
    , [HomeFax]
    , [HomeEmail]
    , [JobTitle]
    , [MaritalStatus]
    , [SpouseName]
    , [College]
    , [ClassYear]
    , [PreferredClassYear]
    , [Major1]
    , [Major2]
    , [ShareName]
    , [ShareAddress]
    , [Gender]
    , [GradDate]
    , [Email]
    , [grad_student]
    , [Barcode]
    , [AD_Username]
    , CAST([show_pic] AS int) AS show_pic
    , CAST([preferred_photo] AS int) AS preferred_photo
    , [Country]
    , [Major1Description]
    , [Major2Description]
FROM [CCT].[cache].[Alumni]
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Buildings] AS
SELECT BLDG_CDE, BUILDING_DESC
FROM TmsEPrd.dbo.BUILDING_MASTER
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ChapelEvent] AS
SELECT
    ROWID
    , CHBarEventID
    , ID_NUM
    , CHBarcode
    , CHEventID
    , CHCheckerID
    , CHDate
    , CHTime
    , CHSource
    , CHTermCD
    , Attended
    , Required
    , LiveID
FROM MyGordon.dbo.view_cct_ChapelEvent
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ChapelEventSource] AS
SELECT [EventNumber]
    ,CONVERT(VARCHAR(200),
        HASHBYTES('MD5', 
	        CONVERT(VARCHAR, EventNumber) + '|' + CONVERT(VARCHAR, StartDate) + '|' + CONVERT(VARCHAR, EndDate) + '|' + EventTitle)
        , 2) 
    as EventHash
    ,[EventTitle]
    ,[EventLocation]
    ,[EventLocation2]
    ,[StartDate]
    ,[EndDate]
    ,[StartTime]
    ,[EndTime]
    ,[EventType]
    ,[EventCategory]
    ,[Credit]
    ,[Chapel_reason]
    ,[SubmittedDate]
    ,[Status]
    ,[StatusDate]
FROM [MyGordon].[dbo].[events]
WHERE [Status] = 'Approved'
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[CM_SESSION_MSTR] AS
SELECT 
SESS_CDE, SESS_DESC, SESS_BEGN_DTE, SESS_END_DTE, WHEEL_BEGN_DTE, WHEEL_END_DTE, YRTRM_CDE_2, YRTRM_CDE_4
FROM TmsEPrd.dbo.GORD_CCT_CM_SESSION_MSTR
WHERE SESS_DESC is not null
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Countries] AS
SELECT ISO2LetterCode AS CTY, [Name] as COUNTRY
FROM TmsEPrd.dbo.CountryDefinition
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Dining_Meal_Choice_Desc] AS
SELECT [TABLE_VALUE] as Meal_Choice_Id,[TABLE_DESC] as Meal_Choice_Desc FROM [TmsEPrd].[dbo].[TABLE_DETAIL] where column_name='meal_plan'
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Dining_Meal_Plan_Change_History] AS
SELECT ID_NUMBER as ID_NUM,
CASE
	WHEN SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2) = '""' THEN NULL
	ELSE SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2)
END as OLD_PLAN,
map1.meal_plan_id as OLD_PLAN_ID,
MCD.Meal_Choice_Desc as OLD_PLAN_DESC,
CASE
	WHEN SUBSTRING(ITEM_DESCRIPTION,CHARINDEX(' to ',ITEM_DESCRIPTION,15) + 4,2) = '""' THEN NULL
	ELSE SUBSTRING(ITEM_DESCRIPTION,CHARINDEX(' to ',ITEM_DESCRIPTION,15) + 4,2)
END as NEW_PLAN,
map2.meal_plan_id as NEW_PLAN_ID,
SUBSTRING(ITEM_DESCRIPTION,CHARINDEX(' for session ',ITEM_DESCRIPTION,30) + 13,6) as SESS_CDE,
ITEM_DATE as CHANGE_DATE
FROM TmsEPrd.dbo.ITEMS I
LEFT JOIN CCT.dbo.Dining_Meal_Plan_Id_Mapping as map1
    ON (CASE
        WHEN SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2) = '""' THEN NULL
        ELSE SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2)
        END) = map1.meal_choice_id
LEFT JOIN CCT.dbo.Dining_Meal_Plan_Id_Mapping as map2 
    ON (CASE
        WHEN SUBSTRING(ITEM_DESCRIPTION,CHARINDEX(' to ',ITEM_DESCRIPTION,15) + 4,2) = '""' THEN NULL
        ELSE SUBSTRING(ITEM_DESCRIPTION,CHARINDEX(' to ',ITEM_DESCRIPTION,15) + 4,2)
        END) = map2.meal_choice_id
LEFT JOIN CCT.dbo.Dining_Meal_Choice_Desc MCD 
    ON (CASE
        WHEN SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2) = '""' THEN NULL
        ELSE SUBSTRING(ITEM_DESCRIPTION,CHARINDEX('from ',ITEM_DESCRIPTION,15) + 5,2)
        END) = MCD.Meal_Choice_Id
WHERE ACTION_CODE = 'SAMPCH'
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Dining_Meal_Plan_Id_Mapping] AS
SELECT * FROM [synergy].[dbo].[Blackboard_Jenzabar_Meal_Plan_Mapping]
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Dining_Mealplans] AS
SELECT Meal_Plan_Description, Meal_Plan_Type, Meal_Plan_ID, Meal_Plan_Initial_Balance
FROM synergy.dbo.Blackboard_Transact_MealPlans
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Dining_Student_Meal_Choice] AS
--Show 14 days ahead of actual session start date.
select SSA.ID_NUM,
    meal_plan as MEAL_CHOICE_ID,
    SESS_CDE
from [TmsEPrd].[dbo].[STUD_SESS_ASSIGN] SSA
where sess_cde = (
    SELECT max(SESS_CDE) as DEFAULT_SESS_CDE
    FROM CCT.dbo.CM_SESSION_MSTR
    WHERE getdate() + 14 > SESS_BEGN_DTE --and SESS_END_DTE >= getdate()
        and ltrim(rtrim(right(sess_cde,3))) in ('1','5','9')
)
GO
");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[DiningInfo] AS
SELECT 
    choice.id_num as StudentId, 
    sess_cde as SessionCode, 
    descs.Meal_Choice_Desc as ChoiceDescription,
    plans.Meal_Plan_Description as PlanDescriptions,
    plans.Meal_Plan_ID as PlanId,
    plans.Meal_Plan_Type as PlanType,
    plans.Meal_Plan_Initial_Balance as InitialBalance
FROM CCT.dbo.Dining_Student_Meal_Choice as choice
INNER JOIN CCT.dbo.Dining_Meal_Plan_Id_Mapping as map
       ON choice.Meal_Choice_Id = map.meal_choice_id
INNER JOIN CCT.dbo.Dining_Meal_Choice_Desc as descs
       ON choice.Meal_Choice_Id = descs.Meal_Choice_Id
INNER JOIN CCT.dbo.Dining_Mealplans as plans
       ON map.meal_plan_id = plans.Meal_Plan_ID
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[EmergencyContact] AS
SELECT 
    [APPID]
	,A.AD_Username
    ,[EMRG_SEQ_NUM] as SEQ_NUM
	,ID_NUM
    ,[ID_NUM_EMRG_CNTCT]
    ,[EMRG_PREFIX] as prefix
    ,[EMRG_LAST_NME] as lastname
    ,[EMRG_FIRST_NME] as firstname
    ,[EMRG_MIDDLE_NME] as middlename
    ,[EMRG_SUFFIX] as suffix
    ,[EMRG_HOME_PHN] as HomePhone
    ,[EMRG_HOME_EXT] as HomeExt
    ,[EMRG_WRK_PHN] as WorkPhone
    ,[EMRG_WRK_EXT] as WorkExr
    ,[EMRG_MOBL_PHN] as MobilePhone
    ,[EMRG_MOBL_EXT] as MobileExt
    ,[EMRG_NOTES] as notes
    ,[EMRG_EMAIL_ADDRESS] as EmailAddress
    ,[EMRG_HOME_ADDR_CDE] as HomeAddrCode
    ,[EMRG_WRK_ADDR_CDE] as WorkAddrCode
    ,[EMRG_MOBL_ADDR_CDE] as MobileAddrCode
    ,[EMRG_EMAIL_ADDR_CDE] as EmailAddrCode
    ,[EMRG_ADDRESS_ADDR_CDE] as AddressAddrCode
    ,[EMRG_RELATIONSHIP] as relationship
    ,[EMRG_PRIORITY]
    ,[EMRG_ADDR_1] as addr_1
    ,[EMRG_ADDR_2] as addr_2
    ,[EMRG_CITY] as city
    ,[EMRG_STATE]
    ,[EMRG_ZIP] as zip
    ,[EMRG_COUNTRY] as country
    ,[APPROWVERSION] as ApprowVersion
    ,[USER_NAME] as UserName
    ,[JOB_NAME] as JobName
    ,[JOB_TIME] as JobTime
FROM CCT.dbo.ACCOUNT as A 
inner join TmsEPrd.dbo.CM_EMERG_CONTACTS as B
on A.gordon_id = B.ID_NUM
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[FacStaff] AS
SELECT
    CONVERT(VARCHAR(9), ISNULL(facstaff.ID, - 1)) AS ID
    , facstaff.Title
    , facstaff.FirstName
    , facstaff.MiddleName   
    , facstaff.LastName
    , facstaff.Suffix
    , facstaff.MaidenName
    , facstaff.Nickname
    , facstaff.OnCampusDepartment
    , facstaff.OnCampusBuilding
    , facstaff.OnCampusRoom
    , facstaff.OnCampusPhone
    , facstaff.OnCampusPrivatePhone
    , facstaff.OnCampusFax
    , facstaff.HomeStreet1
    , facstaff.HomeStreet2
    , facstaff.HomeCity
    , facstaff.HomeState
    , facstaff.HomePostalCode
    , facstaff.HomeCountry
    , facstaff.HomePhone
	, facstaff.MobilePhone
    , facstaff.HomeFax
    , facstaff.KeepPrivate
    , facstaff.JobTitle
    , facstaff.SpouseName
    , facstaff.Dept
    , facstaff.Barcode
    , facstaff.Gender
    , facstaff.Email
    , facstaff.ActiveAccount
    , facstaff.Type
    , acct.AD_Username
    , acct.office_hours
    , acct.preferred_photo
    , acct.show_pic
    , build.BUILDING_DESC AS BuildingDescription
    , cou.COUNTRY AS Country
    , acct.Mail_Location
    , mail.description as Mail_Description
FROM MyGordon.dbo.view_cct_facstaff AS facstaff LEFT OUTER JOIN
    CCT.dbo.ACCOUNT AS acct ON facstaff.ID = acct.gordon_id LEFT JOIN
    webSQL.dbo.Mailstops mail ON acct.Mail_Location = mail.code LEFT OUTER JOIN
    CCT.dbo.Buildings AS build ON facstaff.OnCampusBuilding = build.BLDG_CDE LEFT OUTER JOIN
    CCT.dbo.Countries AS cou ON facstaff.HomeCountry = cou.CTY
WHERE acct.AD_Username is not null
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Graduation] AS
WITH CTE_Student_Data AS (
	SELECT A.ID_NUM, A.EXPECT_GRAD_YR, A.EXPECT_GRAD_TRM, A.DEGR_CDE, A.UDEF_5A_2, A.UDEF_3A_2, A.DTE_DEGR_CONFERRED,
	CASE
	    WHEN A.DTE_DEGR_CONFERRED IS NOT NULL THEN FORMAT(A.DTE_DEGR_CONFERRED, 'MMMM yyyy')
	END AS CF_ACTUAL_GRAD,
	CASE
	    WHEN A.EXPECT_GRAD_TRM = 'SP' THEN 'May '
	    WHEN A.EXPECT_GRAD_TRM = 'SU' THEN 'August '
	    WHEN A.EXPECT_GRAD_TRM = 'FA' THEN 'December '
	    WHEN A.EXPECT_GRAD_TRM = 'WS' THEN 'June '
	    WHEN A.EXPECT_GRAD_TRM = 'SF' THEN 'December '
	END +
	CASE
	    WHEN A.EXPECT_GRAD_TRM = 'SP' THEN CAST(A.EXPECT_GRAD_YR + 1 AS char(4))
	    WHEN A.EXPECT_GRAD_TRM = 'SU' THEN CAST(A.EXPECT_GRAD_YR + 1 AS char(4))
	    WHEN A.EXPECT_GRAD_TRM = 'FA' THEN CAST(A.EXPECT_GRAD_YR AS char(4))
	    WHEN A.EXPECT_GRAD_TRM = 'WS' THEN CAST(A.EXPECT_GRAD_YR + 1 AS char(4))
	    WHEN A.EXPECT_GRAD_TRM = 'SF' THEN CAST(A.EXPECT_GRAD_YR AS char(4))
	END AS DH_EXPECTED_GRAD,
	CASE
	    WHEN LEFT(A.UDEF_5A_2, 1) = 'A' THEN 'April '
	    WHEN LEFT(A.UDEF_5A_2, 1) = 'J' THEN 'June '
	    WHEN LEFT(A.UDEF_5A_2, 1) = 'S' THEN 'September '
	    WHEN LEFT(A.UDEF_5A_2, 1) = 'D' THEN 'December '
	    WHEN LEFT(A.UDEF_3A_2, 1) = 'M' THEN 'May '
	    WHEN LEFT(A.UDEF_3A_2, 1) = 'S' THEN 'August '
	    WHEN LEFT(A.UDEF_3A_2, 1) = 'D' THEN 'December '
	END +
	CASE
	    WHEN A.UDEF_5A_2 IS NOT NULL THEN '20' + RIGHT(A.UDEF_5A_2, 2)
	    ELSE '20' + RIGHT(A.UDEF_3A_2, 2)
	END AS GF_EXPECTED_GRAD
	FROM TmsEPrd.dbo.DEGREE_HISTORY A
	WHERE A.CUR_DEGREE = 'Y'
	    AND NOT (A.DTE_DEGR_CONFERRED IS NULL AND A.EXPECT_GRAD_YR IS NULL AND A.UDEF_3A_2 IS NULL AND A.UDEF_5A_2 IS NULL)
	    AND A.DEGR_CDE <> 'LICEN'
)
SELECT ID_NUM, COALESCE(CF_ACTUAL_GRAD, GF_EXPECTED_GRAD, DH_EXPECTED_GRAD) AS WHEN_GRAD,
CASE
	WHEN DTE_DEGR_CONFERRED IS NOT NULL THEN 'Y'
END AS HAS_GRADUATED
FROM CTE_Student_Data
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Internships_as_Involvements] AS
SELECT DISTINCT 'INTERN' as ACT_CDE
    ,[ID_NUM]
    ,SCH.YR_CDE
    ,SCH.TRM_CDE
    ,ST.SESS_CDE
    ,[BEGIN_DTE]
    ,[END_DTE]
    ,CASE
        WHEN CRS_TITLE in ('INTERNSHIP','INTERNSHIP II','INTERNSHIP I') AND [CRS_TITLE_2] IS NULL THEN NULL
        WHEN CRS_TITLE_2 IS NOT NULL THEN CRS_TITLE_2
        ELSE synergy.dbo.ProperCase(CRS_TITLE)
    END as COMMENT_TXT
FROM [TmsEPrd].[dbo].[STUDENT_CRS_HIST] SCH
INNER JOIN CCT.dbo.ACCOUNT A ON SCH.ID_NUM = A.gordon_id
LEFT JOIN TmsEPrd.dbo.SESS_TABLE ST on SCH.YR_CDE = ST.YR_CDE and SCH.TRM_CDE = ST.TRM_CDE
where crs_title like '%INTERNSHIP%'
AND WITHDRAWAL_DTE IS NULL
AND (DROP_FLAG <> 'D' or DROP_FLAG IS NULL)
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[InvolvementOffering] AS
SELECT
    ACD.ACT_CDE as ActivityCode,
    ACD.ACT_DESC as ActivityDescription,
    ACD.ACT_TYPE as ActivityType,
    td.TABLE_DESC as ActivityTypeDescription,
    SAM.SESS_CDE as SessionCode
FROM TmsEPrd.dbo.ACT_CLUB_DEF ACD 
inner join TmsEPrd.dbo.TABLE_DETAIL TD on ACD.ACT_TYPE = TD.TABLE_VALUE and TD.COLUMN_NAME = 'act_type'
inner join TmsEPrd.dbo.SESS_ACT_MASTER SAM on ACD.ACT_CDE = SAM.ACT_CDE
WHERE ACD.ACT_STS = 'A'
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[JENZ_ACT_CLUB_DEF] AS
SELECT ACT_CDE, ACT_DESC, ACT_TYPE, ACT_TYPE_DESC, VPM_IM, VPM_CC, VPM_LS, VPM_LW, VPL_IM, VPL_CC, VPL_LS, VPL_LW
FROM TMSEPRD.DBO.GORD_CCT_ACT_CLUB_DEF
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Mailboxes] AS
SELECT UniqueID as BoxId,
BoxNo as BoxNo,
Combination as Combination,
Type as Type,
A.account_id as HolderAccountId
FROM [synergy].[dbo].[Mailroom_Mailboxes] MM
LEFT JOIN websql.dbo.account A ON MM.BoxNo = A.mail_server
GO
");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Majors] AS
SELECT MajorCode, MajorDescription
FROM TmsEPrd.dbo.GORD_CCT_MAJORS
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[MembershipView] AS
SELECT MEMBERSHIP_ID as MembershipID,
    TRIM(mem.ACT_CDE) as ActivityCode,
    act_info.ACT_DESC as ActivityDescription,
    act_info.ACT_IMG_PATH as ActivityImagePath,
    TRIM(mem.SESS_CDE) as SessionCode,
    sess.SESS_DESC as SessionDescription,
    acct.AD_Username as Username,
    acct.firstname as FirstName,
    acct.lastname as LastName,
    TRIM(mem.PART_CDE) as Participation,
    TRIM(part.PART_DESC) as ParticipationDescription,
    mem.BEGIN_DTE as StartDate,
    mem.END_DTE as EndDate,
    mem.COMMENT_TXT as Description,
    mem.GRP_ADMIN as GroupAdmin,
    mem.PRIVACY as Privacy,
    CASE WHEN alum.ID IS NOT NULL THEN 1 ELSE 0 END as IsAlumni
FROM MEMBERSHIP as mem
INNER JOIN ACT_INFO as act_info
    ON mem.ACT_CDE = act_info.ACT_CDE
INNER JOIN CM_SESSION_MSTR as sess
    ON mem.SESS_CDE = sess.SESS_CDE
INNER JOIN ACCOUNT as acct
    ON mem.ID_NUM = acct.gordon_id
INNER JOIN PART_DEF as part
    ON mem.PART_CDE = part.PART_CDE
LEFT JOIN Alumni alum
    ON mem.ID_NUM = alum.ID
WHERE acct.Private != 1
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Minors] AS
SELECT 
    TRIM(MAJOR_CDE) as MinorCode,
    TRIM(MAJOR_MINOR_DESC) as MinorDescription
FROM TMSEPRD.dbo.GORD_MAJOR_MINOR_DEF
WHERE MAJOR_MINOR_BOTH in ('N','B') AND ACTIVE = 'Y'
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[PART_DEF] AS
SELECT  PART_CDE, PART_DESC
FROM TmsEPrd.dbo.GORD_CCT_PART_DEF
WHERE PART_CDE not in (
    'AC',
    --'ADV',
    'ALUM',
    'CAPT' ,
    'CODIR',
    'CORD',
    'CORE',
    'DIREC',
    'FACI',
    --'GUEST',
    --'LEAD',
    --'MEMBR',
    'OCREW',
    'PART',
    'PRES',
    'RA1',
    'RA2', 
    'RA3',
    'SEC',
    'VICEC',
    'VICEP',
    ''
)
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[RequestView] AS
SELECT [REQUEST_ID] as RequestID,
    R.[DATE_SENT] as DateSent,
    TRIM(R.ACT_CDE) as ActivityCode,
    act_info.ACT_DESC as ActivityDescription,
    act_info.ACT_IMG_PATH as ActivityImagePath,
    TRIM(R.SESS_CDE) as SessionCode,
    sess.SESS_DESC as SessionDescription,
    acct.AD_Username as Username,
    acct.firstname as FirstName,
    acct.lastname as LastName,
    TRIM(R.PART_CDE) as Participation,
    part.PART_DESC as ParticipationDescription,
    R.COMMENT_TXT as [Description],
    R.[STATUS] as [Status]
FROM [CCT].[dbo].[REQUEST] R
INNER JOIN ACT_INFO as act_info
    ON R.ACT_CDE = act_info.ACT_CDE
INNER JOIN CM_SESSION_MSTR as sess
    ON R.SESS_CDE = sess.SESS_CDE
INNER JOIN ACCOUNT as acct
    ON R.ID_NUM = acct.gordon_id
INNER JOIN PART_DEF as part
    ON R.PART_CDE = part.PART_CDE
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[RoomAssign] AS
SELECT SESS_CDE, BLDG_LOC_CDE, BLDG_CDE, ROOM_CDE, ROOM_SLOT_NUM, ID_NUM, ROOM_TYPE, ROOM_ASSIGN_STS, ASSIGN_DTE
FROM TmsEPrd.dbo.GORD_CCT_ROOM_ASSIGN
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ScheduleCourse] AS
with courses as (
    select
        sm.YR_CDE,
        sm.TRM_CDE,
        sm.SUBTERM_CDE,
        sm.CRS_CDE,
        sm.CRS_TITLE,
        sm.LEAD_INSTRUCTR_ID AS INSTRUCTOR_ID,
        ss.BLDG_CDE,
        ss.ROOM_CDE,
        ss.MONDAY_CDE,
        ss.TUESDAY_CDE,
        ss.WEDNESDAY_CDE,
        ss.THURSDAY_CDE,
        ss.FRIDAY_CDE,
        ss.SATURDAY_CDE,
        ss.SUNDAY_CDE,
        cast(ss.BEGIN_TIM as time) as BeginTime,
        cast(ss.END_TIM as time) as EndTime,
        cast(ss.BEGIN_DTE as date) as BeginDate,
        cast(ss.END_DTE as date) as EndDate
    from TmsEPrd.dbo.SECTION_MASTER sm
        inner join TmsEPrd.dbo.SECTION_SCHEDULES ss on
            ss.CRS_CDE = sm.CRS_CDE
            and sm.YR_CDE = ss.YR_CDE
            and sm.TRM_CDE = ss.TRM_CDE
            and sm.DIVISION_CDE = 'UG' and sm.TRM_CDE in ('FA','SP','SU')
        left join TmsEPrd.dbo.YEAR_TERM_TABLE yt on sm.YR_CDE = yt.YR_CDE and sm.TRM_CDE = yt.TRM_CDE
)

select
    acct.AD_Username as Username,
    'Student' as [Role],
    crs.*
from CCT.dbo.ACCOUNT acct
    inner join TmsEPrd.dbo.STUDENT_CRS_HIST sch on sch.ID_NUM = acct.gordon_id
    inner join courses crs on 
        sch.YR_CDE = crs.YR_CDE
        and sch.TRM_CDE = crs.TRM_CDE
        and sch.CRS_CDE = crs.CRS_CDE
where isnull(sch.DROP_FLAG, '') = ''

union 

select
    acct.AD_Username as Username,
    'Instructor' as [Role],
    crs.*
from CCT.dbo.ACCOUNT acct
inner join courses crs on acct.gordon_id = crs.INSTRUCTOR_ID
GO
");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[ScheduleTerm] AS
with term as (
    select
        yt.YR_CDE as YearCode,
        yt.TRM_CDE as TermCode,
        sm.SESS_DESC as [Description],
        cast(yt.TRM_BEGIN_DTE as date) as BeginDate,
        cast( yt.TRM_END_DTE as date) as EndDate
    from CCT.dbo.CM_SESSION_MSTR sm
        left join TmsEPrd.dbo.YEAR_TERM_TABLE yt on sm.YRTRM_CDE_4 = CONCAT(yt.YR_CDE, yt.TRM_CDE)
)

select
    term.YearCode,
    term.TermCode,
    COALESCE(st.SUBTERM_DESC,term.[Description]) as Description,
    COALESCE(cast(ytst.SBTRM_BEGIN_DTE as date), term.BeginDate) as BeginDate,
    COALESCE(cast(ytst.SBTRM_END_DTE as date), term.EndDate) as EndDate,
    st.SUBTERM_CDE  as SubTermCode,
    st.SUBTERM_SORT_ORDER as SubTermSortOrder
from term
    left join TmsEPrd.dbo.YR_TRM_SBTRM_TABLE ytst on term.YearCode = ytst.YR_CDE and term.TermCode = ytst.TRM_CDE
    left join TmsEPrd.dbo.SUBTERM_DEF st on ytst.SBTRM_CDE = st.SUBTERM_CDE

UNION

select term.*,
    null as SubTermCode,
    null as SubTermSortOrder
from term
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[States] AS
SELECT Name, Abbreviation
FROM TmsEPrd.dbo.StateDefinition
GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[Student] AS
SELECT 
    CONVERT(VARCHAR(9), student.ID) as ID
    , student.Title
    , student.FirstName
    , student.MiddleName
    , student.LastName
    , student.Suffix
    , student.MaidenName
    , student.NickName
    , student.OnOffCampus
    , student.OnCampusBuilding
    , student.OnCampusRoom
    , student.OnCampusPhone
    , student.OnCampusPrivatePhone
    , student.OnCampusFax
    , student.OffCampusStreet1
    , student.OffCampusStreet2
    , student.OffCampusCity
    , student.OffCampusState
    , student.OffCampusPostalCode
    , student.OffCampusCountry
    , student.OffCampusPhone
    , student.OffCampusFax
    , student.HomeStreet1
    , student.HomeStreet2
    , student.HomeCity
    , student.HomeState
    , student.HomePostalCode
    , student.HomeCountry
    , student.HomePhone
    , student.HomeFax
    , student.Cohort
    , student.Class
    , student.KeepPrivate
    , student.Barcode
    , student.AdvisorIDs
    , student.Married
    , student.Commuter
    , student.Major
    , student.Major2
    , student.Major3
    , student.Email
    , student.Gender
    , student.grad_student
    , CAST (student.GradDate as varchar(255)) as GradDate
    , student.Minor1
    , student.Minor2
    , student.Minor3
    , student.MobilePhone
    , student.IsMobilePhonePrivate
    , acct.AD_Username
    , acct.show_pic
    , acct.preferred_photo, cou.[Name] AS Country
    , build.BUILDING_DESC AS BuildingDescription
    , MajorDef1.MajorDescription AS Major1Description
    , MajorDef2.MajorDescription AS Major2Description
    , MajorDef3.MajorDescription AS Major3Description
    , MinorDef1.MajorDescription AS Minor1Description
    , MinorDef2.MajorDescription AS Minor2Description
    , MinorDef3.MajorDescription AS Minor3Description
    , acct.Mail_Location
    , acct.Chapel_Required AS ChapelRequired
    , acct.Chapel_Attended AS ChapelAttended
    , cus.PlannedGradYear AS PlannedGradYear
FROM MyGordon.dbo.view_cct_student AS student
LEFT OUTER JOIN CCT.dbo.ACCOUNT AS acct ON student.ID = acct.gordon_id 
LEFT OUTER JOIN TmsEPrd.dbo.CountryDefinition AS cou on student.HomeCountry = cou.ISO2LetterCode 
LEFT JOIN CCT.dbo.Buildings AS build ON student.OnCampusBuilding = build.BLDG_CDE
LEFT OUTER JOIN CCT.dbo.Majors AS MajorDef1 ON student.Major = MajorDef1.MajorCode
LEFT OUTER JOIN CCT.dbo.Majors AS MajorDef2 ON student.Major2 = MajorDef2.MajorCode
LEFT OUTER JOIN CCT.dbo.Majors AS MajorDef3 ON student.Major3 = MajorDef3.MajorCode
LEFT OUTER JOIN CCT.dbo.Majors AS MinorDef1 ON student.Minor1 = MinorDef1.MajorCode
LEFT OUTER JOIN CCT.dbo.Majors AS MinorDef2 ON student.Minor2 = MinorDef2.MajorCode
LEFT OUTER JOIN CCT.dbo.Majors AS MinorDef3 ON student.Minor3 = MinorDef3.MajorCode
LEFT OUTER JOIN CCT.dbo.CUSTOM_PROFILE AS cus ON acct.AD_Username = cus.username
WHERE acct.AD_Username is not null

GO");

        migrationBuilder.Sql(@"
CREATE VIEW [dbo].[UserCourses] AS

with courses as (
    select
        sm.YR_CDE,
        sm.TRM_CDE,
        st.SUBTERM_DESC,
        st.SUBTERM_SORT_ORDER,
        sm.CRS_CDE,
        sm.CRS_TITLE,
        sm.LEAD_INSTRUCTR_ID AS INSTRUCTOR_ID,
        ss.BLDG_CDE,
        ss.ROOM_CDE,
        ss.MONDAY_CDE,
        ss.TUESDAY_CDE,
        ss.WEDNESDAY_CDE,
        ss.THURSDAY_CDE,
        ss.FRIDAY_CDE,
        ss.SATURDAY_CDE,
        ss.SUNDAY_CDE,
        cast(ss.BEGIN_TIM as time) as BEGIN_TIME,
        cast(ss.END_TIM as time) as END_TIME,
        cast(ss.BEGIN_DTE as date) as BEGIN_DATE,
        cast(ss.END_DTE as date) as END_DATE
    from TmsEPrd.dbo.SECTION_MASTER sm
        inner join TmsEPrd.dbo.SECTION_SCHEDULES ss on
            ss.CRS_CDE = sm.CRS_CDE
            and sm.YR_CDE = ss.YR_CDE
            and sm.TRM_CDE = ss.TRM_CDE
            and sm.DIVISION_CDE = 'UG' and sm.TRM_CDE in ('FA','SP','SU')
        left join TmsEPrd.dbo.SUBTERM_DEF st on sm.SUBTERM_CDE = st.SUBTERM_CDE
)

SELECT
    acct.AD_Username as Username,
    'Student' as [Role],
    crs.*
from CCT.dbo.ACCOUNT acct
    inner join TmsEPrd.dbo.STUDENT_CRS_HIST sch on sch.ID_NUM = acct.gordon_id
    inner join courses crs on 
        sch.YR_CDE = crs.YR_CDE
        and sch.TRM_CDE = crs.TRM_CDE
        and sch.CRS_CDE = crs.CRS_CDE
where isnull(sch.DROP_FLAG, '') = ''
union 
SELECT
    acct.AD_Username as Username,
    'Instructor' as [Role],
    crs.*
from CCT.dbo.ACCOUNT acct
inner join courses crs on acct.gordon_id = crs.INSTRUCTOR_ID

GO");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ACT_INFO");

        migrationBuilder.DropTable(
            name: "AffiliationPoints",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Clifton_Strengths");

        migrationBuilder.DropTable(
            name: "Config");

        migrationBuilder.DropTable(
            name: "CUSTOM_PROFILE");

        migrationBuilder.DropTable(
            name: "CustomParticipant",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ERROR_LOG");

        migrationBuilder.DropTable(
            name: "Housing_Applicants");

        migrationBuilder.DropTable(
            name: "Housing_HallChoices");

        migrationBuilder.DropTable(
            name: "Housing_Halls");

        migrationBuilder.DropTable(
            name: "Information_Change_Request");

        migrationBuilder.DropTable(
            name: "MatchBracket",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "MatchParticipant",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "MatchTeam",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "MEMBERSHIP");

        migrationBuilder.DropTable(
            name: "ParticipantActivity",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ParticipantNotification",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ParticipantStatusHistory",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "REQUEST");

        migrationBuilder.DropTable(
            name: "SeriesSurface",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "SeriesTeam",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Slider_Images");

        migrationBuilder.DropTable(
            name: "Statistic",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "StudentNewsExpiration");

        migrationBuilder.DropTable(
            name: "Housing_Applications");

        migrationBuilder.DropTable(
            name: "Match",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "MatchTeamStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "PrivType",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ParticipantStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ParticipantTeam",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "MatchStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Series",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Surface",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Participant",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "RoleType",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "SeriesStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "SeriesType",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Team",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Activity",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Affiliation",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "TeamStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ActivityStatus",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "ActivityType",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "SeriesSchedule",
            schema: "RecIM");

        migrationBuilder.DropTable(
            name: "Sport",
            schema: "RecIM");

        migrationBuilder.DropSequence(
            name: "Information_Change_Request_Seq");

        migrationBuilder.Sql("DROP VIEW [dbo].[ACCOUNT]
GO");
    }
}
