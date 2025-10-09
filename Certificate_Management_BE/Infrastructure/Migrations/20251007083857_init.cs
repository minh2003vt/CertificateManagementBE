using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    CertificateCode = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CertificateTemplateId = table.Column<string>(type: "text", nullable: false),
                    IssuedByUserId = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SignDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CertificateUrl = table.Column<string>(type: "text", nullable: false),
                    RevocationReason = table.Column<string>(type: "text", nullable: true),
                    RevocationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IncludesRelearn = table.Column<bool>(type: "boolean", nullable: false),
                    RelearnSubjects = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                });

            migrationBuilder.CreateTable(
                name: "CertificateTemplates",
                columns: table => new
                {
                    CertificateTemplateId = table.Column<string>(type: "text", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TemplateFile = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    TemplateStatus = table.Column<string>(type: "text", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTemplates", x => x.CertificateTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InstructorId = table.Column<string>(type: "text", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassId);
                });

            migrationBuilder.CreateTable(
                name: "ClassTraineeAssignations",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    TraineeAssignationId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTraineeAssignations", x => new { x.ClassId, x.TraineeAssignationId });
                    table.ForeignKey(
                        name: "FK_ClassTraineeAssignations_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "ClassId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCertificates",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCertificates", x => new { x.CertificateId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CourseCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "text", nullable: false),
                    CourseName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CourseLevel = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Progress = table.Column<string>(type: "text", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

            migrationBuilder.CreateTable(
                name: "CourseSubjectSpecialties",
                columns: table => new
                {
                    SpecialtyId = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    CourseId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSubjectSpecialties", x => new { x.SpecialtyId, x.SubjectId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_CourseSubjectSpecialties_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Decisions",
                columns: table => new
                {
                    DecisionId = table.Column<string>(type: "text", nullable: false),
                    DecisionCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IssuedByUserId = table.Column<string>(type: "text", nullable: false),
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    DecisionTemplateId = table.Column<string>(type: "text", nullable: false),
                    SignDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DecisionStatus = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decisions", x => x.DecisionId);
                    table.ForeignKey(
                        name: "FK_Decisions_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DecisionTemplates",
                columns: table => new
                {
                    DecisionTemplateId = table.Column<string>(type: "text", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: false),
                    TemplateContent = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TemplateStatus = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionTemplates", x => x.DecisionTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<string>(type: "text", nullable: false),
                    DepartmentName = table.Column<string>(type: "text", nullable: false),
                    DepartmentDescription = table.Column<string>(type: "text", nullable: false),
                    SpecialtyId = table.Column<string>(type: "text", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Sex = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CitizenId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalCertificates",
                columns: table => new
                {
                    ExternalCertificateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateCode = table.Column<string>(type: "text", nullable: false),
                    CertificateName = table.Column<string>(type: "text", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Exp_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateFileUrl = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalCertificates", x => x.ExternalCertificateId);
                    table.ForeignKey(
                        name: "FK_ExternalCertificates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<string>(type: "text", nullable: false),
                    ReportName = table.Column<string>(type: "text", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    GeneratedByUserId = table.Column<string>(type: "text", nullable: false),
                    GenerateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_Users_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<string>(type: "text", nullable: false),
                    RequestUserId = table.Column<string>(type: "text", nullable: false),
                    RequestType = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Requests_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Requests_Users_RequestUserId",
                        column: x => x.RequestUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyId = table.Column<string>(type: "text", nullable: false),
                    SpecialtyName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyId);
                    table.ForeignKey(
                        name: "FK_Specialties_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Specialties_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    SubjectName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MinAttendance = table.Column<int>(type: "integer", nullable: true),
                    MinPracticeExamScore = table.Column<double>(type: "double precision", nullable: true),
                    MinFinalExamScore = table.Column<double>(type: "double precision", nullable: true),
                    MinTotalScore = table.Column<double>(type: "double precision", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.SubjectId);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanId = table.Column<string>(type: "text", nullable: false),
                    PlanName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: true),
                    SpecialtyId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.PlanId);
                    table.ForeignKey(
                        name: "FK_Plans_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Plans_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserSpecialty",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SpecialtyId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpecialty", x => new { x.UserId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_UserSpecialty_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSpecialty_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorAssignations",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    InstructorId = table.Column<string>(type: "text", nullable: false),
                    AssignedByUserId = table.Column<string>(type: "text", nullable: false),
                    AssignDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RequestStatus = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorAssignations", x => new { x.SubjectId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_InstructorAssignations_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorAssignations_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstructorAssignations_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCertificates",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCertificates", x => new { x.CertificateId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_SubjectCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectCertificates_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraineeAssignations",
                columns: table => new
                {
                    TraineeAssignationId = table.Column<string>(type: "text", nullable: false),
                    TraineeId = table.Column<string>(type: "text", nullable: false),
                    RequestStatus = table.Column<string>(type: "text", nullable: false),
                    AssignedByUserId = table.Column<string>(type: "text", nullable: true),
                    AssignDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "text", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestId = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    TotalScore = table.Column<double>(type: "double precision", nullable: false),
                    Attendance = table.Column<int>(type: "integer", nullable: true),
                    PracticeExamScore = table.Column<double>(type: "double precision", nullable: true),
                    FinalExamScore = table.Column<double>(type: "double precision", nullable: true),
                    ResitPracticeExamScore = table.Column<double>(type: "double precision", nullable: true),
                    ResitFinalExamScore = table.Column<double>(type: "double precision", nullable: true),
                    GradeStatus = table.Column<string>(type: "text", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    GradedByInstructorId = table.Column<string>(type: "text", nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraineeAssignations", x => x.TraineeAssignationId);
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Users_GradedByInstructorId",
                        column: x => x.GradedByInstructorId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_TraineeAssignations_Users_TraineeId",
                        column: x => x.TraineeId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanCertificates",
                columns: table => new
                {
                    CertificateId = table.Column<string>(type: "text", nullable: false),
                    PlanId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanCertificates", x => new { x.CertificateId, x.PlanId });
                    table.ForeignKey(
                        name: "FK_PlanCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanCertificates_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyRecords",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "text", nullable: false),
                    PlanId = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyRecords", x => new { x.CourseId, x.PlanId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_StudyRecords_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyRecords_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyRecords_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_ApprovedByUserId",
                table: "Certificates",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateCode",
                table: "Certificates",
                column: "CertificateCode");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateTemplateId",
                table: "Certificates",
                column: "CertificateTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_IssuedByUserId",
                table: "Certificates",
                column: "IssuedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserId",
                table: "Certificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTemplates_ApprovedByUserId",
                table: "CertificateTemplates",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTemplates_CreatedByUserId",
                table: "CertificateTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_InstructorId",
                table: "Classes",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTraineeAssignations_ClassId",
                table: "ClassTraineeAssignations",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTraineeAssignations_TraineeAssignationId",
                table: "ClassTraineeAssignations",
                column: "TraineeAssignationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_CertificateId",
                table: "CourseCertificates",
                column: "CertificateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCertificates_CourseId",
                table: "CourseCertificates",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedByUserId",
                table: "Courses",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSubjectSpecialties_CourseId",
                table: "CourseSubjectSpecialties",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSubjectSpecialties_SpecialtyId",
                table: "CourseSubjectSpecialties",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSubjectSpecialties_SubjectId",
                table: "CourseSubjectSpecialties",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_CertificateId",
                table: "Decisions",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_DecisionTemplateId",
                table: "Decisions",
                column: "DecisionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_IssuedByUserId",
                table: "Decisions",
                column: "IssuedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Decisions_UserId",
                table: "Decisions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionTemplates_ApprovedByUserId",
                table: "DecisionTemplates",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionTemplates_CreatedByUserId",
                table: "DecisionTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_SpecialtyId",
                table: "Departments",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCertificates_CertificateCode",
                table: "ExternalCertificates",
                column: "CertificateCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalCertificates_UserId",
                table: "ExternalCertificates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAssignations_AssignedByUserId",
                table: "InstructorAssignations",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAssignations_InstructorId",
                table: "InstructorAssignations",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorAssignations_SubjectId",
                table: "InstructorAssignations",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanCertificates_CertificateId",
                table: "PlanCertificates",
                column: "CertificateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanCertificates_PlanId",
                table: "PlanCertificates",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_CreatedByUserId",
                table: "Plans",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_SpecialtyId",
                table: "Plans",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_GeneratedByUserId",
                table: "Reports",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ApprovedByUserId",
                table: "Requests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestDate",
                table: "Requests",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestUserId",
                table: "Requests",
                column: "RequestUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_CreatedByUserId",
                table: "Specialties",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Specialties_UpdatedByUserId",
                table: "Specialties",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRecords_CourseId",
                table: "StudyRecords",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRecords_PlanId",
                table: "StudyRecords",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyRecords_SubjectId",
                table: "StudyRecords",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCertificates_CertificateId",
                table: "SubjectCertificates",
                column: "CertificateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCertificates_SubjectId",
                table: "SubjectCertificates",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CreatedByUserId",
                table: "Subjects",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_ApprovedByUserId",
                table: "TraineeAssignations",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_AssignedByUserId",
                table: "TraineeAssignations",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_GradedByInstructorId",
                table: "TraineeAssignations",
                column: "GradedByInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_RequestId",
                table: "TraineeAssignations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_SubjectId",
                table: "TraineeAssignations",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TraineeAssignations_TraineeId",
                table: "TraineeAssignations",
                column: "TraineeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecialty_SpecialtyId",
                table: "UserSpecialty",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecialty_UserId",
                table: "UserSpecialty",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_CertificateTemplates_CertificateTemplateId",
                table: "Certificates",
                column: "CertificateTemplateId",
                principalTable: "CertificateTemplates",
                principalColumn: "CertificateTemplateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Users_ApprovedByUserId",
                table: "Certificates",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Users_IssuedByUserId",
                table: "Certificates",
                column: "IssuedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Users_UserId",
                table: "Certificates",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateTemplates_Users_ApprovedByUserId",
                table: "CertificateTemplates",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CertificateTemplates_Users_CreatedByUserId",
                table: "CertificateTemplates",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Users_InstructorId",
                table: "Classes",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTraineeAssignations_TraineeAssignations_TraineeAssigna~",
                table: "ClassTraineeAssignations",
                column: "TraineeAssignationId",
                principalTable: "TraineeAssignations",
                principalColumn: "TraineeAssignationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseCertificates_Courses_CourseId",
                table: "CourseCertificates",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_CreatedByUserId",
                table: "Courses",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubjectSpecialties_Specialties_SpecialtyId",
                table: "CourseSubjectSpecialties",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "SpecialtyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSubjectSpecialties_Subjects_SubjectId",
                table: "CourseSubjectSpecialties",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "SubjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_DecisionTemplates_DecisionTemplateId",
                table: "Decisions",
                column: "DecisionTemplateId",
                principalTable: "DecisionTemplates",
                principalColumn: "DecisionTemplateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_Users_IssuedByUserId",
                table: "Decisions",
                column: "IssuedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Decisions_Users_UserId",
                table: "Decisions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DecisionTemplates_Users_ApprovedByUserId",
                table: "DecisionTemplates",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DecisionTemplates_Users_CreatedByUserId",
                table: "DecisionTemplates",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Specialties_SpecialtyId",
                table: "Departments",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "SpecialtyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ManagerId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Users_CreatedByUserId",
                table: "Specialties");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialties_Users_UpdatedByUserId",
                table: "Specialties");

            migrationBuilder.DropTable(
                name: "ClassTraineeAssignations");

            migrationBuilder.DropTable(
                name: "CourseCertificates");

            migrationBuilder.DropTable(
                name: "CourseSubjectSpecialties");

            migrationBuilder.DropTable(
                name: "Decisions");

            migrationBuilder.DropTable(
                name: "ExternalCertificates");

            migrationBuilder.DropTable(
                name: "InstructorAssignations");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PlanCertificates");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "StudyRecords");

            migrationBuilder.DropTable(
                name: "SubjectCertificates");

            migrationBuilder.DropTable(
                name: "UserSpecialty");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "TraineeAssignations");

            migrationBuilder.DropTable(
                name: "DecisionTemplates");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "CertificateTemplates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Specialties");
        }
    }
}
