namespace PubQuizBackend.Utils
{
    public static class CustomConverter
    {
        private static readonly Dictionary<int, string> _roles = new()
        {
            { 1, "Attendee" },
            { 2, "Organizer" },
            { 3, "Admin" }
        };


        public static string? GetStringRole(int roleId) =>
            _roles.TryGetValue(roleId, out var name) ? name : null;

        public static int? GetIntRole(string roleName)
        {
            var match = _roles.FirstOrDefault(
                r => r.Value.Equals(roleName, StringComparison.OrdinalIgnoreCase)
            );

            return match.Equals(default(KeyValuePair<int, string>)) ? null : match.Key;
        }
    }
}
