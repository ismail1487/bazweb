using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using BazWebApp.Models;
using BazWebApp.Services;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity;

namespace BazWebApp.Handlers
{
    /// <summary>
    /// Excel işlemleri için oluşturulmuş servis sınıfı
    /// </summary>
    public interface IExcelReadWriteHandler
    {
        /// <summary>
        /// Kişi listesini excele import eden metod
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Result<bool> KisiListExcelImport(IFormFile file);
    }

    /// <summary>
    /// Excel handler
    /// </summary>
    public class ExcelReadWriteHandler : IExcelReadWriteHandler
    {
        private readonly IBazCookieService _bazCookieService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKurumService _kurumService;

        /// <summary>
        /// Excel handler const
        /// </summary>
        /// <param name="bazCookieService"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="kurumService"></param>
        public ExcelReadWriteHandler(IBazCookieService bazCookieService, IServiceProvider serviceProvider, IKurumService kurumService)
        {
            _bazCookieService = bazCookieService;
            _serviceProvider = serviceProvider;
            _kurumService = kurumService;
        }

        /// <summary>
        /// Kişi listesi import etme
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Result<bool> KisiListExcelImport(IFormFile file)
        {
            var cookie = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var fileName = file.FileName;
            var guid = Guid.NewGuid().ToString("N").Substring(0, 10);
            fileName = guid + fileName;

            var KisiList = new List<KisiTemelKayitModel>();

            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "TempExcel");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "TempExcel", fileName);

