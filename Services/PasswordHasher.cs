namespace ValidationDemo.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            // BCrypt automatically generates a salt and hashes the password
            // WorkFactor 11 = good balance between security and performance
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        /// <summary>
        /// Verifies if a plain text password matches the hashed password
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // If hash is invalid, return false
                return false;
            }
        }
    }
}
