---
name: characterization-tests
description: Generate characterization (golden-master) tests for a given C# class. Use when the user asks to "create/generate characterization tests", "pin down current behavior", "add golden-master / approval / snapshot tests", or "lock in behavior before refactoring/migrating a dependency" for a specific class or file. Produces an xUnit + Verify test class that captures the class's ACTUAL current output across representative and edge-case inputs, so behavior is frozen before changing a dependency or refactoring.
---

# Characterization Tests Generator

Characterization tests describe the **actual current behavior** of a piece of
code — not the behavior we *wish* it had. They are the safety net you build
*before* changing a dependency or refactoring legacy code. The goal is to make
any behavior change visible as a failing test.

This skill generates an xUnit + [Verify](https://github.com/VerifyTests/Verify)
test class that pins a target class's behavior with golden-master snapshots.

## When to use

The user names a class (by file path or type name) and wants to:
- "Create / generate characterization tests for `OrderProcessor`"
- "Pin down / lock in the current behavior before migrating Newtonsoft.Json"
- "Add golden-master / approval / snapshot tests for this class"

## Why Verify for characterization

The whole point is to capture output we don't want to hand-write assertions
for (large JSON, object graphs, edge-case formatting). Verify records the
*actual* output to a `*.verified.txt` file on first run. That file **is** the
golden master — review it, commit it, and any future behavior change produces a
`*.received.txt` that diffs against it. This matches the project's existing
stack (`Verify.Xunit`, see [GlobalUsings.cs](../../../Module2.CharacterizationTests.Tests/GlobalUsings.cs)).

## Procedure

### 1. Locate and read the target class

Find the `.cs` file for the named class under the `*.App` project (or wherever
the user points). Read it **and** its collaborators (constructor dependencies,
parameter/return types, enums). You cannot characterize behavior you haven't
read. Identify:

- **Public entry points** — each public method/constructor a caller can reach.
- **Inputs that steer behavior** — parameters, and any property on input
  objects that a branch (`if`, `switch`, comparison) reads.
- **Branches** — every `if`/`else`/`switch`/`?:`/early-return and the exact
  boundary it tests (e.g. `Amount <= 0`, `Amount < 10_000m`).
- **Side effects on inputs** — mutation of the passed object (these are
  behavior too and must be characterized).
- **Throw sites** — every `throw`, with its exception type and message.
- **External seams** — calls into third-party libraries (the reason the tests
  exist). The output of that seam is what you are freezing.

### 2. Neutralize non-determinism

Characterization tests must be **repeatable**, or the golden master is
worthless. Before snapshotting, scrub anything that changes run-to-run:

- `DateTime.Now` / `DateTimeOffset.UtcNow` / `Guid.NewGuid()` — set the field to
  a **fixed** value in the input so the code's `== default` branch is exercised
  deterministically, **or** use Verify scrubbers (`.ScrubLinesContaining`,
  `.AddScrubber`, `settings.ScrubInlineGuids()`).
- Machine/culture/path-dependent output — scrub or pin culture.
- Document each scrub with a comment so the reader knows it was intentional.

### 3. Design the input matrix

Pick inputs that, together, **cover every branch** found in step 1. For each
public method aim for:

- A **happy-path** baseline (typical valid input).
- One case per **branch boundary** — both sides of each comparison
  (e.g. `Amount` just below and at/above a threshold; empty vs. non-empty list;
  `null` vs. set optional fields; each relevant enum value).
- Each **exception** path — assert the type AND message with
  `Assert.Throws<T>` (exceptions are behavior; Verify the message text too if
  it's part of the contract).
- **Edge cases the seam is sensitive to** — for JSON/serialization seams:
  nulls, enums, decimals, dates/offsets, empty collections, unicode. These are
  exactly where a dependency swap silently changes output.

Do not assert *correctness* ("the amount should be 100"). Assert *current
behavior* — whatever the code does today, even if it looks like a bug. Pinning
bugs is the point; note suspected bugs in a comment, don't "fix" them in a test.

### 4. Generate the test class

Write `<ClassName>CharacterizationTests.cs` into the `*.Tests` project. Follow
the existing conventions (global usings already provide `Xunit`, `VerifyXunit`,
and the app namespace — don't re-import them). Use this shape:

```csharp
// NOTE: Verify.Xunit 23.x no longer needs [UsesVerify] (it's obsolete and,
// with warnings-as-errors, won't compile). Omit it.
public class OrderProcessorCharacterizationTests
{
    // A factory keeps each test's input explicit and avoids shared mutable state.
    private static Order ValidOrder() => new()
    {
        Id = 1,
        Amount = 100m,
        CustomerName = "Acme",
        // Fixed timestamp so the `CreatedAt == default` enrichment branch is
        // deterministic. (Scrub instead if the code stamps Now unconditionally.)
        CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
        Status = OrderStatus.Pending,
        Lines = { new OrderLine { ProductCode = "A1", Quantity = 2, UnitPrice = 50m } },
    };

    [Fact]
    public Task Process_ValidPendingOrder_ProducesGoldenJson()
    {
        var processor = new OrderProcessor(new OrderSerializer());
        var result = processor.Process(ValidOrder());
        return Verify(result);   // first run writes the *.verified.txt golden master
    }

    [Fact]
    public void Process_ZeroAmount_Throws()
    {
        var order = ValidOrder();
        order.Amount = 0m;
        var processor = new OrderProcessor(new OrderSerializer());

        // Characterizes the exact current exception + message.
        var ex = Assert.Throws<ArgumentException>(() => processor.Process(order));
        Assert.Contains("must be positive", ex.Message);
    }

    [Fact]
    public void Process_EmptyLines_Throws()
    {
        var order = ValidOrder();
        order.Lines.Clear();
        var processor = new OrderProcessor(new OrderSerializer());
        Assert.Throws<ArgumentException>(() => processor.Process(order));
    }

    // One Verify test per behavior-steering branch / edge case the seam is
    // sensitive to: null optional fields, each enum value, decimal precision,
    // the auto-confirm threshold (just under vs. at 10_000m), unicode, etc.
}
```

Rules:
- Verify tests return `Task`/`ValueTask` and end in `return Verify(...)`. (No
  `[UsesVerify]` attribute — obsolete in Verify.Xunit 23.x.)
- One `[Fact]` per behavior; give each a distinct snapshot via a clear method
  name (Verify keys the `.verified.txt` file off the test name). Or use
  `[Theory]` + `[InlineData]` only when cases share one assertion shape — then
  pass the case key to `Verify(result).UseParameters(...)` so each row gets its
  own snapshot file.
- Construct real collaborators (e.g. `new OrderSerializer()`), not mocks —
  characterization freezes the *real* end-to-end output through the seam.

### 5. Run, review, and commit the golden master

```powershell
dotnet test "<...>/Module2.CharacterizationTests.Tests/Module2.CharacterizationTests.Tests.csproj"
```

- First run: Verify tests "fail" and emit `*.received.txt`. DiffEngine may open
  a diff tool locally — that's expected.
- **Read every generated `*.verified.txt`** before accepting it. This is the
  one manual step that matters: you are certifying "this is what the code does
  today." Look for non-determinism that slipped through (timestamps, ordering).
- Accept the snapshots (the Verify diff tool's "accept", or rename
  `*.received.txt` → `*.verified.txt`), then re-run; all tests should pass.
- Tell the user the `.verified.txt` files are the golden master and must be
  committed alongside the tests.

## Output to the user

Report: the class characterized, each branch/edge case covered (and any
deliberately *not* covered, with why), any non-determinism that was pinned or
scrubbed, any current behavior that looks like a bug (pinned, not fixed), and
the `dotnet test` result.
