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
        public bool SistemParamMi { get; set; }
        public string ParamTanim { get; set; }
        public int UstId { get; set; }
        public string ParamKod { get; set; }
        public int ParamIkonId { get; set; }
        public int ParamSira { get; set; }
        public int ParamIlgiliKurumId { get; set; }
        public int ParamIlgiliKisiId { get; set; }
        public int ParamIlgiliUlkeId { get; set; }
        public int EsDilID { get; set; }
        public int TabloID { get; set; }
        public int DilID { get; set; }
        public int KurumID { get; set; }
        public int KisiID { get; set; }
        public int AktifMi { get; set; }
        public int SilindiMi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int KayitEdenID { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int GuncelleyenKisiID { get; set; }
        public DateTime? AktiflikTarihi { get; set; }
        public int? AktifEdenKisiID { get; set; }
        public int? PasifEdenKisiID { get; set; }
        public DateTime? PasiflikTarihi { get; set; }
        public int? SilenKisiID { get; set; }
        public DateTime? SilinmeTarihi { get; set; }
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
        public List<MalzemeTalepEtItem> TalepItems { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Et Item (Her bir satır için)
    /// </summary>
    public class MalzemeTalepEtItem
    {
        /// <summary>
        /// Malzeme talebi essiz ID
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }
        
        /// <summary>
        /// Sevk edilen miktar
        /// </summary>
        public int SevkEdilenMiktar { get; set; }
        
        /// <summary>
        /// Malzeme sevk talebi yapan departman ID
        /// </summary>
        public int MalzemeSevkTalebiYapanDepartmanID { get; set; }
    }

    /// <summary>
    /// Malzeme İade Et Model
    /// </summary>
    public class MalzemeIadeEtModel
    {
        /// <summary>
        /// Malzeme talep süreç takip ID
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }
        public int SurecStatuBildirimTipiID { get; set; }
        public string SurecStatuGirilenNot { get; set; }
    }

    /// <summary>
    /// Toplu Mal Kabul Et Model
    /// </summary>
    public class TopluMalKabulEtModel
    {
        /// <summary>
        /// Mal kabul edilecek süreç takip ID'leri
        /// </summary>
        public List<int> MalzemeTalepSurecTakipIDler { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Filter Model
    /// </summary>
    public class MalzemeTalepFilterModel
    {
        public int? ProjeKodu { get; set; }
        public List<int> TalepSurecStatuIDs { get; set; }
        public string SearchText { get; set; }
        public bool MalzemeTalepEtGetir { get; set; } = false;
        public bool SadeceEkTalepleriGetir { get; set; } = false;
    }

    /// <summary>
    /// Toplu SAT Bilgisi Güncelleme Model
    /// </summary>
    public class TopluSATBilgisiGuncelleModel
    {
        public List<SATBilgisiItem> Items { get; set; }
    }

    /// <summary>
    /// SAT Bilgisi Item
    /// </summary>
    public class SATBilgisiItem
    {
        public int MalzemeTalebiEssizID { get; set; }
        public string BuTalebiKarsilayanSATSeriNo { get; set; }
        public string BuTalebiKarsilayanSATSiraNo { get; set; }
    }

    /// <summary>
    /// Toplu Malzemeleri Hazırla Model
    /// </summary>
    public class TopluMalzemeleriHazirlaModel
    {
        public List<MalzemeleriHazirlaItem> Items { get; set; }
    }

    /// <summary>
    /// Malzemeleri Hazırla Item
    /// </summary>
    public class MalzemeleriHazirlaItem
    {
        public int MalzemeTalepSurecTakipID { get; set; }
        public int HazirlananMiktar { get; set; }
    }

    /// <summary>
    /// Departman Sevkiyat Item
    /// </summary>
    public class DepartmanSevkiyatItem
    {
        /// <summary>
        /// Departman ID
        /// </summary>
        public int DepartmanID { get; set; }
        
        /// <summary>
        /// Talep edilen miktar
        /// </summary>
        public int TalepEdilenMiktar { get; set; }
        
        /// <summary>
        /// Sevk zamanı
        /// </summary>
        public DateTime? SevkZamani { get; set; }
    }

    /// <summary>
    /// Malzeme Talep View Model
    /// </summary>
    public class MalzemeTalepVM
    {
        /// <summary>
        /// Süreç takip ID (Unique satır identifier)
        /// </summary>
        public int MalzemeTalepSurecTakipID { get; set; }
        
        /// <summary>
        /// Grup içindeki süreç takip ID'leri
        /// </summary>
        public List<int> GrupIcindekiSurecTakipIDler { get; set; }
        
        /// <summary>
        /// Ana malzeme talep ID
        /// </summary>
        public int MalzemeTalebiEssizID { get; set; }
        
        /// <summary>
        /// Malzeme talep detay bilgileri
        /// </summary>
        public MalzemeTalepDetay MalzemeTalep { get; set; }
        
        /// <summary>
        /// Bu süreç için talep edilen miktar
        /// </summary>
        public int TalepEdilenMiktar { get; set; }
        
        /// <summary>
        /// Bu kaydın miktarı
        /// </summary>
        public int BuKaydinMiktari { get; set; }
        
        /// <summary>
        /// İşlenen miktar
        /// </summary>
        public int IslenenMiktar { get; set; }
        
        /// <summary>
        /// Hazırlanabilecek miktar
        /// </summary>
        public int HazirlanabilecekMiktar { get; set; }
        
        /// <summary>
        /// Toplam sevk edilen miktar
        /// </summary>
        public int ToplamSevkEdilenMiktar { get; set; }
        
        /// <summary>
        /// Kalan miktar
        /// </summary>
        public int KalanMiktar { get; set; }
        
        /// <summary>
        /// Talep süreç statü ID
        /// </summary>
        public int ParamTalepSurecStatuID { get; set; }
        
        /// <summary>
        /// Süreç oluşturma tarihi
        /// </summary>
        public DateTime? SurecOlusturmaTarihi { get; set; }
        
        /// <summary>
        /// Süreç statüsü için girilen not
        /// </summary>
        public string SurecStatuGirilenNot { get; set; }
        
        /// <summary>
        /// Süreç statüsü bildirim tipi ID
        /// </summary>
        public int? SurecStatuBildirimTipiID { get; set; }
        
        /// <summary>
        /// Bildirim tipi tanımlama
        /// </summary>
        public string BildirimTipiTanimlama { get; set; }

        /// <summary>
        /// Sevk ID
        /// </summary>
        public string SevkID { get; set; }
        
        /// <summary>
        /// Hazır ID
        /// </summary>
        public string HazirId { get; set; }
        
        /// <summary>
        /// Onay ID
        /// </summary>
        public string OnayId { get; set; }
        
        /// <summary>
        /// Sevk ID bazında kayıt sayısı
        /// </summary>
        public int SevkIDBazindaKayitSayisi { get; set; }
        
        /// <summary>
        /// Departman sevkiyatları listesi
        /// </summary>
        public List<DepartmanSevkiyatItem> DepartmanSevkiyatlari { get; set; }
    }

    /// <summary>
    /// Malzeme Talep Detay Model
    /// </summary>
    public class MalzemeTalepDetay
    {
        public int MalzemeTalebiEssizID { get; set; }
        public string Kod { get; set; }
        public int ProjeKodu { get; set; }
        public int TalepGirenKisiKod { get; set; }
        public int ParamDepoID { get; set; }
        public string MalzemeKodu { get; set; }
        public string MalzemeIsmi { get; set; }
        public string SatOlusturmaTarihi { get; set; }
        public string SatSeriNo { get; set; }
        public string SatSiraNo { get; set; }
        public string SatCariHesap { get; set; }
        public string Aciklama { get; set; }
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
        /// Toplu SAT Bilgisi Günceller.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<string> TopluSATBilgisiGuncelle(TopluSATBilgisiGuncelleModel model);

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
        public Result<string> MalzemeTalepEt(MalzemeTalepEtModel model);

        /// <summary>
        /// Toplu Malzemeleri Hazırla.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<string> TopluMalzemeleriHazirla(TopluMalzemeleriHazirlaModel model);
        
        /// <summary>
        /// Malzeme İade Et.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> MalzemeIadeEt(MalzemeIadeEtModel model);
        
        /// <summary>
        /// Malzeme Mal Kabul Et.
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Malzeme talep süreç takip ID</param>
        /// <returns></returns>
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID);
        
        /// <summary>
        /// Toplu Mal Kabul Et.
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Mal kabul edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluMalKabulEt(List<int> malzemeTalepSurecTakipIDler);
        
        /// <summary>
        /// Malzeme Hasarlı Olarak İşaretle.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> HasarliOlarakIsaretle(MalzemeIadeEtModel model);
        
        /// <summary>
        /// Toplu Depo Kabul.
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Depo kabul edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluDepoKabul(List<int> malzemeTalepSurecTakipIDler);
        
        /// <summary>
        /// Toplu Depo Red.
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Depo red edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluDepoRed(List<int> malzemeTalepSurecTakipIDler);
        
        /// <summary>
        /// Malzeme Talep Et Son İşlem Geri Al.
        /// </summary>
        /// <returns></returns>
        public Result<string> MalzemeTalepEtSonIslemGeriAl();
        
        /// <summary>
        /// Depo Hazırlama Son İşlem Geri Al.
        /// </summary>
        /// <returns></returns>
        public Result<string> DepoHazirlamaSonIslemGeriAl();
        
        /// <summary>
        /// Üretim Mal Kabul Son İşlem Geri Al.
        /// </summary>
        /// <returns></returns>
        public Result<string> UretimMalKabulSonIslemGeriAl();
        
        /// <summary>
        /// Kalite Kontrol Son İşlem Geri Al.
        /// </summary>
        /// <returns></returns>
        public Result<string> KaliteKontrolSonIslemGeriAl();
        
        /// <summary>
        /// Depo Karar Son İşlem Geri Al.
        /// </summary>
        /// <returns></returns>
        public Result<string> DepoKararSonIslemGeriAl();
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
                    .Where(t => t.TabloID == 1 || t.TabloID == 2)
                    .ToList();
                
                // "Tümü" seçeneğini listenin başına ekle
                var tumuOption = new TalepSurecStatuleriVM
                {
                    ParamTanim = "Tümü",
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
                malzemeTalepEtGetir = malzemeTalepEtGetir,
                sadeceEkTalepleriGetir = model.SadeceEkTalepleriGetir
            };
            
            var request = _requestHelper.Post<Result<List<MalzemeTalepVM>>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeTalepleriniGetir",
                requestData);

            return request.Result;
        }

        public Result<string> TopluSATBilgisiGuncelle(TopluSATBilgisiGuncelleModel model)
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/TopluSATBilgisiGuncelle",
                model);

            return request.Result;
        }

        public Result<List<SurecStatuleriBildirimTipleriVM>> SurecStatuleriBildirimTipleriList(int tabloID)
        {
            var request = _requestHelper.Get<Result<List<SurecStatuleriBildirimTipleriVM>>>(
                LocalPortlar.IYSService + $"/api/SurecStatuleriBildirimTipleri/SurecStatuleriBildirimTipleriList/{tabloID}");
            
            return request.Result;
        }

        public Result<string> MalzemeTalepEt(MalzemeTalepEtModel model)
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeTalepEt",
                model);

            return request.Result;
        }

        /// <summary>
        /// Toplu malzemeleri hazırla işlemi
        /// </summary>
        /// <param name="model">Toplu malzemeleri hazırla modeli</param>
        /// <returns></returns>
        public Result<string> TopluMalzemeleriHazirla(TopluMalzemeleriHazirlaModel model)
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/TopluMalzemeleriHazirla",
                model);

            return request.Result;
        }

        public Result<bool> MalzemeIadeEt(MalzemeIadeEtModel model)
        {
            var request = _requestHelper.Post<Result<bool>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeIadeEt", 
                model);
            
            return request.Result;
        }

        /// <summary>
        /// Malzeme mal kabul işlemi
        /// </summary>
        /// <param name="malzemeTalepSurecTakipID">Malzeme talep süreç takip ID</param>
        /// <returns></returns>
        public Result<bool> MalKabulEt(int malzemeTalepSurecTakipID)
        {
            var request = _requestHelper.Post<Result<bool>>(
                LocalPortlar.IYSService + $"/api/MalzemeTalepGenelBilgiler/MalKabulEt/{malzemeTalepSurecTakipID}", 
                malzemeTalepSurecTakipID);
            
            return request.Result;
        }

        /// <summary>
        /// Toplu mal kabul işlemi
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Mal kabul edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluMalKabulEt(List<int> malzemeTalepSurecTakipIDler)
        {
            var requestData = new
            {
                malzemeTalepSurecTakipIDler = malzemeTalepSurecTakipIDler
            };
            
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/TopluMalKabulEt", 
                requestData);
            
            return request.Result;
        }

        /// <summary>
        /// Malzeme hasarlı olarak işaretleme işlemi
        /// </summary>
        /// <param name="model">Hasarlı işaretleme modeli</param>
        /// <returns></returns>
        public Result<bool> HasarliOlarakIsaretle(MalzemeIadeEtModel model)
        {
            var request = _requestHelper.Post<Result<bool>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/HasarliOlarakIsaretle", 
                model);
            
            return request.Result;
        }

        /// <summary>
        /// Toplu depo kabul işlemi
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Depo kabul edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluDepoKabul(List<int> malzemeTalepSurecTakipIDler)
        {
            var requestData = new
            {
                malzemeTalepSurecTakipIDler = malzemeTalepSurecTakipIDler
            };
            
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/TopluDepoKabul", 
                requestData);
            
            return request.Result;
        }

        /// <summary>
        /// Toplu depo red işlemi
        /// </summary>
        /// <param name="malzemeTalepSurecTakipIDler">Depo red edilecek süreç takip ID'leri</param>
        /// <returns></returns>
        public Result<string> TopluDepoRed(List<int> malzemeTalepSurecTakipIDler)
        {
            var requestData = new
            {
                malzemeTalepSurecTakipIDler = malzemeTalepSurecTakipIDler
            };
            
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/TopluDepoRed", 
                requestData);
            
            return request.Result;
        }

        /// <summary>
        /// Malzeme talep et son işlem geri al
        /// </summary>
        /// <returns></returns>
        public Result<string> MalzemeTalepEtSonIslemGeriAl()
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/MalzemeTalepEtSonIslemGeriAl",
                new { });
            
            return request.Result;
        }

        /// <summary>
        /// Depo hazırlama son işlem geri al
        /// </summary>
        /// <returns></returns>
        public Result<string> DepoHazirlamaSonIslemGeriAl()
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/DepoHazirlamaSonIslemGeriAl",
                new { });
            
            return request.Result;
        }

        /// <summary>
        /// Üretim mal kabul son işlem geri al
        /// </summary>
        /// <returns></returns>
        public Result<string> UretimMalKabulSonIslemGeriAl()
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/UretimMalKabulSonIslemGeriAl",
                new { });
            
            return request.Result;
        }

        /// <summary>
        /// Kalite kontrol son işlem geri al
        /// </summary>
        /// <returns></returns>
        public Result<string> KaliteKontrolSonIslemGeriAl()
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/KaliteKontrolSonIslemGeriAl",
                new { });
            
            return request.Result;
        }

        /// <summary>
        /// Depo karar son işlem geri al
        /// </summary>
        /// <returns></returns>
        public Result<string> DepoKararSonIslemGeriAl()
        {
            var request = _requestHelper.Post<Result<string>>(
                LocalPortlar.IYSService + "/api/MalzemeTalepGenelBilgiler/DepoKararSonIslemGeriAl",
                new { });
            
            return request.Result;
        }
    }


}
