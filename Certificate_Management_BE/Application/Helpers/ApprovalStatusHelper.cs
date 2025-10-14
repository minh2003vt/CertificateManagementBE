using Domain.Enums;
using Domain.Entities;

namespace Application.Helpers
{
    /// <summary>
    /// Helper class for checking approval status of entities
    /// </summary>
    public static class ApprovalStatusHelper
    {
        /// <summary>
        /// Check if an entity can be modified based on its approval status
        /// </summary>
        /// <param name="status">The approval status to check</param>
        /// <returns>True if entity can be modified, false if approved</returns>
        public static bool CanModify(object status)
        {
            return status switch
            {
                SubjectStatus subjectStatus => subjectStatus != SubjectStatus.Approved,
                CourseStatus courseStatus => courseStatus != CourseStatus.Approved,
                PlanStatus planStatus => planStatus != PlanStatus.Approved,
                _ => true // Default to allowing modification for unknown types
            };
        }

        /// <summary>
        /// Get error message for approved entities
        /// </summary>
        /// <param name="entityType">Type of entity (e.g., "Subject", "Course", "Plan")</param>
        /// <returns>Error message for approved entities</returns>
        public static string GetApprovedEntityErrorMessage(string entityType)
        {
            return $"{entityType} has been approved, please request to modify/delete";
        }

        /// <summary>
        /// Check if a subject can be modified
        /// </summary>
        /// <param name="subject">Subject entity</param>
        /// <returns>True if can modify, false if approved</returns>
        public static bool CanModifySubject(Subject subject)
        {
            return CanModify(subject.Status);
        }

        /// <summary>
        /// Check if a course can be modified
        /// </summary>
        /// <param name="course">Course entity</param>
        /// <returns>True if can modify, false if approved</returns>
        public static bool CanModifyCourse(Course course)
        {
            return CanModify(course.Status);
        }

        /// <summary>
        /// Check if a plan can be modified
        /// </summary>
        /// <param name="plan">Plan entity</param>
        /// <returns>True if can modify, false if approved</returns>
        public static bool CanModifyPlan(Plan plan)
        {
            return CanModify(plan.Status);
        }

        /// <summary>
        /// Check if a course-subject-specialty relationship can be modified
        /// </summary>
        /// <param name="courseSubjectSpecialty">CourseSubjectSpecialty entity</param>
        /// <returns>True if can modify, false if approved</returns>
        public static bool CanModifyCourseSubjectSpecialty(CourseSubjectSpecialty courseSubjectSpecialty)
        {
            // CourseSubjectSpecialty doesn't have its own status, 
            // so we need to check the related entities
            return true; // For now, allow modification
        }

        /// <summary>
        /// Validate that an entity can be modified and throw exception if not
        /// </summary>
        /// <param name="entity">The entity to validate</param>
        /// <param name="entityType">Type of entity for error message</param>
        /// <exception cref="InvalidOperationException">Thrown if entity is approved</exception>
        public static void ValidateCanModify(object entity, string entityType)
        {
            var canModify = entity switch
            {
                Subject subject => CanModifySubject(subject),
                Course course => CanModifyCourse(course),
                Plan plan => CanModifyPlan(plan),
                _ => true
            };

            if (!canModify)
            {
                throw new InvalidOperationException(GetApprovedEntityErrorMessage(entityType));
            }
        }
    }
}


