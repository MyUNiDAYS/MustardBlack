using System;
using System.Globalization;
using System.Reflection;
using MuonLab.Validation;
using MustardBlack.Hosting;
using MustardBlack.Routing;
using NanoIoC;

namespace MustardBlack.Handlers.Binding
{
	public sealed class RequestBinder : IRequestBinder
	{
		readonly IErrorMessageResolver errorMessageResolver;
		readonly IViolationPropertyNameResolver violationPropertyNameResolver;
		readonly IContainer container;

		public RequestBinder(IContainer container, IErrorMessageResolver errorMessageResolver, IViolationPropertyNameResolver violationPropertyNameResolver)
		{
			this.violationPropertyNameResolver = violationPropertyNameResolver;
			this.errorMessageResolver = errorMessageResolver;
			this.container = container;
		}

		/// TODO: Violates CQS, fix this
		public object[] GetAndValidateParameters(object owner, MethodInfo verbMethod, IRequest request, RouteValues routeValues)
		{
			var parameterInfos = verbMethod.GetParameters();

			var parameters = new object[parameterInfos.Length];

			for (var i = 0; i < parameterInfos.Length; i++)
			{
				var parameter = parameterInfos[i];

				var bindingResult = this.Bind(owner, parameter, request, routeValues);
				if (bindingResult == null)
					continue;

				parameters[i] = bindingResult.Object;

				foreach (var error in bindingResult.BindingErrors)
				{
					if (!request.State.AttemptedValues.ContainsKey(error.ParameterKey))
						request.State.AttemptedValues.Add(error.ParameterKey, error.AttemptedValue);

					request.State.AddError(error.ParameterKey, this.errorMessageResolver.GetErrorMessage(new ErrorDescriptor(error.ErrorKey), CultureInfo.CurrentUICulture));
				}

				if (bindingResult.Object != null)
					this.ValidateRequest(bindingResult.Object, request);
			}

			return parameters;
		}

		void ValidateRequest(object resource, IRequest request)
		{
			var type = resource.GetType();

			var validatorType = typeof(IValidator<>).MakeGenericType(type);

			if (!this.container.HasRegistrationsFor(validatorType))
				return;

			var validator = this.container.Resolve(validatorType) as IValidator;

			var validationReport = validator.Validate(resource);

			if (validationReport.IsValid)
				return;

			foreach (var violation in validationReport.Violations)
			{
				var propertyName = this.violationPropertyNameResolver.ResolvePropertyName(violation);
				request.State.AddValidationError(propertyName, violation.Error.Key, this.errorMessageResolver.GetErrorMessage(violation.Error, CultureInfo.CurrentUICulture));
			}
		}

		public BindingResult Bind(object owner, ParameterInfo parameterInfo, IRequest request, RouteValues routeValues)
		{
			// find the binder for the gien parameter
			var binder = BinderCollection.FindBinderFor(parameterInfo.Name, parameterInfo.ParameterType, request, routeValues, owner);

			if (binder == null)
				return null;

			// bind the parameter
			var bindingResult = binder.Bind(parameterInfo.Name, parameterInfo.ParameterType, request, routeValues, false, owner);
			
			// TODO: the dbnull bollocks below is a hack, you can get HasDefault off an attribute somehwere, google it. theres a good SO post

			// use the default value from the parameter if available
			if (bindingResult.Result == BindingResult.ResultType.Default && parameterInfo.DefaultValue != null && parameterInfo.DefaultValue != DBNull.Value)
				bindingResult = new BindingResult(parameterInfo.DefaultValue, BindingResult.ResultType.Default);

			// use the default value from the parameter if available
			if (bindingResult.Result == BindingResult.ResultType.Failure && parameterInfo.DefaultValue != null && parameterInfo.DefaultValue != DBNull.Value)
				bindingResult = new BindingResult(parameterInfo.DefaultValue, bindingResult.BindingErrors, BindingResult.ResultType.Default);

			return bindingResult;
		}
	}
}
