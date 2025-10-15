using Baz.Model.Entity;
using Baz.Model.Entity.Constants;
using Baz.ProcessResult;
using Baz.RequestManager.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Baz.Model.Entity.ViewModel;

namespace BazWebApp.Services
{
    /// <summary>
    /// Genel parametrelerin servis metotlarının yer aldığı interface.
    /// </summary>
    public interface IGenelParametrelerService
    {
        /// <summary>
        /// Dilleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Cinsiyetleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamCinsiyet>> CinsiyetList();

        /// <summary>
        /// Ülkeleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamUlkeler>> UlkeList();

        /// <summary>
        /// Şehirleri listeleyen servis metotu
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        Result<List<ParamUlkeler>> SehirList(int ulkeID);

        /// <summary>
        /// Medeni hali listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamMedeniHal>> MedeniHalList();

        /// <summary>
        /// Adres tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamAdresTipi>> AdresTipiList();

        /// <summary>
        /// Okul tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamOkulTipi>> OkulTipiList();

        /// <summary>
        /// Telefon tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamTelefonTipi>> TelefonTipiList();

        /// <summary>
        /// Dinleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamDinler>> DinlerList();

        /// <summary>
        /// Çalışan sayılarını listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamCalisanSayilari>> CalisanSayilariList();

        /// <summary>
        /// Vergi dairelerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        

        /// <summary>
        /// Kurum lokasyon tiplerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamKurumLokasyonTipi>> KurumLokasyonTipiList();

        /// <summary>
        /// Kurum İlişki türlerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamIliskiTurleri>> KurumIliskiTuruGetir();

        /// <summary>
        /// Kisi İlişki türlerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamIliskiTurleri>> KisiIliskiTuruGetir();
        

        /// <summary>
        /// Yabanci dil seviye listeleyen method
        /// </summary>
        /// <returns></returns>
        Result<List<ParamDilSeviyesi>> YabanciDilSeviyesiList();

        /// <summary>
        /// Sistem Dillerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamDiller>> SistemDilList();

        /// <summary>
        /// Bankaları listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        Result<List<ParamBankalar>> BankaList();

        /// <summary>
        /// Subeleri listeleyen servis metotu bana idye göre
        /// </summary>
        /// <param name="bankaID"></param>
        /// <returns></returns>
        Result<List<ParamBankalar>> SubeList(int bankaID);

        /// <summary>
        /// Ulke Kodlarını ver maskelerini getiren metod
        /// </summary>
        /// <returns></returns>
    }

    /// <summary>
    /// Genel parametrelerin servis metotlarının yer aldığı sınıftır.
    /// </summary>
    public class GenelParametrelerService : IGenelParametrelerService
    {
        private readonly IRequestHelper _requestHelper;
        private readonly IParamService _paramService;
        private readonly int cultureId;

        /// <summary>
        /// Genel parametrelerin servis metotlarının yer aldığı sınıfının yapıcı methodu
        /// </summary>
        /// <param name="requestHelper"></param>
        /// <param name="localizationService"></param>
        /// <param name="paramService"></param>
        public GenelParametrelerService(IRequestHelper requestHelper, ILocalizationService localizationService, IParamService paramService)
        {
            _requestHelper = requestHelper;
            _paramService = paramService;
            cultureId = localizationService.GetCultureId();
        }

        

