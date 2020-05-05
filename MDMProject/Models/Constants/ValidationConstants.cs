namespace MDMProject.Models
{
    public static class ValidationConstants
    {
        public static class User
        {
            public const int MIN_PASSWORD_LENGTH = 8;
            public const int MAX_PASSWORD_LENGTH = 100;
            public const int MAX_EMAIL_LENGTH = 256;
            public const int MIN_CONTACT_NAME_LENGTH = 3;
            public const int MAX_CONTACT_NAME_LENGTH = 256;
            public const int MIN_COMPANY_NAME_LENGTH = 3;
            public const int MAX_COMPANY_NAME_LENGTH = 256;
            public const int MAX_PHONE_NUMBER_LENGTH = 50;
            public const int MAX_ADDITIONAL_COMMENT_LENGTH = 256;
            public const int MAX_COORDINATOR_DETAILS_LENGTH = 256;
            public const int MIN_COORDINATED_REGION_LENGTH = 3;
            public const int MAX_COORDINATED_REGION_LENGTH = 50;
        }

        public static class Address
        {
            public const int MAX_CITY_LENGTH = 256;
            public const int MAX_STREET_NAME_LENGTH = 256;
            public const int MAX_HOUSE_NUMBER_LENGTH = 50;
            public const int MAX_FLAT_NUMBER_LENGTH = 50;
            public const int POSTAL_CODE_LENGTH = 6;
            public const int MAX_LATITUDE_LENGTH = 256;
            public const int MAX_LONGITUDE_LENGTH = 256;
        }

        public static class EquipmentType
        {
            public const int MAX_NAME_LENGTH = 50;
        }

        public static class HelpType
        {
            public const int MAX_NAME_LENGTH = 50;
        }
        public static class OfferedHelp
        {
            public const int MAX_NAME_LENGTH = 50;
            public const int MAX_DESCRIPTION_LENGTH = 256;
        }
        public static class ProtectiveEquipment
        {
            public const int MAX_NAME_LENGTH = 50;
            public const int MAX_COMMENT_LENGTH = 256;
        }
    }
}