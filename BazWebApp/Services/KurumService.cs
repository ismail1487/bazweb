using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using Baz.RequestManager.Abstracts;
using BazWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BazWebApp.Controllers.PanelController;

namespace BazWebApp.Services
{
    /// <summary>
    /// Organizasyon Birimi View Model
    /// </summary>
    public class OrganizasyonBirimVM
    {
        public int KurumId { get; set; }
        public int TabloId { get; set; }
        public int UstId { get; set; }
        public string Tanim { get; set; }
        public int TipId { get; set; }
        public string Koordinat { get; set; }
        public int IlgiliKurumID { get; set; }
        public List<OrganizasyonBirimVM> AltItems { get; set; }
    }

    /// <summary>
    /// Kurum Servisi için oluşturulmuş Interface'dir.
    /// </summary>
    public interface IKurumService
    {

        /// <summary>
        /// Kurum temel verileri getirme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<KurumTemelKayitModel> KurumTemelVerileriGetir(int id);

        /// <summary>
        /// Kurum adına göre kurum temel bilgiler listeleme metodu
        /// </summary>
        /// <param name="kurumAdi"></param>
        /// <returns></returns>
        Result<List<KurumTemelBilgiler>> KurumList(string kurumAdi);

        /// <summary>
        /// Kurum temel bilgiler listeleme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<List<KurumTemelBilgiler>> List(int id = 0);

        /// <summary>
        /// Kurum temel verileri kaydetme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<KurumTemelKayitModel> KurumTemelVerileriKaydet(KurumTemelKayitModel model);



        /// <summary>
        /// Kurum idari verileri getirme metodu
        /// </summary>
        /// <returns></returns>
        Result<KurumIdariProfilModel> KurumIdariVerileriGetir(int kurumID);

        /// <summary>
        /// Kurum temel verileri güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<KurumTemelKayitModel> KurumTemelVerileriGuncelle(KurumTemelKayitModel model);

        /// <summary>
        /// Kurum temel verileri silme metodu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        Result<bool> TemelKurumSilindiYap(int kurumID);

        /// <summary>
        /// Kurum temel bilgileri listeleme kendisi ile
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<List<KurumTemelBilgiler>> ListKendisiIle(int id = 0);

        /// <summary>
        /// Ek parametre listeleme metodu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        Result<List<KurumEkParametreViewModel>> EkParametreListesi(int? kurumID);


        /// <summary>
        /// Ek parametre kaydetme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<bool> EkParametreKayit(KurumEkParametreKayitViewModel model);

        /// <summary>
        /// Hedef kitle getirme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<Baz.Model.Entity.ViewModel.HedefKitleField>> GetTargetGroupFields();

        /// <summary>
        /// Kurum ID ve Tip ID'ye göre organizasyon birimlerini getirme metodu
        /// </summary>
        /// <param name="kurumId">Kurum ID</param>
        /// <param name="tipId">Tip ID (default: 1)</param>
        /// <returns></returns>
        Result<List<OrganizasyonBirimVM>> GetOrganizasyonBirimleriByKurumIdAndTipId(int kurumId, int tipId = 1);

        /// <summary>
        /// Kurum müşteri temsilcisi getirme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<GenelViewModel>> KurumMusteriTemsilcisiGetir();

        /// <summary>
        /// Müşteri temsilcisi bağlı kurumlar listeleme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<KurumTemelBilgiler>> MusteriTemsilcisiBagliKurumlarList();

        /// <summary>
        /// Amirlere ast müşteri temsilcileri getirme metodu
        /// </summary>
        /// <returns></returns>
        Result<List<KurumTemelBilgiler>> AmirlereAstMusteriTemsilcisiKurumlariniGetir();

        /// <summary>
        /// Kişi müşteri temsilcisi mi kontrolü sağlayan metot.
        /// </summary>
        /// <param name="kisiId">kişi Id</param>
        /// <returns>sonucu true veya false olarak döndürür.</returns>
        public Result<bool> KisiMusteriTemsilcisiMi(int kisiId);

