namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class AccessToken
  {
    /// <summary>
    /// The access token.
    /// </summary>
    public string access_token { get; set; }

    /// <summary>
    /// The type of the token.
    /// </summary>
    public string token_type { get; set; }

    /// <summary>
    /// The token expiry in seconds.
    /// </summary>
    public int expires_in { get; set; }
  }
}
