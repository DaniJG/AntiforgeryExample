var app = angular.module('myApp', []);

app.factory('todoService', ['$http', function ($http) {

    var service = {};

    service.getTodos = function() {
        return $http.get('/api/todos');
    }

    service.createTodo = function(todo) {
        return $http.put('/api/todos', todo);
    }

    service.updateTodo = function (todo) {
        return $http.post('/api/todos/' + todo.id, todo);
    }

    return service;
}]);

app.controller('todoController', ['$scope', 'todoService', function ($scope, todoService) {

    $scope.todos = [];
    loadTodos();

    $scope.getTotalTodos = function () {
        return $scope.todos.length;
    };

    function loadTodos() {
        todoService.getTodos().then(function (response) {
            $scope.todos = response.data;
        });
    }

    $scope.addTodo = function () {
        todoService.createTodo({ title: $scope.formTodoTitle, done: false }).then(function (response) {
            $scope.todos.push(response.data);
            $scope.formTodoTitle = '';
        });
    };

    $scope.clearCompleted = function () {
        var promises = $scope.todos
            .filter(function (todo) { return todo.done; })
            .map(function (todo) {
                return todoService.updateTodo(todo);
            });
        
        Promise.all(promises).then(function () {
            $scope.$apply(function () {
                $scope.todos = $scope.todos.filter(function (todo) {
                    return !todo.done;
                });
            });
        });        
    };

    $scope.addTodoWithjQuery = function () {
        var token = readCookie('XSRF-TOKEN');
        var onSuccess = function (data) {
            $scope.$apply(function () {
                $scope.todos.push(data);
                $scope.formTodoTitle = '';
            });
        };

        $.ajax({
            url: '/api/todos',
            method: 'PUT',
            data: JSON.stringify({ title: $scope.formTodoTitle, done: false }),
            contentType: 'application/json',
            headers: { 'X-XSRF-TOKEN': token },
            success: onSuccess
        });
    };

    function readCookie(name) {
        name += '=';
        for (var ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
            if (!ca[i].indexOf(name))
                return ca[i].replace(name, '');
    }

}]);
