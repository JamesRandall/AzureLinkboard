'use strict';
app.controller('tagController', ['$scope', '$routeParams', 'tagService', function ($scope, $routeParams, tagService) {
    function containsTag(tags) {
        return tags.indexOf($scope.tag + ' ') > 0 ||
            tags.indexOf(' ' + $scope.tag) > 0 ||
            tags == $scope.tag;
    }

    function processResults(results) {
        angular.forEach(results.data.Items, function (value) {
            var newTags = [];
            angular.forEach(value.Tags, function (tagValue) {
                newTags.push({
                    Tag: tagValue,
                    IsSelected: tagValue == $scope.tag
                });
            });
            value.Tags = newTags;
        });
        $scope.continuationTokens.push(results.data.ContinuationToken);
        $scope.links = results.data.Items;
    }

    $scope.tag = $routeParams.tag;
    $scope.links = [];
    $scope.continuationTokens = [];
    $scope.$on('newLink', function (ev, savedLink) {
        if ($scope.continuationTokens.length == 1 && containsTag(savedLink.Tags)) { // if on first page insert the new item
            $scope.links.unshift(savedLink);
        }
    });
    $scope.nextPage = function () {
        if ($scope.continuationTokens.length == 0) return;
        var continuationToken = $scope.continuationTokens[$scope.continuationTokens.length - 1];
        if (continuationToken !== null) {
            tagService.getPage($scope.tag, continuationToken).then(function (results) {
                processResults(results);
            });
        }
    };

    tagService.getPage($scope.tag).then(function (results) {
        processResults(results);
    }, function (error) {
        alert(error.data.message);
    });
}])