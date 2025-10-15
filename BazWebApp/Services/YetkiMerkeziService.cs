using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
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
    /// Sayfalara erişim yetkilendirmesiyle ilgili metotların yer aldığı servis sınıfıdır
    /// </summary>
    public interface IYetkiMerkeziService
    {
        /// <summary>
        /// Erişim yetkilendirme tanımları ekleme metodu
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Result<List<ErisimYetkilendirmeTanimlari>> ErisimYetkilendirmeTanimlariKaydet(List<ErisimYetkilendirmeTanimlari> list);

        /// <summary>
        /// Kişi yetkileri listesini getiren metod
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<List<string>> KisiYetkilerListGetir(int kisiID);

        /// <summary>
        /// Erişim yetki tanımları listesini getiren metod
        /// </summary>
        /// <returns></returns>
        Result<List<ErisimYetkilendirmeTanimlariListView>> ErisimYetkiTanimListGetir();

        /// <summary>
        /// Erişim yetki tanımı silen metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<bool> ErisimYetkiTanimiSil(int id);
    }

    /// <summary>
    /// Yetki merkezi servise sınıfı
    /// </summary>
    public class YetkiMerkeziService : IYetkiMerkeziService
    {
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        /// Yetki merkezi servis konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        public YetkiMerkeziService(IRequestHelper requestHelper)
        {
            _requestHelper = requestHelper;
        }

        /// <summary>
        /// Erişim yetkilendirme tanımlarını kaydedilmesi için KurumService API'ına ileten method.
        /// </summary>
        /// <param name="list">kaydedilecek tanımlar listesi.</param>
        /// <returns></returns>
        public Result<List<ErisimYetkilendirmeTanimlari>> ErisimYetkilendirmeTanimlariKaydet(List<ErisimYetkilendirmeTanimlari> list)
        {
            var result = _requestHelper.Post<Result<List<ErisimYetkilendirmeTanimlari>>>(LocalPortlar.KurumService + "/api/YetkiMerkezi/ErisimYetkilendirmeTanimlariKaydet", list);

            return result.Result;
        }

        /// <summary>
        /// KişiID değeri ile ilgili kişinin yetkilendirildiği sayfa tanımlarını getiren method.
        /// </summary>
        /// <param name="kisiID">ilgili kişiID değeri</param>
        /// <returns>yetkilendirilen sayfa tanımları listesiin döndürür.</returns>
        public Result<List<string>> KisiYetkilerListGetir(int kisiID)
        {
            var result = _requestHelper.Get<Result<List<string>>>(LocalPortlar.KurumService + "/api/YetkiMerkezi/KisiYetkilerListGetir/" + kisiID);

            return result.Result;
        }

        /// <summary>
        /// Erişim yetkilendirme tanimlari listesinin tanımlarını getiren method.
        /// </summary>
        /// <returns>erişim yetki tanımları view model listesi döndürür.</returns>
        public Result<List<ErisimYetkilendirmeTanimlariListView>> ErisimYetkiTanimListGetir()
        {
            var result = _requestHelper.Get<Result<List<ErisimYetkilendirmeTanimlariListView>>>(LocalPortlar.KurumService + "/api/YetkiMerkezi/ErisimYetkiTanimListGetir/");

            return result.Result;
        }

        /// <summary>
        /// Erişim yetki tanımı kaydını silen method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>sonuca göre true veya false döndürür.</returns>
        public Result<bool> ErisimYetkiTanimiSil(int id)
        {
            var result = _requestHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/YetkiMerkezi/ErisimYetkiTanimiSil/" + id);

            return result.Result;
        }
    }
}