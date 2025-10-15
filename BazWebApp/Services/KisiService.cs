using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Decor;
using Baz.AOP.Logger.ExceptionLog;
using BazWebApp.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BazWebApp.Services
{
    /// <summary>
    /// Kişi servisine ait metotların yer aldığı servis sınıfıdır
    /// </summary>
    public interface IKisiService
    {
        /// <summary>
        /// Kişi Temel bilgilerinin güncellendiği methodtur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Result<bool> Update(KisiTemelBilgiler model);

        /// <summary>
        /// Id'ye göre Kişi Temel bilgilerinin getirildiği methodtur.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Result<KisiTemelBilgiler> SingleOrDefault(int id);

        /// <summary>
        /// Mail'e şifre yenileme isteği gönderen methodtur.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns></returns>
        Result<SistemLoginSifreYenilemeAktivasyonHareketleri> SifreYenilemeIstegi(string mail);

        /// <summary>
        /// Model'de şifreyi yenileyen methodtur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Result<SifreModel> SifreYenile(SifreModel model);

        /// <summary>
        /// Guid parametresine göre şifre yenilemenin geçerliliğine bakan methodtur.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        Result<bool> SifreYenilemeGecerliMi(string guid);

        /// <summary>
        /// Kişinin profilinden şifresini güncellediği metottur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Result<SifreModel> SifreDeğistir(SifreModel model);

        /// <summary>
        /// Kişi temel kayıdının yapıldığı metottur
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<KisiTemelKayitModel> KisiTemelKaydet(KisiTemelKayitModel model);

        /// <summary>
        /// Kişi temel verilerini kişi Id'ye göre getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<KisiTemelKayitModel> KisiTemelKayitVerileriGetir(int kisiID);

        /// <summary>
        /// Kişi temel verilerini güncelleyen metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<KisiTemelKayitModel> KisiTemelKayitVerileriGuncelle(KisiTemelKayitModel model);

        /// <summary>
        /// Temel kişi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        Result<List<KisiListeModel>> TemelKisiListesiGetir();

        /// <summary>
        /// Temel kişi bilgilerini silindi yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<bool> TemelKisiSilindiYap(int kisiID);

        /// <summary>
        /// Kişi temel bilgilerini kişi Id'ye göre getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<KisiTemelViewModel> KisiTemelKayitVerileriGetirView(int kisiID);

        /// <summary>
        /// Kişi Id'ye göre şifre yenileme tarihini getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<DateTime> KisiSifreSonYenilemeTarihiGetir(int kisiID);

        /// <summary>
        /// KişiId ile kullanıcını şifre yenileme zamanı gelmiş mi kontrolünü gerçekleştiren metot.
        /// </summary>
        /// <returns>işlem sonucuna göre true veya false döndürür.</returns>
        Result<bool> SifreYenilemeZamaniGelmisMi(int kisiId, int kurumId);

        /// <summary>
        /// Mail adresi ile kullanıcı verilerini getiren metot.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>kişi verilerini döndürür.</returns>
        Result<BasicKisiModel> MailAdresiIleGetir(string mail);

        /// <summary>
        /// KişiId ile kullanıcı hesabının deaktive edildiği method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>kişi verilerini döndürür.</returns>
        Result<KisiTemelBilgiler> KisiPasifEtme(int id);

        /// <summary>
        /// Kisi temel bilgiler tablo Id değeri ile kişi hassas bilgilerinin getirildiği HTTPGet metodu.
        /// </summary>
        /// <param name="kisiId">kişinin temelBİlgiler tablosunda kayıtlı Id değeri</param>
        /// <returns>KişiHassasBİlgiler verilerini döner.</returns>
        Result<KisiHassasBilgiler> KisiHassasBilgilerGetir(int kisiId);

        /// <summary>
        /// kisi ek parametre listesi getiren metod
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<List<KisiEkParametreViewModel>> EkParametreListesi(int? kisiID);

        /// <summary>
        /// Kisi ekp parametre kaydı yapan metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<bool> EkParametreKayit(KisiEkParametreKayitViewModel model);


        Result<List<KurumOrganizasyonBirimTanimlari>> KisiOrganizasyonListGetir(int kisiID);

        /// <summary>
        /// Hedef kitle alanlarını getiren metod
        /// </summary>
        /// <returns></returns>
        Result<List<Baz.Model.Entity.ViewModel.HedefKitleField>> GetTargetGroupFields();


        /// <summary>
        /// Gelen Idlere göre kisileri getiren metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<List<KisiTemelBilgiler>> IdlereGoreKisileriGetir(List<int> model);

        /// <summary>
        /// Cookiedeki Idye göre kuruma bağlı kişileri getiren metod
        /// </summary>
        /// <returns></returns>
        Result<List<KisiTemelBilgiler>> KurumaBagliKisiListGetir();

        /// <summary>
        /// parmetreye göre Kuruma bağlı kişileri getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Result<List<KisiTemelBilgiler>> KurumaBagliKisiListGetirById(int id);

        /// <summary>
        /// Organizasyon birimine bağlı kişileri listeler
        /// </summary>
        /// <param name="birimId"></param>
        /// <returns></returns>
        Result<List<KisiTemelBilgiler>> ListUserIdForBirimId(int birimId);

        /// <summary>
        /// Birden fazla kişinin aynı anda kaydedilmesini nsağlayan metod
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Result<List<KisiTemelKayitModel>> ListeIleTemelKisikaydet(List<KisiTemelKayitModel> list);

        /// <summary>
        /// Pasif kişi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        Result<List<KisiTemelBilgiler>> PasifKisiListesiGetir();

        /// <summary>
        /// Temel kişi bilgilerini aktif yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        Result<bool> TemelKisiAktifYap(int kisiID);

        /// <summary>
        /// kisiIdye göre musteri temsilcisilerini getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> AmirlereAstMusteriTemsilcisiKisileriniGetir();

        /// <summary>
        /// musteri temsilcisine bağlı kişileri getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> MusteriTemsilcisiBagliKisilerList();

        /// <summary>
        /// Kisiye sifre atama islemi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> SifreAtama(SifreAtamaModel model);

        /// <summary>
        /// Pozisyona bağlı hiyerarşik ağaçta ast-üst ilişkisi bulunmayan ancak ilgili kişilere irişmesi gereken kullanıcılar için kullanılacak kişi listesi metodu.
        /// </summary>
        /// <returns>KisiListeModel listesi döndürür. <see cref="KisiListeModel"></see></returns>
        public Result<List<KisiListeModel>> HiyerarsiDisiKisilerKisiListesi();

        /// <summary>
        /// İlgili kurum Id ve aktif kişi Id değerine göre o kurumda tanımlı sanal kişi verilerini getiren metot.
        /// </summary>
        /// <param name="kurumId"></param>
        /// <param name="aktifKisiId"></param>
        /// <returns></returns>
        public Result<KisiTemelBilgiler> KurumaBagliSanalKisiGetir(int kurumId, int aktifKisiId);

        /// <summary>
        /// Amir temsilciye göre kişi listesini getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> AmirTemsilciyeGoreKisiListesi(CookieModel cook);

        /// <summary>
        /// AAktifPasifKisiList getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> AktifPasifKisiList();
    }

    /// <summary>
    /// Kişi servisinin methodlarının yer aldığı sınıftır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.IKisiService" />
    public class KisiService : IKisiService
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IRequestHelper _requestHelper;
        private readonly IGenelParametrelerService _genelParametrelerService;
        private readonly IKurumService _kurumService;
        private readonly IPostaciIslemlerService _postaciIslemlerService;
        private readonly IDilService _dilService;

        /// <summary>
        /// Kişi servisinin methodlarının yer aldığı sınıfının yapıcı metodu
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="genelParametrelerService"></param>
        /// <param name="kurumService"></param>
        /// <param name="kureselParametrelerService"></param>
        /// <param name="postaciIslemlerService"></param>
        public KisiService(IRequestHelper requestHelper,IDilService dilService, IBazCookieService bazCookieService, IGenelParametrelerService genelParametrelerService, IKurumService kurumService, IPostaciIslemlerService postaciIslemlerService)
        {
            _requestHelper = requestHelper;
            _bazCookieService = bazCookieService;
            _genelParametrelerService = genelParametrelerService;
            _kurumService = kurumService;
            _postaciIslemlerService = postaciIslemlerService;
            _dilService = dilService;
        }

        /// <summary>
        /// Id'ye göre Kişi Temel bilgilerinin getirildiği methodtur.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Result<KisiTemelBilgiler> SingleOrDefault(int id)
        {
            string url = LocalPortlar.KisiServis + "/api/KisiService/TemelBilgiGetir/" + id;
            var result = _requestHelper.Get<Result<KisiTemelBilgiler>>(url);
            return result.Result;
        }

        /// <summary>
        /// Kişi Temel bilgilerinin güncellendiği methodtur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public Result<bool> Update(KisiTemelBilgiler model)
        {
            var result = _requestHelper.Post<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/TemelBilgiGuncelle", model).Result;
            return result.IsSuccess.ToResult();
        }

        /// <summary>
        /// Mail'e şifre yenileme isteği gönderen methodtur.
        /// </summary>
        /// <param name="mail">The mail.</param>
        /// <returns></returns>
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> SifreYenilemeIstegi(string mail)
        {
            var x = _requestHelper.Get<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.KisiServis + "/api/KisiService/SifreYenileMailile/" + mail);
            return x.Result;
        }

        /// <summary>
        /// Model'de şifreyi yenileyen methodtur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public Result<SifreModel> SifreYenile(SifreModel model)
        {
            var x = _requestHelper.Post<Result<SifreModel>>(LocalPortlar.KisiServis + "/api/KisiService/SifreYenile", model);

            return x.Result;
        }




        /// <summary>
        /// Guid parametresine göre şifre yenilemenin geçerliliğine bakan methodtur.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <returns></returns>
        public Result<bool> SifreYenilemeGecerliMi(string guid)
        {
            var x = _requestHelper.Post<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/SifreYenilemeGecerliMi", guid);
            return x.Result;
        }

        /// <summary>
        /// Kişinin profilinden şifresini güncellediği metottur.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public Result<SifreModel> SifreDeğistir(SifreModel model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KisiID = Convert.ToInt32(cookie.KisiId);
            var kisiHassasBilgiler = this.KisiHassasBilgilerGetir(Convert.ToInt32(cookie.KisiId)).Value;

            var postacimodel = new PostaciBekleyenIslemlerGenel
            {
                PostaciIslemDurumTipiId = 1, // 2 //Param tablosunda "bekleniyor" kaydının Id değeri.bu işlem için sabit değer bu sebeple dinamik seçilmedi.
                KisiID = Convert.ToInt32(cookie.KisiId),
                KurumID = Convert.ToInt32(cookie.KurumId),
                IcerikSablonTanimID=4
            };
            var x = _requestHelper.Post<Result<SifreModel>>(LocalPortlar.KisiServis + "/api/KisiService/SifreDegistir", model);
            var y = _postaciIslemlerService.SifreGuncellemeIslemiEkle(postacimodel);

            return x.Result;
        }


        /// <summary>
        /// Kişi temel kayıdının yapıldığı metottur
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<KisiTemelKayitModel> KisiTemelKaydet(KisiTemelKayitModel model)
        {
            var x = _requestHelper.Post<Result<KisiTemelKayitModel>>(LocalPortlar.KisiServis + "/api/KisiService/KisiTemelKaydet", model);
            return x.Result;
        }

        /// <summary>
        /// Kişi temel verilerini kişi Id'ye göre getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<KisiTemelKayitModel> KisiTemelKayitVerileriGetir(int kisiID)
        {
            var x = _requestHelper.Post<Result<KisiTemelKayitModel>>(LocalPortlar.KisiServis + "/api/KisiService/KisiTemelKayitVerileriGetir", kisiID);
            return x.Result;
        }

        /// <summary>
        /// Kişi temel verilerini güncelleyen metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<KisiTemelKayitModel> KisiTemelKayitVerileriGuncelle(KisiTemelKayitModel model)
        {
            var x = _requestHelper.Post<Result<KisiTemelKayitModel>>(LocalPortlar.KisiServis + "/api/KisiService/KisiTemelKayitVerileriGuncelle", model);
            return x.Result;
        }

        /// <summary>
        /// Temel kişi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> TemelKisiListesiGetir()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var x = _requestHelper.Get<Result<List<KisiListeModel>>>(LocalPortlar.KisiServis + "/api/KisiService/TemelKisiListesiGetir/" + cookie.KurumId);
            return x.Result;
        }

        /// <summary>
        /// Pasif kişi listesini getiren metot
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> PasifKisiListesiGetir()
        {
            var x = _requestHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + "/api/KisiService/PasifKisiListesiGetir");

            return x.Result;
        }

        /// <summary>
        /// Temel kişi bilgilerini silindi yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<bool> TemelKisiSilindiYap(int kisiID)
        {
            var x = _requestHelper.Get<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/TemelKisiSilindiYap/" + kisiID);
            return x.Result;
        }

        /// <summary>
        /// Temel kişi bilgilerini aktif yapan metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<bool> TemelKisiAktifYap(int kisiID)
        {
            var x = _requestHelper.Get<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/TemelKisiAktifYap/" + kisiID);
            return x.Result;
        }

        /// <summary>
        /// Kişi temel bilgilerini kişi Id'ye göre getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<KisiTemelViewModel> KisiTemelKayitVerileriGetirView(int kisiID)
        {
            var model = new KisiTemelViewModel();
            var TemelBilgilerReturnModel = this.SingleOrDefault(kisiID).Value;
            var adresTipleri = _genelParametrelerService.AdresTipiList().Value;
            var cinsiyetTipleri = _genelParametrelerService.CinsiyetList().Value;
            var MedeniHalTipleri = _genelParametrelerService.MedeniHalList().Value;
            var telefonTipleri = _genelParametrelerService.TelefonTipiList().Value;
            var okulTipleri = _genelParametrelerService.OkulTipiList().Value;
            var kurumList = _kurumService.List().Value;
            var ulkeList = _genelParametrelerService.UlkeList().Value;
            var kisiTemelKayitModel = this.KisiTemelKayitVerileriGetir(kisiID).Value;
            var dinList = _genelParametrelerService.DinlerList().Value;
            var yabanciDilSeviyesi = _genelParametrelerService.YabanciDilSeviyesiList().Value;
            var yabanciDiller = _dilService.YabanciDilList().Value;
            var dilList = this.KisiBildigiDilList(kisiID);

            model.TemelBilgiTabloID = TemelBilgilerReturnModel.TabloID;
            model.Adi = kisiTemelKayitModel.Adi;
            model.Soyadi = kisiTemelKayitModel.Soyadi;
            model.EpostaAdresi = kisiTemelKayitModel.EpostaAdresi;
            model.IseGirisTarihi = kisiTemelKayitModel.IseGirisTarihi;
            model.TCKimlikNo = kisiTemelKayitModel.TCKimlikNo;
            model.SicilNo = kisiTemelKayitModel.SicilNo;
            model.BabaAdi = kisiTemelKayitModel.BabaAdi;
            model.AnneAdi = kisiTemelKayitModel.AnneAdi;

            model.DogumTarihi = kisiTemelKayitModel.DogumTarihi.Value;

            model.Kurum = kurumList.Where(x => x.TabloID == kisiTemelKayitModel.Kurum).Select(p => p.KurumTicariUnvani).FirstOrDefault();

            model.Dini = dinList.Where(x => x.TabloID == kisiTemelKayitModel.Dini).Select(p => p.ParamTanim).FirstOrDefault();
            model.Cinsiyeti = cinsiyetTipleri.Where(x => x.TabloID == kisiTemelKayitModel.Cinsiyeti).Select(p => p.ParamTanim).FirstOrDefault();
            model.MedeniHali = MedeniHalTipleri.Where(x => x.TabloID == kisiTemelKayitModel.MedeniHali).Select(p => p.ParamTanim).FirstOrDefault();
            model.DogduguUlke = ulkeList.Where(x => x.TabloID == kisiTemelKayitModel.DogduguUlke).Select(p => p.ParamTanim).FirstOrDefault();

            if (kisiTemelKayitModel.DogduguUlke.HasValue)
            {
                var sehirList = _genelParametrelerService.SehirList(kisiTemelKayitModel.DogduguUlke.Value)?.Value;
                model.DogduguSehir = sehirList.Where(x => x.TabloID == kisiTemelKayitModel.DogduguSehir).Select(p => p.ParamTanim).FirstOrDefault();
            }
            //Kişinin ek bilgilerini getirme
            model.OkulListesi = kisiTemelKayitModel.OkulListesi.Select(a => new OkulViewModel()
            {
                Fakulte = a.Fakulte,
                MezuniyetTarihi = Convert.ToDateTime(a.MezuniyetTarihi),
                OkulAdi = a.OkulAdi,
                OkulTipi = okulTipleri.Where(p => p.TabloID == a.OkulTipi).Select(x => x.ParamTanim).FirstOrDefault()
            }).ToList();

            model.AdresListesi = kisiTemelKayitModel.AdresListesi.Select(a => new AdresViewModel()
            {
                Adres = a.Adres,
                AdresTipi = adresTipleri.Where(p => p.TabloID == a.AdresTipi).Select(x => x.ParamTanim).FirstOrDefault(),
                Ulke = ulkeList.Where(p => p.TabloID == a.Ulke).Select(x => x.ParamTanim).FirstOrDefault(),
                Sehir = a.Ulke.HasValue ? _genelParametrelerService.SehirList(a.Ulke.Value)?.Value.Where(p => p.TabloID == a.Sehir).Select(x => x.ParamTanim).FirstOrDefault() : ""
            }).ToList();


            model.DilListesi = dilList.Value.Select(a => new DilViewModel()
            {
                YabanciDilTipi = yabanciDiller.Where(p => p.TabloID == a.ParamYabanciDilID).Select(x => x.ParamTanim).FirstOrDefault(),
                DilSeviye = yabanciDilSeviyesi.Where(p => p.TabloID == a.ParamDilSeviyesiID).Select(x => x.ParamTanim).FirstOrDefault()
            }).ToList();

            model.TelefonListesi = kisiTemelKayitModel.TelefonListesi.Select(a => new TelefonViewModel()
            {
                TelefonNo = a.TelefonNo,
                TelefonTipi = telefonTipleri.Where(p => p.TabloID == a.TelefonTipi).Select(x => x.ParamTanim).FirstOrDefault()
            }).ToList();

            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListTip";
            //Kişinin organizasyon bilgilerini getirme
            var departmanList = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, new KurumOrganizasyonBirimRequest()
            {
                KurumId = kisiTemelKayitModel.Kurum,
                Name = "departman"
            }).Result.Value;
            var lokasyonList = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, new KurumOrganizasyonBirimRequest()
            {
                KurumId = kisiTemelKayitModel.Kurum,
                Name = "lokasyon"
            }).Result.Value;
            var pozisyonList = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, new KurumOrganizasyonBirimRequest()
            {
                KurumId = kisiTemelKayitModel.Kurum,
                Name = "pozisyon"
            }).Result.Value;
            var rolList = _requestHelper.Post<Result<List<KurumOrganizasyonBirimView>>>(url, new KurumOrganizasyonBirimRequest()
            {
                KurumId = kisiTemelKayitModel.Kurum,
                Name = "rol"
            }).Result.Value;
            model.Departman = departmanList.Where(a => a.TabloId == kisiTemelKayitModel.Departman).Select(s => s.Tanim).FirstOrDefault();
            model.Lokasyon = lokasyonList.Where(a => a.TabloId == kisiTemelKayitModel.Lokasyon).Select(s => s.Tanim).FirstOrDefault();
            model.Pozisyon = pozisyonList.Where(a => a.TabloId == kisiTemelKayitModel.Pozisyon).Select(x => x.Tanim).FirstOrDefault();
            model.Rol = rolList.Where(a => a.TabloId == kisiTemelKayitModel.Rol).Select(x => x.Tanim).FirstOrDefault();

            return model.ToResult();
        }

        /// <summary>
        /// Kişi Id'ye göre şifre yenileme tarihini getiren metot
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<DateTime> KisiSifreSonYenilemeTarihiGetir(int kisiID)
        {
            var x = _requestHelper.Get<Result<DateTime>>(LocalPortlar.KisiServis + "/api/KisiService/KisiSifreSonYenilemeTarihiGetir/" + kisiID);
            return x.Result;
        }
        public Result<List<KisiBildigiDiller>> KisiBildigiDilList(int kisiID)
        {
            var x = _requestHelper.Get<Result<List<KisiBildigiDiller>>>(LocalPortlar.KisiServis + "/api/KisiService/KisiBildigiDilList/" + kisiID);
            return x.Result;
        }

        /// <summary>
        /// KişiId ile kullanıcını şifre yenileme zamanı gelmiş mi kontrolünü gerçekleştiren metot.
        /// </summary>
        /// <returns>işlem sonucuna göre true veya false döndürür.</returns>

        public Result<bool> SifreYenilemeZamaniGelmisMi(int kisiId, int kurumId)
        {
            // TODO : Hakan _kureselParametrelerService kaldırıldı, şifre exprire kontrol return true çeviriyoruz. false dönerse şifre yenileme sayfasına düşer.
            return true.ToResult();
            /*
            KureselParametreModel sifreYenilemeZamaniParam;
            var kureselParamModel = new KureselParametreModel()
            {
                ParamTanim = "ZorunluŞifreYenilemeAralığı",
                KurumID = kurumId,
            };
            
            var kurumResult = _kureselParametrelerService.KurumParamDegeriTanimliysaGetir(kureselParamModel);
            if (kurumResult.Value != null)
            {
                sifreYenilemeZamaniParam = kurumResult.Value;
            }
            else
            {
                var returnVal = _kureselParametrelerService.ZorunluSifreYenilemeAraligiGetir("ZorunluŞifreYenilemeAralığı");
                sifreYenilemeZamaniParam = returnVal.Value;
            }
            var zamanParam = Convert.ToDouble(sifreYenilemeZamaniParam.ParametreBaslangicDegeri);
            var kisiSifreYenilemeTarihResult = this.KisiSifreSonYenilemeTarihiGetir(kisiId);
            var kisiSifreYenilemeTarih = kisiSifreYenilemeTarihResult.Value;
            if (kisiSifreYenilemeTarih.AddDays(zamanParam) > DateTime.Now)
            {
                return true.ToResult();
            }
            else
            {
                return false.ToResult();
            } */
        }

        /// <summary>
        /// Mail adresi ile kullanıcı verilerini getiren metot.
        /// </summary>
        /// <param name="mail"></param>
        /// <returns>kişi verilerini döndürür.</returns>

        public Result<BasicKisiModel> MailAdresiIleGetir(string mail)
        {
            var x = _requestHelper.Get<Result<BasicKisiModel>>(LocalPortlar.KisiServis + "/api/KisiService/MailileGetir/" + mail);
            return x.Result;
        }

        /// <summary>
        /// KişiId ile kullanıcı hesabının deaktive edildiği method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>kişi verilerini döndürür.</returns>

        public Result<KisiTemelBilgiler> KisiPasifEtme(int id)
        {
            var x = _requestHelper.Get<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/HesabiPasifEt/" + id);
            return x.Result;
        }

        /// <summary>
        /// Kisi temel bilgiler tablo Id değeri ile kişi hassas bilgilerinin getirildiği HTTPGet metodu.
        /// </summary>
        /// <param name="kisiId">kişinin temelBİlgiler tablosunda kayıtlı Id değeri</param>
        /// <returns>KişiHassasBİlgiler verilerini döner.</returns>
        public Result<KisiHassasBilgiler> KisiHassasBilgilerGetir(int kisiId)
        {
            var x = _requestHelper.Get<Result<KisiHassasBilgiler>>(LocalPortlar.KisiServis + "/api/KisiService/KisiHassasBilgilerGetir/" + kisiId);

            return x.Result;
        }

        /// <summary>
        /// kisi ek parametre listesi getiren metod
        /// </summary>
        /// <param name="kisiID"></param>
        /// <returns></returns>
        public Result<List<KisiEkParametreViewModel>> EkParametreListesi(int? kisiID)
        {
            var x = _requestHelper.Get<Result<List<KisiEkParametreViewModel>>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/EkParametreListesi/" + kisiID);

            return x.Result;
        }

        /// <summary>
        /// Kisi ekp parametre kaydı yapan metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> EkParametreKayit(KisiEkParametreKayitViewModel model)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            model.KurumID = Convert.ToInt32(cookie.KurumId);
            model.KisiID = Convert.ToInt32(cookie.KisiId);

            var x = _requestHelper.Post<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiEkParametre/EkParametreKayit/", model);

            return x.Result;
        }


        public Result<List<KurumOrganizasyonBirimTanimlari>> KisiOrganizasyonListGetir(int kisiID)
        {
            var x = _requestHelper.Get<Result<List<KurumOrganizasyonBirimTanimlari>>>(LocalPortlar.KisiServis + "/api/KisiService/KisiOrganizasyonListGetir/"+kisiID );
            return x.Result;
        }


        /// <summary>
        /// Hedef kitle alanlarını getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<HedefKitleField>> GetTargetGroupFields()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var id = cookie.KurumId;
            var url = LocalPortlar.KisiServis + "/api/hedefKitle/getfields/" + id;

            var x = _requestHelper.Get<Result<List<HedefKitleField>>>(url);

            return x.Result;
        }



        /// <summary>
        /// Gelen Idlere göre kisileri getiren metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> IdlereGoreKisileriGetir(List<int> model)
        {
            var x = _requestHelper.Post<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + "/api/KisiService/IdlereGoreKisileriGetir/", model);

            return x.Result;
        }

        /// <summary>
        /// Cookiedeki Idye göre kuruma bağlı kişileri getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> KurumaBagliKisiListGetir()
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var id = cookie.KurumId;
            var x = _requestHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + "/api/KisiService/KurumaBagliKisilerList/" + id);

            return x.Result;
        }

        /// <summary>
        /// parmetreye göre Kuruma bağlı kişileri getiren metod
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> KurumaBagliKisiListGetirById(int id)
        {
            var x = _requestHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + "/api/KisiService/KurumaBagliKisilerList/" + id);

            return x.Result;
        }

        /// <summary>
        /// Organizasyon birimine bağlı kişileri listeler
        /// </summary>
        /// <param name="birimId"></param>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> ListUserIdForBirimId(int birimId)
        {
            var url = LocalPortlar.KurumService + "/api/OrganizasyonBirim/ListKisiId/" + birimId;
            var x = _requestHelper.Get<Result<List<KisiTemelBilgiler>>>(url);
            return x.Result;
        }

        /// <summary>
        /// Birden fazla kişinin aynı anda kaydedilmesini nsağlayan metod --- excel ile alakalı
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<List<KisiTemelKayitModel>> ListeIleTemelKisikaydet(List<KisiTemelKayitModel> list)
        {
            var res = _requestHelper.Post<Result<List<KisiTemelKayitModel>>>(LocalPortlar.KisiServis + "/api/KisiService/ListeIleTemelKisikaydet", list);
            return res.Result;
        }

        /// <summary>
        /// musteri temsilcisine bağlı kişileri getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> MusteriTemsilcisiBagliKisilerList()
        {
            var kisiId = _bazCookieService.GetCookie().GetAwaiter().GetResult().KisiId;
            var res = _requestHelper.Get<Result<List<KisiListeModel>>>(LocalPortlar.KisiServis + "/api/KisiService/MusteriTemsilcisiBagliKisilerList/" + kisiId);

            return res.Result;
        }

        /// <summary>
        /// kisiIdye göre musteri temsilcisilerini getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> AmirlereAstMusteriTemsilcisiKisileriniGetir()
        {
            var kisiId = _bazCookieService.GetCookie().GetAwaiter().GetResult().KisiId;
            var res = _requestHelper.Get<Result<List<KisiListeModel>>>(LocalPortlar.KisiServis + "/api/KisiService/AmirlereAstMusteriTemsilcisiKisileriniGetir/" + kisiId);

            return res.Result;
        }

        /// <summary>
        /// Kisiye sifre atama islemi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> SifreAtama(SifreAtamaModel model)
        {
            var res = _requestHelper.Post<Result<bool>>(LocalPortlar.KisiServis + "/api/KisiService/SifreAtama", model);

            return res.Result;
        }

        /// <summary>
        /// Pozisyona bağlı hiyerarşik ağaçta ast-üst ilişkisi bulunmayan ancak ilgili kişilere erişmesi gereken kullanıcılar için kullanılacak kişi listesi metodu.
        /// </summary>
        /// <returns>KisiListeModel listesi döndürür. <see cref="KisiListeModel"></see></returns>
        public Result<List<KisiListeModel>> HiyerarsiDisiKisilerKisiListesi()
        {
            var result = _requestHelper.Get<Result<List<KisiListeModel>>>(LocalPortlar.KisiServis + "/api/KisiService/HiyerarsiDisiKisilerKisiListesi");
            return result.Result;
        }

        /// <summary>
        /// İlgili kurum Id ve aktif kişi Id değerine göre o kurumda tanımlı sanal kişi verilerini getiren metot.
        /// </summary>
        /// <param name="kurumId"></param>
        /// <param name="aktifKisiId"></param>
        /// <returns></returns>
        //[Route("KurumaBagliSanalKisiGetir/{kurumId}/{aktifKisiId}")]
        //[HttpGet]
        public Result<KisiTemelBilgiler> KurumaBagliSanalKisiGetir(int kurumId, int aktifKisiId)
        {
            var result = _requestHelper.Get<Result<KisiTemelBilgiler>>(LocalPortlar.KisiServis + $"/api/KisiService/KurumaBagliSanalKisiGetir/{kurumId}/{aktifKisiId}");
            return result.Result;
        }

        /// <summary>
        /// Amir temsilciye göre kişi listesini getiren metod
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiListeModel>> AmirTemsilciyeGoreKisiListesi(CookieModel cook)
        {
            if (cook.MusteriTemsilcisiIdList != null && cook.MusteriTemsilcisiIdList.Count > 0)
            {
                var result = AmirlereAstMusteriTemsilcisiKisileriniGetir();
                return result;
            }
            else
            {
                var kontrol = _kurumService.KisiMusteriTemsilcisiMi(Convert.ToInt32(cook.KisiId)).Value;
                if (kontrol)
                {
                    var result = MusteriTemsilcisiBagliKisilerList();
                    return result;
                }
                else
                {
                    var result = HiyerarsiDisiKisilerKisiListesi();
                    return result;
                }
            }
        }

        /// <summary>
        /// Aktif ve pasif kişileri birlikte listeleyen metot.
        /// </summary>
        /// <returns></returns>
        public Result<List<KisiTemelBilgiler>> AktifPasifKisiList()
        {
            var result = _requestHelper.Get<Result<List<KisiTemelBilgiler>>>(LocalPortlar.KisiServis + $"/api/KisiService/AktifPasifKisiList");
            return result.Result;
        }
    }
}