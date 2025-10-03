# Resourcerer template

Solution template based on Resourcerer project. It's generating boilerplate solution structure.

## Installation

From root run:
```
dotnet new install .
dotnet new install . --force (if installed already)
```
This step may show an error in the console, but template should be installed and working normally anyway. Run `dotnet new list` to verify it's there.

## Usage

Check if template is installed: `dotnet new list`
Run `dotnet new rsrcfs -o "MyPrefix"` to scaffold new solution.

> DataAccess project must be built before other ones can pickup namespaces from it.

## Updates

After template is updated, from root run:
```
dotnet new install .
dotnet new install . --force (if installed already)
```