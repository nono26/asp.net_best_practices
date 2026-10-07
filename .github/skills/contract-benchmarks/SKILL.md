---
name: contract-benchmarks
description: Generate a BenchmarkDotNet performance benchmark for competing .NET implementations of the same interface or operation. Use when the user wants to measure and compare the speed/allocations of two or more implementations (e.g. serializers, repositories, mappers, hashing) — typically to validate that a replacement library (Newtonsoft → System.Text.Json) is no slower, or to find rollback triggers during a migration. Produces a [MemoryDiagnoser] benchmark class with a [Baseline] benchmark plus one [Benchmark] per implementation, a BenchmarkRunner entry point, and the BenchmarkDotNet package wiring if missing. 
---

# Benchmarks (BenchmarkDotNet)

A **benchmark** measures how fast each implementation runs and how much it
allocates, under a controlled harness. During a migration you run it BEFORE the
swap to capture a baseline, then AFTER to confirm the new implementation is
within acceptable bounds (the `Ratio` column versus the `[Baseline]`). It is the
performance counterpart to a contract test, showing the replacement is
fast enough to ship.

## Inputs

Free text after the command, any phrasing. Auto-discover anything omitted, then
confirm before writing.

- **Interface / operation** — what to benchmark, e.g. `ISerializer`. If omitted, ask.
- **Implementations** — the concrete types to compare, e.g.
  `LegacyNewtonsoftSerializer`, `SystemTextJsonSerializer`. If omitted, `Grep`
  for `: I<Name>` and list what you find. Pick which one is the `[Baseline]`
  (default: the outgoing/legacy implementation).
- **Source project** — where the types live, e.g. `Ps.Mnd.ContractTests.App`.
  Inferred from where the interface is defined.
- **Benchmark project** — where to write benchmarks, e.g.
  `Ps.Mnd.ContractTests.Benchmarks`. If none exists, offer to create one (see
  "Project setup").
- **Operations / workloads** (optional) — which methods and input sizes to
  cover, e.g. "serialize and deserialize, simple + complex payloads".

Example invocations:

```
/contract-benchmarks ISerializer comparing LegacyNewtonsoftSerializer (baseline) and
SystemTextJsonSerializer in Ps.Mnd.ContractTests.Benchmarks
```
```
/contract-benchmarks ISerializer            # discover implementations + benchmark project
```

## The pattern

1. **One benchmark class**, annotated `[MemoryDiagnoser]` (and usually
   `[RankColumn]`), holding instances of each implementation as fields.
2. **`[GlobalSetup]`** builds the test inputs ONCE — representative payloads
   (a small one and a large/nested one). Pre-serialize any inputs that
   deserialize benchmarks will consume, so setup cost stays out of the timing.
3. **One `[Benchmark]` method per (implementation × operation)**. Exactly one is
   marked `[Benchmark(Baseline = true)]` so every other row gets a `Ratio`.
   Each method returns its result so the JIT can't optimize the work away.
4. **A `BenchmarkRunner.Run<T>()` entry point** in `Program.cs`.

```csharp
using BenchmarkDotNet.Attributes;
using Ps.Mnd.ContractTests.App;
using Ps.Mnd.ContractTests.App.Serialization;

namespace Ps.Mnd.ContractTests.Benchmarks;

[MemoryDiagnoser]   // adds the Allocated column
[RankColumn]        // adds a fastest→slowest rank
public class SerializerBenchmarks
{
    private readonly LegacyNewtonsoftSerializer _newtonsoft = new();
    private readonly SystemTextJsonSerializer _stj = new();

    private Order _simpleOrder = null!;
    private string _simpleJson = null!;

    [GlobalSetup]
    public void Setup()
    {
        _simpleOrder = new Order { Id = 1, Amount = 99.99m, Status = OrderStatus.Confirmed };
        _simpleJson  = _newtonsoft.Serialize(_simpleOrder);   // input for deserialize benches
    }

    [Benchmark(Baseline = true, Description = "Newtonsoft — serialize")]
    public string Newtonsoft_Serialize() => _newtonsoft.Serialize(_simpleOrder);

    [Benchmark(Description = "STJ — serialize")]
    public string Stj_Serialize() => _stj.Serialize(_simpleOrder);

    [Benchmark(Description = "Newtonsoft — deserialize")]
    public Order Newtonsoft_Deserialize() => _newtonsoft.Deserialize<Order>(_simpleJson);

    [Benchmark(Description = "STJ — deserialize")]
    public Order Stj_Deserialize() => _stj.Deserialize<Order>(_simpleJson);
}
```

