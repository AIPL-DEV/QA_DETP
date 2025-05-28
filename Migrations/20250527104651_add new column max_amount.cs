using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DETP.Migrations
{
    public partial class addnewcolumnmax_amount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "login_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    ip = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qa_att",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_att", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_step",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Step = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_step", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "sha_request_image",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sha_request_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sha_request_image", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_sub_categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QaViolationCategoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_sub_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violation_sub_categories_qa_violation_categories_QaViolationCategoryId",
                        column: x => x.QaViolationCategoryId,
                        principalTable: "qa_violation_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_flow",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaViolationId = table.Column<long>(type: "bigint", nullable: false),
                    FromId = table.Column<int>(type: "int", nullable: true),
                    ToId = table.Column<int>(type: "int", nullable: true),
                    ToRoleId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Completed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_flow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violation_flow_Role_ToRoleId",
                        column: x => x.ToRoleId,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "penalty_details",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialPenalty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Administrative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QaViolationCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    QaViolationSubCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Max_amount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_penalty_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_penalty_details_qa_violation_categories_QaViolationCategoryId",
                        column: x => x.QaViolationCategoryId,
                        principalTable: "qa_violation_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_penalty_details_qa_violation_sub_categories_QaViolationSubCategoryId",
                        column: x => x.QaViolationSubCategoryId,
                        principalTable: "qa_violation_sub_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department = table.Column<int>(type: "int", nullable: true),
                    app = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    force_reset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    noted_mail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "assignee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    ovservation_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    root_cause_analysis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    corrective_action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    preventive_action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value_of_rectification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    time_loss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    time_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignee", x => x.id);
                    table.ForeignKey(
                        name: "FK_assignee_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "business_head",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    assign_to = table.Column<int>(type: "int", nullable: true),
                    decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    input = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    target_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: false),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_head", x => x.id);
                    table.ForeignKey(
                        name: "FK_business_head_users_assign_to",
                        column: x => x.assign_to,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_business_head_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dept_hod",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    valuerec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeloss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    within_target_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    assign_to = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dept_hod", x => x.id);
                    table.ForeignKey(
                        name: "FK_dept_hod_users_assign_to",
                        column: x => x.assign_to,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dept_hod_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionHeadId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Divisions_users_DivisionHeadId",
                        column: x => x.DivisionHeadId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "eic_detp",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: true),
                    flow_id = table.Column<int>(type: "int", nullable: true),
                    decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    non_conformance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eic_detp", x => x.id);
                    table.ForeignKey(
                        name: "FK_eic_detp_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "head_detp",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: true),
                    flow_id = table.Column<int>(type: "int", nullable: true),
                    decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    non_conformance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_head_detp", x => x.id);
                    table.ForeignKey(
                        name: "FK_head_detp_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hod_qa_sha",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: true),
                    flow_id = table.Column<int>(type: "int", nullable: true),
                    decision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    non_conformance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hod_qa_sha", x => x.id);
                    table.ForeignKey(
                        name: "FK_hod_qa_sha_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "project_incharge",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    valuerec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeloss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    within_target_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_incharge", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_incharge_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qa_officer",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    compliance_satisfactory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    within_slg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_officer", x => x.id);
                    table.ForeignKey(
                        name: "FK_qa_officer_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_approval",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaViolationId = table.Column<long>(type: "bigint", nullable: false),
                    QaViolationFlowId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PenaltyClauseCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PenaltyAmountCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_approval", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violation_approval_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qa_violation_approval_users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_cfo_review",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaViolationId = table.Column<long>(type: "bigint", nullable: false),
                    QaViolationFlowId = table.Column<long>(type: "bigint", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeducationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebitNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_cfo_review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violation_cfo_review_users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qa_violation_head_procurement",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QaViolationId = table.Column<long>(type: "bigint", nullable: false),
                    QaViolationFlowId = table.Column<long>(type: "bigint", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violation_head_procurement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violation_head_procurement_users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_incharge",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: false),
                    flow_id = table.Column<int>(type: "int", nullable: false),
                    valuerec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeloss = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    within_target_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    decision_by = table.Column<int>(type: "int", nullable: true),
                    decision_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_incharge", x => x.id);
                    table.ForeignKey(
                        name: "FK_site_incharge_users_decision_by",
                        column: x => x.decision_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_user_role_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_role_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    department_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    department_abbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_on = table.Column<DateTime>(type: "datetime2", nullable: true),
                    division_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => x.department_id);
                    table.ForeignKey(
                        name: "FK_department_Divisions_division_id",
                        column: x => x.division_id,
                        principalTable: "Divisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qa_observation",
                columns: table => new
                {
                    serial_no = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    visit_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    logged_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    site = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nature_of_work = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type_of_observation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    log_non_confirmance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    log_confirmance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    compliance_target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    type_of_confirmance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nature_of_confirmance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    standard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    basics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vendor_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vendor_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    p_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    site_incharge = table.Column<int>(type: "int", nullable: true),
                    head_detp = table.Column<int>(type: "int", nullable: true),
                    project_incharge = table.Column<int>(type: "int", nullable: true),
                    dept_hod = table.Column<int>(type: "int", nullable: true),
                    business_head = table.Column<int>(type: "int", nullable: true),
                    qa_officer = table.Column<int>(type: "int", nullable: true),
                    observation_by = table.Column<int>(type: "int", nullable: true),
                    observation_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    division_id = table.Column<long>(type: "bigint", nullable: true),
                    number_of_observation = table.Column<int>(type: "int", nullable: true),
                    area_of_concern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hod_qa_sha = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_observation", x => x.serial_no);
                    table.ForeignKey(
                        name: "FK_qa_observation_Divisions_division_id",
                        column: x => x.division_id,
                        principalTable: "Divisions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_hod_qa_sha_hod_qa_sha",
                        column: x => x.hod_qa_sha,
                        principalTable: "hod_qa_sha",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_business_head",
                        column: x => x.business_head,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_dept_hod",
                        column: x => x.dept_hod,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_head_detp",
                        column: x => x.head_detp,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_observation_by",
                        column: x => x.observation_by,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_project_incharge",
                        column: x => x.project_incharge,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_qa_officer",
                        column: x => x.qa_officer,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_observation_users_site_incharge",
                        column: x => x.site_incharge,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qa_flow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    observation_id = table.Column<int>(type: "int", nullable: true),
                    table_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_id = table.Column<int>(type: "int", nullable: true),
                    to_id = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    completed = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_flow", x => x.id);
                    table.ForeignKey(
                        name: "FK_qa_flow_qa_observation_observation_id",
                        column: x => x.observation_id,
                        principalTable: "qa_observation",
                        principalColumn: "serial_no",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_flow_users_from_id",
                        column: x => x.from_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_qa_flow_users_to_id",
                        column: x => x.to_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "qa_violations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObservationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    ObservationId = table.Column<int>(type: "int", nullable: false),
                    QaViolationCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    QaViolationSubCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qa_violations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_qa_violations_qa_observation_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "qa_observation",
                        principalColumn: "serial_no",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qa_violations_qa_violation_categories_QaViolationCategoryId",
                        column: x => x.QaViolationCategoryId,
                        principalTable: "qa_violation_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qa_violations_qa_violation_sub_categories_QaViolationSubCategoryId",
                        column: x => x.QaViolationSubCategoryId,
                        principalTable: "qa_violation_sub_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_qa_violations_users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assignee_decision_by",
                table: "assignee",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_business_head_assign_to",
                table: "business_head",
                column: "assign_to");

            migrationBuilder.CreateIndex(
                name: "IX_business_head_decision_by",
                table: "business_head",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_department_division_id",
                table: "department",
                column: "division_id");

            migrationBuilder.CreateIndex(
                name: "IX_dept_hod_assign_to",
                table: "dept_hod",
                column: "assign_to");

            migrationBuilder.CreateIndex(
                name: "IX_dept_hod_decision_by",
                table: "dept_hod",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_DivisionHeadId",
                table: "Divisions",
                column: "DivisionHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_eic_detp_decision_by",
                table: "eic_detp",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_head_detp_decision_by",
                table: "head_detp",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_hod_qa_sha_decision_by",
                table: "hod_qa_sha",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_penalty_details_QaViolationCategoryId",
                table: "penalty_details",
                column: "QaViolationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_penalty_details_QaViolationSubCategoryId",
                table: "penalty_details",
                column: "QaViolationSubCategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_incharge_decision_by",
                table: "project_incharge",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_qa_flow_from_id",
                table: "qa_flow",
                column: "from_id");

            migrationBuilder.CreateIndex(
                name: "IX_qa_flow_observation_id",
                table: "qa_flow",
                column: "observation_id");

            migrationBuilder.CreateIndex(
                name: "IX_qa_flow_to_id",
                table: "qa_flow",
                column: "to_id");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_business_head",
                table: "qa_observation",
                column: "business_head");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_dept_hod",
                table: "qa_observation",
                column: "dept_hod");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_division_id",
                table: "qa_observation",
                column: "division_id");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_head_detp",
                table: "qa_observation",
                column: "head_detp");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_hod_qa_sha",
                table: "qa_observation",
                column: "hod_qa_sha");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_observation_by",
                table: "qa_observation",
                column: "observation_by");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_project_incharge",
                table: "qa_observation",
                column: "project_incharge");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_qa_officer",
                table: "qa_observation",
                column: "qa_officer");

            migrationBuilder.CreateIndex(
                name: "IX_qa_observation_site_incharge",
                table: "qa_observation",
                column: "site_incharge");

            migrationBuilder.CreateIndex(
                name: "IX_qa_officer_decision_by",
                table: "qa_officer",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_approval_ApprovedById",
                table: "qa_violation_approval",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_approval_RoleId",
                table: "qa_violation_approval",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_cfo_review_ApprovedById",
                table: "qa_violation_cfo_review",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_flow_ToRoleId",
                table: "qa_violation_flow",
                column: "ToRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_head_procurement_ApprovedById",
                table: "qa_violation_head_procurement",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violation_sub_categories_QaViolationCategoryId",
                table: "qa_violation_sub_categories",
                column: "QaViolationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violations_CreatedById",
                table: "qa_violations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violations_ObservationId",
                table: "qa_violations",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violations_QaViolationCategoryId",
                table: "qa_violations",
                column: "QaViolationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_qa_violations_QaViolationSubCategoryId",
                table: "qa_violations",
                column: "QaViolationSubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_site_incharge_decision_by",
                table: "site_incharge",
                column: "decision_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_role_id",
                table: "user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_user_id",
                table: "user_role",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_department",
                table: "users",
                column: "department");

            migrationBuilder.AddForeignKey(
                name: "FK_users_department_department",
                table: "users",
                column: "department",
                principalTable: "department",
                principalColumn: "department_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_users_DivisionHeadId",
                table: "Divisions");

            migrationBuilder.DropTable(
                name: "assignee");

            migrationBuilder.DropTable(
                name: "business_head");

            migrationBuilder.DropTable(
                name: "dept_hod");

            migrationBuilder.DropTable(
                name: "eic_detp");

            migrationBuilder.DropTable(
                name: "head_detp");

            migrationBuilder.DropTable(
                name: "login_logs");

            migrationBuilder.DropTable(
                name: "penalty_details");

            migrationBuilder.DropTable(
                name: "project_incharge");

            migrationBuilder.DropTable(
                name: "qa_att");

            migrationBuilder.DropTable(
                name: "qa_flow");

            migrationBuilder.DropTable(
                name: "qa_officer");

            migrationBuilder.DropTable(
                name: "qa_violation_approval");

            migrationBuilder.DropTable(
                name: "qa_violation_cfo_review");

            migrationBuilder.DropTable(
                name: "qa_violation_flow");

            migrationBuilder.DropTable(
                name: "qa_violation_head_procurement");

            migrationBuilder.DropTable(
                name: "qa_violation_step");

            migrationBuilder.DropTable(
                name: "qa_violations");

            migrationBuilder.DropTable(
                name: "sha_request_image");

            migrationBuilder.DropTable(
                name: "site_incharge");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "qa_observation");

            migrationBuilder.DropTable(
                name: "qa_violation_sub_categories");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "hod_qa_sha");

            migrationBuilder.DropTable(
                name: "qa_violation_categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
