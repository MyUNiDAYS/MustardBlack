﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyModel;
using DependencyContextCompilationOptions = Microsoft.Extensions.DependencyModel.CompilationOptions;

namespace MustardBlack.ViewEngines.Razor
{
	public class CSharpCompiler
	{
		private readonly IHostingEnvironment hostingEnvironment;
		private bool optionsInitialized;
		private CSharpParseOptions parseOptions;
		private CSharpCompilationOptions compilationOptions;
		private EmitOptions emitOptions;
		private bool emitPdb;
		IEnumerable<MetadataReference> compilationReferences;
		
		public CSharpCompiler(IHostingEnvironment hostingEnvironment, IEnumerable<MetadataReference> compilationReferences)
		{
			this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
			this.compilationReferences = compilationReferences;
		}

		public virtual CSharpParseOptions ParseOptions
		{
			get
			{
				this.EnsureOptions();
				return this.parseOptions;
			}
		}

		public virtual CSharpCompilationOptions CSharpCompilationOptions
		{
			get
			{
				this.EnsureOptions();
				return this.compilationOptions;
			}
		}

		public virtual bool EmitPdb
		{
			get
			{
				this.EnsureOptions();
				return this.emitPdb;
			}
		}

		public virtual EmitOptions EmitOptions
		{
			get
			{
				this.EnsureOptions();
				return this.emitOptions;
			}
		}

		public SyntaxTree CreateSyntaxTree(SourceText sourceText)
		{
			return CSharpSyntaxTree.ParseText(sourceText, options: this.ParseOptions);
		}

		public CSharpCompilation CreateCompilation(string assemblyName)
		{
			return CSharpCompilation.Create(assemblyName, options: this.CSharpCompilationOptions, references: this.compilationReferences);
		}

		// Internal for unit testing.
		protected internal virtual DependencyContextCompilationOptions GetDependencyContextCompilationOptions()
		{
			if (!string.IsNullOrEmpty(this.hostingEnvironment.ApplicationName))
			{
				var applicationAssembly = Assembly.Load(new AssemblyName(this.hostingEnvironment.ApplicationName));
				var dependencyContext = DependencyContext.Load(applicationAssembly);
				if (dependencyContext?.CompilationOptions != null)
				{
					return dependencyContext.CompilationOptions;
				}
			}

			return DependencyContextCompilationOptions.Default;
		}

		private void EnsureOptions()
		{
			if (!this.optionsInitialized)
			{
				var dependencyContextOptions = this.GetDependencyContextCompilationOptions();
				this.parseOptions = GetParseOptions(this.hostingEnvironment, dependencyContextOptions);
				this.compilationOptions = GetCompilationOptions(this.hostingEnvironment, dependencyContextOptions);
				this.emitOptions = this.GetEmitOptions(dependencyContextOptions);

				this.optionsInitialized = true;
			}
		}

		private EmitOptions GetEmitOptions(DependencyContextCompilationOptions dependencyContextOptions)
		{
			// Assume we're always producing pdbs unless DebugType = none
			this.emitPdb = true;
			DebugInformationFormat debugInformationFormat;
			if (string.IsNullOrEmpty(dependencyContextOptions.DebugType))
			{
				debugInformationFormat = true ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb;
			}
			else
			{
				// Based on https://github.com/dotnet/roslyn/blob/1d28ff9ba248b332de3c84d23194a1d7bde07e4d/src/Compilers/CSharp/Portable/CommandLine/CSharpCommandLineParser.cs#L624-L640
				switch (dependencyContextOptions.DebugType.ToLower())
				{
					case "none":
						// There isn't a way to represent none in DebugInformationFormat.
						// We'll set EmitPdb to false and let callers handle it by setting a null pdb-stream.
						this.emitPdb = false;
						return new EmitOptions();
					case "portable":
						debugInformationFormat = DebugInformationFormat.PortablePdb;
						break;
					case "embedded":
						// Roslyn does not expose enough public APIs to produce a binary with embedded pdbs.
						// We'll produce PortablePdb instead to continue providing a reasonable user experience.
						debugInformationFormat = DebugInformationFormat.PortablePdb;
						break;
					case "full":
					case "pdbonly":
						debugInformationFormat = true ? DebugInformationFormat.Pdb : DebugInformationFormat.PortablePdb;
						break;
					default:
						throw new InvalidOperationException("Resources.FormatUnsupportedDebugInformationFormat(dependencyContextOptions.DebugType)");
				}
			}

			var emitOptions = new EmitOptions(debugInformationFormat: debugInformationFormat);
			return emitOptions;
		}

		private static CSharpCompilationOptions GetCompilationOptions(
			IHostingEnvironment hostingEnvironment,
			DependencyContextCompilationOptions dependencyContextOptions)
		{
			var csharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			// Disable 1702 until roslyn turns this off by default
			csharpCompilationOptions = csharpCompilationOptions.WithSpecificDiagnosticOptions(
				new Dictionary<string, ReportDiagnostic>
				{
					{"CS1701", ReportDiagnostic.Suppress}, // Binding redirects
					{"CS1702", ReportDiagnostic.Suppress},
					{"CS1705", ReportDiagnostic.Suppress}
				});

			if (dependencyContextOptions.AllowUnsafe.HasValue)
			{
				csharpCompilationOptions = csharpCompilationOptions.WithAllowUnsafe(
					dependencyContextOptions.AllowUnsafe.Value);
			}

			OptimizationLevel optimizationLevel;
			if (dependencyContextOptions.Optimize.HasValue)
			{
				optimizationLevel = dependencyContextOptions.Optimize.Value ? OptimizationLevel.Release : OptimizationLevel.Debug;
			}
			else
			{
				optimizationLevel = hostingEnvironment.IsDevelopment() ? OptimizationLevel.Debug : OptimizationLevel.Release;
			}

			csharpCompilationOptions = csharpCompilationOptions.WithOptimizationLevel(optimizationLevel);

			if (dependencyContextOptions.WarningsAsErrors.HasValue)
			{
				var reportDiagnostic = dependencyContextOptions.WarningsAsErrors.Value ? ReportDiagnostic.Error : ReportDiagnostic.Default;
				csharpCompilationOptions = csharpCompilationOptions.WithGeneralDiagnosticOption(reportDiagnostic);
			}

			return csharpCompilationOptions;
		}

		private static CSharpParseOptions GetParseOptions(
			IHostingEnvironment hostingEnvironment,
			DependencyContextCompilationOptions dependencyContextOptions)
		{
			var configurationSymbol = hostingEnvironment.IsDevelopment() ? "DEBUG" : "RELEASE";
			var defines = dependencyContextOptions.Defines.Concat(new[] {configurationSymbol});

			var parseOptions = new CSharpParseOptions(preprocessorSymbols: defines);

			if (!string.IsNullOrEmpty(dependencyContextOptions.LanguageVersion))
			{
				if (LanguageVersionFacts.TryParse(dependencyContextOptions.LanguageVersion, out var languageVersion))
				{
					parseOptions = parseOptions.WithLanguageVersion(languageVersion);
				}
				else
				{
					Debug.Fail($"LanguageVersion {languageVersion} specified in the deps file could not be parsed.");
				}
			}

			return parseOptions;
		}
	}
}