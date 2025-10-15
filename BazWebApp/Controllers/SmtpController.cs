using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baz.Attributes;
using Baz.Model.Entity;
using Baz.ProcessResult;
using BazWebApp.Models;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace BazWebApp.Controllers
{
    /// <summary>
    /// Smtp kontrol metodlarının yer aldığı sınıf
    /// </summary>
    public class SmtpController : Controller
    {
        private readonly ISmtpService _smtpService;

        /// <summary>
        /// Smtp kontrol metodlarının yer aldığı konst.
        /// </summary>
        /// <param name="smtpService"></param>
        public SmtpController(ISmtpService smtpService)
        {
            _smtpService = smtpService;
        }

        #region View Metots

        /// <summary>
        /// Sistem smtp ekleme sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/smtp/systemsmtpadd")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/smtp/systemsmtpadd")]
        public IActionResult SistemSmtpAdd()
        {
            return View("SistemSmtpAdd");
        }

        /// <summary>
        /// Sistem smtp listeleme sayfasına yönlendiren metod
        /// </summary>
        /// <returns></returns>
        [Route("/smtp/systemsmtplist")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/smtp/systemsmtplist")]
        public IActionResult SistemSmtpList()
        {
            return View("SistemSmtpList");
        }

        /// <summary>
        /// Sistem smtp güncelleme sayfasına yönlendiren metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [Route("/smtp/systemsmtpupdate/{tabloID}")]
        [Handlers.PolicyBasedAuthorize(yetkiKimlikAdi: "/smtp/systemsmtpupdate/{tabloID}")]
        public IActionResult SistemSmtpUpdate(int tabloID)
        {
            return View("SistemSmtpUpdate", tabloID);
        }

        #endregion View Metots

        #region Api Metots

        /// <summary>
        /// Sistem Smtp Değerleri kaydetme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Smtp Değerleri Kaydet")]
        [Route("/smtp/smtpadd")]
        [HttpPost]
        public Result<SistemSMTPDegerleri> SmtpKaydet([FromBody] SistemSMTPDegerleri model)
        {
            var result = _smtpService.SmtpKaydet(model);
            return result;
        }

        /// <summary>
        /// Sistem Smtp Değerleri Güncelleme metodu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Smtp Değerleri Güncelle")]
        [Route("/smtp/smtpupdate")]
        [HttpPost]
        public Result<SistemSMTPDegerleri> SmtpGuncelle([FromBody] SistemSMTPDegerleri model)
        {
            var result = _smtpService.SmtpGuncelle(model);
            return result;
        }

        /// <summary>
        /// Sistem Smtp Değerleri Silindi  metodu
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Smtp Değerleri Sil")]
        [Route("smtp/systemsmtpdelete/{tabloID}")]
        [HttpGet]
        public Result<bool> SmtpSil(int tabloID)
        {
            var result = _smtpService.SmtpSil(tabloID);
            return result;
        }

        /// <summary>
        /// Tablo Id'ye göre Sistem Smtp Değerlerini getiren metod
        /// </summary>
        /// <param name="tabloID"></param>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Smtp Değerleri Getir")]
        [Route("smtp/getsmtpbyid/{tabloID}")]
        [HttpGet]
        public Result<SistemSMTPDegerleri> SmtpGetir(int tabloID)
        {
            var result = _smtpService.SmtpGetir(tabloID);
            return result;
        }

        /// <summary>
        /// Sistem Smtp Değerleri listeleme metodu
        /// </summary>
        /// <returns></returns>
        [ProcessName(Name = "Sistem Smtp Değerleri Listele")]
        [Route("smtp/getsystemsmtplist")]
        [HttpGet]
        public Result<List<SistemSMTPDegerleri>> SmtpList()
        {
            var result = _smtpService.SmtpList();
            return result;
        }

        #endregion Api Metots
    }
}