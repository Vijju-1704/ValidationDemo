namespace ValidationDemo.Constants
{
    public static class AppClaims
    {
        // Permission Claims
        public const string CanEditUsers = "Permission.User.Edit";
        public const string CanDeleteUsers = "Permission.User.Delete";
        public const string CanViewUsers = "Permission.User.View";
        public const string CanViewDeletedUsers = "Permission.User.ViewDeleted";
        public const string CanRestoreUsers = "Permission.User.Restore";

        // User-specific Claims
        public const string UserId = "UserId";
        public const string Role = "Role";
    }
}