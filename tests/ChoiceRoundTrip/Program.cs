using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using DC2PCMesTextTool;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var sjis = Encoding.GetEncoding(932);
var output = args.Length > 3 ? Path.GetFullPath(args[3]) :
    Path.Combine(Path.GetTempPath(), "DC2PC-choice-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(output);
int assertions = 0;
void Check(bool success, string message)
{
    assertions++;
    if (!success) throw new Exception(message);
}
MesScript Read(byte[] bytes)
{
    var script = new MesScript();
    script.Load(bytes);
    return script;
}
byte[] Fixture(params (byte Op, string Text)[] rows)
{
    var tokens = rows.Select(row =>
    {
        var bytes = sjis.GetBytes(row.Text);
        if (row.Op is >= 0x4A and <= 0x4D)
            for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)(bytes[i] - 0x20);
        return new[] { row.Op }.Concat(bytes).Append((byte)0).ToArray();
    }).ToArray();
    using var stream = new MemoryStream();
    using var writer = new BinaryWriter(stream);
    writer.Write(tokens.Length);
    int offset = 2;
    foreach (var token in tokens) { writer.Write(offset); offset += token.Length; }
    writer.Write((ushort)0x9969);
    foreach (var token in tokens) writer.Write(token);
    return stream.ToArray();
}
byte[] TokenBytes(MesScript script, Token token) =>
    script.RawData.AsSpan(script.BytecodeOffset + 2 + token.Offset, token.Length).ToArray();
string[] Choices(MesScript script) => script.ExtractText(sjis)
    .Where(e => e.Message.TrimStart().StartsWith('○')).Select(e => e.Message).ToArray();
void CheckFlow(MesScript original, MesScript rebuilt, string name)
{
    Check(original.Tokens.Select(t => t.Opcode).SequenceEqual(rebuilt.Tokens.Select(t => t.Opcode)),
        name + ": opcode order changed");
    Check(original.LabelCount == rebuilt.LabelCount, name + ": label count changed");
    var positions = original.Tokens.Select((t, i) => (t.Offset, i)).ToDictionary(x => x.Offset + 2, x => x.i);
    for (int i = 0; i < original.LabelCount; i++)
    {
        int before = BitConverter.ToInt32(original.RawData, 4 + i * 4);
        int after = BitConverter.ToInt32(rebuilt.RawData, 4 + i * 4);
        if (positions.TryGetValue(before, out int token))
            Check(after == rebuilt.Tokens[token].Offset + 2, name + ": branch target changed");
    }
}

// Stress word wrapping, space normalization, punctuation, and missing markers.
var fixture = Read(Fixture((0x4D, "○　元の選択"), (0x4C, "「会話です。」"),
    (0x4D, "○　別の選択"), (0x4D, "2")));
var expected = "○ Ini pilihan Indonesia panjang yang tetap lengkap meskipun batas wrap hanya sepuluh karakter ~ ya!";
var edited = new List<TextEntry>
{
    new() { Message = "  ○Ｉｎｉ\t pilihan　Indonesia\r\npanjang yang tetap lengkap\\nmeskipun batas wrap hanya sepuluh karakter ~ ya!  " },
    new() { Message = "「Dialog biasa tetap mengikuti word wrap.」" },
    new() { Message = "  Marker   dihapus\noleh editor  " },
    new() { Message = "2" }
};
var imported = Read(fixture.ImportText(edited, sjis, new TextFormatter { MaxLineLength = 10, MinLineLength = 5 }));
Check(Choices(imported).SequenceEqual(new[] { expected, "○ Marker dihapus oleh editor" }), "Edited choice normalization failed");
Check(imported.ExtractText(sjis).Where(e => e.Message.StartsWith('○')).All(e => e.IsChoice), "Choice classification missing");
Check(imported.ExtractText(sjis).Last().Message == "2", "Numeric branch ID changed");
CheckFlow(fixture, imported, "synthetic");
var choiceRaw = TokenBytes(imported, imported.Tokens[0]);
Check(choiceRaw.Skip(1).Take(choiceRaw.Length - 2).All(b => b != 0), "Choice contains an embedded terminator");
Check(choiceRaw.Zip(choiceRaw.Skip(1)).Any(p => p.First == 0x61 && p.Second == 0x20), "Missing safe MES space encoding");

