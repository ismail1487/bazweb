using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Postacı işlemlerine ait metotların yer aldığı servis intefacei
    /// </summary>
    public interface IPostaciIslemlerService
    {
        /// <summary>
        /// Şifre güncelleme işlemi sonrası bilgilendirme yapmayı sağlayan metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<PostaciBekleyenIslemlerGenel> SifreGuncellemeIslemiEkle(PostaciBekleyenIslemlerGenel model);
    }
    /// <summary>
    /// Postacı işlemlerine ait metotların yer aldığı servis sınıfıdır
    /// </summary>
    public class PostaciIslemlerService : IPostaciIslemlerService
    {
        private readonly IRequestHelper _requestHelper;
        /// <summary>
        /// Postacı işlemlerine ait metotların yer aldığı servis sınıfı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        public PostaciIslemlerService(IRequestHelper requestHelper)
        {
            _requestHelper = requestHelper;
        }

        
        /// <summary>
        /// Şifre güncelleme işlemi sonrası bilgilendirme yapmayı sağlayan metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<PostaciBekleyenIslemlerGenel> SifreGuncellemeIslemiEkle(PostaciBekleyenIslemlerGenel model)
        {
            var guid = Guid.NewGuid().ToString();
            var postModel = new PostaciBekleyenIslemlerGenel()
            {
                KisiID = model.KisiID,
                KurumID = model.KurumID,
                TetiklemeIlgiliKisiId = model.KisiID,
                TetiklemeIlgiliKurumId = model.KurumID,
                TetiklemeBaglantiliUrl = "",
                KayitEdenID = model.KisiID,
                TetiklemeEpostaMi = true,
                TetiklemeSmsmi = false,
                TetiklemeBildirimMi = false,
                PostaciIslemReferansNo = guid,
                PostaciIslemDurumTipiId = model.PostaciIslemDurumTipiId,
                IcerikSablonTanimID=model.IcerikSablonTanimID,
            };

            try
            {
                var result = _requestHelper.Post<Result<PostaciBekleyenIslemlerGenel>>(LocalPortlar.PostaciService + "/api/PostaciService/PostaciBekleyenGenelIslemEkle", postModel);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return result.Result;
                }
                else return Results.Fail("API bağlantısında bir sorun yaşandı.");
            }
            catch (Exception ex)
            {
                return Results.Fail(ex.InnerException.InnerException.Message);
            }

        }

        
    }
}
