using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.Model.Entity;
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
    public class ModuleControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly ModuleController _moduleController;

        public ModuleControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var modulService = _testServer.Services.GetService<IModulService>();

            _moduleController = new ModuleController(modulService);
        }

        [TestMethod()]
        public void ViewModuleController()
        {
            //Act - ModuleDefinition
            var ModuleDefinition = _moduleController.ModuleDefinition() as ViewResult;
            _helper.Get<ViewResult>("/Module/Definition");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(ModuleDefinition.ViewName) || ModuleDefinition.ViewName == "ModuleDefinition");

            //Act -ModuleList
            var ModuleList = _moduleController.ModuleList() as ViewResult;
            _helper.Get<ViewResult>("/Module/List");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(ModuleList.ViewName) || ModuleList.ViewName == "ModuleList");

            //Act -ModuleUpdate
            var ModuleUpdate = _moduleController.ModuleUpdate() as ViewResult;
            _helper.Get<ViewResult>("/Module/Update/25");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(ModuleUpdate.ViewName) || ModuleUpdate.ViewName == "ModuleUpdate");
        }

        [TestMethod()]
        public void ApiModuleController()
        {
            // ModulAddOrUpdate

            var modulEkle = _moduleController.AddOrUpdate(new ModulDetayKayitModel
            {
                Name = "testUnit",
                SayfaId = new List<int>() { 1, 2 }
            });
            Assert.AreEqual(modulEkle.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(modulEkle.Value);

            // ModulAddOrUpdateOlumsuz
            var ModulAddOrUpdateOlumsuz = _moduleController.AddOrUpdate(new ModulDetayKayitModel
            {
            });
            Assert.IsNull(ModulAddOrUpdateOlumsuz.Value);

            //Assert negativeUpdate
            var negativeUpdate = _moduleController.AddOrUpdate(new ModulDetayKayitModel
            {
                TabloID = modulEkle.Value.TabloID
            });
            Assert.IsNull(negativeUpdate.Value);

            //Assert negativeUpdate2
            var negativeUpdate2 = _moduleController.AddOrUpdate(new ModulDetayKayitModel
            {
                TabloID = modulEkle.Value.TabloID,
                Name = "testUnitGüncel",
            });
            Assert.IsNull(negativeUpdate2.Value);

            //Assert update
            var update = _moduleController.AddOrUpdate(new ModulDetayKayitModel
            {
                TabloID = modulEkle.Value.TabloID,
                Name = "testUnitGüncel",
                SayfaId = new List<int>()
                {
                    3,
                    4
                }
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(update.Value);

            //list
            var list = _moduleController.ModulListForView(1);
            Assert.AreEqual(list.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(list.Value);

            //Assert negativeGet
            var negativeGet = _moduleController.GetModulveDetayData(0);
            Assert.IsFalse(negativeGet.IsSuccess);

            //Assert get
            var get = _moduleController.GetModulveDetayData(modulEkle.Value.TabloID);
            Assert.AreEqual(get.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsNotNull(get.Value);

            //Assert negativeDelete
            var negativeDelete = _moduleController.Remove(0);
            Assert.IsFalse(negativeDelete.Value);

            //Assert Delete
            var delete = _moduleController.Remove(modulEkle.Value.TabloID);
            Assert.AreEqual(delete.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(delete.Value);
        }
    }
}