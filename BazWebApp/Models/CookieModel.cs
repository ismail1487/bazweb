using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Models
{
    /// <summary>
    /// Kullanıcı cookie bilgisinin içerisinde barınacak olan bilgileri içeren nesnedir.
    /// </summary>
    public class CookieModel
    {
        /// <summary>
        /// KisiId property
        /// </summary>
        public string KisiId { get; set; }

        /// <summary>
        /// KurumId property
        /// </summary>
        public string KurumId { get; set; }

        /// <summary>
        /// Mail property
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Name property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// KurumName property
        /// </summary>
        public string KurumName { get; set; }

        /// <summary>
        /// SessionId property
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// MusteriTemsilcisiIdList property
        /// </summary>
        public List<int> MusteriTemsilcisiIdList { get; set; }

        /// <summary>
        /// Lisans bitiş tarihi
        /// </summary>
        //public DateTime? LisansBitisTarihi { get; set; }

        public bool AdminMi { get; set; }
    }
}