            if (file.Length > 0)
            {
                ISheet _sheet;
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                stream.Position = 0;
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (extension == ".xls")
                {
                    //HSSFWorkbook xlsBook = new(stream);
                    //_sheet = xlsBook.GetSheetAt(0);

                    XSSFWorkbook xlsxBook = new(stream);
                    _sheet = xlsxBook.GetSheetAt(0);

                    for (int i = (_sheet.FirstRowNum + 1); i <= _sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = _sheet.GetRow(i);
                        if (row == null)
                            continue;

                        var kisiAdi = row.GetCell(0) == null ? "" : row.GetCell(0).ToString();
                        var soyadi = row.GetCell(1) == null ? "" : row.GetCell(1).ToString();
                        var eposta = row.GetCell(2) == null ? "" : row.GetCell(2).ToString();
                        if (string.IsNullOrEmpty(kisiAdi) || string.IsNullOrEmpty(soyadi) || string.IsNullOrEmpty(eposta))
                        {
                            continue;
                        }
                        var kurum = KurumGetir(row.GetCell(3) == null ? "" : row.GetCell(3).ToString());
                        if (kurum == 0)
                            return Results.Fail("Belirttiğiniz isimnde bir kurum bulunmamaktadır, lütfen kontrol ediniz.");
                        var dogumtrh = row.GetCell(5)?.DateCellValue;
                        var isegiristarh = row.GetCell(9)?.DateCellValue;
                        var Kisimodel = new KisiTemelKayitModel
                        {
                            Adi = kisiAdi,
                            Soyadi = soyadi,
                            EpostaAdresi = eposta,
                            Kurum = kurum,
                            TCKimlikNo = row.GetCell(4) == null ? "" : row.GetCell(4).ToString(),
                            DogumTarihi = dogumtrh == null ? null : Convert.ToDateTime(dogumtrh),
                            AnneAdi = row.GetCell(6) == null ? "" : row.GetCell(6).ToString(),
                            BabaAdi = row.GetCell(7) == null ? "" : row.GetCell(7).ToString(),
                            SicilNo = row.GetCell(8) == null ? "" : row.GetCell(8).ToString(),
                            IseGirisTarihi = isegiristarh == null ? null : Convert.ToDateTime(isegiristarh),
                            //Kurum = Convert.ToInt32(cookie.KurumId),
                            DogduguUlke = UlkeIdGetir(row.GetCell(10) == null ? "" : row.GetCell(10).ToString())
                        };
                        Kisimodel.DogduguSehir = SehirIdGetir(Kisimodel.DogduguUlke, row.GetCell(11) == null ? "" : row.GetCell(11).ToString());
                        Kisimodel.Cinsiyeti = CinsiyetGetir(row.GetCell(12) == null ? "" : row.GetCell(12).ToString());
                        Kisimodel.MedeniHali = MedeniHalGetir(row.GetCell(13) == null ? "" : row.GetCell(13).ToString());
                        Kisimodel.Dini = 0;
                        Kisimodel.Departman = DepartmanGetir(row.GetCell(15) == null ? "" : row.GetCell(15).ToString(), Kisimodel.Kurum);
                        Kisimodel.Pozisyon = PozisyonGetir(row.GetCell(16) == null ? "" : row.GetCell(16).ToString(), Kisimodel.Kurum);
                        Kisimodel.Rol = RolGetir(row.GetCell(17) == null ? "" : row.GetCell(17).ToString(), Kisimodel.Kurum);
                        Kisimodel.Lokasyon = LokasyonGetir(row.GetCell(18) == null ? "" : row.GetCell(18).ToString(), Kisimodel.Kurum);
                        Kisimodel.AdresListesi = new List<AdresModel>();
                        Kisimodel.DilListesi = new List<DilModel>();
                        Kisimodel.TelefonListesi = new List<TelefonModel>();
                        Kisimodel.OkulListesi = new List<OkulModel>();

                        KisiList.Add(Kisimodel);
                    }
                }
                else if (extension == ".xlsx")
                {
                    XSSFWorkbook xlsxBook = new(stream);
                    _sheet = xlsxBook.GetSheetAt(0);

                    for (int i = (_sheet.FirstRowNum + 1); i <= _sheet.LastRowNum; i++) //Read Excel File
                    {
                        IRow row = _sheet.GetRow(i);
                        if (row == null)
                            continue;

                        var kisiAdi = row.GetCell(0) == null ? "" : row.GetCell(0).ToString();
                        var soyadi = row.GetCell(1) == null ? "" : row.GetCell(1).ToString();
                        var eposta = row.GetCell(2) == null ? "" : row.GetCell(2).ToString();

                        if (string.IsNullOrEmpty(kisiAdi) || string.IsNullOrEmpty(soyadi) || string.IsNullOrEmpty(eposta))
                        {
                            continue;
                        }

                        var kurum = KurumGetir(row.GetCell(3) == null ? "" : row.GetCell(3).ToString());
                        if (kurum == 0)
                            return Results.Fail("Belirttiğiniz isimnde bir kurum bulunmamaktadır, lütfen kontrol ediniz.");
                        var dogumtrh = row.GetCell(5)?.DateCellValue;
                        var isegiristarh = row.GetCell(9)?.DateCellValue;
                        var Kisimodel = new KisiTemelKayitModel
                        {
                            Adi = kisiAdi,
                            Soyadi = soyadi,
                            EpostaAdresi = eposta,
                            Kurum = kurum,
                            TCKimlikNo = row.GetCell(4) == null ? "" : row.GetCell(4).ToString(),
                            DogumTarihi = dogumtrh == null ? null : Convert.ToDateTime(dogumtrh),
                            AnneAdi = row.GetCell(6) == null ? "" : row.GetCell(6).ToString(),
                            BabaAdi = row.GetCell(7) == null ? "" : row.GetCell(7).ToString(),
                            SicilNo = row.GetCell(8) == null ? "" : row.GetCell(8).ToString(),
                            IseGirisTarihi = isegiristarh == null ? null : Convert.ToDateTime(isegiristarh),
                            //Kurum = Convert.ToInt32(cookie.KurumId),
                            DogduguUlke = UlkeIdGetir(row.GetCell(10) == null ? "" : row.GetCell(10).ToString())
                        };
                        Kisimodel.DogduguSehir = SehirIdGetir(Kisimodel.DogduguUlke, row.GetCell(11) == null ? "" : row.GetCell(11).ToString());
                        Kisimodel.Cinsiyeti = CinsiyetGetir(row.GetCell(12) == null ? "" : row.GetCell(12).ToString());
                        Kisimodel.MedeniHali = MedeniHalGetir(row.GetCell(13) == null ? "" : row.GetCell(13).ToString());
                        Kisimodel.Dini = 0;
                        Kisimodel.Departman = DepartmanGetir(row.GetCell(15) == null ? "" : row.GetCell(15).ToString(), Kisimodel.Kurum);
                        Kisimodel.Pozisyon = PozisyonGetir(row.GetCell(16) == null ? "" : row.GetCell(16).ToString(), Kisimodel.Kurum);
                        Kisimodel.Rol = RolGetir(row.GetCell(17) == null ? "" : row.GetCell(17).ToString(), Kisimodel.Kurum);
                        Kisimodel.Lokasyon = LokasyonGetir(row.GetCell(18) == null ? "" : row.GetCell(18).ToString(), Kisimodel.Kurum);
                        Kisimodel.AdresListesi = new List<AdresModel>();
                        Kisimodel.DilListesi = new List<DilModel>();
                        Kisimodel.TelefonListesi = new List<TelefonModel>();
                        Kisimodel.OkulListesi = new List<OkulModel>();

                        KisiList.Add(Kisimodel);
                    }
                }

                if (File.Exists(path))
                {
                    // If file found, delete it
                    File.Delete(path);
                }
                var _kisiService = _serviceProvider.GetService<IKisiService>();
                var result = _kisiService.ListeIleTemelKisikaydet(KisiList);
                if (result.Value == null)
                    return false.ToResult().WithReasons(result.Reasons);

                return true.ToResult();
            }
            if (File.Exists(path))
            {
                // If file found, delete it
                File.Delete(path);
            }