        /// <summary>
        /// Pozisyona bağlı hiyerarşik ağaçta ast-üst ilişkisi bulunmayan ancak ilgili kurumlara erişmesi gereken kullanıcılar için kullanılacak kurum listesi metodu.
        /// </summary>
        /// <returns>KisiListeModel listesi döndürür. <see cref="KurumTemelBilgiler"></see></returns>
        public Result<List<KurumTemelBilgiler>> HiyerarsiDisiKisilerKurumListesi();

        /// <summary>
        /// Amir temsilciye göre kurum listesi getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> AmirTemsilciyeGoreKurumListesi(CookieModel cook);


        /// İçerik Kategorielri Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamIcerikKategoriler>> IcerikKategorileriGetir();


        /// <summary>
        /// Oluşturulan içeriklerin kayıt edilmesi
        /// </summary>
        /// <returns></returns>
        public Result<IcerikKutuphanesiMedyalarVM> IcerikKaydet(IcerikKutuphanesiMedyalarVM model);


        /// <summary>
        /// İçeriklerin Güncelleme Sayfasına Aktarılması
        /// </summary>
        /// <returns></returns>
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikVeriGetir(int urlId);


        /// <summary>
        /// Icerik Listesinde bağımsız olanları getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikListesi();

        public Result<List<SistemSayfalari>> SistemSayfalariGetir();


        /// <summary>
        /// Oluşturulan içeriklerin Güncellenmesi
        /// </summary>
        /// <returns></returns>
        public Result<IcerikKutuphanesiMedyalarVM> IcerikGuncelle(IcerikKutuphanesiMedyalarVM model);


        /// <summary>
        /// İçeriklerin Silinmesi
        /// </summary>
        /// <returns></returns>
        public Result<bool> IcerikSil(int TabloId);


        /// <summary>
        /// UrunKategorilerin alınması
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamUrunKategoriler>> ParamUrunKategorileriGetir();

        /// <summary>
        /// ParaBirim Kategorilerinin Alınması
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamParaBirimleri>> UrunParaBirimiGetir();

        /// <summary>
        /// Urun Kayıteden Metod
        /// </summary>

        /// <returns></returns>
        public Result<UrunKutuphanesiMedyalarVM> UrunKaydet(UrunKutuphanesiMedyalarVM model);

        /// <summary>
        /// Urun Listeleyen Metod
        /// </summary>

        /// <returns></returns>
        public Result<List<UrunKutuphanesiMedyalarVM>> UrunListesi();

        /// <summary>
        /// Ürünleri Silme
        /// </summary>
        /// <returns></returns>

        public Result<bool> UrunSil(int TabloId);

        /// <summary>
        /// Ürün Güncelleme Sayfasına Aktarılması
        /// </summary>
        /// <returns></returns>
        public Result<List<UrunKutuphanesiMedyalarVM>> UrunVeriGetir(int urlId);

        /// <summary>
        ///  Ürünleri Güncelleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<UrunKutuphanesiMedyalarVM> UrunGuncelle(UrunKutuphanesiMedyalarVM model);


        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        public Result<List<ParamUrunMarkalar>> ParamUrunMarkalarınıGetir();

        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamOlcumBirimleri>> UrunParametreDegerGetir();
        public Result<List<ParamOlcumBirimleri>> UrunOlcumBirimiData();
        public Result<List<ParamIcerikBlokKategorileri>> IcerikBlokKategorilerGetir();
        public Result<List<IcerikKutuphanesi>> IcerikKutuphanesiGetir();
        /// <summary>
        /// Kaynak Rezerve verilerini Listeleyen Metod
        /// </summary>
        public Result<List<ParamKaynakTipleri>> KaynakTipiGetir();
        public Result<KaynakTanimlariRezerveVM> RezerveKaynakGuncelle(KaynakTanimlariRezerveVM model);

