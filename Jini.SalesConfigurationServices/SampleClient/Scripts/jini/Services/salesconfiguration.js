jiniApp.factory('SalesConfigurationService', function ($http, __jiniEnv)
{
    return {
        //get lookup data from RefSalesForm table
        IsPublished: function (isbn) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl+"/v1/SalesConfiguration/Published/" + isbn)
              .success(function (data) {
                  d.resolve(data);
              }).error(function (data, status) {
                  alert("Call to server failed. Error status : " + status);
              });;
            return d.promise();
        },
        GetConfiguration: function (isbn, institutionNumber) {
            var d = $.Deferred();
            $http.get(__jiniEnv.jiniApiUrl + "/v1/SalesConfiguration/Get/" + isbn + "/InstitutionNumber/" + institutionNumber)
              .success(function (data) {
                  d.resolve(data);
              }).error(function (data, status) {
                  alert("Call to server failed. Error status : " + status);
              });;
            return d.promise();
        },
        GetPrice: function (isbn, institutionNumber, config)
        {
            var d = $.Deferred();
            var jsonData =JSON.stringify(config);
            $http.post(__jiniEnv.jiniApiUrl + "/v1/SalesConfiguration/Price/" + isbn + "/" + institutionNumber, jsonData )
              .success(function (data) {
                  d.resolve(data);
              }).error(function (data, status) {
                  alert("Call to server failed. Error status : " + status);
              });;
            return d.promise();
        }
    };

});