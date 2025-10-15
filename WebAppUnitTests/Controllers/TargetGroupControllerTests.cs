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
    public class TargetGroupControllerTests
    {
        private readonly RequestHelper _helper;
        private readonly RequestHelper _globalHelper;
        private readonly TargetGroupController _targetGroupController;

        public TargetGroupControllerTests()
        {
            _helper = (RequestHelper)new TestServerRequestHelper().CreateHelper();
            var _testServer = new TestServerController().CreateServer();
            _globalHelper = new RequestHelper();
            _globalHelper._headers = _helper._headers;
            var kurumService = _testServer.Services.GetService<IKurumService>();
            var kisiService = _testServer.Services.GetService<IKisiService>();
            var paramService = _testServer.Services.GetService<IParamService>();
            var bazCookieService = _testServer.Services.GetService<IBazCookieService>();
            var targetGroupService = _testServer.Services.GetService<ITargetGroupService>();

            _targetGroupController = new TargetGroupController(kurumService, kisiService, paramService,
                bazCookieService, targetGroupService);
        }

        [TestMethod()]
        public void ViewTargetGroupController()
        {
            // Act - Add
            var Create = _targetGroupController.Add() as ViewResult;
            _helper.Get<ViewResult>("/TargetGroup/Add");

            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Create.ViewName) || Create.ViewName == "Add");

            //Act - Update
            var Update = _targetGroupController.Update() as ViewResult;
            _helper.Get<ViewResult>("/TargetGroup/Update");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(Update.ViewName) || Update.ViewName == "Update");

            //Act - List
            var List = _targetGroupController.List() as ViewResult;
            _helper.Get<ViewResult>("/TargetGroup/List");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(List.ViewName) || List.ViewName == "List");

            //Act - Test
            var SystemCreate = _targetGroupController.Test() as ViewResult;
            _helper.Get<ViewResult>("/TargetGroup/Test");
            //Assert
            Assert.IsTrue(string.IsNullOrEmpty(SystemCreate.ViewName) || SystemCreate.ViewName == "Test");
        }

        [TestMethod()]
        public void ApiTargetGroupController()
        {
            //Assert Add
            var add = _targetGroupController.Create(new HedefKitle
            {
                Filters = new List<HedefKitleFilter>()
                {
                    new()
                    {
                        LocigalOperator="AND",
                        Operator=null,
                        MemberName=null,
                        Value=null,
                        FilterType="group",
                        FieldType=null,
                        InputType=null,
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName=null,
                        TableName=null
                    },
                    new()
                    {
                       LocigalOperator="AND",
                        Operator="Contains",
                        MemberName="Kurum Kısa Adı",
                        Value="Yancılar",
                        FilterType="expression",
                        FieldType="System.String",
                        InputType="text",
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName="KurumKisaUnvan",
                        TableName=null
                    },
                    new(){
                        LocigalOperator="AND",
                        Operator="IsGreaterThan",
                        MemberName="Şube Sayısı",
                        Value="3",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="number",
                        ParametreTableName=null,
                        EkParametreId=3,
                        FieldName="Deger",
                        TableName="KurumEkParametreGerceklesenler"
                    },
                    new()
                    {
                       LocigalOperator="AND",
                        Operator="IsEqualTo",
                        MemberName="Ülke",
                        Value="1",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="select",
                        ParametreTableName="ParamUlkeler",
                        EkParametreId=0,
                        FieldName="KurumUlkeId",
                        TableName="KurumTemelBilgiler"
                    },
                    new()
                    {
                        LocigalOperator="AND",
                        Operator="IsLessThan",
                        MemberName="Şube Sayısı",
                        Value="6",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="number",
                        ParametreTableName=null,
                        EkParametreId=3,
                        FieldName="Deger",
                        TableName="KurumEkParametreGerceklesenler"
                    },
                    new()
                    {
                        LocigalOperator="AND",
                        Operator="DoesNotContain",
                        MemberName="Eposta Adresi",
                        Value="ğ",
                        FilterType="expression",
                        FieldType="System.String",
                        InputType="text",
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName="WebSitesi",
                        TableName="KurumTemelBilgiler"
                    }
                },
                Tanim = "Unit Test",
                KisiId = 129,
                KurumId = 82,
                HedefKitleTipi = "kurum"
            });

            Assert.AreEqual(add.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(add.IsSuccess);
            Assert.IsNotNull(add.Value);

            // GetTargetGroups
            var GetTargetGroups = _targetGroupController.GetTargetGroups();
            Assert.AreEqual(GetTargetGroups.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetTargetGroups.IsSuccess);

            var lastItem = GetTargetGroups.Value.LastOrDefault();

            //Assert update
            var update = _targetGroupController.Create(new HedefKitle
            {
                Filters = new List<HedefKitleFilter>()
                {
                    new()
                    {
                        LocigalOperator="AND",
                        Operator=null,
                        MemberName=null,
                        Value=null,
                        FilterType="group",
                        FieldType=null,
                        InputType=null,
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName=null,
                        TableName=null
                    },
                    new()
                    {
                       LocigalOperator="AND",
                        Operator="Contains",
                        MemberName="Kurum Kısa Adı",
                        Value="Yancılar",
                        FilterType="expression",
                        FieldType="System.String",
                        InputType="text",
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName="KurumKisaUnvan",
                        TableName=null
                    },
                    new(){
                        LocigalOperator="AND",
                        Operator="IsGreaterThan",
                        MemberName="Şube Sayısı",
                        Value="3",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="number",
                        ParametreTableName=null,
                        EkParametreId=3,
                        FieldName="Deger",
                        TableName="KurumEkParametreGerceklesenler"
                    },
                    new()
                    {
                       LocigalOperator="AND",
                        Operator="IsEqualTo",
                        MemberName="Ülke",
                        Value="1",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="select",
                        ParametreTableName="ParamUlkeler",
                        EkParametreId=0,
                        FieldName="KurumUlkeId",
                        TableName="KurumTemelBilgiler"
                    },
                    new()
                    {
                        LocigalOperator="AND",
                        Operator="IsLessThan",
                        MemberName="Şube Sayısı",
                        Value="6",
                        FilterType="expression",
                        FieldType="System.Int32",
                        InputType="number",
                        ParametreTableName=null,
                        EkParametreId=3,
                        FieldName="Deger",
                        TableName="KurumEkParametreGerceklesenler"
                    },
                    new()
                    {
                        LocigalOperator="AND",
                        Operator="DoesNotContain",
                        MemberName="Eposta Adresi",
                        Value="ğ",
                        FilterType="expression",
                        FieldType="System.String",
                        InputType="text",
                        ParametreTableName=null,
                        EkParametreId=0,
                        FieldName="WebSitesi",
                        TableName="KurumTemelBilgiler"
                    }
                },
                Tanim = "Unit Test",
                KisiId = 129,
                KurumId = 82,
                HedefKitleTipi = "kurum",      
                TabloID = lastItem.TabloID
            });
            Assert.AreEqual(update.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(update.IsSuccess);
            Assert.IsNotNull(update.Value);

            // GetPersonTargetGroups
            var GetPersonTargetGroups = _targetGroupController.GetPersonTargetGroups();
            Assert.AreEqual(GetPersonTargetGroups.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetPersonTargetGroups.IsSuccess);
            Assert.IsNotNull(GetPersonTargetGroups.Value);

            // GetCompanyTargetGroups
            var GetCompanyTargetGroups = _targetGroupController.GetCompanyTargetGroups();
            Assert.AreEqual(GetCompanyTargetGroups.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetCompanyTargetGroups.IsSuccess);
            Assert.IsNotNull(GetCompanyTargetGroups.Value);

            // GetTargetGroupsByKurumId
            var GetTargetGroupsByKurumId = _targetGroupController.GetCompanyTargetGroups();
            Assert.AreEqual(GetTargetGroupsByKurumId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetTargetGroupsByKurumId.IsSuccess);
            Assert.IsNotNull(GetTargetGroupsByKurumId.Value);

            // SingleOrDefaultForView
            var SingleOrDefaultForView = _targetGroupController.SingleOrDefaultForView(lastItem.TabloID);
            Assert.AreEqual(SingleOrDefaultForView.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(SingleOrDefaultForView.IsSuccess);
            Assert.IsNotNull(SingleOrDefaultForView.Value);

            // GetDynamicValueForKurum
            var GetDynamicValueForKurum = _targetGroupController.GetDynamicValueForKurum(lastItem.TabloID);
            Assert.AreEqual(GetDynamicValueForKurum.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetDynamicValueForKurum.IsSuccess);
            Assert.IsNotNull(GetDynamicValueForKurum.Value);

            // GetParamValue
            var GetParamValue = _targetGroupController.GetParamValue("ParamCinsiyet");
            Assert.AreEqual(GetParamValue.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetParamValue.IsSuccess);
            Assert.IsNotNull(GetParamValue.Value);

            //// GetTargetGroupFields Kişi
            //var GetTargetGroupFieldskisi = _targetGroupController.GetTargetGroupFields("kisi");
            //Assert.AreEqual(GetTargetGroupFieldskisi.StatusCode, (int)ResultStatusCode.Success);
            //Assert.IsTrue(GetTargetGroupFieldskisi.IsSuccess);

            //// GetTargetGroupFields Kurum
            //var GetTargetGroupFieldskurum = _targetGroupController.GetTargetGroupFields("kurum");
            //Assert.AreEqual(GetTargetGroupFieldskurum.StatusCode, (int)ResultStatusCode.Success);
            //Assert.IsTrue(GetTargetGroupFieldskurum.IsSuccess);

            // TargetGroupMembersListByGroupId
            var TargetGroupMembersListByGroupId = _targetGroupController.TargetGroupMembersListByGroupId(2153);
            Assert.AreEqual(TargetGroupMembersListByGroupId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(TargetGroupMembersListByGroupId.IsSuccess);
            Assert.IsNotNull(TargetGroupMembersListByGroupId.Value);

            // RunExpressionReturnUser Kişi
            var RunExpressionReturnUserkisi = _targetGroupController.RunExpressionReturnUser(2153, "kisi");
            Assert.AreEqual(RunExpressionReturnUserkisi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(RunExpressionReturnUserkisi.IsSuccess);
            Assert.IsNotNull(RunExpressionReturnUserkisi.Value);

            // RunExpressionReturnUser Kurum
            var RunExpressionReturnUserkurum = _targetGroupController.RunExpressionReturnUser(lastItem.TabloID, "kurum");
            Assert.AreEqual(RunExpressionReturnUserkurum.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(RunExpressionReturnUserkurum.IsSuccess);
            Assert.IsNotNull(RunExpressionReturnUserkurum.Value);


            // RunExpression Kişi
            var RunExpressionisi = _targetGroupController.RunExpression(1195, "kisi");
            Assert.AreEqual(RunExpressionisi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(RunExpressionisi.IsSuccess);
            Assert.IsNotNull(RunExpressionisi.Value);

            // RunExpression Kurum
            var RunExpressionkurum = _targetGroupController.RunExpression(lastItem.TabloID, "kurum");
            Assert.AreEqual(RunExpressionkurum.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(RunExpressionkurum.IsSuccess);
            Assert.IsNotNull(RunExpressionkurum.Value);

            // GetDynamicValueForKisi
            var GetDynamicValueForKisi = _targetGroupController.GetDynamicValueForKisi(1195);
            Assert.AreEqual(GetDynamicValueForKisi.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetDynamicValueForKisi.IsSuccess);
            Assert.IsNotNull(GetDynamicValueForKisi.Value);


            // GetParamValueWithUstID
            var GetParamValueWithUstID = _targetGroupController.GetParamValueWithUstID("ParamUlkeler", 1);
            Assert.AreEqual(GetParamValueWithUstID.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(GetParamValueWithUstID.IsSuccess);
            Assert.IsNotNull(GetParamValueWithUstID.Value);


            // HedefKitleSil
            var HedefKitleSil = _targetGroupController.HedefKitleSil(lastItem.TabloID);
            Assert.AreEqual(HedefKitleSil.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(HedefKitleSil.IsSuccess);

            // GetTargetGroupFields
            var getTargetGroupFields = _targetGroupController.GetTargetGroupFields("Kurum");
            Assert.AreEqual(getTargetGroupFields.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(getTargetGroupFields.IsSuccess);

            // GetTargetGroupFields Kisi
            var getTargetGroupFields1 = _targetGroupController.GetTargetGroupFields("Kişi");
            Assert.AreEqual(getTargetGroupFields1.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(getTargetGroupFields1.IsSuccess);

            // GetTargetGroupsByKurumId
            var getTargetGroupsByKurumId = _targetGroupController.GetTargetGroupsByKurumId();
            Assert.AreEqual(getTargetGroupsByKurumId.StatusCode, (int)ResultStatusCode.Success);
            Assert.IsTrue(getTargetGroupsByKurumId.IsSuccess);


        }
    }
}