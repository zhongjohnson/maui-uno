using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Devices;
using NUnit.Framework;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class OnPlatformOptimization : ContentPage
	{
		public OnPlatformOptimization()
		{
			InitializeComponent();
		}

		public OnPlatformOptimization(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		public class Tests
		{
			[Test]
			public void OnPlatformExtensionsAreSimplified([Values("net6.0-ios", "net6.0-android")] string targetFramework)
			{
				MockCompiler.Compile(typeof(OnPlatformOptimization), out var methodDef, targetFramework);
				Assert.That(!methodDef.Body.Instructions.Any(instr=>InstructionIsOnPlatformExtensionCtor(methodDef, instr)), "This Xaml still generates a new OnPlatformExtension()");
			}

			[Test]
			public void OnPlatformAreSimplified([Values("net6.0-ios", "net6.0-android")] string targetFramework)
			{
				MockCompiler.Compile(typeof(OnPlatformOptimization), out var methodDef, targetFramework);
				Assert.That(!methodDef.Body.Instructions.Any(instr => InstructionIsOnPlatformCtor(methodDef, instr)), "This Xaml still generates a new OnPlatform()");
				Assert.That(methodDef.Body.Instructions.Any(instr => InstructionIsLdstr(methodDef.Module, instr, "Mobile, eventually ?")), $"This Xaml doesn't generate content for {targetFramework}");
				Assert.That(!methodDef.Body.Instructions.Any(instr => InstructionIsLdstr(methodDef.Module, instr, "Desktop, maybe ?")), $"This Xaml generates content not required for {targetFramework}");
			}

			bool InstructionIsOnPlatformCtor(MethodDefinition methodDef, Mono.Cecil.Cil.Instruction instruction)
			{
				if (instruction.OpCode != OpCodes.Newobj)
					return false;
				if (instruction.Operand is not MethodReference methodRef)
					return false;
				if (!Build.Tasks.TypeRefComparer.Default.Equals(methodRef.DeclaringType, methodDef.Module.ImportReference(typeof(OnPlatform<View>))))
					return false;
				return true;
			}

			bool InstructionIsOnPlatformExtensionCtor(MethodDefinition methodDef, Mono.Cecil.Cil.Instruction instruction)
			{
				if (instruction.OpCode != OpCodes.Newobj)
					return false;
				if (instruction.Operand is not MethodReference methodRef)
					return false;
				if (!Build.Tasks.TypeRefComparer.Default.Equals(methodRef.DeclaringType, methodDef.Module.ImportReference(typeof(OnPlatformExtension))))
					return false;
				return true;
			}

			bool InstructionIsLdstr(ModuleDefinition module, Mono.Cecil.Cil.Instruction instruction, string str)
			{
				if (instruction.OpCode != OpCodes.Ldstr)
					return false;
				return instruction.Operand as string == str;
			}
		}
	}
}