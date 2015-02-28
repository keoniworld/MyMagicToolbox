(function () {
    'use strict';

    angular
        .module('myMagicCollectionApp')
        .controller('myMagicCollectionController', myMagicCollectionController);

    myMagicCollectionController.$inject = ['$scope', 'myMagicCollection'];

    function myMagicCollectionController($scope) {
        $scope.title = 'myMagicCollectionController';

        $scope.Collection = myMagicCollection.query();

        //// TODO: Query data
        //activate();

        //function activate() { }
    }
})();