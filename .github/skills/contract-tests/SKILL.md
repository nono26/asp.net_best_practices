---
name: contract-tests
description: Generate a contract-test suite for a .NET interface that has multiple implementations. Use when the user wants to verify that two or more implementations of the same interface (e.g. serializers, repositories, cache providers, HTTP clients) behave identically — common when migrating/replacing a library (Newtonsoft → System.Text.Json) or running an old + new implementation side by side. Produces an abstract xUnit base class defining the behavioral contract plus one concrete subclass per implementation, so the same suite runs against every implementation.
---

# Contract Tests

A **contract test** pins down the behavior that every implementation of an
interface MUST honor, then runs that single suite against each implementation.
If a new implementation diverges, a test fails — surfacing the regression
*before* it reaches production. This is the safety net for swapping one library
or service for another behind a shared interface.

## Inputs

The user supplies these as free text after the command (any phrasing). None are
strictly required — auto-discover anything omitted, then confirm before writing.

- **Interface** — the contract to test, e.g. `ISerializer`. If omitted, ask.
- **Implementations** — the concrete types to run the suite against, e.g.
  `LegacyNewtonsoftSerializer`, `SystemTextJsonSerializer`. If omitted, `Grep`
  for `: I<Name>` across the solution and list what you find for confirmation.
- **Source project** — where the interface and implementations live, e.g.
  `Ps.Mnd.ContractTests.App`. If omitted, infer from where the interface is
  defined.
- **Test project** — where to write the suite, e.g. `Ps.Mnd.ContractTests.Tests`.
  If omitted, find the project that references the source project (or matches a
  `*.Tests` naming convention) and use it.
- **Specific behaviors** (optional) — any particular contracts the user calls
  out (e.g. "nulls must be preserved"). Fold these in alongside the standard
  checklist below.

Example invocations:

```
/contract-tests ISerializer in Ps.Mnd.ContractTests.App, implementations
LegacyNewtonsoftSerializer and SystemTextJsonSerializer, tests go in
Ps.Mnd.ContractTests.Tests
```
```
/contract-tests for IPaymentGateway          # discover implementations + projects
```

After parsing the inputs, resolve the type names to real files with `Grep`/
`Glob`, verify the test project references the source project, and report what
you'll generate (interface, implementations, target file) before writing.

## The pattern

1. **One abstract base class** holds all the `[Fact]`/`[Theory]` tests. It
   never references a concrete type — only the interface, obtained from a
   single abstract factory method.
2. **One concrete subclass per implementation** overrides the factory to return
   that implementation. xUnit discovers and runs the full suite inside each
   subclass automatically.

```csharp
public abstract class WidgetStoreContractTests
{
    // The ONLY implementation-specific seam. Return a fresh instance each call.
    protected abstract IWidgetStore CreateSut();

    [Fact]
    public void Save_ThenGet_ReturnsSameWidget()
    {
        var sut = CreateSut();
        sut.Save(new Widget { Id = 1, Name = "A" });
        Assert.Equal("A", sut.Get(1).Name);
    }
    // ... more contract tests, all using CreateSut() ...
}

// One subclass per implementation — no test bodies, just the factory.
public class InMemoryWidgetStoreContractTests : WidgetStoreContractTests
{
    protected override IWidgetStore CreateSut() => new InMemoryWidgetStore();
}

public class SqlWidgetStoreContractTests : WidgetStoreContractTests
{
    protected override IWidgetStore CreateSut() => new SqlWidgetStore(/* test conn */);
}
```

## How to build it

1. **Read the interface.** Find every member and its intended behavior. If the
   interface has XML-doc or comments stating guarantees, those are contract
   tests waiting to be written.
2. **Find all implementations.** `Grep` for `: IInterfaceName` (and
   `class X : IInterfaceName`). There must be at least two for a contract suite
   to be meaningful — confirm with the user if only one exists.
3. **Read each implementation** to learn the *intended* shared behavior — but do
   NOT encode an implementation's quirks as the contract. The contract is what
   callers depend on, not what one library happens to do. Where implementations
   legitimately differ (e.g. the exception *type* thrown on bad input), assert
   the weaker shared guarantee (e.g. `Assert.ThrowsAny<Exception>`).
4. **Write the abstract base** with a `protected abstract <Interface> CreateSut();`
   factory. Every test calls `CreateSut()` for a fresh instance — never share
   state between tests.
5. **Add one subclass per implementation**, each overriding the factory.
6. **Run** `dotnet test` and confirm the suite passes for *every* subclass. A
   failure means either a real divergence (report it) or a contract assertion
   that's too strict (relax to the shared guarantee).

## What to cover (checklist)

Tailor to the interface, but these categories catch the most regressions:

- **Round-trip / inverse operations** — `Deserialize(Serialize(x)) == x`,
  `Get(Save(x)) == x`. Cover scalars, nested objects, and collections.
- **Null / empty / default handling** — the #1 source of silent divergence.
  Are nulls preserved? Omitted? Do empty inputs yield defaults or throw?
- **Type-specific edge cases** — enums (string vs numeric), dates/offsets,
  decimals/precision, large values, unicode, empty collections.
- **Determinism / idempotency** — same input produces same output across calls.
- **Error handling** — invalid input throws (assert the *shared* guarantee, not
  a library-specific exception type).
- **Boundary values** — min/max, zero, negative.

## Conventions

- Match the existing test project's style: framework (xUnit/NUnit/MSTest),
  `GlobalUsings.cs`, naming (`Method_Scenario_ExpectedResult`), and file layout.
- Name the file `<Interface-without-I>ContractTests.cs` and put the base class
  plus all subclasses in it (or split if the project prefers one type per file).
- Add a class-level `<summary>` on the base explaining it is THE contract and
  that any implementation failing it is not a valid replacement.
- If the interface has only one implementation today, still write the base +
  one subclass — the value lands the moment a second implementation appears.

