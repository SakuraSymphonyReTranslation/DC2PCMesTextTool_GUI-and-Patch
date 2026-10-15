# Choice import/export regression checks

Run the synthetic fixtures without installing test frameworks:

```powershell
dotnet run --project tests/ChoiceRoundTrip -c Release
```

To also verify translated scripts, supply four arguments after `--`:

1. Translated JSON directory.
2. Matching original MES directory.
3. The previous released `DC2PCMesTextTool_GUI.dll`, outside the build directory.
4. A separate output directory for disposable test files and `results.json`.

The corpus test compares non-choice token bytes with the previous released
binary, verifies opcode order and relocated branch targets, checks choice text
against JSON, stresses a wrap width of 10, and runs MES → JSON → MES cycles.
It writes only to the supplied test output directory. It does not install MES
files into the game or modify input JSON/templates.
