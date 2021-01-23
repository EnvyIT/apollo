namespace Apollo.Util.Types
{ 
    public class TokenClaims
    {
        public string Uuid { get;}

        public TokenClaims(string uuid)
        {
            Uuid = uuid;
        }
    }
}
