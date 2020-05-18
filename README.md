# ValidationAdapter
The ValidationAdapter libraries intend to provide a simple interfaces for handling validation in different frameworks. It is based on .NET and coded in C# language.

## Project status
![.NET Core Build and Test](https://github.com/BanallyMe/ValidationAdapter/workflows/.NET%20Core%20Build%20and%20Test/badge.svg)

## Why use it
Most frameworks already have their own way of validating user input and reacting appropriately. While it can be a good idea to use these ready-to-use features it can lead to code that is coupled very tightly to the framework you use. This makes code harder to change and also harder to test.
The project itself emerged from the wish to have an injectable interface for validation in ASP.NET Core to make simulating validation errors easier and independent from the BaseController.

## Implementations <a id="validationadapter_implementations"></a>
At the moment there is a basic package for defining the rules of the ValidationAdapter interface and an implemetation of this very interface for the ASP.NET Core framework.

[Basic package](#validationadapter_basicpackage)

### Basic package <a id="validationadapter_basicpackage"></a>
The basic package provides an IValidationAdapter interface which provides basic access to validation errors. This interface makes use of three different immutable objects to store information about validation results and errors.
As such it can be used as a plan to construct a new implementation of a validation adapter.
**Consider that this basic package doesn't provide you with actual functionality to access validation. If this is what you're looking for, you should use one of the [framework implementations](#validationadapter_implementations).**

#### Dependencies
The basic package provides simple interfaces and classes only, so the only dependency is a .NET Standard library of version 1.3 at least. The dependcies will differ for different implementations, as different frameworks require different versions of the .NET Standard library.
Users of .NET Standard > 2.1 will be able to benefit from the new Nullable reference types though.

#### Installation
The is a [NuGet package](https://www.nuget.org/packages/BanallyMe.ValidationAdapter/) of the basic ValidationAdapter available. You can use the package manager to add this package to your project.
``` shell
dotnet add package BanallyMe.ValidationAdapter
```

#### Implementing a new ValidationAdapter
For implementing a new ValidationAdapter there should be at least an implementation of the [IValidationAdapter interface](https://github.com/BanallyMe/ValidationAdapter/blob/master/ValidationAdapter/ValidationAdapter/Adapters/IValidationAdapter.cs). In general it is also a good idea to provide users with a simple method to add the implementation to the dependency injection container of their choice.
The implementation should dock to the specific framework's validating solution and read validation errors directly from this solution.

#### Mapping validation errors to the package's model
When implementing the IValidationAdapter interface you will need to map your validation errors to an enumerable of [ValidationError](https://github.com/BanallyMe/ValidationAdapter/blob/master/ValidationAdapter/ValidationAdapter/ValidationResults/ValidationError.cs) objects. You can simply use the object's factory methods to instantiate them.
``` csharp
using BanallyMe.ValidationAdapter.ValidationResults;
/* [...] */

// Use the CreateGlobalError factory to create errors, which don't refer to specific properties
// but to the model in it's entirety
var globalValidationError = ValidationError.CreateGlobalError("any error message");

// Use the CreateErrorAtPath facotry to create errors which refer to a specific property
// of the validated model. The path to this property should be compatible to your
// framework's validation implementation.
var validationError = ValidationError.CreateErrorAtPath("any error message", "any.path");
```
You will also need to provide an instance of [ValidationResult](https://github.com/BanallyMe/ValidationAdapter/blob/master/ValidationAdapter/ValidationAdapter/ValidationResults/ValidationResult.cs) as a reply of the interface's *Validate* method. Again use the object's factory methods to create the result.
``` csharp
using BanallyMe.ValidationAdapter.ValidationResults;
/* [...] */

// Use the CreateValidResult factory if there hasn't been any validation error.
var validResult = ValidationResult.CreateValidResult();

// Use the CreateInvalidResultFromValidationErrors to create a result based on the errors
// that occured in the validation process.
var validationErrors = GetAllValidationErrors();
var validationResult = ValidationResult.CreateInvalidResultFromValidationErrors(validationErrors);
```
