namespace SchoolHub.Data.Common
{
    public static class ModelsValidationConstraints
    {
        public const string DateTimeFormat = "dd-MM-yyyy";
        public const byte SchoolMemberFullNameMinLength = 6;
        public const byte SchoolMemberFullNameMaxLength = 120;

        // School
        public const byte SchoolNameMinLength = 3;
        public const byte SchoolNameMaxLength = 100;
        public const byte SchoolAddressMinLength = 10;
        public const byte SchoolAddressMaxLength = 200;
        public const byte SchoolWebsiteMinLength = 6;
        public const byte SchoolWebsiteMaxLength = 120;

        // Class
        public const byte ClassNameMinLength = 2;
        public const byte ClassNameMaxLength = 50;

        // Subject
        public const byte SubjectNameMinLength = 2;
        public const byte SubjectNameMaxLength = 70;
        public const byte SubjectDescriptionMinLength = 15;
        public const int SubjectDescriptionMaxLength = 625;

        // Category
        public const byte CategoryNameMinLength = 4;
        public const byte CategoryNameMaxLength = 50;

        // Grade
        public const byte GradeScoreMinValue = 2;
        public const byte GradeScoreMaxValue = 6;

        // Remark
        public const byte RemarkDescriptionMinLength = 15;
        public const int RemarkDescriptionMaxLength = 625;

        // Chat
        public const int ChatMessageMaxLength = 600;
    }
}
