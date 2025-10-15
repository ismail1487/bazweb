using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baz.Model.Entity.Resources
{
    //[AttributeUsage(AttributeTargets.Property)]
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class EnsureOneElementAttribute : ValidationAttribute
    {
        /// <summary>
        /// Valid kontrol
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var list = value as IList;
            if (list != null)
            {
                return list.Count > 0;
            }
            return false;
        }

    }
}
