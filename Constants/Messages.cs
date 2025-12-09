namespace ValidationDemo.Constants
{
    public class Messages
    {
        // Registration
        public const string UserNotFound = "User not found.";
        public const string UsernameRequired = "Username is required";
        public const string UsernameExists = "Username already exists";
        public const string EmailRequired = "Email is required";
        public const string EmailExists = "Email already exists";
        public const string PasswordRequired = "Password is required";
        public const string UserRegistered = "User {0} registered successfully";

        // Update
        public const string UserUpdated = "User {0} updated successfully";

        // Delete / Restore
        public const string UserDeleted = "User {0} has been deleted successfully";
        public const string UserAlreadyDeleted = "User is already deleted";
        public const string UserRestored = "User {0} has been restored successfully";
        public const string FailedToDeleteUser = "Failed to delete user";
        public const string FailedToRestoreUser = "Failed to restore user";

        // Validation helpers
        public const string Empty = "";

        // Repository messages
        public const string RepoUserNotFound = "User with the specified ID was not found.";
        public const string RepoUserByUsernameNotFound = "User with the specified username was not found.";
        public const string RepoUserByEmailNotFound = "User with the specified email was not found.";


    }
}
