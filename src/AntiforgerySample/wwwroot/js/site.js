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
        $scope.todos
            .filter(function (todo) { return todo.done; })
            .forEach(function (todo) {
                todoService.updateTodo(todo);
            });

        $scope.todos = $scope.todos.filter(function (todo) {
            return !todo.done;
        });
    };

    $scope.addTodoWithjQuery = function () {
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
            success: onSuccess
        });
    };

}]);
