# Back end Alone

## Introduction

Christmas is coming.
You are working as a back-end developer in Santa Clause Company Inc.
Due to high demand for Christmas gifts and a lot of work, Santa's elves decided that they should have an app where they can plan Santa’s routes to optimize them.
The front-end team has already prepared a web application for that purpose, but immediately after that they have left for their holiday break, leaving you only with a specification of a back-end API call.
Unfortunately, they haven't discussed all the necessary details with you, and the API call they've prepared is far from what you would expect.
But this is not the time to complain! Santa is counting on you!
Actually, your API endpoint is already prepared according to the specification.
The only thing left to do, is to ensure that the data that comes from the front end is bound properly to the MVC action parameter.
Your job is to implement `ModelBinder` that will do this.


## Problem Statement

Implement  `PathBinderProvider` class, to ensure that `PathsController.Post` action will use your custom model binder to have its parameters bound properly. 

The data that comes from the front-end app can contain none, one or many directed acyclic graphs, in which nodes are city names and indices are connections between locations.

The front-end request is `POST` request on `api/paths` URI.
Its content type is `application/x-www-form-urlencoded`.

The content can include a list of key-value fields filled in with data that can be divided into two categories:

Let’s see some examples:
```
locations[0].Name : "New York"
locations[1].Name : "London"
locations[2].Name : "Moscow"
locations[3].Name : "Tokyo"
paths[2].to : "1"
paths[2].to : "0"
paths[3].to : "2"
```

Santa’s path will lead:
from Moscow to London - because there is a path from location at `index 2` (Moscow) to location at `index 1` (London), 
from Moscow to New York - because there is a path from location at `index 2` (Moscow) to location at `index 0` (New York), 
from Tokyo to Moscow - because there is a path from location at `index 3` (Tokyo) to location at `index 2` (Moscow). 

This means that Santa will visit four cities in the following order: "Tokyo", "Moscow", then he can choose either "New York" or "London", then he has to visit the remaining city.
`PathsController.Post` method is already implemented and it sequentially calls `AddPath` method, passing `Path` instances into it.
Unfortunately, your company is not the owner of the code that created this method and it will have to call an external API to store the path. The implementation of the method will be created by ThirdPartyCompany Inc. 
and as it is the busiest period of the year, it is yet unknown when it will be ready. So far, they have only delivered the `ThirdPartyApi` project for your solution, along with the `IThirdPartyApi` interface and `Path` class declaration 
that states the `Name` (location) of the path and a list of incoming paths: `From`.
Your company has used the ThirdPartyCompany guidelines to implement the mock of that interface in your project just for testing purposes.
The mock is called `ThirdPartyTestApi`. Its instance uses a variable as a persistent store and it works just as ThirdPartyCompany described, i.e., it sequentially takes the Path object and saves it to the persistent store with a given Id.
There is a small catch though, if there are any references to other paths, it **is expected that for each of those other paths, `AddPath` method had already been called.** 
This is because, they will try to find referencing paths in their persistent store first and if they fail, they will return `ArgumentException`.

In other words, it can be seen from the above example that there are two possible sequences that can be passed to `AddPath` method:
London,New York,Moscow,Tokyo
or 
New York,London,Moscow,Tokyo

**Your task consists of two objectives:**
1. **Make sure that `PathsController.Post` method receives properly bound paths list, which represents the graph from the incoming request.**
2. **Make sure that the order of the paths that will be passed to IThirdPartyApi.AddPath will meet the described requirements. You can do it in the model binder or in the controller action just before looping through the paths.**


The detailed description of the front-end request is as follows:

### Location names
Names of the cities (nodes of graph) that will be visited.
Every key has the following form:
`locations[locationIndex].Name` and its value is the name of the city at `locationIndex`.
`locationIndex` is an integer non-negative number.
It is guaranteed that `locationIndex` will increase by one with each subsequent key-value pair.

### Paths
Paths (indices of graph) denoting the connections between cities.
Every key has the following form:
`paths[pathIndex].to` and its value can be one of possible location indices.
`pathIndex` is an integer non-negative number. 
It is unknown in what order `pathIndex` will appear.

Path class is used to store the information concerning both: the name of the location and the indices incoming to that location.


# Hints
* `BackendAlone.Tests` project in solution contains two `IThirdPartyApi` implementation mocks to test each of your tasks. 
The first one is called `ThirdPartyApiNaiveMock` and is only used to test if paths are binding the works, as it only expects the instances of Path class.
The second one is called `ThirdPartyApiMock` and it also expects Path class instances, but additionally it requires that they are in a specific order, which guarantees that the predecessor of each incoming path is already in its path collection, otherwise the mock will fail.
* There may be many graphs, or no graphs, or just one graph in the data. Every test case is prepared in `BackendAlone.Tests` project.
* If there are multiple separate graphs in the incoming data, just make sure that the order of Path instances that goes into `AddPath` method fulfills the requirements.
* The front-end request with case described in this readme is also present in `BackendAlone.Tests` project.
* It is guaranteed that graph(s) (if any) will be directed and acyclic.
* It is guaranteed that the incoming request will have a proper format with proper semantics described earlier.
* There is no front end available for `BackendAlone` project, so you should work with it using a unit test project.
* The maximum graph complexity, which you should expect is 50 nodes and up to 1000 indices.
