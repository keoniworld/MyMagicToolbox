(function () {
    'use strict';

    angular.module('MagicCollectionApp', [
        // Angular modules 
        'ngRoute',

        // Custom modules 
        'MagicCollectionServices'

        // 3rd Party Modules
        
    ]);
})();
(function () {
    'use strict';

    angular
        .module('MagicCollectionApp')
        .controller('MagicCollectionController', magicCollectionController);

    magicCollectionController.$inject = ['$scope', 'MagicCollection'];

    function magicCollectionController($scope) {
        $scope.title = 'My Controller';

       // $scope.magicCollection = MagicCollection.query();
    }
})();

//(function () {
//    'use strict';

//    angular
//        .module('MagicCollectionApp')
//        .controller('MagicCollectionController', ['$scope', function($scope) {
        
//            $scope.title = 'My Controller';

//            // $scope.magicCollection = MagicCollection.query();
    
//        }])
//})();
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