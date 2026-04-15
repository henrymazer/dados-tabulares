Starting CodeRabbit review in plain text mode...

Review directory: /home/henry_mazer/dados-tabulares

Connecting to review service
Setting up
Summarizing
Reviewing

============================================================================
File: src/Data/PersistenceRecords.cs
Line: 256
Type: potential_issue

Comment:
Null-forgiving operator on Geometria may mask null reference issues.

Using default! suppresses compiler warnings but doesn't prevent NullReferenceException if Geometria is accessed before being assigned. Consider using a nullable type (MultiPolygon?) to make the nullability explicit, or ensure all code paths initialize this property before access.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/PersistenceRecords.cs at line 256, The Geometria property currently uses the null-forgiving default! which hides potential null ref issues; change the property declaration from MultiPolygon Geometria to a nullable MultiPolygon? and update code that reads Geometria (search for usages of the Geometria property and any accessors in the PersistenceRecords class) to handle null (null-checks or null-coalescing) or ensure assignment during object construction; alternatively, if Geometria must never be null, initialize it in the class constructor or factory method rather than using default! to make non-nullability explicit.

============================================================================
File: src/Etl/Ibge/IbgeSemanticCatalog.cs
Line: 52 to 57
Type: potential_issue

Comment:
TextFieldParser closes the underlying stream on disposal.

Unlike WriteCsv which uses leaveOpen: true, TextFieldParser will dispose the input stream when it's disposed. This inconsistency may surprise callers who expect to reuse the stream. Consider wrapping the stream in a StreamReader with leaveOpen: true if stream reuse is needed.


🛡️ Proposed fix to preserve stream

-using var parser = new TextFieldParser(input)
+using var reader = new StreamReader(input, leaveOpen: true);
+using var parser = new TextFieldParser(reader)
 {
     TextFieldType = FieldType.Delimited,
     HasFieldsEnclosedInQuotes = true,
     TrimWhiteSpace = true
 };




TextFieldParser stream disposal behavior C#

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeSemanticCatalog.cs around lines 52 - 57, The TextFieldParser created as "using var parser = new TextFieldParser(input)" will close the underlying stream on Dispose; to preserve the input stream, construct a StreamReader over the input with leaveOpen: true (e.g., new StreamReader(input, leaveOpen: true)) and pass that StreamReader to the TextFieldParser(TextReader) constructor, then dispose only the parser (and the StreamReader if desired) so the original input stream remains open; update the code in IbgeSemanticCatalog where "parser" is created to use a StreamReader wrapper instead of passing the raw input stream.

============================================================================
File: src/Data/PersistenceRecords.cs
Line: 235
Type: potential_issue

Comment:
Type inconsistency: MunicipioCodigoIbge is string here but int in other records.

This record now uses string MunicipioCodigoIbge, while other records in this file (e.g., ZonaEleitoralRecord, SecaoEleitoralRecord, ResultadoEleitoralRecord, PerfilEleitoradoRecord, and several Dado*Record types) use int MunicipioCodigoIbge. This type mismatch may cause data conversion issues or confusion when joining/comparing data across these entities.

Additionally, per the summary, this is a type change from int to string — verify that all consumers of SetorCensitarioRecord are updated accordingly.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Data/PersistenceRecords.cs at line 235, The MunicipioCodigoIbge property on SetorCensitarioRecord is declared as string but other related records (e.g., ZonaEleitoralRecord, SecaoEleitoralRecord, ResultadoEleitoralRecord, PerfilEleitoradoRecord and various Dado*Record types) expect an int; change the MunicipioCodigoIbge property in SetorCensitarioRecord to int (with an appropriate default like 0 or nullable int if needed) to restore type consistency, update any constructors/initializers in SetorCensitarioRecord that set this property, and then run/adjust any callers that consume SetorCensitarioRecord to compile with the int type (or confirm they already handle ints) to ensure no conversion/runtime issues remain.

