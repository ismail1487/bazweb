using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity.Constants;
using Baz.Model.Entity.ViewModel;
using Baz.ProcessResult;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class KureselParametrelerControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly KureselParametrelerController _kureselParametrelerController;
        public KureselParametrelerControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var kureselParametrelerService = _testServer.Services.GetService<IKureselParametrelerService>();

            _kureselParametrelerController = new KureselParametrelerController(kureselParametrelerService);

        }

        [TestMethod()]
        public void ViewKureselParametrelerController()
        {
            //Act - 1 SistemKureselparametreTanimla
            var actual = _kureselParametrelerController.SistemKureselparametreTanimla() as ViewResult;

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "SistemKureselParametreDegeriTanimla");

            //Act - 2 KureselParametreDegeriTanimla
            var actual1 = _kureselParametrelerController.KureselparametreTanimla() as ViewResult;
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "KureselParametreDegeriTanimla");
        }
        
        [TestMethod()]
        public void ApiKureselParametrelerController()
        {
            // Kullanılacak Model
            var model = new SistemKureselParametreDegeriView()
            {
                SistemMi = 2,
                KureselParams = new List<ParamKureselParam>()
                {
                    new ParamKureselParam()
                    {
                        ID = 29,
                        Adi = "Session Timeout",
                        Deger = 12200
                    }
                }
            };

            var model2 = new SistemKureselParametreDegeriView()
            {
                SistemMi = 1,
                KureselParams = new List<ParamKureselParam>()
                {
                    new ParamKureselParam()
                    {
                        ID = 0,
                        Adi = "Session Timeout",
                        Deger = 18000
                    }
                }
            };

            //defaultdegerler Modele göre
            var modellist = new List<ParamKureselParam>();
            foreach (var item in model.KureselParams)
            {
                var x = _globalHelper.Post<Result<KureselParametreModel>>(LocalPortlar.IYSService+"/api/KureselParametreler/IsmeGoreParamGetir", item.Adi).Result.Value;
                var y = new ParamKureselParam()
                {
                    Adi = x.ParamTanim,
                    Deger = x.ParametreBaslangicDegeri.Value,
                    MetinDegeri = x.ParametreMetinDegeri
                };
                modellist.Add(y);
            }


            // Assert-1 Kurum Add
            var kurumAdd = _kureselParametrelerController.ParamTanimiEkleGuncelle(model);
            Assert.AreEqual(kurumAdd.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(kurumAdd.IsSuccess);
            Assert.IsNotNull(kurumAdd.Value); 
            
            // Assert-1.0 Sistem Add
            var sistemAdd = _kureselParametrelerController.SystemParamTanimiEkleGuncelle(model2);
            Assert.AreEqual(sistemAdd.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(sistemAdd.IsSuccess);
            Assert.IsNotNull(sistemAdd.Value);

            model.KureselParams = null;

            // Assert-1.1 Kurum Add Negative
            var kurumAddNegative = _kureselParametrelerController.ParamTanimiEkleGuncelle(model);
            Assert.IsFalse(kurumAddNegative.IsSuccess);
            Assert.IsFalse(kurumAddNegative.Value);


            model2.KureselParams = null;
            // Assert-1.2 System Add Negative
            var systemAddNegative = _kureselParametrelerController.SystemParamTanimiEkleGuncelle(model2);
            Assert.IsFalse(systemAddNegative.IsSuccess);
            Assert.IsFalse(systemAddNegative.Value);

            // Assert-2 IdGoreKurumParamDegeriListele
            var idGoreKurumParamDegeriListele = _kureselParametrelerController.IdGoreKurumParamDegeriListele();
            Assert.AreEqual(idGoreKurumParamDegeriListele.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(idGoreKurumParamDegeriListele.IsSuccess);
            Assert.IsNotNull(idGoreKurumParamDegeriListele.Value);

            // Assert-3 IdGoreSistemParamDegeriListele
            var idGoreSistemParamDegeriListele = _kureselParametrelerController.IdGoreSistemParamDegeriListele();
            Assert.AreEqual(idGoreSistemParamDegeriListele.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(idGoreSistemParamDegeriListele.IsSuccess);
            Assert.IsNotNull(idGoreSistemParamDegeriListele.Value);

            // Assert-4 IdGoreParamAdiGetir
            var idGoreParamAdiGetir = _kureselParametrelerController.IdGoreParamAdiGetir(Convert.ToInt32(idGoreKurumParamDegeriListele.Value[0].KureselParametreId));
            Assert.AreEqual(idGoreParamAdiGetir.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(idGoreParamAdiGetir.IsSuccess);
            Assert.IsNotNull(idGoreParamAdiGetir.Value);


            // Assert-4.1 IdGoreParamAdiGetir Negative
            var idGoreParamAdiGetirNegative = _kureselParametrelerController.IdGoreParamAdiGetir(0);
            Assert.IsFalse(idGoreParamAdiGetirNegative.IsSuccess);
            Assert.IsNull(idGoreParamAdiGetirNegative.Value);
        }
    }
}