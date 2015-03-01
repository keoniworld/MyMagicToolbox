(function () {
    'use strict';

    var service = angular.module('MagicCollectionServices', ['ngResource']);

    service.factory('MagicCollection', ['$resource',
            function ($resource) {
                return $resource('api/MagicCollection/', {}, {
                    query: { method: 'GET', params: {}, isArray: true }
                });
            }]);
})();