```csharp
// Program.cs — run with: dotnet run -c Release
BenchmarkRunner.Run<SerializerBenchmarks>();   // needs: using BenchmarkDotNet.Running;
```

## How to build it

1. **Read the interface and each implementation** to learn the operations worth
   measuring (the public methods callers actually use).
2. **Find or create the benchmark project** (see "Project setup").
3. **Design representative workloads** in `[GlobalSetup]`: at least a *simple*
   and a *complex/nested* input. Real-world-shaped data beats trivial data.
   Pre-compute any serialized strings the deserialize benchmarks need.
4. **Write one `[Benchmark]` per implementation per operation.** Mark one as
   `Baseline = true` (the legacy/outgoing one). Give each a clear `Description`.
   Return the result from every method — never discard it.
5. **Wire `Program.cs`** with `BenchmarkRunner.Run<TBenchmark>()`.
6. **Verify it builds and the harness starts** in Release:
   `dotnet build -c Release`, then a short smoke run (see "Verifying"). Do NOT
   run a full benchmark to completion unless the user asks — it can take minutes.

## Project setup

BenchmarkDotNet needs a **console** project (`<OutputType>Exe</OutputType>`),
compiled in **Release**. If no benchmark project exists, create one referencing
the source project:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <Optimize>true</Optimize>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="BenchmarkDotNet" Version="0.15.8" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\<SourceProject>\<SourceProject>.csproj" />
  </ItemGroup>
</Project>
```

Add the new project to the solution (`dotnet sln <sln> add <csproj>`, or the
`.slnx` `<Project Path=.../>` list). Match the repo's existing BenchmarkDotNet
version rather than pulling the latest.

## Critical rules (benchmarks lie if you break these)

- **Release only.** Debug-mode numbers are meaningless. The entry point comment
  and any docs must say `dotnet run -c Release`.
- **Return the result** from every `[Benchmark]` so dead-code elimination can't
  delete the work being measured.
- **Setup outside the measured method.** Building inputs inside a `[Benchmark]`
  measures the setup, not the operation. Use `[GlobalSetup]` / `[IterationSetup]`.
- **Reuse implementation instances** as fields — unless construction cost is
  explicitly what you're measuring.
- **`[MemoryDiagnoser]`** is almost always wanted — allocation differences are
  often the real story (e.g. STJ vs Newtonsoft), not just wall-clock time.
- **No shared mutable state** between benchmarks that would skew later runs.

## Reading / using results

- **Mean** — average time per operation. **Ratio** — relative to `[Baseline]`
  (1.00 = same; <1 faster; >1 slower). **Allocated** — bytes per op.
- For a migration **rollback trigger**, agree a threshold up front (e.g.
  "candidate must be ≤ 1.10× the baseline") and read it off the `Ratio` column.
- A matching xUnit perf-gate test can assert the threshold in CI; the
  BenchmarkDotNet run is the authoritative measurement.

## Verifying (smoke run, fast)

A full run is slow. To confirm the harness works without waiting minutes, build
in Release and do a short job:

```
dotnet build -c Release
dotnet run -c Release --project <BenchmarkProject> -- --job short --filter *
```

Report that it builds and launches; let the user trigger the full
`dotnet run -c Release` when they want real numbers.

