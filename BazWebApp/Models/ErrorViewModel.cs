using System;

namespace BazWebApp.Models
{
    /// <summary>
    /// Hata için model
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// RequestId property
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// ShowRequestId property
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
