using System;

namespace BarionClientLibrary
{
    /// <summary>
    /// Barion specific settings.
    /// </summary>
    public class BarionSettings
    {
        /// <summary>
        /// The private key of the shop.
        /// </summary>
        public Guid POSKey { get; set; }

        /// <summary>
        /// The base address of the Barion API.
        /// </summary>
        public Uri BaseUrl { get; set; }

        /// <summary>
        /// The email address of the Barion user who will receive the payments.
        /// </summary>
        public string? Payee { get; set; }

        public BarionSettings(Guid posKey, Uri? baseUrl = null)
        {
            POSKey = posKey;
            BaseUrl = baseUrl ?? new Uri("https://api.barion.com/");
        }
    }
}