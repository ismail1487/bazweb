using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ParamControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly ParamController _paramController;

        public ParamControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var paramService = _testServer.Services.GetService<IParamService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();

            _paramController = new ParamController(paramService, bazCookieService);
        }

        [TestMethod()]
        public void ViewParamController()
        {
            //Act
            var actual = _paramController.Index() as ViewResult;
            _helper.Get<ViewResult>("/panel/param/");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "Index");
        }

        [TestMethod()]
        public void ApiParamController()
        {
            // add
            var add = _paramController.Add(new ParametreRequest
            {
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "TestDin",
                UstId = 0
            });
            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(add.Value);

            // addNegative
            var addNegative = _paramController.Add(new ParametreRequest
            {
                UstId = 0
            });
            Assert.AreEqual(addNegative.Value, 0);
            Assert.IsFalse(addNegative.IsSuccess);

            // update
            var update = _paramController.Update(new ParametreRequest
            {
                TabloID = add.Value,
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "DinTest",
                UstId = 0
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(update.Value);

            // updateNegative
            var updateNegative = _paramController.Update(new ParametreRequest
            {
                UstId = 0,
                TabloID = add.Value,
            });
            Assert.AreEqual(updateNegative.Value, 0);
            Assert.IsFalse(updateNegative.IsSuccess);

            //list
            var list = _paramController.List(new ParametreRequest()
            {
                ModelName = "ParamDinler",
                DilID = 1,
                EsDilID = 1,
                Tanim = "test"
            });
            Assert.AreEqual(list.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Value);

            // listParam
            var listParam = _paramController.ListParam(new ParametreRequest()
            {
                ModelName = "ParamDinler",
                DilID = 1,
                EsDilID = 1,
                Tanim = "test"
            });
            Assert.AreEqual(listParam.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(listParam.Value);

            // delete
            var delete = _paramController.Delete(new ParametreRequest
            {
                TabloID = add.Value,
                ModelName = "ParamDinler",
                KurumId = 40,
                Tanim = "test"
            });
            Assert.AreEqual(delete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(delete.Value);
        }
    }
}