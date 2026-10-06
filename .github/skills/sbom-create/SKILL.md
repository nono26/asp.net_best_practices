---
name: sbom-create
description: Generate a CycloneDX JSON software bill of materials for a .NET solution or project using the CycloneDX .NET tool. Use when asked to create or update an SBOM, software bill of materials, dependency inventory, or CycloneDX file.
---

# Creer un SBOM CycloneDX

## Instructions

1. Determine the target from the user's request. Prefer the requested `.sln`, `.slnx`, or project file. If no target is specified, search the repository for solution files and use the solution if exactly one is found. If multiple solutions are found, ask which one to scan. If no solution exists and multiple projects could be the target, ask rather than guessing.
2. Check that the CycloneDX .NET tool is available. If it is missing, install it with `dotnet tool install --global CycloneDX`. If installation cannot complete, stop and report the prerequisite instead of producing a hand-written or incomplete SBOM.
3. Generate JSON at `docs/sbom/sbom.json`, creating the output directory if needed:

   ```sh
   dotnet CycloneDX "<target>" --output docs/sbom --filename sbom.json --output-format Json
   ```

   Replace `<target>` with the selected solution or project path. Keep development, test, and transitive dependencies unless the user explicitly requests exclusions.
4. Verify that `docs/sbom/sbom.json` exists, is non-empty, contains valid JSON, and has `bomFormat` set to `CycloneDX`. Use an available JSON parser; do not edit the generated BOM manually to make validation pass.
5. Report the target scanned, output path, and whether validation succeeded. If generation or validation fails, report the actual error and do not claim the SBOM is complete.