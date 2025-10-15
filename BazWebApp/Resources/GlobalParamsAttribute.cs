using Baz.Model.Entity.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baz.Model.Entity.Resources
{
    /// <summary>
    /// Küresel parametrelerin model validationı için oluşturulan sınıf
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GlobalParamsAttribute : ValidationAttribute
    {
        /// <summary>
        /// session timeout 600 küçük veya zorunlu şifre yenileme aralığı 20 günden küçük mü
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            var list = value as IList<ParamKureselParam>;
            foreach (var item in list)
            {
                switch (item.Adi)
                {
                    case "Session Timeout":
                        if (item.Deger < 600)
                        {
                            return false;
                        }
                        break;
                    case "ZorunluŞifreYenilemeAralığı":
                        if (item.Deger < 20)
                        {
                            return false;
                        }
                        break;
                    default:
                        return true;
                }
            }
            return true;
        }
    }
}