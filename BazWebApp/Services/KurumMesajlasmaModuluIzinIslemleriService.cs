using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using BazWebApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BazWebApp.Services
{
    /// <summary>
    /// Kurum mesajlaşma modülü izin işlemlerini yöneten servis arayüzü
    /// </summary>
    public interface IKurumMesajlasmaModuluIzinIslemleriService
    {
        /// <summary>
        /// Kurum bünyesinde mesajlaşma modülü kullanmasına izin verilmeyen pozisyonların kayıt işleminin gerçekleştiren metot
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>sonucu döndürür.</returns>
        Result<bool> IzinVerilmeyenPozisyonKaydi(KurumMesajlasmaModuluIzinIslemleriViewModel model);

        /// <summary>
        /// Mesajlaşma modülü kulllanımına izin verilmeyen pozisyon kayıtlarını silen metot
        /// </summary>
        /// <param name="id"> silinecek kayıt Id değeri</param>
        /// <returns>sonucu döndürür.</returns>
        Result<bool> DeleteRecord(int id);

        /// <summary>
        /// Meesajlaşma modülü kullanımına izin veirlmeyen pozisyonları listeleyen metot.
        /// </summary>
        /// <returns>sonuçlar listesini döndürür.</returns>
        Result<List<KurumMesajlasmaModuluIzinIslemleriViewModel>> ListForView();

        /// <summary>
        /// izin işlemleri işin kurum bazında izin veirlmeyen pozisyonlar Idlerini getiren metot.
        /// </summary>
        /// <returns>Id listesi döndürür</returns>
        public Result<List<int>> IzinVerilmeyenPozisyonlarIdList();
    }

    /// <summary>
    /// Kurum mesajlaşma modülü izin işlemlerini yöneten servis sınıfı
    /// </summary>
    public class KurumMesajlasmaModuluIzinIslemleriService : IKurumMesajlasmaModuluIzinIslemleriService
    {
        private readonly IRequestHelper _requestHelper;

        /// <summary>
        /// Kurum mesajlaşma modülü izin işlemlerini yöneten servis sınıfı yapıcı metodu
        /// </summary>
        public KurumMesajlasmaModuluIzinIslemleriService(IServiceProvider serviceProvider)
        {
            _requestHelper = new RequestHelper(LocalPortlar.KurumService + "/api/", new RequestManagerHeaderHelperForCookie(serviceProvider).SetDefaultHeader());
        }

        /// <summary>
        /// Kurum bünyesinde mesajlaşma modülü kullanmasına izin verilmeyen pozisyonların kayıt işleminin gerçekleştiren metot
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>sonucu döndürür.</returns>
        public Result<bool> IzinVerilmeyenPozisyonKaydi(KurumMesajlasmaModuluIzinIslemleriViewModel model)
        {
            var result = _requestHelper.Post<Result<bool>>("KurumService/MessageModuleIzinVerilmeyenPozisyonKaydi", model);
            return result.Result;
        }

        /// <summary>
        /// Mesajlaşma modülü kulllanımına izin verilmeyen pozisyon kayıtlarını silen metot
        /// </summary>
        /// <param name="id"> silinecek kayıt Id değeri</param>
        /// <returns>sonucu döndürür.</returns>
        public Result<bool> DeleteRecord(int id)
        {
            var result = _requestHelper.Get<Result<bool>>("KurumService/MessageModuleDeleteRecord/" + id);
            return result.Result;
        }

        /// <summary>
        /// Meesajlaşma modülü kullanımına izin veirlmeyen pozisyonları listeleyen metot.
        /// </summary>
        /// <returns>sonuçlar listesini döndürür.</returns>
        public Result<List<KurumMesajlasmaModuluIzinIslemleriViewModel>> ListForView()
        {
            var result = _requestHelper.Get<Result<List<KurumMesajlasmaModuluIzinIslemleriViewModel>>>("KurumService/MessageModuleListForView");
            return result.Result;
        }

        /// <summary>
        /// izin işlemleri işin kurum bazında izin veirlmeyen pozisyonlar Idlerini getiren metot.
        /// </summary>
        /// <returns>Id listesi döndürür</returns>
        public Result<List<int>> IzinVerilmeyenPozisyonlarIdList()
        {
            var result = _requestHelper.Get<Result<List<int>>>("KurumService/IzinVerilmeyenPozisyonlarIdList");
            return result.Result;
        }
    }
}