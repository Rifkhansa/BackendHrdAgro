namespace BackendHrdAgro.Models
{
    public class AuthorizationFactory
    {
        static readonly string[] Exclusive = new string[] { "DP006", "TL019", "0808003", "DS002", "DS003" };
        public static bool Entrance(string id) => Exclusive.Contains(id);
        public static bool Exit(string id) => id == "DS006";
    }
}