        /// <summary>
        /// Sistem Dillerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamDiller>> SistemDilList()
        {
            var request = new ParametreRequest()
            {
                ModelName = "ParamDiller",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            List<ParamDiller> res = new List<ParamDiller>();
            if (x.Value!=null)
            {
               res = x.Value.Select(s => new ParamDiller()
                    {
                        ParamTanim = s.Tanim,
                        TabloID = s.TabloID,
                        UstId = s.UstId
                    }).ToList();
            }
            
            return res.ToResult();
        }
       
        /// <summary>
        /// Cinsiyetleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamCinsiyet>> CinsiyetList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamCinsiyet",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamCinsiyet>().ToResult();
            var res = x.Value.Select(s => new ParamCinsiyet()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Ülkeleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamUlkeler>> UlkeList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamUlkeler",
                Tanim = "test",
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamUlkeler>().ToResult();

            var res = x.Value.Select(s => new ParamUlkeler()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Şehirleri listeleyen servis metotu
        /// </summary>
        /// <param name="ulkeID"></param>
        /// <returns></returns>
        public Result<List<ParamUlkeler>> SehirList(int ulkeID)
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamUlkeler",
                UstId = ulkeID,
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamUlkeler>().ToResult();

            var res = x.Value.Select(s => new ParamUlkeler()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Bankaları listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamBankalar>> BankaList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamBankalar",
                Tanim = "test",
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamBankalar>().ToResult();

            var res = x.Value.Select(s => new ParamBankalar()
            {
                ParamTanim = s.Tanim,
                ParamKod = s.ParamKod,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Subeleri listeleyen servis metotu bana idye göre
        /// </summary>
        /// <param name="bankaID"></param>
        /// <returns></returns>
        public Result<List<ParamBankalar>> SubeList(int bankaID)
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamBankalar",
                UstId = bankaID,
                Tanim = "test",
                ParamKod = ""
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamBankalar>().ToResult();

            var res = x.Value.Select(s => new ParamBankalar()
            {
                ParamTanim = s.Tanim,
                ParamKod = s.ParamKod,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        

        /// <summary>
        /// Medeni hali listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamMedeniHal>> MedeniHalList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamMedeniHal",
                Tanim = "test",
            };

            var x = _paramService.List(request);
            if(x.Value == null)
                return new List<ParamMedeniHal>().ToResult();
            var res = x.Value.Select(s => new ParamMedeniHal()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Adres tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamAdresTipi>> AdresTipiList()
        {
            
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamAdresTipi",
                Tanim = "test"
            };
            var x = _paramService.List(request);
            if (x.Value == null)  // boş ise boş dön
                return new List<ParamAdresTipi>().ToResult();

            var res = x.Value.Select(s => new ParamAdresTipi()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();




            return res.ToResult();
        }

        /// <summary>
        /// Okul tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamOkulTipi>> OkulTipiList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamOkulTipi",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null) // Boş ise boş dön
                return new List<ParamOkulTipi>().ToResult();
            var res = x.Value.Select(s => new ParamOkulTipi()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Telefon tipini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamTelefonTipi>> TelefonTipiList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamTelefonTipi",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamTelefonTipi>().ToResult();
            var res = x.Value.Select(s => new ParamTelefonTipi()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Dinleri listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamDinler>> DinlerList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamDinler",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamDinler>().ToResult();
            var res = x.Value.Select(s => new ParamDinler()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Çalışan sayılarını listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamCalisanSayilari>> CalisanSayilariList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamCalisanSayilari",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamCalisanSayilari>().ToResult();
            var res = x.Value.Select(s => new ParamCalisanSayilari()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        

        /// <summary>
        /// Kurum lokasyon tiplerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamKurumLokasyonTipi>> KurumLokasyonTipiList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamKurumLokasyonTipi",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x.Value == null)
                return new List<ParamKurumLokasyonTipi>().ToResult();
            var res = x.Value.Select(s => new ParamKurumLokasyonTipi()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }

        /// <summary>
        /// Kurum İlişki türlerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamIliskiTurleri>> KurumIliskiTuruGetir()
        {
            var x = _requestHelper.Get<Result<List<ParamIliskiTurleri>>>(LocalPortlar.IYSService + "/api/GenelParametreler/KodaGoreIliskiTurleri/F");
            if (x == null) return new List<ParamIliskiTurleri>().ToResult();
            var newList = x.Result.Value.FindAll(x => x.DilID == cultureId);
            return newList.ToResult();
        }

        /// <summary>
        /// Kisi İlişki türlerini listeleyen servis metotu
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamIliskiTurleri>> KisiIliskiTuruGetir()
        {
            var x = _requestHelper.Get<Result<List<ParamIliskiTurleri>>>(LocalPortlar.IYSService + "/api/GenelParametreler/KodaGoreIliskiTurleri/K");
            if (x == null) return new List<ParamIliskiTurleri>().ToResult();
            var newList = x.Result.Value.FindAll(x => x.DilID == cultureId);
            return newList.ToResult();
        }

        /// <summary>
        /// Yabanci dil seviye listeleyen method
        /// </summary>
        /// <returns></returns>
        public Result<List<ParamDilSeviyesi>> YabanciDilSeviyesiList()
        {
            var request = new ParametreRequest()
            {
                DilID = cultureId,
                ModelName = "ParamDilSeviyesi",
                Tanim = "test"
            };

            var x = _paramService.List(request);
            if (x == null) return new List<ParamDilSeviyesi>().ToResult();
            var res = x.Value.Select(s => new ParamDilSeviyesi()
            {
                ParamTanim = s.Tanim,
                TabloID = s.TabloID,
                UstId = s.UstId
            }).ToList();
            return res.ToResult();
        }
    }
}