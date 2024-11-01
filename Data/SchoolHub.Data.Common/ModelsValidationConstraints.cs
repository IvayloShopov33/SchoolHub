﻿namespace SchoolHub.Data.Common
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
        public const byte ClassNameMinLength = 3;
        public const byte ClassNameMaxLength = 50;

        // Subject
        public const byte SubjectNameMinLength = 2;
        public const byte SubjectNameMaxLength = 70;
        public const byte SubjectDescriptionMinLength = 15;
        public const int SubjectDescriptionMaxLength = 625;

        // Topic
        public const byte TopicNameMinLength = 7;
        public const byte TopicNameMaxLength = 60;

        // Category
        public const byte CategoryNameMinLength = 4;
        public const byte CategoryNameMaxLength = 50;

        // Remark
        public const byte RemarkDescriptionMinLength = 15;
        public const int RemarkDescriptionMaxLength = 625;
    }
}
