"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.AjaxConfiguration = exports.ServiceConfiguration = exports.Service = void 0;
var Service = /** @class */ (function () {
    function Service(serviceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
    }
    Service.prototype.add = function (entity) {
        throw new Error("Method not implemented.");
    };
    Service.prototype.update = function (entity) {
        throw new Error("Method not implemented.");
    };
    Service.prototype.remove = function (entity) {
        throw new Error("Method not implemented.");
    };
    Service.prototype.single = function (id) {
    };
    Service.prototype.postData = function (url, data, configuration) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: configuration.type.toString(),
                dataType: configuration.dataType,
                contentType: configuration.contentType,
                data: data,
                url: url,
                success: function (result) {
                    resolve(result);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    };
    Service.prototype.getData = function (url, data, configuration) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                type: configuration.type.toString(),
                dataType: configuration.dataType,
                contentType: configuration.contentType,
                data: data,
                url: url,
                success: function (result) {
                    resolve(result);
                },
                error: function (error) {
                    reject(error);
                }
            });
        });
    };
    return Service;
}());
exports.Service = Service;
var ServiceConfiguration = /** @class */ (function () {
    function ServiceConfiguration() {
    }
    ServiceConfiguration.prototype.CreateDefault = function (coreUrl) {
        var config = new ServiceConfiguration();
        config.Create = AjaxConfiguration.postDefault();
        config.Update = AjaxConfiguration.postDefault();
        config.Remove = AjaxConfiguration.postDefault();
        config.Single = AjaxConfiguration.getDafault();
        config.List = AjaxConfiguration.getDafault();
        config.Create.url = coreUrl + "/add";
        config.Update.url = coreUrl + "/update";
        config.Remove.url = coreUrl + "/remove";
        config.List.url = coreUrl + "/list";
        config.Single.url = coreUrl + "/single";
    };
    return ServiceConfiguration;
}());
exports.ServiceConfiguration = ServiceConfiguration;
var AjaxConfiguration = /** @class */ (function () {
    function AjaxConfiguration() {
    }
    AjaxConfiguration.getDafault = function () {
        var item = new AjaxConfiguration();
        item.type = AjaxType.GET;
        item.dataType = "json";
        return item;
    };
    AjaxConfiguration.postDefault = function () {
        var item = new AjaxConfiguration();
        item.type = AjaxType.POST;
        item.dataType = "json";
        return item;
    };
    return AjaxConfiguration;
}());
exports.AjaxConfiguration = AjaxConfiguration;
var AjaxType;
(function (AjaxType) {
    AjaxType["GET"] = "GET";
    AjaxType["POST"] = "POST";
})(AjaxType || (AjaxType = {}));
//# sourceMappingURL=IRepository.js.map