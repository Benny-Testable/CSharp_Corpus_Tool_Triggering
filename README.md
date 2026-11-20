# OrderKit

OrderKit is a small C# order-management library: orders and order lines, pricing,
risk scoring, stock allocation and reporting. It targets .NET 8 (`net8.0`) and is
built and tested with the .NET SDK only.

## Layout

| Path | Contents |
|---|---|
| `src/OrderKit.Domain` | Order, order lines, statuses, stock ledger, SKU and customer-tier rules, batch processors |
| `src/Services/OrderKit.Pricing` | Price calculation (tier discount, region, expedite fee, loyalty credit) and an async gateway |
| `src/Services/OrderKit.Risk` | Risk scoring for hazmat, fragile and flagged orders |
| `src/Services/OrderKit.Reporting` | Reports, SQL text builders, JSON export, audit fingerprints, report files |
| `tests/OrderKit.Tests` | NUnit tests for every project |

## Build and test

```bash
dotnet restore OrderKit.sln
dotnet build OrderKit.sln -c Release
dotnet test OrderKit.sln -c Release --collect:"XPlat Code Coverage"
```

`global.json` accepts any 8.0.x SDK from 8.0.100 (`rollForward: latestFeature`).
Packages come from nuget.org only (`nuget.config`).

## Packages

Package versions are declared once in `Directory.Packages.props` (Central Package
Management). Several pins are intentionally old so dependency audits have something
to report: `Newtonsoft.Json` 12.0.3, `System.Text.Json` 8.0.0,
`System.Text.RegularExpressions` 4.3.0 and `System.Data.SqlClient` 4.8.3.
The test stack is NUnit 3.13.3 with `Microsoft.NET.Test.Sdk` 17.2.0,
`NUnit3TestAdapter` 4.6.0 and `coverlet.collector` 6.0.2.

When a tool injects its own `Directory.Build.props` (for example an analyzer bundle with
versioned `PackageReference` items), Central Package Management switches itself off and the
project files take the same versions from the properties in `Directory.Packages.props`.

## Mutation testing

`stryker-config.json` points Stryker.NET at the test project:

```bash
dotnet tool install -g dotnet-stryker
dotnet stryker
```

## Notes

Contact details, keys and tokens in the sources are documentation examples
(`example.com` addresses, the public AWS example key pair), not real credentials.
<!-- dummy PR 2026-11-19 07:28:52 -->
<!-- dummy PR 2026-11-19 08:03:03 -->
<!-- dummy PR 2026-11-20 08:57:18 -->
