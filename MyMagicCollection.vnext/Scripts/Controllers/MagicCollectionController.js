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