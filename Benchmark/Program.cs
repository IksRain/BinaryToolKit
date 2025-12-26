// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

BenchmarkRunner.Run(typeof(Program).Assembly);