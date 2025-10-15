using Baz.AOP;
using Baz.AOP.Logger.ExceptionLog;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using Baz.SharedSession;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using System.Net;
using Decor;
using BazWebApp.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BazWebApp.Services
{
    /// <summary>
    /// Login Register Servisi için oluşturulmuş Interface'dir.
    /// </summary>
    public interface ILoginRegisterService
    {
        /// <summary>
        /// Giriş metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<string> Login(LoginModel model);

        /// <summary>
        /// Bireysel Kayıt metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<SistemLoginSifreYenilemeAktivasyonHareketleri> BireyselRegister(KurumKisiPostModel model);

        /// <summary>
        /// Kayıt metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<SistemLoginSifreYenilemeAktivasyonHareketleri> Register(KurumKisiPostModel model);

        /// <summary>
        /// Hesap aktifleştirme metodu
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Result<KisiTemelBilgiler> HesapAktiflestirme(string guid);

        /// <summary>
        /// Hesap aktivasyon linkinin geçerliliğini kontrol eden metod
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        Result<bool> HesapAktivasyonLinkiGecerliMi(string guid);

        /// <summary>
        /// Kullanıcı adı kontrolü yapan metod
        /// </summary>
        /// <param name="mailOrUsername"></param>
        /// <returns></returns>
        Result<bool> KullaniciAdiKontrolu(string mailOrUsername);

        /// <summary>
        /// Çıkış kaydı atma metodu
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Result<bool> LogoutKaydiAt(string token);

        /// <summary>
        /// Son giriş tarihini veren metod
        /// </summary>
        /// <returns></returns>
        Result<string> SonGiris();

        /// <summary>
        /// başarısız login sayısını getiren metod
        /// </summary>
        /// <param name="kisiId"></param>
        /// <returns></returns>
        Result<SistemLoginSonDurum> BasarisizLoginSayisiGetir(int kisiId);

        /// <summary>
        /// başarısız login sayısını güncelleyen metod
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Result<SistemLoginSonDurum> BasarisizLoginSayisiGuncelle(SistemLoginSonDurum model);

        /// <summary>
        /// Sistemdeki tüm üyeleri getirir
        /// </summary>
        /// <returns></returns>
        Result<List<UyeListViewModel>> UyeleriGetir();
    }

    /// <summary>
    /// Login Register için oluşturulan methodları barındıran servis sınıfıdır.
    /// </summary>
    /// <seealso cref="BazWebApp.Services.ILoginRegisterService" />
    public class LoginRegisterService : ILoginRegisterService
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IRequestHelper _requestHelper;
        private readonly ISharedSession _sharedSession;
        private readonly IKisiService _kisiService;
        private readonly ILocalizationService _localizationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPostaciIslemlerService _postaciIslemlerService;
        /// <summary>
        /// Login Register için oluşturulan methodları barındıran servis sınıfı konst.
        /// </summary>
        /// <param name="localizationService"></param>
        /// <param name="bazCookieService"></param>
        /// <param name="requestHelper"></param>
        /// <param name="sharedSession"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="kisiService"></param>
        /// <param name="postaciIslemlerService"></param>
        public LoginRegisterService(ILocalizationService localizationService, IBazCookieService bazCookieService, IRequestHelper requestHelper, ISharedSession sharedSession, IKisiService kisiService, IServiceProvider serviceProvider, IPostaciIslemlerService postaciIslemlerService)
        {
            _serviceProvider = serviceProvider;
            _bazCookieService = bazCookieService;
            _requestHelper = requestHelper;
            _sharedSession = sharedSession;
            _kisiService = kisiService;
            _localizationService = localizationService;
            _postaciIslemlerService = postaciIslemlerService;
        }

        /// <summary>
        /// Kisi Pasife düserken kurum admin ise aktif maili oluşturan metod
        /// </summary>
        /// <param name="kisi"></param>
        private void KisiHesabiPasifOldugundaAktiflestirmeMailiGonderKurumAdminIcin(BasicKisiModel kisi)
        {
            var id = kisi.TabloID;
            var y = _requestHelper.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/HesapAktivasyonMailiOlustur", id);
            var config = _serviceProvider.GetService<IConfiguration>();
            if (config == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }

            // TODO : bildirim Kapatıldı, hatırlatma servisi kullanılmıyor.

            //var bildirim = new HatirlatmaKayitlar()
            //{
            //    AktifEdenKisiID = id,
            //    KurumID = kisi.KisiBagliOlduguKurumId.Value,
            //    KayitEdenID = id,
            //    HatirlatmaKayıtEdenKisiId = id,
            //    KisiID = id,
            //    HatirlatmaMetni = config.GetValue<string>("WebAppLive") + y.Result.Value.HesapAktivasyonSayfasiGeciciUrl,
            //    HatirlatmaZamani = DateTime.Now,
            //    KayitTarihi = DateTime.Now,
            //    GuncellenmeTarihi = DateTime.Now,
            //    HatirlatmaTipi = "HesapAktivasyon",
            //    HatirlatmaSmsyollayacakMi = false,
            //    HatirlatmaEpostaYollayacakMi = true,
            //};
            //_hatirlatmaKayitService.Add(bildirim);
        }

        /// <summary>
        /// Login işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Login bilgilerini içeren LoginModel parametresi.</param>
        /// <returns></returns>

        public Result<string> Login(LoginModel model)
        {
            var x = _requestHelper.Post<Result<string>>(LocalPortlar.UserLoginregisterService + "/api/LoginRegister/Login", model);

            if (string.IsNullOrEmpty(x.Result.Value))
            {
                if (x.Result.Reasons.FirstOrDefault().Message.Contains("pasif"))
                {
                    return Results.Fail(x.Result.Reasons.FirstOrDefault().Message);
                }

                var kisiVerileri = _kisiService.MailAdresiIleGetir(model.EmailOrUserName);
                if (kisiVerileri.Value == null)
                {
                    return Results.Fail("Kullanıcı adı ya da şifre hatalıdır.");
                }
                // Login son durum kontrolleri
                var kisiLoginSonDurumModel = this.BasarisizLoginSayisiGetir(kisiVerileri.Value.TabloID).Value;

                kisiLoginSonDurumModel ??= new SistemLoginSonDurum();

                var basarisizLoginSayisi = kisiLoginSonDurumModel.KacinciBasarisizLogin;
                if (basarisizLoginSayisi < 2)
                {
                    basarisizLoginSayisi++;
                    kisiLoginSonDurumModel.KacinciBasarisizLogin = basarisizLoginSayisi;
                    this.BasarisizLoginSayisiGuncelle(kisiLoginSonDurumModel);
                    return Results.Fail("Kullanıcı adı ya da şifre hatalıdır.");
                }
                else if (basarisizLoginSayisi >= 2)
                {
                    //kisi hesabı pasif et
                    _kisiService.KisiPasifEtme(kisiVerileri.Value.TabloID);
                    basarisizLoginSayisi++;
                    kisiLoginSonDurumModel.KacinciBasarisizLogin = basarisizLoginSayisi;
                    this.BasarisizLoginSayisiGuncelle(kisiLoginSonDurumModel);
                    var id = kisiVerileri.Value.TabloID;
                    var adminMİ = _requestHelper.Get<Result<bool>>(LocalPortlar.UserLoginregisterService + "/api/LoginRegister/GiristeKurumAdminKontrol/" + id);
                    if (adminMİ.Result.Value)
                    {
                        KisiHesabiPasifOldugundaAktiflestirmeMailiGonderKurumAdminIcin(kisiVerileri.Value);
                        return Results.Fail("Üç kez hatalı giriş denediğiniz için hesabınız deaktive edilmiştir! Hesap kurtarma linkiniz mailinize gönderilmiştir.");
                    }
                    return Results.Fail("Üç kez hatalı giriş denediğiniz için hesabınız deaktive edilmiştir! Yöneticinizle iletişime geçiniz.");
                }
            }
            var data = _sharedSession.Get<KullaniciSession>(x.Result.Value).GetAwaiter().GetResult();
            var sifreYenilemeKontrol = _kisiService.SifreYenilemeZamaniGelmisMi(data.KisiID, data.KurumID).Value;
            //if (data.KullaniciYetkiListesi.Count ==0)
            //{
            //}
            //Cookie modeli oluşturma ve atama
            var cookiemodel = new CookieModel()
            {
                KisiId = data.KisiID.ToString(),
                Name = data.Ad + " " + data.SoyAd,
                Mail = model.EmailOrUserName,
                SessionId = x.Result.Value,
                KurumId = data.KurumID.ToString(),
                KurumName = data.KurumAdi
            };
            _bazCookieService.SetCookie(cookiemodel);
            var dilID = data.DilID;
            if (data.DilID == 0)
            {
                dilID = _localizationService.GetCultureId();
            }
            var dilkod = _requestHelper.Get<Result<string>>(LocalPortlar.IYSService + "/api/Dil/GetParamKod/" + dilID).Result.Value;
            _localizationService.SetCulture(dilkod);

            var LoginCariModel = new SistemLoginCariDurum()
            {
                LoginOlanKisiId = data.KisiID,
                LoginZamani = DateTime.Now,
                LoginSistemToken = x.Result.Value,
                GuncellenmeTarihi = DateTime.Now,
                KayitTarihi = DateTime.Now,
                AktifMi = 1,
                KisiID = data.KisiID,
                KurumID = data.KurumID
            };
            if (model.Google)
                LoginCariModel.ExternalAppName = "google";
            if (model.Facebook)
                LoginCariModel.ExternalAppName = "facebook";
            LoginCariModel.ExternalAppIdentity = model.ExternalId;
            //Login kaydı oluşturma
            _requestHelper.Post<Result<SistemLoginCariDurum>>(LocalPortlar.UserLoginregisterService + "/api/LoginDurumlari/LoginKaydiOlustur", LoginCariModel);
            if (sifreYenilemeKontrol)
            {
                return x.Result.Value.ToResult();
            }

            return ("SifreYenile/" + data.KisiID + "").ToResult();
        }

        /// <summary>
        /// Bireysel Register işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Kayıt bilgilerini içeren KurumKisiPostModel parametresi.</param>
        /// <returns></returns>
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> BireyselRegister(KurumKisiPostModel model)
        {
            var config = _serviceProvider.GetService<IConfiguration>();
            if (config == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            model.KisiModel.KurumsalMi = false;
            var x = _requestHelper.Post<Result<KurumKisiPostModel>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/KurumKisikaydet", model);
            if (x.Result==null)
                return Results.Fail("Lütfen kurum yöneticinizle görüşünüz.", ResultStatusCode.ReadError);

            if (x.Result!=null && x.Result.StatusCode == 902)
            {
                return Results.Fail("Kurum mevcuttur. Lütfen kurum yöneticinizle görüşünüz.", ResultStatusCode.ReadError);
            }
            
            int id = x.Result.Value.KisiModel.TabloID;
            var y = _requestHelper.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/HesapAktivasyonMailiOlustur", id);
            

            // TODO : Hakan - Hatırltma servisi kapatıldı.
            //Kişiye hesap aktivasyon maili gönderme
            //var bildirim = new HatirlatmaKayitlar()
            //{
            //    AktifEdenKisiID = id,
            //    KurumID = x.Result.Value.KurumModel.TabloID,
            //    KayitEdenID = id,
            //    HatirlatmaKayıtEdenKisiId = id,
            //    KisiID = id,
            //    HatirlatmaMetni = config.GetValue<string>("WebAppLive") + y.Result.Value.HesapAktivasyonSayfasiGeciciUrl,
            //    HatirlatmaZamani = DateTime.Now,
            //    KayitTarihi = DateTime.Now,
            //    GuncellenmeTarihi = DateTime.Now,
            //    HatirlatmaTipi = "HesapAktivasyon",
            //    HatirlatmaSmsyollayacakMi = false,
            //    HatirlatmaEpostaYollayacakMi = true,
            //};
            //_hatirlatmaKayitService.Add(bildirim);
            return y.Result;
        }

        /// <summary>
        /// Register işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="model"> Kayıt bilgilerini içeren KurumKisiPostModel parametresi.</param>
        /// <returns></returns>
        public Result<SistemLoginSifreYenilemeAktivasyonHareketleri> Register(KurumKisiPostModel model)
        {
            var config = _serviceProvider.GetService<IConfiguration>();
            if (config == null)
            {
                throw new OctapullException(OctapullExceptions.ServiceProviderError);
            }
            model.KisiModel.KurumsalMi = true;
            var x = _requestHelper.Post<Result<KurumKisiPostModel>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/KurumKisikaydet", model);

            if (x.Result.StatusCode == 902)
            {
                return Results.Fail("Kurum mevcuttur. Lütfen kurum yöneticinizle görüşünüz.");
            }
            int id = x.Result.Value.KisiModel.TabloID;
            var y = _requestHelper.Post<Result<SistemLoginSifreYenilemeAktivasyonHareketleri>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/HesapAktivasyonMailiOlustur", id);
            if (y != null)
            {
                // Burada Postacı Servis teki PostaciBekleyenGenelIslemEkle ye talimat gönderiyruz. mail işleri oradan yürür.

                var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();

                

                var postacimodel = new PostaciBekleyenIslemlerGenel
                {
                    PostaciIslemDurumTipiId = 1, //Param tablosunda "bekleniyor" kaydının Id değeri.bu işlem için sabit değer bu sebeple dinamik seçilmedi.
                    //TetiklemeTanimiTablosuSatirNo = 0, // "IcerikKurumsalSablonTanimlari" tablosunda "Şifre Güncellendi Bildirimi Şablonu" değerinin tabloID'si. bu işlem için sabit değer bu sebeple dinamik seçilmedi.
                    //TetiklemeyeKonuTabloSatirNo = id,
                    KisiID = x.Result.Value.KisiModel.TabloID,
                    KurumID = x.Result.Value.KurumModel.TabloID,
                    TetiklemeIlgiliKisiId= x.Result.Value.KisiModel.TabloID,
                    TetiklemeIlgiliKurumId= x.Result.Value.KurumModel.TabloID,
                    TetiklemeEpostaMi =true,
                    IcerikSablonTanimID=1,// Yeni Hesap Oluştur
                    
                };
                

                // Henüz login olmadığı için postacıservice işlemi kabul etmeyecektir. İşlemin login olmadan da yapılasını sağlarız ama bu güvenlik açığı meydana getirir.
                // Talimat tablosunda buradan kayıt düşeeğiz. 
                // Önceki yapıda da hatırlatma servisine kayıt düşülüyordu. PostacıService ile haberleşmemiz bu adımda database üzerinden olacaktır.
                // Dinlenen tabloya buradan kayıt düşeceğiz, gerisini postacı servis halledecektir.


                /*
                var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
                model.KisiID = Convert.ToInt32(cookie.KisiId);
                var kisiHassasBilgiler = this.KisiHassasBilgilerGetir(Convert.ToInt32(cookie.KisiId)).Value;

                var postacimodel = new PostaciBekleyenIslemlerGenel
                {
                    PostaciIslemDurumTipiId = 2, //Param tablosunda "bekleniyor" kaydının Id değeri.bu işlem için sabit değer bu sebeple dinamik seçilmedi.
                    TetiklemeTanimiTablosuSatirNo = 8, // "IcerikKurumsalSablonTanimlari" tablosunda "Şifre Güncellendi Bildirimi Şablonu" değerinin tabloID'si. bu işlem için sabit değer bu sebeple dinamik seçilmedi.
                    TetiklemeyeKonuTabloSatirNo = kisiHassasBilgiler.TabloID,
                    KisiID = Convert.ToInt32(cookie.KisiId),
                    KurumID = Convert.ToInt32(cookie.KurumId)
                };
                var x = _requestHelper.Post<Result<SifreModel>>(LocalPortlar.KisiServis + "/api/KisiService/SifreDegistir", model);
                var y = _postaciIslemlerService.SifreGuncellemeIslemiEkle(postacimodel);
                */
            }



            //Kişiye hesap aktivasyon maili gönderme
            // TODO : Hakan - Hatırlatma servisi kapatıldı.
            //var bildirim = new HatirlatmaKayitlar()
            //{
            //    AktifEdenKisiID = id,
            //    KurumID = x.Result.Value.KurumModel.TabloID,
            //    KayitEdenID = id,
            //    HatirlatmaKayıtEdenKisiId = id,
            //    KisiID = id,
            //    HatirlatmaMetni = config.GetValue<string>("WebAppLive") + y.Result.Value.HesapAktivasyonSayfasiGeciciUrl,
            //    HatirlatmaZamani = DateTime.Now,
            //    KayitTarihi = DateTime.Now,
            //    GuncellenmeTarihi = DateTime.Now,
            //    HatirlatmaTipi = "HesapAktivasyon",
            //    HatirlatmaSmsyollayacakMi = false,
            //    HatirlatmaEpostaYollayacakMi = true,
            //};
            //_hatirlatmaKayitService.Add(bildirim);




            return y.Result;
        }

        /// <summary>
        /// Hesap aktifleştirme işlemini Web API'ye yönlendiren method.
        /// </summary>
        /// <param name="guid"> etkinleştirilecek hesabı belirten GUID parametresi.</param>
        /// <returns></returns>

        public Result<KisiTemelBilgiler> HesapAktiflestirme(string guid)
        {
            var x = _requestHelper.Post<Result<KisiTemelBilgiler>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/HesapAktivasyonIslemi", guid);

            return x.Result;
        }

        /// <summary>
        /// Hesap aktifleştirme isteği halen katif mi kontrolü işlemini Web API'a yönlendiren method.
        /// </summary>
        /// <param name="guid">kontrol edilecek hesap aktifleştirme isteği için tanımlı GUID parametresi</param>
        /// <returns>kotrol sonucuna göre true veya false döner.</returns>

        public Result<bool> HesapAktivasyonLinkiGecerliMi(string guid)
        {
            var x = _requestHelper.Post<Result<bool>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/HesapAktivasyonLinkiGecerliMi", guid);
            return x.Result;
        }

        /// <summary>
        ///  Kişi kullanıcı adı ve mail adresinin bulunup bulunmadığının kontrol edilmesi için Web API'a gönderen method.
        /// </summary>
        /// <param name="mailOrUsername"> kontrol edilecek mail adresi/ kullanıcı adı</param>
        /// <returns>kontrol edilen değer yoksa true, varsa false döner.</returns>

        public Result<bool> KullaniciAdiKontrolu(string mailOrUsername)
        {
            var x = _requestHelper.Post<Result<bool>>(LocalPortlar.UserLoginregisterService + "/api/LoginRegister/KullaniciAdiKontrolu", mailOrUsername);
            return x.Result;
        }

        /// <summary>
        /// Logout işlemi sonrası kayıt oluşturulması için sessionId değerini Web API'a gönderen method.
        /// </summary>
        /// <param name="token">ilgili logout işlemine dair sessionId değeri</param>
        /// <returns>işlem sonucuna göre true veya false döndürür.</returns>

        public Result<bool> LogoutKaydiAt(string token)
        {
            var logoutKaydiReturn = _requestHelper.Post<Result<SistemLoginCariDurum>>(LocalPortlar.UserLoginregisterService + "/api/LoginDurumlari/LogoutKayitAt", token);

            return true.ToResult();
        }

        /// <summary>
        /// Kullanıcının son giriş yaptığı tarihi getiren metot
        /// </summary>
        /// <returns></returns>
        public Result<string> SonGiris()
        {
            var id = _bazCookieService.GetCookie().GetAwaiter().GetResult().KisiId;
            var url = LocalPortlar.UserLoginregisterService + "/api/LoginRegister/SonGirisTarihiGetir/" + id;
            return _requestHelper.Get<Result<string>>(url).Result;
        }

        /// <summary>
        /// Kullanıcın başarısız olduğu login sayısını getiren metot
        /// </summary>
        /// <param name="kisiId"></param>
        /// <returns></returns>
        public Result<SistemLoginSonDurum> BasarisizLoginSayisiGetir(int kisiId)
        {
            var x = _requestHelper.Get<Result<SistemLoginSonDurum>>(LocalPortlar.UserLoginregisterService + "/api/LoginDurumlari/BasarisizLoginSayisiGetir/" + kisiId);

            return x.Result;
        }

        /// <summary>
        /// Kullanıcının hesabının deaktif olduğu durumu değiştirmek için kullanılan metot
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<SistemLoginSonDurum> BasarisizLoginSayisiGuncelle(SistemLoginSonDurum model)
        {
            var x = _requestHelper.Post<Result<SistemLoginSonDurum>>(LocalPortlar.UserLoginregisterService + "/api/LoginDurumlari/BasarisizLoginSayisiGuncelle", model);
            return x.Result;
        }

        /// <summary>
        /// Sistemdeki üyeleri getirir
        /// </summary>
        /// <returns></returns>
        public Result<List<UyeListViewModel>> UyeleriGetir()
        {
            var x = _requestHelper.Get<Result<List<UyeListViewModel>>>(LocalPortlar.UserLoginregisterService + "/api/KurumKisiKayit/UyeleriGetir");
            return x.Result;
        }
    }
}