            return false.ToResult();
        }

        private int UlkeIdGetir(string parameter)
        {
            var _genelParametrelerService = _serviceProvider.GetService<IGenelParametrelerService>();
            var ulkeList = _genelParametrelerService.UlkeList().Value;
            var ulke = ulkeList.Where(a => a.ParamTanim.ToLower() == parameter.ToLower()).Select(x => x.TabloID).FirstOrDefault();
            return ulke;
        }

        private int SehirIdGetir(int? ulkeId, string parameter)
        {
            var _genelParametrelerService = _serviceProvider.GetService<IGenelParametrelerService>();
            var sehir = 0;
            if (ulkeId > 0)
            {
                var sehirList = _genelParametrelerService.SehirList(ulkeId.Value)?.Value;
                sehir = sehirList.Where(x => x.ParamTanim.ToLower() == parameter.ToLower()).Select(p => p.TabloID).FirstOrDefault();
            }
            return sehir;
        }

        private int CinsiyetGetir(string parameter)
        {
            var _genelParametrelerService = _serviceProvider.GetService<IGenelParametrelerService>();
            var cinsiyetList = _genelParametrelerService.CinsiyetList().Value;
            var cinsiyet = cinsiyetList.Where(a => a.ParamTanim.ToLower() == parameter.ToLower()).Select(x => x.TabloID).FirstOrDefault();
            return cinsiyet;
        }

        private int MedeniHalGetir(string parameter)
        {
            var _genelParametrelerService = _serviceProvider.GetService<IGenelParametrelerService>();
            var medeniHalList = _genelParametrelerService.MedeniHalList().Value;
            var medeniHal = medeniHalList.Where(a => a.ParamTanim.ToLower() == parameter.ToLower()).Select(x => x.TabloID).FirstOrDefault();
            return medeniHal;
        }

        private int DepartmanGetir(string parameter, int kurumId)
        {
            var _orgBirimService = _serviceProvider.GetService<IParamOrganizasyonBirimleriService>();

            var departmanList = _orgBirimService.ListTip(new KurumOrganizasyonBirimRequest()
            {
                KurumId = kurumId,
                Name = "departman"
            }).Value;
            if (departmanList == null)
                return 0;
            var departman = departmanList.Where(a => a.Tanim.ToLower() == parameter.ToLower()).Select(s => s.TabloId).FirstOrDefault();
            return departman;
        }

        private int PozisyonGetir(string parameter, int kurumId)
        {
            var _orgBirimService = _serviceProvider.GetService<IParamOrganizasyonBirimleriService>();

            var pozisyonList = _orgBirimService.ListTip(new KurumOrganizasyonBirimRequest()
            {
                KurumId = kurumId,
                Name = "pozisyon"
            }).Value;
            if (pozisyonList == null)
                return 0;
            var pozisyon = pozisyonList.Where(a => a.Tanim.ToLower() == parameter.ToLower()).Select(s => s.TabloId).FirstOrDefault();
            return pozisyon;
        }

        private int RolGetir(string parameter, int kurumId)
        {
            var _orgBirimService = _serviceProvider.GetService<IParamOrganizasyonBirimleriService>();

            var rolList = _orgBirimService.ListTip(new KurumOrganizasyonBirimRequest()
            {
                KurumId = kurumId,
                Name = "rol"
            }).Value;
            if (rolList == null)
                return 0;
            var rol = rolList.Where(a => a.Tanim.ToLower() == parameter.ToLower()).Select(s => s.TabloId).FirstOrDefault();
            return rol;
        }

        private int LokasyonGetir(string parameter, int kurumId)
        {
            var _orgBirimService = _serviceProvider.GetService<IParamOrganizasyonBirimleriService>();

            var lokasyonList = _orgBirimService.ListTip(new KurumOrganizasyonBirimRequest()
            {
                KurumId = kurumId,
                Name = "lokasyon"
            }).Value;
            if (lokasyonList == null)
                return 0;
            var lokasyon = lokasyonList.Where(a => a.Tanim.ToLower() == parameter.ToLower()).Select(s => s.TabloId).FirstOrDefault();
            return lokasyon;
        }

        private int KurumGetir(string parameter)
        {
            var cook = _bazCookieService.GetCookie().GetAwaiter().GetResult();
            var kurumList = _kurumService.AmirTemsilciyeGoreKurumListesi(cook).Value ?? new List<KurumTemelBilgiler>();
            var kurum = kurumList.FirstOrDefault(a => a.KurumKisaUnvan.ToLower() == parameter.ToLower()) ?? new KurumTemelBilgiler();

            return kurum.TabloID;
        }
    }
}