var processor = new MesToolProcessor();
string syntheticMes = Path.Combine(output, "synthetic.mes");
string syntheticJson = Path.Combine(output, "synthetic.json");
File.WriteAllBytes(syntheticMes, imported.RawData);
Check(processor.ExportMesToJson(syntheticMes, syntheticJson) == 0, "Synthetic export failed");
var exported = JsonSerializer.Deserialize<List<TextEntry>>(File.ReadAllText(syntheticJson))!;
Check(exported[0].Name == null && exported[0].Message == expected, "Choice was merged into following dialogue as a speaker name");
Check(processor.ImportJsonToMes(syntheticJson, syntheticMes, Path.Combine(output, "synthetic-cycle.mes"), 10) == 0, "Synthetic reimport failed");
Check(Choices(Read(File.ReadAllBytes(Path.Combine(output, "synthetic-cycle.mes")))).SequenceEqual(Choices(imported)), "Synthetic round trip changed choices");
foreach (string invalid in new[] { "", "  ○  ", "\t", "○ bad\u0001label" })
{
    var invalidEntries = edited.Select(e => new TextEntry { Message = e.Message }).ToList();
    invalidEntries[0].Message = invalid;
    bool rejected = false;
    try { fixture.ImportText(invalidEntries, sjis); }
    catch (InvalidDataException) { rejected = true; }
    Check(rejected, "Invalid choice was accepted");
}

int files = 0, choiceCount = 0, choiceFiles = 0, nonChoiceTokens = 0;
if (args.Length >= 3)
{
    // The released binary is loaded separately, so non-choice regressions are
    // checked against what the user actually ran, not a second copy of new code.
    var context = new AssemblyLoadContext("previous-tool", isCollectible: true);
    var previous = context.LoadFromAssemblyPath(Path.GetFullPath(args[2]));
    var type = previous.GetType("DC2PCMesTextTool.MesToolProcessor", true)!;
    object oldProcessor = Activator.CreateInstance(type)!;
    var oldImport = type.GetMethod("ImportJsonToMes")!;
    var jsonDirectory = Path.GetFullPath(args[0]);
    var templateDirectory = Path.GetFullPath(args[1]);
    foreach (string json in Directory.EnumerateFiles(jsonDirectory, "*.json").Order())
    {
        string name = Path.GetFileNameWithoutExtension(json);
        string template = Path.Combine(templateDirectory, name + ".mes");
        if (!File.Exists(template)) continue;
        string currentPath = Path.Combine(output, "current.mes");
        string oldPath = Path.Combine(output, "previous.mes");
        Check((int)oldImport.Invoke(oldProcessor, new object[] { json, template, oldPath, 70 })! == 0, name + ": baseline import failed");
        Check(processor.ImportJsonToMes(json, template, currentPath, 70) == 0, name + ": current import failed");
        var before = Read(File.ReadAllBytes(oldPath));
        var after = Read(File.ReadAllBytes(currentPath));
        var original = Read(File.ReadAllBytes(template));
        CheckFlow(original, after, name);
        Check(before.Tokens.Count == after.Tokens.Count, name + ": token count changed from release");
        var originalChoices = original.ExtractText(sjis).Where(e => e.Message.StartsWith('○')).Select(e => e.Offset).ToHashSet();
        for (int i = 0; i < after.Tokens.Count; i++)
        {
            if (originalChoices.Contains(original.Tokens[i].Offset)) continue;
            Check(TokenBytes(before, before.Tokens[i]).SequenceEqual(TokenBytes(after, after.Tokens[i])), name + $": non-choice token {i} changed");
            nonChoiceTokens++;
        }
        var labels = Choices(after);
        if (labels.Length > 0)
        {
            var jsonEntries = JsonSerializer.Deserialize<List<TextEntry>>(File.ReadAllText(json))!;
            var expectedLabels = jsonEntries.Where(e => e.Message.TrimStart().StartsWith('○'))
                .Select(e => "○ " + e.Message.Trim()[1..].TrimStart()).ToArray();
            Check(labels.SequenceEqual(expectedLabels), name + ": choice text differs from translated JSON");
            Check(labels.All(s => !s.Contains('\n') && !s.Contains('\r') && !s.Contains("  ")), name + ": multiline/double-spaced choice");
            // Deliberately choose a tiny wrap setting; choices must remain whole.
            Check(processor.ImportJsonToMes(json, template, currentPath, 10) == 0, name + ": narrow-wrap import failed");
            Check(Choices(Read(File.ReadAllBytes(currentPath))).SequenceEqual(labels), name + ": wrap affected choices");
            string cycleJson = Path.Combine(output, "cycle.json");
            string cycleMes = Path.Combine(output, "cycle.mes");
            Check(processor.ExportMesToJson(currentPath, cycleJson) == 0, name + ": re-export failed");
            Check(processor.ImportJsonToMes(cycleJson, currentPath, cycleMes, 70) == 0, name + ": re-import failed");
            Check(Choices(Read(File.ReadAllBytes(cycleMes))).SequenceEqual(labels), name + ": round trip changed choices");
            choiceCount += labels.Length;
            choiceFiles++;
        }
        files++;
        if (files % 200 == 0) Console.WriteLine($"Checked {files} files, {choiceCount} choices...");
    }
    Check(files > 0 && choiceCount > 0, "Corpus did not contain any choices");
    context.Unload();
}
var report = new { passed = true, assertions, files, choiceFiles, choiceCount, nonChoiceTokens, output };
string result = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(Path.Combine(output, "results.json"), result);
Console.WriteLine(result);
