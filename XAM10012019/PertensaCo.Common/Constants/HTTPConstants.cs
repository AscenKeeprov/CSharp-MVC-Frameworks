namespace PertensaCo.Common.Constants
{
	public static class HTTPConstants
	{
		public const string AntiforgeryCookieName = "PerCo.AFT";
		public const string ApplicationCookieName = "PerCo.App";
		public const bool AuthenticationCookieIsHttpOnly = true;
		public const bool AuthenticationCookieIsRequired = true;
		public const string AuthenticationCookieName = "PerCo.Sign";
		public const string BinaryMimeType = "application/octet-stream";
		public const bool ConsentCookieIsRequired = true;
		public const string ConsentCookieName = "GDPR.Consent";
		public const string GoogleSmtpServerName = "smtp.gmail.com";
		public const string GoogleSmtpServerPassword = "P3rtensaC0@gma!l";
		public const string GoogleSmtpServerPrincipal = "pertensaco@gmail.com";
		public const int GoogleSmtpServerSSLPort = 465;
		public const int GoogleSmtpServerTLSPort = 587;
		public const string HtmlMimeType = "text/html";
		public const string ImageMimeTypePrefix = "image/";
		public const string JpegMimeType = "image/jpeg";
		public const int MimeTypeMaxLength = 96;
		public const string PngMimeType = "image/png";
		public const string RootCookiePath = "/";
		public const bool SessionCookieIsHttpOnly = true;
		public const bool SessionCookieIsRequired = true;
		public const string SessionCookieName = "PerCo.SID";
		public const int SessionTimeoutInSeconds = 600;
		public const string TextMimeType = "text/plain";
		public const int UrlMaxLength = 2048;
	}
}
