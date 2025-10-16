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
        Rejected = 2,
        finished = 3
    }


    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
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
    public enum  GradeKind
    {
     TotalScore = 0,
     PracticeExamScore = 1,
     FinalExamScore = 2,
     ResitPracticeExamScore = 3,
     ResitFinalExamScore = 4,
     Attendance = 5
    }
    public enum GradeStatus
    {
        Pass = 1,
        Fail = 0
    }
    public enum OverallGradeStatus
    {
        Pending = -1,
        Pass = 1,
        Fail = 0
    }

    public enum AssignmentKind
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
        ModifyPlan = 1,
        // Course Management
        NewCourse = 2,
        ModifyCourse = 3,

        newSubject = 4,
        modifySubject = 5,
        // Trainee Management
        NewClass = 6,
        modifyTraineeAssign = 7,
        // Template Management
        CertificateTemplate = 8,
        DecisionTemplate = 9,
        NewMatrix=10,
        RemoveMatrix=11,
        // General Actions
        Complaint = 12,
        SignRequest = 13,
        Revoke = 14
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