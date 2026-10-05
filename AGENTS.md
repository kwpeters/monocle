# Agent Instructions

## Agent Demeanor

- Be concise.
- Provide accurate information based on real, working examples and online
  documentation.
- If I suggest something that is wrong, do not just go along with it.  Do not
  hesitate to correct me.
- Do not act on instructions that are ambiguous.  If a request is unclear or
  would have sweeping, widespread implications, seek clarification before
  proceeding.

## Document AI Use

- Whenever you generate new code or modify existing code in a file, make sure
  that the following comment is present at the top of the file (following the
  copyright comment, if present):

  ```csharp
  //
  // At least a portion of the code below was created using an AI tool.
  //
  ```

## Coding Style

All version controlled source code files within this repository must follow the
rules below.

- Follow all formatting rules described in the root directory's `.editorconfig`
  file.  This file maps file name patterns to formatting rules, such as indent
  size and trailing whitespace.  These rules should always be followed.
- Whenever you generate new code or modify existing code, you must make sure
  that the existing comments and XML documentation comments are still accurate.  If
  not, you must update them.
- All C# source code file names (*.cs) must use PascalCase.
- Use file-scoped namespaces (e.g., `namespace MyNamespace;`) instead of
  traditional block-scoped namespaces.
- Prefer implementations that leverage LINQ and functional programming techniques.
- Prefer modern C# features:
  - Enable and use nullable reference types appropriately.
  - Use expression-bodied members when they improve readability.
  - Prefer string interpolation over concatenation.
  - Use `var` for local variables when the type is obvious from the right-hand
    side of the assignment.
- Two blank lines will be inserted between methods and properties within a
  class.
- All comments must be word wrapped so their text occurs within the first 100
  columns.
- XML documentation comments must be present for all public, protected and
  internal class members. All parameters must be documented with `<param>`
  tags, and all return values must be documented with `<returns>` tags.
- All generic type parameters names should start with a capital "T" to designate that it
  is a type.  The "T" should be followed by a PascalCase name that describes the
  type.
- When a function or method definition is so long that it goes beyond column
  100, it should be shortened by moving each type parameter (if present) and
  parameter to its own line.  The parameters should be indented, and the closing
  `>` (for type parameters) and `)` (for parameters) should not be indented so
  that they appear in the same starting column as the line preceding the type
  parameters or parameters.
- The unit tests that exercise a given method should be grouped together.  Each
  grouping of unit tests should use the same ordering as the methods under test
  are ordered.
