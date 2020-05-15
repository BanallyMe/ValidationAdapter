using BanallyMe.ValidationAdapter.AspNetCore.ActionFilters.AutomaticValidation;
using BanallyMe.ValidationAdapter.AspNetCore.ControllerOutput;
using BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.OperationFilters;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace BanallyMe.ValidationAdapter.AspNetCore.Swashbuckle.UnitTests.OperationFilters
{
    public class AutoValidationOperationFilterTests
    {
        private readonly IOperationFilter testedFilter;
        private readonly Mock<ISchemaGenerator> schemaGenerator;

        public AutoValidationOperationFilterTests()
        {
            testedFilter = new AutoValidationOperationFilter();
            schemaGenerator = new Mock<ISchemaGenerator>();
        }

        [Fact]
        public void Apply_Adds422ResponseIfActionDecoratedWithAutoValidateAttribute()
        {
            Assert422ResponseIsAddedForMethod(GetAutoValidateDecoratedMethod());
        }

        [Fact]
        public void Apply_Adds422ResponseIfControllerDecoratedWithAutoValidateAttribute()
        {
            Assert422ResponseIsAddedForMethod(GetMethodFromAutoValidateDecoratedClass());
        }

        private void Assert422ResponseIsAddedForMethod(MethodInfo method)
        {
            var fakeContext = new OperationFilterContext(null, schemaGenerator.Object, null, method);
            var fakeSchema = new OpenApiSchema();
            schemaGenerator.Setup(gen => gen.GenerateSchema(typeof(SerializableValidationResult), fakeContext.SchemaRepository, null, null)).Returns(fakeSchema);
            var fakeOperation = new OpenApiOperation { Responses = new OpenApiResponses() };
            var expectedAppendedSchema = new OpenApiSchema { Type = "array", Items = fakeSchema };

            testedFilter.Apply(fakeOperation, fakeContext);

            fakeOperation.Responses.Should().ContainKey("422");
            fakeOperation.Responses["422"].Content.Should().ContainKey("application/json");
            fakeOperation.Responses["422"].Description.Should().Be("Validation of the model failed.");
            fakeOperation.Responses["422"].Content["application/json"].Schema.Should().BeEquivalentTo(expectedAppendedSchema);
        }

        [Fact]
        public void Apply_DoesntAdd422ResponseIfActionAlreadyHas422Response()
        {

            var fakeApiResponses = new OpenApiResponses { ["422"] = new OpenApiResponse() };
            var fakeOperation = new OpenApiOperation { Responses = fakeApiResponses };
            var fakeContext = new OperationFilterContext(null, schemaGenerator.Object, null, GetAutoValidateDecoratedMethod());

            var fakeSchema = new OpenApiSchema();
            schemaGenerator.Setup(gen => gen.GenerateSchema(typeof(SerializableValidationResult), fakeContext.SchemaRepository, null, null)).Returns(fakeSchema);

            var expectedApiResponses = fakeApiResponses.ToArray();

            testedFilter.Apply(fakeOperation, fakeContext);

            fakeOperation.Responses.Should().BeEquivalentTo(expectedApiResponses);
        }

        [Fact]
        public void Apply_DoesntAdd422ResponseIfActionAndControllerNotDecoratedWithAutoValidateAttribute()
        {
            var fakeOperation = new OpenApiOperation();
            var fakeContext = new OperationFilterContext(null, null, null, typeof(FakeControllerWithoutAutoValidate).GetMethod(nameof(FakeControllerWithoutAutoValidate.FakeActionWithoutAutoValidate)));

            testedFilter.Apply(fakeOperation, fakeContext);

            fakeOperation.Responses.Should().BeEmpty();
        }

        [Fact]
        public void Apply_DoesntThrowExceptionIfOperationIsNull()
        {
            Action applying = () => testedFilter.Apply(null, new OperationFilterContext(null, null, null, null));

            applying.Should().NotThrow();
        }

        [Fact]
        public void Apply_DoesntThrowExceptionIfContextIsNull()
        {
            Action applying = () => testedFilter.Apply(Mock.Of<OpenApiOperation>(), null);

            applying.Should().NotThrow();
        }

        private MethodInfo GetAutoValidateDecoratedMethod()
            => typeof(FakeControllerWithoutAutoValidate).GetMethod(nameof(FakeControllerWithoutAutoValidate.FakeActionWithAutoValidate));

        private MethodInfo GetMethodFromAutoValidateDecoratedClass()
            => typeof(FakeControllerWithAutoValidate).GetMethod(nameof(FakeControllerWithAutoValidate.FakeActionWithoutAutoValidate));


        private class FakeControllerWithoutAutoValidate
        {
            [AutoValidate]
            public void FakeActionWithAutoValidate() { }

            public void FakeActionWithoutAutoValidate() { }
        }

        [AutoValidate]
        private class FakeControllerWithAutoValidate : FakeControllerWithoutAutoValidate { }
    }
}
