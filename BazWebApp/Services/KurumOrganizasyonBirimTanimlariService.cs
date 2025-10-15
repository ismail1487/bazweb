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
    /// Kurum Organizasyon birim tanımlarının metotlarının yer aldığı interface
    /// </summary>
    public interface IKurumOrganizasyonBirimTanimlariService
    {
        /// <summary>
        /// Kurum organizayon birimi ağaç yapısını getiren metod
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> GetTree(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Ilgili Kurum Id'ye göre Kurum organizayon birimi ağaç pozisyon yapısını getiren metod
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> GetTree2(KurumOrganizasyonBirimRequest request);

        /// <summary>
        ///  Kurum organizayon birimi ekleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> Add(KurumOrganizasyonBirimView item);

        /// <summary>
        ///  Kurum organizayon birimi ekleme metodu (pozisyon)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> AddPoz(KurumOrganizasyonBirimView item);

        /// <summary>
        /// Kurum organizayon birimi listeleme metodu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> List(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Kurum organizayon birimi listeleme metodu(Pozisyon)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> List2(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Kurum organizayon birimi kurum güncelleme
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateForKurum(KurumOrganizasyonBirimView item);

        /// <summary>
        /// Kurum organizayon birimi kurum güncelleme(pozisyon)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateForKurum2(KurumOrganizasyonBirimView item);

        /// <summary>
        /// Kurum organizayon birimi isim güncelleme metodu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateName(KurumOrganizasyonBirimView item);

        /// <summary>
        /// Kurum organizayon birimi seviyeye göre listeleme metodu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListForLevel(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Kurum organizayon birimi seviyeye göre listeleme metodu(Pozisyon)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListForLevel2(KurumOrganizasyonBirimRequest request);

        /// <summary>
        /// Kurum organizayon birimi silme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> Delete(int id);

        /// <summary>
        /// Kişi Rol Admin mi Kontrolü.
        /// </summary>
        /// <returns></returns>
        public Result<bool> AdminMi();
    }

    /// <summary>
    ///  Kurum Organizasyon birim tanımlarının metotlarının yer aldığı sınıf
    /// </summary>
    public class KurumOrganizasyonBirimTanimlariService : IKurumOrganizasyonBirimTanimlariService
    {
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        ///  Kurum Organizasyon birim tanımlarının metotlarının yer aldığı konst.
        /// </summary>
        /// <param name="requestHelper"></param>
        public KurumOrganizasyonBirimTanimlariService(IRequestHelper requestHelper)
        {
            _requestHelper = requestHelper;
        }

        /// <summary>
        /// Ağaç yapısını getiren servis metotudur
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> GetTree(KurumOrganizasyonBirimRequest request)
        {
            var url = Baz.Model.Entity.Constants.LocalPortlar.KurumService + "/api/OrganizasyonBirim/GetTree";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Ağaç yapısını getiren servis metotudur
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> GetTree2(KurumOrganizasyonBirimRequest request)
        {
            var url = Baz.Model.Entity.Constants.LocalPortlar.KurumService + "/api/OrganizasyonBirim/GetTree2";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları ekleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> Add(KurumOrganizasyonBirimView item)
        {
            var url = Baz.Model.Entity.Constants.LocalPortlar.KurumService + "/api/OrganizasyonBirim/Add";
            var x = _requestHelper.Post<Result<int>>(url, item);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları ekleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<int> AddPoz(KurumOrganizasyonBirimView item)
        {
            var url = Baz.Model.Entity.Constants.LocalPortlar.KurumService + "/api/OrganizasyonBirim/AddPoz";
            var x = _requestHelper.Post<Result<int>>(url, item);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> List(KurumOrganizasyonBirimRequest request)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListForKurum";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları listeleme metotu
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> List2(KurumOrganizasyonBirimRequest request)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListForKurum2";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları güncelleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateForKurum(KurumOrganizasyonBirimView item)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/UpdateForKurum";
            var x = _requestHelper.Post<Result<bool>>(url, item);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları güncelleme metotu (pozisyon)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateForKurum2(KurumOrganizasyonBirimView item)
        {
            //item.IlgiliKurumID = item.KurumId
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/UpdateForKurum2";
            var x = _requestHelper.Post<Result<bool>>(url, item);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımları güncelleme metotu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Result<bool> UpdateName(KurumOrganizasyonBirimView item)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/UpdateName";
            var x = _requestHelper.Post<Result<bool>>(url, item);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birimlerini seviyelerine göre listeleyen metot
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListForLevel(KurumOrganizasyonBirimRequest request)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListForLevel";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birimlerini seviyelerine göre listeleyen metot(Pozisyon)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Result<List<KurumOrganizasyonBirimView>> ListForLevel2(KurumOrganizasyonBirimRequest request)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListForLevel2";
            var x = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, request);
            return x.Result;
        }

        /// <summary>
        /// Kurum organizasyon birim tanımlarını silindi yapan metot
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> Delete(int id)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/Delete/" + id;
            var x = _requestHelper.Get<Result<bool>>(url);
            return x.Result;
        }

        /// <summary>
        /// Kişi Rol Admin mi Kontrolü.
        /// </summary>
        /// <returns></returns>
        public Result<bool> AdminMi()
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/AdminMi";
            var x = _requestHelper.Get<Result<bool>>(url);
            return x.Result;
        }
    }
}