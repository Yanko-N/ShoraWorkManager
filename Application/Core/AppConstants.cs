namespace Application.Core
{
    public static class AppConstants
    {
        public static class General
        {
            public const int MAX_TRIES = 3;
        }

        public static class Roles
        {
            public const string ADMIN = "Admin";
            public const string USER = "User";

            public const string ALL_ROLES = ADMIN + "," + USER;
        }

        public static class FilePaths
        {
            public const string INDEX_JSON = "wwwroot/Documentos/IndexJson";
            public const string INDEX_PHOTOS = "wwwroot/Documentos/IndexPhotos";
            public const string INDEX_PHOTOS_URL = "/Documentos/IndexPhotos/";
            public static class IndexSections
            {
                public const string ONE = "IndexOne.json";
                public const string TWO = "IndexTwo.json";
                public const string THREE = "IndexThree.json";
            }
            public static class IndexPhotos
            {
                public const string ONE = "IndexOne";
                public const string TWO = "IndexTwo";
                public const string THREE = "IndexThree";
            }
        }
        
    }
}
