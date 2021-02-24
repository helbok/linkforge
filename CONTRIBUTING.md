# Contributing

Thanks for your interest in contributing to LinkForge. This project is a
teaching example, but it follows the same contribution workflow we would use
for a real library.

## Ground rules

- Open an issue before starting on anything non-trivial.
- Keep commits small and focused — one logical change per commit.
- Use [Conventional Commits](https://www.conventionalcommits.org/) for commit
  messages.
- Add or update tests for any behaviour change.

## Development setup

```console
$ dotnet restore
$ dotnet build
$ dotnet test
```

## Submitting changes

1. Create a branch off `main`.
2. Make your changes with clear commits.
3. Run the full test suite.
4. Open a pull request and describe what changed and why.
