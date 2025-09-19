namespace Domain.Enums
{
    public enum Location
    {
        SectionA = 0,
        SectionB = 1
    }

    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public enum Room
    {
        // First floor
        R001 = 0, R002 = 1, R003 = 2, R004 = 3, R005 = 4, R006 = 5, R007 = 6, R008 = 7, R009 = 8,
        R101 = 9, R102 = 10, R103 = 11, R104 = 12, R105 = 13, R106 = 14, R107 = 15, R108 = 16, R109 = 17,
        // Second floor
        R201 = 18, R202 = 19, R203 = 20, R204 = 21, R205 = 22, R206 = 23, R207 = 24, R208 = 25, R209 = 26,
        R301 = 27, R302 = 28, R303 = 29, R304 = 30, R305 = 31, R306 = 32, R307 = 33, R308 = 34, R309 = 35,
        // Third floor
        R401 = 36, R402 = 37, R403 = 38, R404 = 39, R405 = 40, R406 = 41, R407 = 42, R408 = 43, R409 = 44,
        // Fourth floor
        R501 = 45, R502 = 46, R503 = 47, R504 = 48, R505 = 49, R506 = 50, R507 = 51, R508 = 52, R509 = 53
    }
    public enum AccountStatus
    {
        Active = 1,
        Deactivated = 0
    }

    public enum CertificateStatus
    {
        Active = 1,
        Expired = 2,
        Revoked = 3,
        Pending = 0
    }

    public enum CourseStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum CourseLevel
    {
        Initial = 0,
        Recurrent = 1,
        Relearn = 2,
        Professional = 3
    }

    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Updating = 3,
        Deleting = 4
    }

    public enum Progress
    {
        NotYet = 0,
        Ongoing = 1,
        Completed = 2
    }
    public enum DepartmentStatus
    {
        Active = 0,
        Inactive = 1
    }
    public enum CandidateStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
    }
    public enum VerificationStatus
    {
        Pending = 0,
        Verified = 1, Rejected = 2
    }
    public enum PlanLevel
    {
        Initial = 0,
        Recurrent = 1,
        Relearn = 2
    }

    public enum GradeStatus
    {
        Pending = -1,
        Pass = 1,
        Fail = 0
    }
    public enum GradeComponentType
    {
        Participation,
        ProgressTest,
        Assignments,
        GroupProject,
        FinalExam,
        PracticalExam,
        FinalExamResit,
        PracticalExamResit
    }

    public enum DigitalSignatureStatus
    {
        Active = 1, Expired = 0, Revoked = 2
    }

    public enum TemplateStatus
    {
        Active = 1, Inactive = 0
    }
    public enum RequestType
    {
        // Plan Management
        NewPlan = 0,

        // Course Management
        NewCourse = 1,
        UpdateCourse = 2,
        DeleteCourse = 3,

        // Instructor Management
        AssignInstructor = 4,

        // Class Management
        ClassSchedule = 5,

        // Trainee Management
        AssignTrainee = 6,
        AddTraineeAssignation = 7,

        // Template Management
        CertificateTemplate = 8,
        DecisionTemplate = 9,

        // Creation Types
        CreateNew = 10,
        CreateRecurrent = 11,
        CreateRelearn = 12,
        CandidateImport = 13,

        // General Actions
        Complaint = 14,
        Update = 15,
        Delete = 16,
        SignRequest = 17,
        Revoke = 18
    }
    public enum DecisionStatus
    {
        Draft = 0, Signed = 1, Revoked = 2
    }
    public enum ReportType
    {
        ExpiredCertificate = 1, CourseResult = 2, TraineeResult = 3, PlanResult = 4
    }
    public enum ResultStatus
    {
        Draft = 0, Submitted = 1, Approved = 2, Rejected = 3
    }
    public enum ScheduleStatus
    {
        Pending = 0, Approved = 1, Incoming = 2, Canceled = 3, Completed = 4
    }
}