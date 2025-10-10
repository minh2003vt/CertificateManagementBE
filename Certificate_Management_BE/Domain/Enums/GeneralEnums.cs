namespace Domain.Enums
{
   public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public enum AccountStatus
    {
        Pending = -1,
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
    public enum SubjectStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
    public enum PlanStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }


    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Updating = 3,
        Deleting = 4
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
    
    public enum GradeKind
    {
        Initial = 0,
        Relearn=1,
        Reccurent= 2

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
}