        public Result<KaynakTanimlariRezerveVM> KaynakRezerveKayit(KaynakTanimlariRezerveVM model);
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveListele();
        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveVeriGetir(int urlId);
        public Result<bool> SilKaynak(int id);
        /// <returns></returns>


        /// <summary>
        /// Urun Listeleyen Metod
        /// </summary>

        /// <returns></returns>
        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderListesi();

        /// <summary>
        /// Ürünleri Silme
        /// </summary>
        /// <returns></returns>

        public Result<bool> SliderSil(int TabloId);

        /// <summary>
        /// Slider Oluşturma
        /// </summary>
        /// <returns></returns>
        public Result<SliderTemelBilgilerMedyalarVM> SliderKaydet(SliderTemelBilgilerMedyalarVM model);

        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderVeriGetir(int urlId);

        public Result<SliderTemelBilgilerMedyalarVM> SliderGuncelle(SliderTemelBilgilerMedyalarVM model);




    }

    /// <summary>
    /// Kurum Servisi için oluşturulmuş methodların yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.IKurumService" />
    public class KurumService : IKurumService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly string url = LocalPortlar.KurumService;
        private readonly IBazCookieService _bazCookieService;

        /// <summary>
        /// Kurum Servisi için oluşturulmuş methodların yer aldığı konst. sınıfı
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        public KurumService(IRequestHelper requestHelper, IBazCookieService bazCookieService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
        }

        /// <summary>
        /// Dİnamik kurum listesi için, kurum adı değerini Web API'a ileten method.
        /// </summary>
        /// <param name="kurumAdi"> dinamik liste oluşturulması için kullanılacak kurumAdi parametresi.</param>
        /// <returns>İlgili listeyi döndürür.</returns>
        public Result<List<KurumTemelBilgiler>> KurumList(string kurumAdi)
        {
            var x = _requestHelper.Post<Result<List<KurumTemelBilgiler>>>(url + "/api/KurumService/KurumList", kurumAdi);
            return x.Result;
        }

        /// <summary>
        /// Kurum temel bilgileri kendi kurumu ile birlikte listeleme metodu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> ListKendisiIle(int id = 0)
        {
            string fullUrl = url + "/api/KurumService/ListKendisiIle";
            if (id > 0)
                fullUrl = fullUrl + "/" + id;

            var x = _requestHelper.Get<Result<List<KurumTemelBilgiler>>>(fullUrl);
            return x.Result;
        }
        public Result<List<SistemSayfalari>> SistemSayfalariGetir()
        {
            var x = _requestHelper.Get<Result<List<SistemSayfalari>>>(url + "/api/YetkiMerkezi/SistemSayfalariGetir/");
            var a = x.Result;
            return a;
        }

        /// <summary>
        /// Sistemde kayıtlı ve aktif durumdaki tüm kurumları listeleyen servis metotu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> List(int id = 0)
        {
            string fullUrl = url + "/api/KurumService/List";
            if (id > 0)
                fullUrl = fullUrl + "/" + id;

            var x = _requestHelper.Get<Result<List<KurumTemelBilgiler>>>(fullUrl);
            return x.Result;
        }

        /// <summary>
        /// Kurum temel verileri kaydeden servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<KurumTemelKayitModel> KurumTemelVerileriKaydet(KurumTemelKayitModel model)
        {
            var x = _requestHelper.Post<Result<KurumTemelKayitModel>>(url + "/api/KurumService/KurumTemelVerileriKaydet", model);
            return x.Result;
        }

        /// <summary>
        /// Kurum temel verileri getiren servis metotu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<KurumTemelKayitModel> KurumTemelVerileriGetir(int id)
        {
            var x = _requestHelper.Get<Result<KurumTemelKayitModel>>(url + "/api/KurumService/KurumTemelVerileriGetir/" + id);
            return x.Result;
        }

        /// <summary>
        /// Kurum idari verileri getiren servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<KurumIdariProfilModel> KurumIdariVerileriGetir(int kurumID)
        {
            if (kurumID == 0)
            {
                var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
                kurumID = Convert.ToInt32(cookie.KurumId);
            }

            var x = _requestHelper.Get<Result<KurumIdariProfilModel>>(url + "/api/KurumService/KurumIdariVerileriGetir/" + kurumID);
            return x.Result;
        }

