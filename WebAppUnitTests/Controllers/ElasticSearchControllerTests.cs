using Microsoft.VisualStudio.TestTools.UnitTesting;
using BazWebApp.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Baz.ElasticSearch.Abstract;
using Baz.ElasticSearch.Model;
using Baz.RequestManager;
using BazWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using WebAppUnitTests.Helper;

namespace WebAppUnitTests
{
    [TestClass()]
    public class ElasticSearchControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly ElasticSearchController _elasticSearchController;

        public ElasticSearchControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;

            var elasticService = _testServer.Services.GetService<IElasticService<KurumKisiSearchModel>>();
            var searchService = _testServer.Services.GetService<ISearchService>();
            var client = _testServer.Services.GetService<ElasticClient>();
            var kisiService = _testServer.Services.GetService<IKisiService>();
            var kurumService = _testServer.Services.GetService<IKurumService>();
            _elasticSearchController = new ElasticSearchController(searchService);
        }

        [TestMethod()]
        public void ViewElasticSearchController()
        {
            //Act-1 Index
            var actual = _elasticSearchController.Index() as ViewResult;
            _helper.Get<ViewResult>("/ElasticSearch/Index");

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual.ViewName) || actual.ViewName == "Index");


            //Act-2 CustomSearch
            var actual1 = _elasticSearchController.CustomSearch("bilal") as ViewResult;
            var actual2 = _elasticSearchController.CustomSearch(null) as ViewResult;
            _helper.Get<ViewResult>("/ElasticSearch/CustomSearch/" + "bilal");
           

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(actual1.ViewName) || actual1.ViewName == "Index");
            Assert.IsTrue(string.IsNullOrEmpty(actual2.ViewName) || actual2.ViewName == "Index");


        } 
        
    }
}