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
    public class SmtpControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly SmtpController _smtpController;

        public SmtpControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var smtpService = _testServer.Services.GetService<ISmtpService>();

            _smtpController = new SmtpController(smtpService);
        }

        [TestMethod()]
        public void ViewSmtpController()
        {
            //Act  SistemSmtpAdd
            var SistemSmtpAdd = _smtpController.SistemSmtpAdd() as ViewResult;
            _helper.Get<ViewResult>("/smtp/systemsmtpadd");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SistemSmtpAdd.ViewName) || SistemSmtpAdd.ViewName == "SistemSmtpAdd");

            //Act SistemSmtpList
            var SistemSmtpList = _smtpController.SistemSmtpList() as ViewResult;
            _helper.Get<ViewResult>("/smtp/systemsmtplist");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SistemSmtpList.ViewName) || SistemSmtpList.ViewName == "SistemSmtpList");

            //Act SistemSmtpUpdate
            var SistemSmtpUpdate = _smtpController.SistemSmtpUpdate(5) as ViewResult;
            _helper.Get<ViewResult>("/smtp/systemsmtpupdate/5");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SistemSmtpUpdate.ViewName) || SistemSmtpUpdate.ViewName == "SistemSmtpUpdate");
        }

        [TestMethod()]
        public void ApiSmtpController()
        {
            //Assert-1 Add
            var add = _smtpController.SmtpKaydet(new SistemSMTPDegerleri()
            {
                SMTPAdi = "Test Yeni Ayar3",
                SmtpBaglantiKullaniciAdi = "test1@mail.com",
                SmtpBaglantiDizisi = "test1.mail.com",
                SmtpBaglantiSifre = "123",
                SMTPPort = 110,
                IlgiliKurumID = 82,
                KurumID = 82,
                KayitEdenID = 129,
                KisiID = 129
            });
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(add.Value);

            //Assert-2 NegativeAdd(Smtp kullanıcı adı 50 karakter sınırı aşımı)
            var negativeadd = _smtpController.SmtpKaydet(new SistemSMTPDegerleri()
            {
                SMTPAdi = "Test Yeni Ayar3",
                SmtpBaglantiKullaniciAdi = "test1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.com",
                SmtpBaglantiDizisi = "test1.mail.com",
                SmtpBaglantiSifre = "123",
                SMTPPort = 110,
                IlgiliKurumID = 82,
                KurumID = 82,
                KayitEdenID = 129,
                KisiID = 129
            });
            Assert.IsFalse(negativeadd.IsSuccess);

            //Assert-3 Update
            var update = _smtpController.SmtpGuncelle(new SistemSMTPDegerleri()
            {
                TabloID = add.Value.TabloID,
                SMTPAdi = "Test Yeni Ayar45",
                SmtpBaglantiKullaniciAdi = "test1@mail.com",
                SmtpBaglantiDizisi = "test1.mail.com",
                SmtpBaglantiSifre = "123",
                SMTPPort = 110
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(update.Value);

            //Assert-4 NegativeUpdate(Smtp kullanıcı adı 50 karakter sınırı aşımı)
            var negativeudate = _smtpController.SmtpGuncelle(new SistemSMTPDegerleri()
            {
                TabloID = add.Value.TabloID,
                SMTPAdi = "Test Yeni Ayar3",
                SmtpBaglantiKullaniciAdi = "test1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.comtest1@mail.com",
                SmtpBaglantiDizisi = "test1.mail.com",
                SmtpBaglantiSifre = "123",
                SMTPPort = 110,
                IlgiliKurumID = 82,
                KurumID = 82,
                KayitEdenID = 129,
                KisiID = 129
            });
            Assert.IsFalse(negativeudate.IsSuccess);

            //Assert-5 List
            var list = _smtpController.SmtpList();
            Assert.AreEqual(list.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Value);

            //Assert-6 negativeGetId
            var negativeGetId = _smtpController.SmtpGetir(0);
            Assert.IsNull(negativeGetId.Value);

            //Assert-7 GetId
            var getId = _smtpController.SmtpGetir(add.Value.TabloID);
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(getId.Value);

            //Assert-8 negativeDelete
            var negativeDelete = _smtpController.SmtpSil(0);
            Assert.IsFalse(negativeDelete.Value);

            //Assert-9 Delete
            var delete = _smtpController.SmtpSil(add.Value.TabloID);
            Assert.AreEqual(delete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.Value);
        }
    }
}