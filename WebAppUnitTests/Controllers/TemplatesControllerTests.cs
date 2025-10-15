using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class TemplatesControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly TemplatesController _templatesController;

        public TemplatesControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var icerikKurumsalSablonTanimlariService = _testServer.Services.GetService<IIcerikKurumsalSablonTanimlariService>();
            var cookie = _testServer.Services.GetService<IBazCookieService>();

            _templatesController = new TemplatesController(icerikKurumsalSablonTanimlariService, cookie);
        }

        [TestMethod()]
        public void ViewTemplatesController()
        {
            //Act - 1
            var Create = _templatesController.Create() as ViewResult;
            _helper.Get<ViewResult>("/Templates/Create");

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Create.ViewName) || Create.ViewName == "Create");

            //Act - Update
            var Update = _templatesController.Update() as ViewResult;
            _helper.Get<ViewResult>("/Templates/Update/" + 9);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Update.ViewName) || Update.ViewName == "Update");

            //Act - List
            var List = _templatesController.List() as ViewResult;
            _helper.Get<ViewResult>("/Templates/List");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(List.ViewName) || List.ViewName == "List");

            //Act - SystemCreate
            var SystemCreate = _templatesController.CreateForSystem() as ViewResult;
            _helper.Get<ViewResult>("/Templates/CreateForSystem");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SystemCreate.ViewName) || SystemCreate.ViewName == "SystemCreate");

            //Act - UpdateForSystem
            var UpdateForSystem = _templatesController.UpdateForSystem() as ViewResult;
            _helper.Get<ViewResult>("/Templates/UpdateForSystem/" + 9);
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(UpdateForSystem.ViewName) || UpdateForSystem.ViewName == "SystemUpdate");

            //Act - ListForSystem
            var ListForSystem = _templatesController.ListForSystem() as ViewResult;
            _helper.Get<ViewResult>("/Templates/ListForSystem");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(ListForSystem.ViewName) || ListForSystem.ViewName == "SystemList");
        }

        [TestMethod()]
        public void ApiTemplatesController()
        {
            //Assert Kurumsal negativeAdd
            var negativeAdd = _templatesController.SaveTemplate(new IcerikKurumsalSablonTanimlari
            {
                IcerikTamMetin = "Test metni Kurumsal",
                KurumID = 82,
                AktifMi = 1,
                IcerikBaslik = "Test başlık",
                IcerikTanim = "Test icerik tanım",
            });
            Assert.IsFalse(negativeAdd.IsSuccess);

            //Assert kurumsalAdd
            var add = _templatesController.SaveTemplate(new IcerikKurumsalSablonTanimlari
            {
                GonderimTipi = "test",
                SablonIcerikTipiId = 1,
                IcerikTamMetin = "Test metni Kurumsal",
                KurumID = 82,
                AktifMi = 1,
                AktiflikTarihi = DateTime.Now,
                GuncellenmeTarihi = DateTime.Now,
                IcerikBaslik = "Test başlık",
                KayitTarihi = DateTime.Now,
                IcerikTanim = "Test icerik tanım",
            });
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(add.IsSuccess);
            Assert.IsNotNull(add.Value);

            //Assert negativeUpdate Kurumsal
            var negativeUpdate = _templatesController.UpdateTemplate(new IcerikKurumsalSablonTanimlari
            {
                IcerikTamMetin = "Test metni Kurumsal",
                KurumID = 82,
                AktifMi = 1,
                IcerikBaslik = "Test başlık",
                IcerikTanim = "Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım ",
                TabloID = add.Value.TabloID
            });

            Assert.IsFalse(negativeUpdate.IsSuccess);
            Assert.IsNull(negativeUpdate.Value);

            //Assert update Kurumsal
            var update = _templatesController.UpdateTemplate(new IcerikKurumsalSablonTanimlari
            {
                GonderimTipi = "test",
                SablonIcerikTipiId = 1,
                IcerikTamMetin = "Güncel metin Kurumsal",
                KurumID = 82,
                AktifMi = 1,
                AktiflikTarihi = DateTime.Now,
                GuncellenmeTarihi = DateTime.Now,
                KayitTarihi = add.Value.KayitTarihi,
                IcerikBaslik = "Güncel başlık",
                IcerikTanim = "Güncel Tanim",
                TabloID = add.Value.TabloID
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);

            Assert.IsTrue(update.IsSuccess);
            Assert.IsNotNull(update.Value);

            //Assert negativeget Kurumsal
            var negativeGetKurumsal = _templatesController.GetTemplate(0);
            Assert.IsFalse(negativeGetKurumsal.IsSuccess);
            Assert.IsNull(negativeGetKurumsal.Value);

            //Assert get Kurumsal
            var getKurumsal = _templatesController.GetTemplate(add.Value.TabloID);
            Assert.AreEqual(getKurumsal.StatusCode, (int)ResultStatusCode.Success);

            Assert.IsTrue(getKurumsal.IsSuccess);
            Assert.IsNotNull(getKurumsal.Value);

            //Assert Sistem negativeAdd
            var negativeAddSistem = _templatesController.SaveTemplateForSystem(new IcerikKurumsalSablonTanimlari
            {
                IcerikTamMetin = "Test metni Sistemsel",
                KurumID = 82,
                AktifMi = 1,
                IcerikBaslik = "Test başlık",
                IcerikTanim = "Test icerik tanım",
            });
            Assert.IsFalse(negativeAddSistem.IsSuccess);

            //Assert Sistem Add
            var addSistem = _templatesController.SaveTemplateForSystem(new IcerikKurumsalSablonTanimlari
            {
                GonderimTipi = "test",
                SablonIcerikTipiId = 1,
                IcerikTamMetin = "Test metni Sistemsel",
                KurumID = 82,
                AktifMi = 1,
                AktiflikTarihi = DateTime.Now,
                GuncellenmeTarihi = DateTime.Now,
                IcerikBaslik = "Test başlık",
                KayitTarihi = DateTime.Now,
                IcerikTanim = "Test icerik tanım",
            });
            Assert.AreEqual(addSistem.StatusCode, (int)ResultStatusCode.Success);

            Assert.IsTrue(addSistem.IsSuccess);
            Assert.IsNotNull(addSistem.Value);

            //Assert negativeUpdate Sistem
            var negativeUpdateSistem = _templatesController.UpdateTemplateForSystem(new IcerikKurumsalSablonTanimlari
            {
                IcerikTamMetin = "Test metni Sistemsel",
                KurumID = 82,
                AktifMi = 1,
                IcerikBaslik = "Test başlık",
                IcerikTanim = "Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım Test icerik tanım ",
                TabloID = addSistem.Value.TabloID
            });

            Assert.IsFalse(negativeUpdateSistem.IsSuccess);
            Assert.IsNull(negativeUpdateSistem.Value);

            //Assert update Sistemsel
            var updateSistemsel = _templatesController.UpdateTemplateForSystem(new IcerikKurumsalSablonTanimlari
            {
                GonderimTipi = "test",
                SablonIcerikTipiId = 1,
                IcerikTamMetin = "Güncel metin Sistemsel",
                KurumID = 82,
                AktifMi = 1,
                AktiflikTarihi = DateTime.Now,
                GuncellenmeTarihi = DateTime.Now,
                KayitTarihi = addSistem.Value.KayitTarihi,
                IcerikBaslik = "Güncel başlık",
                IcerikTanim = "Güncel Tanim",
                TabloID = addSistem.Value.TabloID
            });
            Assert.AreEqual(updateSistemsel.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(updateSistemsel.IsSuccess);
            Assert.IsNotNull(updateSistemsel.Value);

            //Assert negativeget Sistem
            var negativeGetSistem = _templatesController.GetTemplate(0);
            Assert.IsFalse(negativeGetSistem.IsSuccess);
            Assert.IsNull(negativeGetSistem.Value);

            //Assert get Sistem
            var getSistem = _templatesController.GetTemplate(add.Value.TabloID);
            Assert.AreEqual(getSistem.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(getSistem.IsSuccess);
            Assert.IsNotNull(getSistem.Value);

            // ListSystem
            var ListSystem = _templatesController.ListSystem();
            Assert.AreEqual(ListSystem.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(ListSystem.IsSuccess);
            Assert.IsNotNull(ListSystem.Value);

            // ListForKurum
            var ListForKurum = _templatesController.ListForKurum();
            Assert.AreEqual(ListForKurum.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(ListForKurum.IsSuccess);
            Assert.IsNotNull(ListForKurum.Value);

            // IcerikSablonTipleriList
            var IcerikSablonTipleriList = _templatesController.IcerikSablonTipleriList();
            Assert.AreEqual(IcerikSablonTipleriList.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(IcerikSablonTipleriList.IsSuccess);

            //Assert negativeDelete
            var negativeDelete = _templatesController.SetDeleted(0);

            Assert.IsFalse(negativeDelete.IsSuccess);
            Assert.IsNull(negativeDelete.Value);

            //Assert setdelete Kurumsal
            var delete = _templatesController.SetDeleted(add.Value.TabloID);

            Assert.AreEqual(delete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.IsSuccess);
            Assert.IsNotNull(delete.Value);

            //Assert setdelete Sistem
            var setdelete = _templatesController.SetDeleted(addSistem.Value.TabloID);

            Assert.AreEqual(setdelete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(setdelete.IsSuccess);
            Assert.IsNotNull(setdelete.Value);
        }
    }
}