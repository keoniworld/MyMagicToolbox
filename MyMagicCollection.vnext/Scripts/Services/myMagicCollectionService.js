(function () {
    'use strict';

    angular
        .module('myMagicCollectionService', ['ngResource'])
        .factory('myMagicCollection', ['$resource',
            function ($resource) {
                return $resource('api/myMagicCollection/', {}, {
                    query: { method: 'GET', params: {}, isArray: true }
                });
            }]);

    //myMagicCollectionService.$inject = ['$http'];

    //function myMagicCollectionService($http) {
    //    var service = {
    //        getData: getData
    //    };

    //    return service;

    //    function getData() { }
    }
})();