============================================================================
File: src/Etl/CargaBrutaSnapshotService.cs
Line: 20 to 28
Type: potential_issue

Comment:
SingleOrDefaultAsync will throw if multiple records match.

If database contains duplicate current snapshots for the same Fonte/Dataset/Escopo/Hash combination (e.g., due to a race condition or data migration issue), this will throw InvalidOperationException. Consider whether FirstOrDefaultAsync is more appropriate, or add a unique constraint to enforce this invariant at the database level.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/CargaBrutaSnapshotService.cs around lines 20 - 28, The query in CargaBrutaSnapshotService that assigns snapshotExistente uses SingleOrDefaultAsync which will throw if multiple matching rows exist; change SingleOrDefaultAsync(...) to FirstOrDefaultAsync(...) on the IQueryable (the call chain starting with context.CargasBrutasSnapshots.AsNoTracking()) to return the first match instead of throwing, or alternatively add a database unique constraint/index on (Fonte, Dataset, Escopo, HashSnapshot, IsCurrent) to enforce uniqueness at the DB level if you want to keep SingleOrDefaultAsync; update any related tests/assumptions accordingly.

============================================================================
File: src/Etl/Tse/TseLocalVotacaoBrutoStagingPipeline.cs
Line: 19 to 25
Type: potential_issue

Comment:
Consider specifying encoding for TSE CSV files.

TSE files often use Latin-1 (ISO-8859-1) encoding rather than UTF-8. Without explicit encoding, TextFieldParser defaults to UTF-8, which may corrupt accented characters in Brazilian municipality and location names.




🔧 Proposed fix to specify encoding

 using var entryStream = csvEntry.Open();
-using var parser = new TextFieldParser(entryStream)
+using var reader = new StreamReader(entryStream, Encoding.Latin1);
+using var parser = new TextFieldParser(reader)
 {
     TextFieldType = FieldType.Delimited,
     HasFieldsEnclosedInQuotes = true,
     TrimWhiteSpace = true
 };


Add at the top of the file:
using System.Text;

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Tse/TseLocalVotacaoBrutoStagingPipeline.cs around lines 19 - 25, The TextFieldParser in TseLocalVotacaoBrutoStagingPipeline.cs is created from entryStream without an explicit encoding (defaults to UTF-8) which can mangle Latin-1 characters; fix by importing System.Text and wrap entryStream in a StreamReader with the correct encoding (e.g., Encoding.GetEncoding("ISO-8859-1") or Encoding.Latin1) and pass that StreamReader into the TextFieldParser constructor (the code that currently does using var parser = new TextFieldParser(entryStream) should be changed to use a StreamReader created from entryStream with the Latin-1 encoding).

============================================================================
File: src/Etl/Ibge/IbgeSemanticCatalog.cs
Line: 19 to 22
Type: potential_issue

Comment:
Add null check for entries parameter.

The constructor doesn't validate entries before calling ToArray(), which will throw a NullReferenceException if null is passed. Other methods in this class use ArgumentNullException.ThrowIfNull() for consistency.


🛡️ Proposed fix

 public IbgeSemanticCatalog(IEnumerable entries)
 {
+    ArgumentNullException.ThrowIfNull(entries);
     Entries = entries.ToArray();
 }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeSemanticCatalog.cs around lines 19 - 22, The constructor IbgeSemanticCatalog currently calls entries.ToArray() without validation; add a null check using ArgumentNullException.ThrowIfNull(entries) at the start of the IbgeSemanticCatalog(IEnumerable entries) constructor, then assign Entries = entries.ToArray(); so it matches the class's other null-checking pattern and prevents a NullReferenceException.

============================================================================
File: src/Etl/Ibge/IbgeSemanticCatalogPersistenceService.cs
Line: 26 to 36
Type: potential_issue

Comment:
Potential duplicate categoria records.

