using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BazWebApp.Services
{
    /// <summary>
    /// IYS Servisi için oluşturulmuş Interface'dir.
    /// </summary>
    public interface IIYSService
    {
        /// <summary>
        /// Rezerve Kayit methodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet( KaynakRezervasyonCariDegerlerVM model);
        public Result<bool> EventSil(int id);
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id);
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele();

        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle(KaynakRezervasyonCariDegerlerVM model);
        /// <summary>
        /// Kaynak Rezerve Takvim Verilerini Getirir.
        /// <summary></summary>
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir();
        public Result<List<KaynakTanimlariRezerveVM>> KaynakSelectVeriGetir(List<int> id);//Select Verileri
    }
    public class  IYSService : IIYSService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IBazCookieService _bazCookieService;

        public IYSService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }
        /// <summary>
        /// Kaynak Rezerve Takvim Verilerini Getirir.
        /// <summary></summary>
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveTakvimVeriGetir()
        {
            var request = _requestHelper.Get<Result<List<KaynakTanimlariRezerveVM>>>(LocalPortlar.IYSService +$"/api/Takvim/KaynakRezerveTakvimVeriGetir");
            return request.Result;
        }

        /// <summary>
        /// Kaynak Rezerve Takvim Select Verilerini Getirir.
        /// <summary></summary>
        public Result<List<KaynakTanimlariRezerveVM>> KaynakSelectVeriGetir(List<int> id)
        {
            var request = _requestHelper.Post<Result<List<KaynakTanimlariRezerveVM>>>(LocalPortlar.IYSService + $"/api/Takvim/KaynakSelectVeriGetir", id);
            return request.Result;
        }

        public Result<KaynakRezervasyonCariDegerlerVM> EventKaydet( KaynakRezervasyonCariDegerlerVM model)
        { 
            var request = _requestHelper.Post<Result<KaynakRezervasyonCariDegerlerVM>>(LocalPortlar.IYSService + $"/api/Takvim/EventKaydet", model);
            return request.Result;
        }
        public Result<bool> EventSil(int id)
        {
            var request = _requestHelper.Post<Result<bool>>(LocalPortlar.IYSService + $"/api/Takvim/EventSil/"+id, id);
            return request.Result;
        }
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventVeriGetir(int id)
        {
            var request = _requestHelper.Get<Result<List<KaynakRezervasyonCariDegerlerVM>>>(LocalPortlar.IYSService + $"/api/Takvim/EventVeriGetir/"+ id);
            return request.Result;
        }
        public Result<List<KaynakRezervasyonCariDegerlerVM>> EventListele()
        {
            var request = _requestHelper.Get<Result<List<KaynakRezervasyonCariDegerlerVM>>>(LocalPortlar.IYSService + $"/api/Takvim/EventListele");
            return request.Result;
        }

        public Result<KaynakRezervasyonCariDegerlerVM> EventGuncelle(KaynakRezervasyonCariDegerlerVM model)
        {
            var request = _requestHelper.Post<Result<KaynakRezervasyonCariDegerlerVM>>(LocalPortlar.IYSService + $"/api/Takvim/EventGuncelle", model);
            return request.Result;
        }
    }


}