        /// <summary>
        /// Kurum temel verileri güncelleyen servis metotu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<KurumTemelKayitModel> KurumTemelVerileriGuncelle(KurumTemelKayitModel model)
        {
            var x = _requestHelper.Post<Result<KurumTemelKayitModel>>(url + "/api/KurumService/KurumTemelVerileriGuncelle/", model);
            return x.Result;
        }

        /// <summary>
        /// Kurum temel bilgileri silindi yapan servis metotu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        public Result<bool> TemelKurumSilindiYap(int kurumID)
        {
            var x = _requestHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumService/TemelKurumSilindiYap/" + kurumID);
            return x.Result;
        }

        /// <summary>
        /// Ek parametreleri listeleme metodu
        /// </summary>
        /// <param name="kurumID"></param>
        /// <returns></returns>
        public Result<List<KurumEkParametreViewModel>> EkParametreListesi(int? kurumID)
        {
            var x = _requestHelper.Get<Result<List<KurumEkParametreViewModel>>>(LocalPortlar.KurumService + "/api/KurumEkParametre/EkParametreListesi/" + kurumID);
            return x.Result;
        }

        /// <summary>
        /// Ek parametre kayıt etme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> EkParametreKayit(KurumEkParametreKayitViewModel model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumID = Convert.ToInt32(cookie.KurumId);
            model.KisiID = Convert.ToInt32(cookie.KisiId);