Unlike variables, the categorias collection has no deduplication. If catalog.Entries contains multiple entries with the same (Pacote, Variavel, Categoria) tuple, duplicate records will be inserted. If the database has a unique constraint on these columns, this will cause a runtime error.

Consider whether deduplication is needed here as well.


♻️ Proposed fix if deduplication is needed

         var categorias = catalog.Entries
             .Where(entry => !string.IsNullOrWhiteSpace(entry.Categoria))
+            .GroupBy(entry => new { entry.Pacote, entry.Variavel, entry.Categoria })
+            .Select(group => group.First())
             .Select(entry => new IbgeCatalogoCategoriaRecord

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeSemanticCatalogPersistenceService.cs around lines 26 - 36, The categorias projection can produce duplicate IbgeCatalogoCategoriaRecord rows when catalog.Entries contains the same (Pacote, Variavel, Categoria) tuple; update the LINQ pipeline that builds categorias to deduplicate by that tuple before calling ToList. For example, either GroupBy(entry => new { entry.Pacote, entry.Variavel, entry.Categoria }) and select a single representative, or project to IbgeCatalogoCategoriaRecord and then apply DistinctBy(r => new { r.Pacote, r.Variavel, r.Categoria }) so the final categorias list contains unique (Pacote, Variavel, Categoria) combinations and will not violate unique constraints when inserted.

============================================================================
File: src/Etl/Ibge/GeoPackageGeometryReader.cs
Line: 12 to 32
Type: potential_issue

Comment:
Missing bounds validation before WKB slice.

The initial check only validates blob.Length >= 8, but wkbOffset can be 40, 56, or 72 depending on the envelope indicator. A truncated blob with a non-zero envelope type will cause ArgumentOutOfRangeException at line 32.



🛡️ Proposed fix to add bounds validation

         var envelopeBytes = envelopeIndicator switch
         {
             0 => 0,
             1 => 32,
             2 => 48,
             3 => 48,
             4 => 64,
             _ => throw new InvalidOperationException($"Envelope GeoPackage não suportado: {envelopeIndicator}.")
         };

+        var wkbOffset = 8 + envelopeBytes;
+        if (blob.Length

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/GeoPackageGeometryReader.cs around lines 12 - 32, The code computes wkbOffset from envelopeIndicator/envelopeBytes and slices blob[wkbOffset..] without ensuring the blob is long enough; add a bounds check after computing envelopeBytes/wkbOffset (using the same variables in GeoPackageGeometryReader.cs: envelopeIndicator, envelopeBytes, wkbOffset) to verify blob.Length >= wkbOffset (and optionally >= wkbOffset + 1) and throw a clear InvalidOperationException (or similar) if not, so ReadInt32 and new WKBReader().Read(blob[wkbOffset..]) cannot cause ArgumentOutOfRangeException.

============================================================================
File: src/Etl/Ibge/IbgeGeoPackageSetorIngestionPipeline.cs
Line: 11
Type: potential_issue

Comment:
Add validation for filePath parameter.

The method does not validate that filePath is non-null and non-empty before passing it to the reader. This could result in less informative exceptions propagating from the reader.




🛡️ Proposed fix

 public async Task IngerirAsync(string filePath, CancellationToken ct = default)
 {
+    ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
+
     await using var transaction = await context.Database.BeginTransactionAsync(ct);

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeGeoPackageSetorIngestionPipeline.cs at line 11, In IngerirAsync validate the filePath parameter at the start of the method (in class IbgeGeoPackageSetorIngestionPipeline) and throw ArgumentNullException or ArgumentException when filePath is null, empty or whitespace before passing it to the reader; update the method to check string.IsNullOrWhiteSpace(filePath) and throw a clear exception message referencing the parameter name so callers get an informative failure instead of a reader-originating exception.

============================================================================
File: src/Etl/CargaBrutaSnapshotService.cs
Line: 123 to 131
Type: potential_issue

Comment:
Loading entire files into memory may cause OOM for large CSVs.

File.ReadAllBytes(file) loads each file entirely into memory. For directories with large CSV files, this can cause excessive memory pressure or OutOfMemoryException. Use streaming to stay consistent with ComputeSha256.



♻️ Proposed fix using streaming

             foreach (var file in files)
             {
                 var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(file));
                 sha256.TransformBlock(fileNameBytes, 0, fileNameBytes.Length, null, 0);
 
-                var fileBytes = File.ReadAllBytes(file);
-                totalBytes += fileBytes.LongLength;
-                sha256.TransformBlock(fileBytes, 0, fileBytes.Length, null, 0);
+                using var fileStream = File.OpenRead(file);
+                totalBytes += fileStream.Length;
+                
+                var buffer = new byte[81920]; // 80 KB buffer
+                int bytesRead;
+                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
+                {
+                    sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
+                }
             }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/CargaBrutaSnapshotService.cs around lines 123 - 131, The loop in CargaBrutaSnapshotService currently calls File.ReadAllBytes(file), which can OOM for large CSVs; replace that with a streaming read using FileStream (or similar) and a reusable buffer, reading chunks and calling sha256.TransformBlock(buffer, 0, bytesRead, null, 0) per chunk while accumulating totalBytes, and after finishing each file ensure the hash state is properly finalized (e.g., call TransformFinalBlock with an empty buffer or follow your ComputeSha256 pattern) so the code mirrors the streaming approach used in ComputeSha256 and avoids loading whole files into memory.

============================================================================
File: src/Etl/Ibge/IbgeGeoPackageReader.cs
Line: 88 to 90
Type: potential_issue

Comment:
Same InvalidCastException risk for geom column.

The same null safety issue flagged in ReadMunicipios applies here.

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeGeoPackageReader.cs around lines 88 - 90, CreateSetor currently casts reader["geom"] directly to byte[] which can throw InvalidCastException for DBNull; mirror the fix from ReadMunicipios by checking for DBNull/null before casting: use reader.IsDBNull(columnIndex) or check reader["geom"] is DBNull.Value, then only cast to byte[] and call GeoPackageGeometryReader.ReadMultiPolygon when non-null (otherwise set geometria to null or an appropriate empty geometry), so CreateSetor and GeoPackageGeometryReader.ReadMultiPolygon invocations are safe.

============================================================================
File: src/Etl/Ibge/IbgeAggregateStagingPipeline.cs
Line: 25
Type: potential_issue

Comment:
Multiple delimiters may cause incorrect field parsing.

SetDelimiters(";", ",") treats both characters as valid delimiters simultaneously. If a CSV uses semicolons as the delimiter but contains commas within field values, those commas will incorrectly split the field even when quoted handling is enabled for the outer delimiter.

Consider detecting the actual delimiter from the header line or processing files with a known single delimiter.


🛠️ Possible approach: detect delimiter from first line

+       // Detect delimiter by checking which appears more frequently in header
+       using var reader = new StreamReader(csvEntry.Open());
+       var firstLine = await reader.ReadLineAsync(ct);
+       var delimiter = firstLine?.Count(c => c == ';') > firstLine?.Count(c => c == ',') ? ";" : ",";
+       
        using var entryStream = csvEntry.Open();
        using var parser = new TextFieldParser(entryStream)
        {
            TextFieldType = FieldType.Delimited,
            HasFieldsEnclosedInQuotes = true,
            TrimWhiteSpace = true
        };

-       parser.SetDelimiters(";", ",");
+       parser.SetDelimiters(delimiter);

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeAggregateStagingPipeline.cs at line 25, The parser is being configured with SetDelimiters(";", ",") which treats both characters as simultaneous delimiters and can split fields incorrectly when the file uses a single delimiter and contains the other character inside quoted values; update the IbgeAggregateStagingPipeline logic to detect the actual delimiter from the first (header) line or accept a known single delimiter and then call parser.SetDelimiters with just that one character (e.g., determine delimiterChar from header sniffing and call parser.SetDelimiters(delimiterChar.ToString())); ensure the detection handles common choices (',' or ';') and falls back to a default if ambiguous.

============================================================================
File: src/Etl/CargaBrutaSnapshotService.cs
Line: 75 to 94
Type: potential_issue

Comment:
Split SaveChangesAsync calls risk partial commit on failure.

If the second SaveChangesAsync (line 94) fails, the snapshot will be persisted but the audit record will not, leaving the system in an inconsistent state. Wrap both operations in an explicit transaction.



🔒 Proposed fix using explicit transaction

     public async Task RegistrarCargaAplicadaAsync(CargaBrutaSnapshotPreparado snapshot, int registrosImportados, CancellationToken ct = default)
     {
         ArgumentNullException.ThrowIfNull(snapshot);
 
         var agora = DateTimeOffset.UtcNow;
+        await using var transaction = await context.Database.BeginTransactionAsync(ct);
+
         var snapshotsAtuais = await context.CargasBrutasSnapshots
             .Where(x => x.Fonte == snapshot.Fonte &&
                         x.Dataset == snapshot.Dataset &&
                         x.Escopo == snapshot.Escopo &&
                         x.IsCurrent)
             .ToListAsync(ct);
 
         foreach (var atual in snapshotsAtuais)
         {
             atual.IsCurrent = false;
             atual.SubstituidoEmUtc = agora;
         }
 
         var novoSnapshot = new CargaBrutaSnapshotRecord
         {
             // ... properties ...
         };
 
         await context.CargasBrutasSnapshots.AddAsync(novoSnapshot, ct);
         await context.SaveChangesAsync(ct);
 
         await context.CargasBrutasAuditorias.AddAsync(new CargaBrutaAuditoriaRecord
         {
             // ... properties ...
         }, ct);
 
         await context.SaveChangesAsync(ct);
+        await transaction.CommitAsync(ct);
     }

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/CargaBrutaSnapshotService.cs around lines 75 - 94, The code currently calls context.SaveChangesAsync twice (after adding novoSnapshot to CargasBrutasSnapshots and again after adding CargaBrutaAuditoriaRecord to CargasBrutasAuditorias) which can leave the snapshot persisted if the audit save fails; wrap these operations in an explicit transaction using context.Database.BeginTransactionAsync(ct) so both additions are atomic: begin a transaction, AddAsync novoSnapshot (and SaveChangesAsync if you need the generated Id), AddAsync the CargaBrutaAuditoriaRecord (using novoSnapshot.Id), call SaveChangesAsync once (or call SaveChangesAsync after each add but only commit at the end), then await transaction.CommitAsync(ct) and ensure rollback/dispose in a catch/finally; reference the entities CargasBrutasSnapshots, CargasBrutasAuditorias, the CargaBrutaAuditoriaRecord creation, novoSnapshot and context.SaveChangesAsync when making the change.

============================================================================
File: src/Etl/EtlRunner.cs
Line: 73 to 84
Type: potential_issue

Comment:
Unnecessary file stream opened for .gpkg path.

The stream at line 73 is opened before checking the .gpkg extension. When the .gpkg branch is taken (lines 77-80), the pipeline receives command.FilePath and likely opens the file itself, leaving the stream on line 73 unused but held open until method completion.

Move the stream opening to after the .gpkg check to avoid holding an unnecessary file handle.



🔧 Proposed fix

     if (dataset == IbgeDataset.SetoresCensitarios)
     {
         if (Path.GetExtension(command.FilePath).Equals(".gpkg", StringComparison.OrdinalIgnoreCase))
         {
             var realPipeline = new IbgeGeoPackageSetorIngestionPipeline(context);
             return await realPipeline.IngerirAsync(command.FilePath, ct);
         }

+        await using var stream = File.OpenRead(command.FilePath);
         var spatialPipeline = new IbgeSpatialIngestionPipeline(context);
         return await spatialPipeline.IngerirSetoresCensitariosAsync(stream, ct);
     }

+    await using var stream = File.OpenRead(command.FilePath);
     var pipeline = new IbgeIngestionPipeline(context);
     return await pipeline.IngerirAsync(dataset, stream, ct);
-
-    await using var stream = File.OpenRead(command.FilePath);
-    if (dataset == IbgeDataset.SetoresCensitarios)
-    ...


Or restructure with early returns:

-    await using var stream = File.OpenRead(command.FilePath);
     if (dataset == IbgeDataset.SetoresCensitarios)
     {
         if (Path.GetExtension(command.FilePath).Equals(".gpkg", StringComparison.OrdinalIgnoreCase))
         {
             var realPipeline = new IbgeGeoPackageSetorIngestionPipeline(context);
             return await realPipeline.IngerirAsync(command.FilePath, ct);
         }

+        await using var spatialStream = File.OpenRead(command.FilePath);
         var spatialPipeline = new IbgeSpatialIngestionPipeline(context);
-        return await spatialPipeline.IngerirSetoresCensitariosAsync(stream, ct);
+        return await spatialPipeline.IngerirSetoresCensitariosAsync(spatialStream, ct);
     }

+    await using var stream = File.OpenRead(command.FilePath);
     var pipeline = new IbgeIngestionPipeline(context);
     return await pipeline.IngerirAsync(dataset, stream, ct);

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/EtlRunner.cs around lines 73 - 84, The code opens a File stream (stream) before checking for a .gpkg path, which leaves the stream unused when IbgeGeoPackageSetorIngestionPipeline.IngerirAsync(command.FilePath, ct) is called; move the File.OpenRead(command.FilePath) call so it only executes when the code will call IbgeSpatialIngestionPipeline.IngerirSetoresCensitariosAsync(stream, ct). Concretely: keep the existing dataset check for IbgeDataset.SetoresCensitarios, first test Path.GetExtension(command.FilePath) for ".gpkg" and call new IbgeGeoPackageSetorIngestionPipeline(context).IngerirAsync(command.FilePath, ct) as the early-return branch, and only if not .gpkg create the stream and call new IbgeSpatialIngestionPipeline(context).IngerirSetoresCensitariosAsync(stream, ct).

============================================================================
File: src/Etl/Ibge/IbgeGeoPackageReader.cs
Line: 39
Type: potential_issue

Comment:
Potential InvalidCastException if geom column is NULL.

Direct cast from reader["geom"] to byte[] will fail if the database value is DBNull. Consider adding a null check or a helper method consistent with GetString/GetDouble.




🛡️ Proposed fix

-                Geometria = GeoPackageGeometryReader.ReadMultiPolygon((byte[])reader["geom"])
+                Geometria = reader.IsDBNull(0) ? null : GeoPackageGeometryReader.ReadMultiPolygon((byte[])reader["geom"])


Alternatively, create a helper method:

private static byte[]? GetBytes(SqliteDataReader reader, int ordinal)
    => reader.IsDBNull(ordinal) ? null : (byte[])reader.GetValue(ordinal);

Prompt for AI Agent:
Verify each finding against the current code and only fix it if needed.

In @src/Etl/Ibge/IbgeGeoPackageReader.cs at line 39, The direct cast in the Geometria assignment (Geometria = GeoPackageGeometryReader.ReadMultiPolygon((byte[])reader["geom"])) can throw InvalidCastException when reader["geom"] is DBNull; add a null-check or a small helper like GetBytes(SqliteDataReader reader, int ordinal) that returns null when reader.IsDBNull(ordinal) and otherwise returns the byte[] value, then call GeoPackageGeometryReader.ReadMultiPolygon only if the bytes are not null (or pass the nullable bytes if the method accepts null) to avoid the cast from DBNull.

Review completed: 15 findings ✔
