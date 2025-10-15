using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Resources
{
    /// <summary>
    /// Datetime Kontrol
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DateTimeElementAttr : ValidationAttribute
    {
        /// <summary>
        /// Valid kontrol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            try
            {
                Convert.ToDateTime(value.ToString());
                return true;
            }
            catch (Exception)
            {
                return false;

            }

        }
    }
}
