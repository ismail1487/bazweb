using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BazWebApp.Services
{
    /// <summary>
    /// Proje Genel Bilgileri View Model
    /// </summary>
    public class ProjeGenelBilgilerVM
    {
        public string ProjeAdi { get; set; }
        public int ProjeKodu { get; set; }
        public int TabloID { get; set; }
        public int DilID { get; set; }
        public int KurumID { get; set; }
        public int KisiID { get; set; }
        public int AktifMi { get; set; }
        public int SilindiMi { get; set; }
    }

    /// <summary>
    /// Talep Süreç Statüleri View Model
    /// </summary>
    public class TalepSurecStatuleriVM
    {
        public string Statu { get; set; }
        public int TabloID { get; set; }
        public int DilID { get; set; }
        public int KurumID { get; set; }
        public int KisiID { get; set; }
        public int AktifMi { get; set; }
        public int SilindiMi { get; set; }
    }

    /// <summary>
    /// Süreç Statüleri Bildirim Tipleri View Model
    /// </summary>
    public class SurecStatuleriBildirimTipleriVM
    {
        public int ParamTalepSurecStatuID { get; set; }
        public string BildirimTipiTanimlama { get; set; }
        public int TabloID { get; set; }
        public int DilID { get; set; }
        public int KurumID { get; set; }
        public int KisiID { get; set; }
        public int AktifMi { get; set; }
        public int SilindiMi { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Et Model
    /// </summary>
    public class MalzemeTalepEtModel
    {
        public int MalzemeTalebiEssizID { get; set; }
        public int SevkEdilenMiktar { get; set; }
        public int SevkTalepEdenKisiID { get; set; }
        public int MalzemeSevkTalebiYapanDepartmanID { get; set; }
        public int MalzemeSevkTalebiYapanKisiID { get; set; }
        public string SurecStatuGirilenNot { get; set; }
        public int SurecStatuBildirimTipiID { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Filter Model
    /// </summary>
    public class MalzemeTalepFilterModel
    {
        public int? ProjeKodu { get; set; }
        public List<int> TalepSurecStatuIDs { get; set; }
        public string SearchText { get; set; }
        public bool MalzemeTalepEtGetir { get; set; }
    }

    /// <summary>
    /// Malzeme Talep View Model
    /// </summary>
    public class MalzemeTalepVM
    {
        public MalzemeTalepDetay MalzemeTalep { get; set; }
        public int ToplamSevkEdilenMiktar { get; set; }
        public int KalanMiktar { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Detay Model
    /// </summary>
    public class MalzemeTalepDetay
    {
        public int MalzemeTalebiEssizID { get; set; }
        public int ProjeKodu { get; set; }
        public int TalepGirenKisiKod { get; set; }
        public int ParamDepoID { get; set; }
        public string MalzemeKodu { get; set; }
        public string MalzemeIsmi { get; set; }
        public string SatOlusturmaTarihi { get; set; }
        public string SatSeriNo { get; set; }
        public string SatSiraNo { get; set; }
        public int MalzemeOrijinalTalepEdilenMiktar { get; set; }
        public int BaglantiliMalzemeTalebiEssizID { get; set; }
        public string BuTalebiKarsilayanSATSeriNo { get; set; }
        public string BuTalebiKarsilayanSATSiraNo { get; set; }
        public int TabloID { get; set; }
        public int DilID { get; set; }
        public int KurumID { get; set; }
        public int KisiID { get; set; }
        public int AktifMi { get; set; }
        public int SilindiMi { get; set; }
        public string KayitTarihi { get; set; }
        public int KayitEdenID { get; set; }
        public string GuncellenmeTarihi { get; set; }
        public int GuncelleyenKisiID { get; set; }
        public string AktiflikTarihi { get; set; }
        public int? AktifEdenKisiID { get; set; }
        public int? PasifEdenKisiID { get; set; }
        public string PasiflikTarihi { get; set; }
        public int? SilenKisiID { get; set; }
        public string SilinmeTarihi { get; set; }
    }

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
        
        /// <summary>
        /// Proje Genel Bilgileri Listesini Getirir.
        /// </summary>
        /// <returns></returns>
        public Result<List<ProjeGenelBilgilerVM>> ProjeGenelBilgilerList();
        
        /// <summary>
        /// Talep Süreç Statüleri Listesini Getirir.
        /// </summary>
        /// <returns></returns>
        public Result<List<TalepSurecStatuleriVM>> TalepSurecStatuleriList();
        
        /// <summary>
        /// Malzeme Taleplerini Getirir.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<List<MalzemeTalepVM>> MalzemeTalepleriniGetir(MalzemeTalepFilterModel model);
        
        /// <summary>
        /// Süreç Statüleri Bildirim Tiplerini Getirir.
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        public Result<List<SurecStatuleriBildirimTipleriVM>> SurecStatuleriBildirimTipleriList(int tabloID);
        
        /// <summary>
        /// Malzeme Talep Et.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> MalzemeTalepEt(MalzemeTalepEtModel model);
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

        public Result<List<ProjeGenelBilgilerVM>> ProjeGenelBilgilerList()
        {
            var request = _requestHelper.Get<Result<List<ProjeGenelBilgilerVM>>>(LocalPortlar.IYSService + "/api/ProjeGenelBilgiler/ProjeGenelBilgilerList");
            
            if (request.Result.IsSuccess && request.Result.Value != null)
            {
                // "Tümü" seçeneğini listenin başına ekle
                var tumüOption = new ProjeGenelBilgilerVM
                {
                    ProjeAdi = "Tümü",
                    ProjeKodu = 0,
                    TabloID = 0,
                    DilID = 0,
                    KurumID = 0,
                    KisiID = 0,
                    AktifMi = 1,
                    SilindiMi = 0
                };
                
                request.Result.Value.Insert(0, tumüOption);
            }
            
            
            return request.Result;
        }

        public Result<List<TalepSurecStatuleriVM>> TalepSurecStatuleriList()
        {
            var request = _requestHelper.Get<Result<List<TalepSurecStatuleriVM>>>(LocalPortlar.IYSService + "/api/GenelParametreler/TalepSurecStatuleriList");
            
            if (request.Result.IsSuccess && request.Result.Value != null)
            {
                // Sadece TabloID 1 ve 2 olanları filtrele
                request.Result.Value = request.Result.Value
                    .Where(x => x.TabloID == 1 || x.TabloID == 2)
                    .ToList();
                
                // "Tümü" seçeneğini listenin başına ekle
                var tumuOption = new TalepSurecStatuleriVM
                {
                    Statu = "Tümü",
                    TabloID = 0,
                    DilID = 0,
                    KurumID = 0,
                    KisiID = 0,
                    AktifMi = 1,
                    SilindiMi = 0
                };
                
                request.Result.Value.Insert(0, tumuOption);
            }
            
            return request.Result;
        }

        public Result<List<MalzemeTalepVM>> MalzemeTalepleriniGetir(MalzemeTalepFilterModel model)
        {
            // Proje kodu Tümü seçiliyse null gönder
            int? projeKodu = (model.ProjeKodu == 0) ? null : model.ProjeKodu;
            
            // Statü kontrolü
            List<int> statuIDs = new List<int>();
            bool malzemeTalepEtGetir = false;
            
            // Boş liste veya null gelirse Tümü demektir
            if (model.TalepSurecStatuIDs == null || model.TalepSurecStatuIDs.Count == 0)
            {
                // Tümü seçili - boş liste gönder
                statuIDs = new List<int>();
                malzemeTalepEtGetir = true;
            }
            else if (model.TalepSurecStatuIDs.Contains(0))
            {
                // Tümü seçili (0 değeri ile) - boş liste gönder
                statuIDs = new List<int>();
                malzemeTalepEtGetir = true;
            }
            else
            {
                // Tekil seçim - seçilen ID'leri gönder
                statuIDs = model.TalepSurecStatuIDs;
                malzemeTalepEtGetir = model.MalzemeTalepEtGetir;
            }

            var requestData = new
            {
                projeKodu = projeKodu,
                talepSurecStatuIDs = statuIDs,
                searchText = model.SearchText ?? "",
                malzemeTalepEtGetir = malzemeTalepEtGetir
            };
            
            Console.WriteLine("Gönderilen İstek Verisi:");
            Console.WriteLine($"Proje Kodu: {requestData.projeKodu}");
            Console.WriteLine($"Talep Süreç Statu IDs: {string.Join(", ", requestData.talepSurecStatuIDs)}");
            Console.WriteLine($"Search Text: {requestData.searchText}");
            Console.WriteLine($"Malzeme Talep Et Getir: {requestData.malzemeTalepEtGetir}");
            
            var request = _requestHelper.Post<Result<List<MalzemeTalepVM>>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeTalepleriniGetir", 
                requestData);
            
            return request.Result;
        }

        public Result<List<SurecStatuleriBildirimTipleriVM>> SurecStatuleriBildirimTipleriList(int tabloID)
        {
            var request = _requestHelper.Get<Result<List<SurecStatuleriBildirimTipleriVM>>>(
                LocalPortlar.IYSService + $"/api/SurecStatuleriBildirimTipleri/SurecStatuleriBildirimTipleriList/{tabloID}");
            
            return request.Result;
        }

        public Result<bool> MalzemeTalepEt(MalzemeTalepEtModel model)
        {
            var request = _requestHelper.Post<Result<bool>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeTalepEt", 
                model);
            
            return request.Result;
        }
    }


}