            var x = _requestHelper.Post<Result<bool>>(LocalPortlar.KurumService + "/api/KurumEkParametre/EkParametreKayit/", model);
            return x.Result;
        }

        /// <summary>
        /// Hedef kitle listeleme metodu
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleField>> GetTargetGroupFields()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var id = cookie.KurumId;
            var _url = LocalPortlar.KurumService + "/api/hedefKitle/getfields/" + id;
            var x = _requestHelper.Get<Result<List<HedefKitleField>>>(_url);
            return x.Result;
        }

        /// <summary>
        /// Kuruma ait Müşteri temsilcilerini getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<GenelViewModel>> KurumMusteriTemsilcisiGetir()
        {
            var _url = LocalPortlar.KurumService + "/api/KurumService/KurumMusteriTemsilcisiGetir";
            var x = _requestHelper.Get<Result<List<GenelViewModel>>>(_url);
            return x.Result;
        }

        /// <summary>
        /// Müşteri temsilcisine bağlı kurumları listeleyen metot
        /// </summary>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> MusteriTemsilcisiBagliKurumlarList()
        {
            var kisiId = _bazCookieService.GetCookie().GetAwaiter().GetResult().KisiId;
            var _url = LocalPortlar.KurumService + "/api/KurumService/MusteriTemsilcisiBagliKurumlarList/" + kisiId;
            var x = _requestHelper.Get<Result<List<KurumTemelBilgiler>>>(_url);
            return x.Result;
        }

        /// <summary>
        /// Amirlerin astı Müşteri temsilerine bağlı kurumları listeleyen metot
        /// </summary>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> AmirlereAstMusteriTemsilcisiKurumlariniGetir()
        {
            var kisiId = _bazCookieService.GetCookie().GetAwaiter().GetResult().KisiId;
            var _url = LocalPortlar.KurumService + "/api/KurumService/AmirlereAstMusteriTemsilcisiKurumlariniGetir/" + kisiId;
            var x = _requestHelper.Get<Result<List<KurumTemelBilgiler>>>(_url);
            return x.Result;
        }

        /// <summary>
        /// Kişi müşteri temsilcisi mi kontrolü sağlayan metot.
        /// </summary>
        /// <param name="kisiId">kişi Id</param>
        /// <returns>sonucu true veya false olarak döndürür.</returns>
        public Result<bool> KisiMusteriTemsilcisiMi(int kisiId)
        {
            var result = _requestHelper.Get<Result<bool>>(LocalPortlar.KurumService + "/api/KurumService/KisiMusteriTemsilcisiMi/" + kisiId);
            return result.Result;
        }

        /// <summary>
        /// Pozisyona bağlı hiyerarşik ağaçta ast-üst ilişkisi bulunmayan ancak ilgili kurumlara erişmesi gereken kullanıcılar için kullanılacak kurum listesi metodu.
        /// </summary>
        /// <returns>KisiListeModel listesi döndürür. <see cref="KurumTemelBilgiler"></see></returns>
        public Result<List<KurumTemelBilgiler>> HiyerarsiDisiKisilerKurumListesi()
        {
            var result = _requestHelper.Get<Result<List<KurumTemelBilgiler>>>(LocalPortlar.KurumService + "/api/KurumService/HiyerarsiDisiKisilerKurumListesi");
            return result.Result;
        }

        /// <summary>
        /// Amir temsilciye göre kurum listesi getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KurumTemelBilgiler>> AmirTemsilciyeGoreKurumListesi(CookieModel cook)
        {
            if (cook.MusteriTemsilcisiIdList != null && cook.MusteriTemsilcisiIdList.Count > 0)
            {
                var result = AmirlereAstMusteriTemsilcisiKurumlariniGetir();
                return result;
            }
            else
            {
                var kontrol = KisiMusteriTemsilcisiMi(Convert.ToInt32(cook.KisiId)).Value;
                if (kontrol)
                {
                    var result = MusteriTemsilcisiBagliKurumlarList();
                    return result;
                }
                else
                {
                    var result = HiyerarsiDisiKisilerKurumListesi();
                    return result;
                }
            }
        }
        /// <summary>
        /// İçerik Kategorileri Listeleen Metod
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamIcerikKategoriler>> IcerikKategorileriGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamIcerikKategoriler>>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikKategorileriGetir");
            return result.Result;
        }

        /// <summary>
        /// İçeriklerin Güncelleme Sayfasına Aktarılması
        /// </summary>
        /// <returns></returns>
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikVeriGetir(int urlId)
        {
            var result = _requestHelper.Get<Result<List<IcerikKutuphanesiMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikVeriGetir/" + urlId);
            return result.Result;
        }


        /// <summary>
        ///  İçeriklerin Kaydeden Metod
        /// </summary>
        /// <returns></returns>
        public Result<IcerikKutuphanesiMedyalarVM> IcerikKaydet(IcerikKutuphanesiMedyalarVM model)
        {
            var x = _requestHelper.Post<Result<IcerikKutuphanesiMedyalarVM>>(url + "/api/KurumService/IcerikKaydet", model);
            return x.Result;
        }

        /// <summary>
        ///  İçerikKutuphanesindeki bağımsız içerikleri getiren Metod
        /// </summary>
        /// <returns></returns>
        public Result<List<IcerikKutuphanesiMedyalarVM>> IcerikListesi()
        {
            var result = _requestHelper.Get<Result<List<IcerikKutuphanesiMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikListesi");
            return result.Result;
        }

        /// <summary>
        ///  İçeriklerin Güncelleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<IcerikKutuphanesiMedyalarVM> IcerikGuncelle(IcerikKutuphanesiMedyalarVM model)
        {
            try
            {
                var result = _requestHelper.Post<Result<IcerikKutuphanesiMedyalarVM>>(LocalPortlar.KurumService + "/api/KurumService/IcerikGuncelle", model);
                if (result.IsSuccess)
                {
                    return result.Result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// İçeriklerin Silinmesi
        /// </summary>
        /// <returns></returns>
        public Result<bool> IcerikSil(int TabloId)
        {
            var result = _requestHelper.Post<Result<bool>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikSil/" + TabloId, TabloId);
            return result.Result;
        }

        #region UrunGiris
        /// <summary>
        /// Urun Kategorilerinin Alınması
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamUrunKategoriler>> ParamUrunKategorileriGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamUrunKategoriler>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunKategorileriGetir");
            return result.Result;
        }

        /// <summary>
        /// ParaBirim Kategorilerinin Alınması
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamParaBirimleri>> UrunParaBirimiGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamParaBirimleri>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunParaBirimiGetir");
            return result.Result;
        }

        public Result<UrunKutuphanesiMedyalarVM> UrunKaydet(UrunKutuphanesiMedyalarVM model)
        {
            var result = _requestHelper.Post<Result<UrunKutuphanesiMedyalarVM>>(LocalPortlar.KurumService + $"/api/KurumService/UrunKaydet", model);
            return result.Result;
        }

        public Result<List<UrunKutuphanesiMedyalarVM>> UrunListesi()
        {
            var result = _requestHelper.Get<Result<List<UrunKutuphanesiMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunListesi");
            return result.Result;
        }

        /// <summary>
        /// Ürünlerin Silinmesi
        /// </summary>
        /// <returns></returns>
        public Result<bool> UrunSil(int TabloId)
        {
            var result = _requestHelper.Post<Result<bool>>(LocalPortlar.KurumService + $"/api/KurumService/UrunSil/" + TabloId, TabloId);
            return result.Result;
        }

        /// <summary>
        /// Ürün Güncelleme Sayfasına Aktarılması
        /// </summary>
        public Result<List<UrunKutuphanesiMedyalarVM>> UrunVeriGetir(int urlId)
        {
            var result = _requestHelper.Get<Result<List<UrunKutuphanesiMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunVeriGetir/" + urlId);
            return result.Result;
        }

        /// <summary>
        ///  Ürünleri Güncelleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<UrunKutuphanesiMedyalarVM> UrunGuncelle(UrunKutuphanesiMedyalarVM model)
        {
            try
            {
                var result = _requestHelper.Post<Result<UrunKutuphanesiMedyalarVM>>(LocalPortlar.KurumService + "/api/KurumService/UrunGuncelle", model);
                if (result.IsSuccess)
                {
                    return result.Result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        public Result<List<ParamUrunMarkalar>> ParamUrunMarkalarınıGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamUrunMarkalar>>>(LocalPortlar.KurumService + $"/api/KurumService/ParamUrunMarkalarınıGetir");
            return result.Result;
        }


        /// <summary>
        /// Ürün verilerini Listeleyen Metod
        /// </summary>
        /// <returns></returns>
        /// 
        public Result<List<ParamOlcumBirimleri>> UrunParametreDegerGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamOlcumBirimleri>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunParametreDegerGetir");
            return result.Result;
        }
        public Result<List<ParamOlcumBirimleri>> UrunOlcumBirimiData()
        {
            var result = _requestHelper.Get<Result<List<ParamOlcumBirimleri>>>(LocalPortlar.KurumService + $"/api/KurumService/UrunOlcumBirimiData");
            return result.Result;
        }
        public Result<List<ParamIcerikBlokKategorileri>> IcerikBlokKategorilerGetir()
        {
            var result = _requestHelper.Get<Result<List<ParamIcerikBlokKategorileri>>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikBlokKategorilerGetir");
            return result.Result;
        }
        public Result<List<IcerikKutuphanesi>> IcerikKutuphanesiGetir()
        {
            var result = _requestHelper.Get<Result<List<IcerikKutuphanesi>>>(LocalPortlar.KurumService + $"/api/KurumService/IcerikKutuphanesiGetir");
            return result.Result;
        }
        #endregion


        #region Kaynak Rezerve
        public Result<List<ParamKaynakTipleri>> KaynakTipiGetir()
        {
            var request = _requestHelper.Get<Result<List<ParamKaynakTipleri>>>(LocalPortlar.KurumService + $"/api/KurumService/KaynakTipiGetir");
            return request.Result;
        }

        public Result<KaynakTanimlariRezerveVM> KaynakRezerveKayit(KaynakTanimlariRezerveVM model)
        {
            var request = _requestHelper.Post<Result<KaynakTanimlariRezerveVM>>(LocalPortlar.KurumService + $"/api/KurumService/KaynakRezerveKayit",model);
            return request.Result;
        }

        public Result<KaynakTanimlariRezerveVM> RezerveKaynakGuncelle(KaynakTanimlariRezerveVM model)
        {
            var request = _requestHelper.Post<Result<KaynakTanimlariRezerveVM>>(LocalPortlar.KurumService + $"/api/KurumService/RezerveKaynakGuncelle", model);
            return request.Result;
        }

        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveVeriGetir(int urlId)
        {
            var request = _requestHelper.Get<Result<List<KaynakTanimlariRezerveVM>>>(LocalPortlar.KurumService + $"/api/KurumService/KaynakRezerveVeriGetir/" + urlId);
            return request.Result;
        }

        public Result<List<KaynakTanimlariRezerveVM>> KaynakRezerveListele()
        {
            var request = _requestHelper.Get<Result<List<KaynakTanimlariRezerveVM>>>(LocalPortlar.KurumService + $"/api/KurumService/KaynakRezerveListele");
            return request.Result;
        }

        public Result<bool> SilKaynak(int id){
            var request = _requestHelper.Post<Result<bool>>(LocalPortlar.KurumService + $"/api/KurumService/SilKaynak/" + id, id);
            return request.Result;
        }
        #endregion

        #region Slider
        #endregion
        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderListesi()
        {
            var result = _requestHelper.Get<Result<List<SliderTemelBilgilerMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/SliderListesi");
            return result.Result;
        }
        /// <summary>
        /// Ürünlerin Silinmesi
        /// </summary>
        /// <returns></returns>
        public Result<bool> SliderSil(int TabloId)
        {
            var result = _requestHelper.Post<Result<bool>>(LocalPortlar.KurumService + $"/api/KurumService/SliderSil/" + TabloId, TabloId);
            return result.Result;
        }

        public Result<SliderTemelBilgilerMedyalarVM> SliderKaydet(SliderTemelBilgilerMedyalarVM model)
        {
            var result = _requestHelper.Post<Result<SliderTemelBilgilerMedyalarVM>>(LocalPortlar.KurumService + $"/api/KurumService/SliderKaydet", model);
            return result.Result;
        }
        /// <summary>
        /// Ürün Güncelleme Sayfasına Aktarılması
        /// </summary>
        public Result<List<SliderTemelBilgilerMedyalarVM>> SliderVeriGetir(int urlId)
        {
            var result = _requestHelper.Get<Result<List<SliderTemelBilgilerMedyalarVM>>>(LocalPortlar.KurumService + $"/api/KurumService/SliderVeriGetir/" + urlId);
            return result.Result;
        }

        /// <summary>
        ///  Ürünleri Güncelleyen Metod
        /// </summary>
        /// <returns></returns>
        public Result<SliderTemelBilgilerMedyalarVM> SliderGuncelle(SliderTemelBilgilerMedyalarVM model)
        {
            try
            {
                var result = _requestHelper.Post<Result<SliderTemelBilgilerMedyalarVM>>(LocalPortlar.KurumService + "/api/KurumService/SliderGuncelle", model);
                if (result.IsSuccess)
                {
                    return result.Result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Kurum ID ve Tip ID'ye göre organizasyon birimlerini getirme metodu
        /// </summary>
        /// <param name="kurumId">Kurum ID</param>
        /// <param name="tipId">Tip ID (default: 1)</param>
        /// <returns></returns>
        public Result<List<OrganizasyonBirimVM>> GetOrganizasyonBirimleriByKurumIdAndTipId(int kurumId, int tipId = 1)
        {
            var result = _requestHelper.Get<Result<List<OrganizasyonBirimVM>>>(
                LocalPortlar.KurumService + $"/api/OrganizasyonBirim/GetByKurumIdAndTipId/{kurumId}?tipId={tipId}");
            return result.Result;
        }
    }
}