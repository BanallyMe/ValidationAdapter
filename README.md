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
Users of .NET Standard >= 2.1 will be able to benefit from the new Nullable reference types though.

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

### ASP.NET Core implementation
The ASP.NET Core implementation of the ValidationAdapter contains an implementation of the IValidationAdapter interface residing on top of the ASP.NET Core ModelState.
Furthermore it provides an ActionFilter to automatically validate user input when a controller action is called. Finally this filter can also hook into [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to add the result of this filter to generated swagger documentations.

#### Dependencies
Being an implementation of the ASP.NET Core framework the logical dependency is the ASP.NET Core framework itself. This implicitly adds a dependency on the .NET Standard >= 2.0.
If you are using the Swashbuckle integration there is also a dependency on Swashbuckle.AspNetCore, of course.
Using .NET Standard >= 2.1 you will also be able to benefit from nullable reference types.

#### Installation
There is one NuGet [package for the Asp.Net implementation](https://www.nuget.org/packages/BanallyMe.ValidationAdapter.AspNetCore/) and one [package for the Swashbuckle integration](https://www.nuget.org/packages/BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle/) respectively. This allows for looser coupling as there is no dependency to Swashbuckle in the basic ASP.NET Core implementation. You can use a package manager or use the dotnet command to add the packages to your project.
``` shell
# Use this to add ASP.NET Core integration to your project
dotnet add package BanallyMe.ValidationAdapter.AspNetCore

# If you need Swashbuckle integration also add the Swashbuckle integration package
dotnet add package BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle
```

#### Activating ValidationAdapter
To start using the ASP.NET Core implementation of ValidationAdapter, simply add the package to your dependency injection container in the ConfigureServices method of your startup.cs.
``` csharp
using BanallyMe.ValidationAdapter.AspNetCore.DependencyInjection;
// If using swashbuckle integration:
using BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    // [...]
    
    // Add Asp.Net Core implementation of IValidationAdapter and AutoValidate filter.
    services.AddAspNetCoreValidationAdapter();
    
    // If you wish for Swashbuckle integration also add that:
    services.AddValidationAdapterToSwashbuckle();
}
```

#### Usage
##### Injecting IValidationAdapter
You can access the validation now by injecting IValidationAdapter into your objects. The interface provides simple methods for accessing the state of model validation.
``` csharp
using BanallyMe.ValidationAdapter.Adapters;
using BanallyMe.ValidationAdapter.ValidationResults;
// [...]

public class Example
{
    private readonly IValidationAdapter validationAdapter;
    
    public Example(IValidationAdapter validationAdapter)
    {
        this.validationAdapter = validationAdapter;
    }
    
    public void AnyMethod()
    {
        // Check if validation errors occured
        bool hasErrors = validationAdapter.HasValidationErrors();
        
        // Get an Enumerable of all validation errors
        IEnumerable<ValidationError> allErrors = validationAdapter.GetAllValidationErrors();
        
        // Get an Enumerable of all validation errors for specific model property
        IEnumerable<ValidationError> errorsForId = validationAdapter.GetValidationErrorsAtPath("path.to.id");
        
        // Get a ValidationResult object for an object with errors grouped by property path
        ValidationResult validationResult = validationAdapter.Validate();
    }
}
```

##### Working with ValidationError object
ValidationError is a simple container for storing an error which occured while validating the model. It contains the path to the property whose validation failed and the error message the validation led to.
``` csharp
// [...]
ValidationError validationError;

// Checks if the error refers to the whole model (true) or a specific property (false)
bool errorIsGlobal = validationError.IsGlobal;
// Path to the property, this validation error is referring to
string errorPath = validationError.ErrorPath;
// Validation error message
string errorMessage = validationError.ErrorMessage;
```

##### Working with ValidationResult object
ValidationResult as the name states contains the result of a validation process. It does contain an Enumerable of PathValidationErrorsCollection which is a grouping of error messages for each property path whose value was invalid.
It also provides basic information of the state of validation and the number of errors that occured.
``` csharp
// [...]
ValidationResult validationResult;

// Checks if the model was valid (true) or not (false)
bool modelIsValid = validationResult.IsValid;

// Gets the number of totally occured validation errors
int validationErrors = validationResult.CountErrors;

// Gets a grouped Collection of validation errors
IEnumerable<PathValidationErrorsCollection> errors = validationResult.ValidationErrors;

// Iterating over validation errors
foreach(var errorCollection in errors)
{
    // Output path to the property, these errors are referring to
    Console.WriteLine($"Property: {errorCollection.Path}");
    // Output number of errors for this path
    Console.WriteLine($"Number of errors for this property: {errorCollection.Count});"
    // Output all error messages
    Console.Write(string.Join('\n', errorCollection));
}
```

##### Automatically validating Controller input
You can use the AutoValidateAttribute to decorate controllers and actions. Input to actions decorated with this attrbiute will automatically be validated. If an error occurs a collection of validation errors the action won't execute but return a HTTP-Statuscode 422 with validation errors in it's JSON body.
The body will contain an Enumerable of error messages grouped by the path to the model's property, the errors are referring to. Due to the lack of System.Text.Json to use non-standard constructors, an Enumerable of [SerializableValidationResult](https://github.com/BanallyMe/ValidationAdapter/blob/master/ValidationAdapter/ValidationAdapter.AspNetCore/ControllerOutput/SerializableValidationResult.cs) will be returned.
``` csharp
using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;

// Add AutoValidate here to decorate all actions in this controller
[AutoValidate]
public class AnyController : Controller
{
    // Add AutoValidate here to validate only this action automatically
    [AutoValidate]
    public async Task<IActionResult> AnyAction()
    {
        //...
    }
}
```
If you're also using the swashbuckle integration package and activated it by putting it into ConfigureServices of your Startup.cs you won't have to take further action as the decorated actions will automatically get a documentation for the HTTP 422 result in case of an invalid model.

## Contributing
Feel free to provide pull requests to improve ValidationAdapter. Please also make sure to update any tests affected by changed code.

## License
ValidationAdapter is published under the [MIT license](https://choosealicense.com/licenses/mit/).
