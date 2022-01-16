using System;

namespace Appts.Common.Constants
{
  /// <summary>
  /// Constants for Identity related use.
  /// </summary>
  public static class IdentityK
  {
    public const string NameIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    //public const string IsSubscriber = "ext_isSub";
    public const string OrganizationId = "ext_orgId";
    public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    // from fb                   http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress

    public const string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
    public const string SurName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
    
    // Either sp or client
    public const string Type = "Type";
   
    /// paid, trial, wassp
    public const string SubStat = "SubStat";
  }
}
