About TomlReaderForUnity

Use the TomlReaderForUnity package to load, parse, and validate TOML configuration files in Unity projects. For example, use TomlReaderForUnity to define game settings, balancing values, or environment configurations in external .toml files that can be edited without rebuilding the project. The TomlReaderForUnity package also includes runtime loading support, structured data mapping, and validation utilities for safe configuration handling.

Installing TomlReaderForUnity

To install this package, follow the instructions in the Package Manager documentation
.

If you are installing from a local package or Git repository, add the package via Package Manager → Add package from disk… or Add package from git URL….

After installation, ensure the following setup steps are complete:

Create a StreamingAssets folder in your project if one does not already exist. Configuration files can be placed here for runtime loading.

Add your .toml configuration files (for example, settings.toml) into Assets/StreamingAssets/.