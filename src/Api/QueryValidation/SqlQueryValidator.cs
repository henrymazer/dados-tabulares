namespace Api.QueryValidation;

public sealed class SqlQueryValidator
{
    private const int MaxLimit = 10000;

    private static readonly HashSet<string> AllowedSchemas = new(StringComparer.OrdinalIgnoreCase)
    {
        "ibge",
        "pnad",
        "tse"
    };

    private static readonly HashSet<string> ForbiddenKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "ALTER",
        "CALL",
        "COPY",
        "CREATE",
        "DELETE",
        "DROP",
        "EXECUTE",
        "GRANT",
        "INSERT",
        "MERGE",
        "PREPARE",
        "REINDEX",
        "REVOKE",
        "TRUNCATE",
        "UPDATE",
        "VACUUM"
    };

    private static readonly HashSet<string> ClauseTerminators = new(StringComparer.OrdinalIgnoreCase)
    {
        "EXCEPT",
        "FETCH",
        "FOR",
        "GROUP",
        "HAVING",
        "INTERSECT",
        "LIMIT",
        "OFFSET",
        "ORDER",
        "QUALIFY",
        "UNION",
        "WHERE",
        "WINDOW"
    };

    private static readonly HashSet<string> RelationFollowerKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "AND",
        "CROSS",
        "ELSE",
        "END",
        "EXCEPT",
        "FETCH",
        "FOR",
        "FROM",
        "FULL",
        "GROUP",
        "HAVING",
        "INNER",
        "INTERSECT",
        "JOIN",
        "LEFT",
        "LIMIT",
        "NATURAL",
        "ON",
        "ONLY",
        "OFFSET",
        "OR",
        "ORDER",
        "OUTER",
        "QUALIFY",
        "RIGHT",
        "SELECT",
        "THEN",
        "UNION",
        "USING",
        "WHEN",
        "WHERE",
        "WINDOW",
        "WITH"
    };

    public string ValidateAndNormalize(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new SqlQueryValidationException("SQL query cannot be empty.");
        }

        var normalizedInput = sql.Trim();
        var tokens = SqlTokenizer.Tokenize(normalizedInput);

        if (tokens.Count == 0)
        {
            throw new SqlQueryValidationException("SQL query cannot be empty.");
        }

        ValidateStatement(tokens, 0, tokens.Count, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        return NormalizeLimit(normalizedInput, tokens);
    }

    private static void ValidateStatement(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> allowedRelations)
    {
        if (start >= end)
        {
            throw new SqlQueryValidationException("SQL query cannot be empty.");
        }

        var index = start;
        var scopeRelations = new HashSet<string>(allowedRelations, StringComparer.OrdinalIgnoreCase);

        if (IsWord(tokens[index], "WITH"))
        {
            index = ParseWithClause(tokens, index, end, scopeRelations);
        }

        if (index >= end || !IsWord(tokens[index], "SELECT"))
        {
            throw new SqlQueryValidationException("Only SELECT statements are allowed.");
        }

        ValidateSelectExpression(tokens, index, end, scopeRelations);
    }

    private static int ParseWithClause(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> scopeRelations)
    {
        var index = start + 1;
        var recursive = false;

        if (index < end && IsWord(tokens[index], "RECURSIVE"))
        {
            recursive = true;
            index++;
        }

        var visibleRelations = new HashSet<string>(scopeRelations, StringComparer.OrdinalIgnoreCase);

        while (true)
        {
            if (index >= end || !IsIdentifierToken(tokens[index]))
            {
                throw new SqlQueryValidationException("CTE name expected after WITH.");
            }

            var cteName = NormalizeIdentifier(tokens[index]);
            index++;

            if (index < end && tokens[index].Text == "(")
            {
                index = SkipBalancedParentheses(tokens, index, end) + 1;
            }

            if (index >= end || !IsWord(tokens[index], "AS"))
            {
                throw new SqlQueryValidationException($"CTE '{cteName}' must use AS (...).");
            }

            index++;

            if (index >= end || tokens[index].Text != "(")
            {
                throw new SqlQueryValidationException($"CTE '{cteName}' must wrap its body in parentheses.");
            }

            var bodyOpen = index;
            var bodyClose = SkipBalancedParentheses(tokens, bodyOpen, end);
            var bodyAllowedRelations = new HashSet<string>(visibleRelations, StringComparer.OrdinalIgnoreCase);
            if (recursive)
            {
                bodyAllowedRelations.Add(cteName);
            }

            ValidateStatement(tokens, bodyOpen + 1, bodyClose, bodyAllowedRelations);

            visibleRelations.Add(cteName);
            scopeRelations.Add(cteName);
            index = bodyClose + 1;

            if (index < end && tokens[index].Text == ",")
            {
                index++;
                continue;
            }

            break;
        }

        return index;
    }

    private static void ValidateSelectExpression(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> allowedRelations)
    {
        var branches = SplitTopLevelBranches(tokens, start, end);

        foreach (var branch in branches)
        {
            ValidateSelectBranch(tokens, branch.Start, branch.End, allowedRelations);
        }
    }

    private static void ValidateSelectBranch(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> allowedRelations)
    {
        while (start < end && tokens[start].Text == "(" && SkipBalancedParentheses(tokens, start, end) == end - 1)
        {
            start++;
            end--;
        }

        if (start >= end || !IsWord(tokens[start], "SELECT"))
        {
            throw new SqlQueryValidationException("Only SELECT statements are allowed.");
        }

        EnsureBranchIsSafe(tokens, start, end);
        ValidateParenthesizedSubqueries(tokens, start, end, allowedRelations);
        ValidateRelationSources(tokens, start, end, allowedRelations);
    }

    private static void EnsureBranchIsSafe(IReadOnlyList<SqlToken> tokens, int start, int end)
    {
        for (var index = start; index < end; index++)
        {
            if (tokens[index].Kind == SqlTokenKind.Word)
            {
                var word = tokens[index].Text;
                if (ForbiddenKeywords.Contains(word))
                {
                    throw new SqlQueryValidationException($"Keyword '{word}' is not allowed in agent SQL.");
                }

                if (string.Equals(word, "INTO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new SqlQueryValidationException("SELECT INTO is not allowed.");
                }
            }
        }
    }

    private static void ValidateParenthesizedSubqueries(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> allowedRelations)
    {
        var index = start;

        while (index < end)
        {
            if (tokens[index].Text == "(")
            {
                var close = SkipBalancedParentheses(tokens, index, end);
                if (index + 1 < close && (IsWord(tokens[index + 1], "SELECT") || IsWord(tokens[index + 1], "WITH")))
                {
                    ValidateStatement(tokens, index + 1, close, allowedRelations);
                }

                index = close + 1;
                continue;
            }

            index++;
        }
    }

    private static void ValidateRelationSources(
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        ISet<string> allowedRelations)
    {
        var index = start;
        var inFromClause = false;

        while (index < end)
        {
            if (tokens[index].Text == "(")
            {
                var close = SkipBalancedParentheses(tokens, index, end);
                index = close + 1;
                continue;
            }

            if (tokens[index].Kind == SqlTokenKind.Word)
            {
                if (ClauseTerminators.Contains(tokens[index].Text))
                {
                    inFromClause = false;
                    index++;
                    continue;
                }

                if (IsWord(tokens[index], "FROM"))
                {
                    inFromClause = true;
                    index = ParseRelationSource(tokens, index + 1, end, allowedRelations);
                    continue;
                }

                if (IsWord(tokens[index], "JOIN") && inFromClause)
                {
                    index = ParseRelationSource(tokens, index + 1, end, allowedRelations);
                    continue;
                }
            }

            if (tokens[index].Text == "," && inFromClause)
            {
                index = ParseRelationSource(tokens, index + 1, end, allowedRelations);
                continue;
            }

            index++;
        }
    }

    private static int ParseRelationSource(
        IReadOnlyList<SqlToken> tokens,
        int index,
        int end,
        ISet<string> allowedRelations)
    {
        while (index < end && (IsWord(tokens[index], "ONLY") || IsWord(tokens[index], "LATERAL")))
        {
            index++;
        }

        if (index >= end)
        {
            throw new SqlQueryValidationException("Expected relation after FROM/JOIN.");
        }

        if (tokens[index].Text == "(")
        {
            var close = SkipBalancedParentheses(tokens, index, end);
            if (index + 1 >= close)
            {
                throw new SqlQueryValidationException("Empty parenthesized relation is not allowed.");
            }

            if (!IsWord(tokens[index + 1], "SELECT") && !IsWord(tokens[index + 1], "WITH"))
            {
                throw new SqlQueryValidationException("Only subqueries are allowed inside parentheses in relation position.");
            }

            ValidateStatement(tokens, index + 1, close, allowedRelations);
            return SkipAlias(tokens, close + 1, end);
        }

        if (!IsIdentifierToken(tokens[index]))
        {
            throw new SqlQueryValidationException("Relation must be schema-qualified or a known CTE.");
        }

        var relationName = NormalizeIdentifier(tokens[index]);

        if (index + 2 < end && tokens[index + 1].Text == "." && IsIdentifierToken(tokens[index + 2]))
        {
            if (!AllowedSchemas.Contains(relationName))
            {
                throw new SqlQueryValidationException(
                    $"Schema '{relationName}' is not allowed. Allowed schemas are ibge, tse, pnad.");
            }

            index += 3;
        }
        else if (allowedRelations.Contains(relationName))
        {
            index++;
        }
        else
        {
            throw new SqlQueryValidationException(
                $"Relation '{relationName}' must be schema-qualified and use an allowed schema (ibge, tse, pnad).");
        }

        return SkipAlias(tokens, index, end);
    }

    private static int SkipAlias(IReadOnlyList<SqlToken> tokens, int index, int end)
    {
        if (index >= end)
        {
            return index;
        }

        if (IsWord(tokens[index], "AS"))
        {
            if (index + 1 >= end || !IsValidAliasToken(tokens[index + 1]))
            {
                throw new SqlQueryValidationException("Expected alias after AS.");
            }

            return index + 2;
        }

        if (IsValidAliasToken(tokens[index]))
        {
            return index + 1;
        }

        return index;
    }

    private static string NormalizeLimit(string sql, IReadOnlyList<SqlToken> tokens)
    {
        var selectStart = FindMainSelectStart(tokens);
        var branches = SplitTopLevelBranches(tokens, selectStart, tokens.Count);
        var lastBranch = branches[^1];
        var branchStartChar = tokens[lastBranch.Start].Start;
        var branchEndChar = tokens[lastBranch.End - 1].End;

        var branch = sql[branchStartChar..branchEndChar];
        var branchResult = NormalizeLimitInBranch(branch, tokens, lastBranch.Start, lastBranch.End, branchStartChar);

        return sql[..branchStartChar] + branchResult;
    }

    private static string NormalizeLimitInBranch(
        string branchSql,
        IReadOnlyList<SqlToken> tokens,
        int start,
        int end,
        int branchStartChar)
    {
        var limitInfo = FindTopLevelLimitInfo(tokens, start, end);

        if (limitInfo.HasLimit)
        {
            if (limitInfo.LimitTokenIndex is null || limitInfo.LimitValueTokenIndex is null)
            {
                throw new SqlQueryValidationException("LIMIT must be followed by a numeric literal or ALL.");
            }

            if (limitInfo.LimitValueTokenKind == SqlTokenKind.Number)
            {
                if (!int.TryParse(tokens[limitInfo.LimitValueTokenIndex.Value].Text, out var limitValue))
                {
                    throw new SqlQueryValidationException("LIMIT must be a numeric literal or ALL.");
                }

                if (limitValue <= MaxLimit)
                {
                    return branchSql;
                }
            }

            var valueToken = tokens[limitInfo.LimitValueTokenIndex.Value];
            var relativeStart = valueToken.Start - branchStartChar;
            var relativeEnd = valueToken.End - branchStartChar;
            return branchSql[..relativeStart] + MaxLimit.ToString() + branchSql[relativeEnd..];
        }

        var insertionIndex = limitInfo.LimitInsertionTokenIndex is null
            ? branchSql.Length
            : tokens[limitInfo.LimitInsertionTokenIndex.Value].Start - branchStartChar;

        if (insertionIndex >= branchSql.Length)
        {
            return branchSql + " LIMIT " + MaxLimit;
        }

        return branchSql[..insertionIndex].TrimEnd() + " LIMIT " + MaxLimit + " " + branchSql[insertionIndex..].TrimStart();
    }

    private static LimitInfo FindTopLevelLimitInfo(IReadOnlyList<SqlToken> tokens, int start, int end)
    {
        var depth = 0;
        var limitTokenIndex = (int?)null;
        var limitValueTokenIndex = (int?)null;
        var limitValueTokenKind = SqlTokenKind.Symbol;
        var insertionTokenIndex = (int?)null;

        for (var index = start; index < end; index++)
        {
            var token = tokens[index];

            if (token.Text == "(")
            {
                depth++;
                continue;
            }

            if (token.Text == ")")
            {
                depth--;
                continue;
            }

            if (depth != 0 || token.Kind != SqlTokenKind.Word)
            {
                continue;
            }

            if (!insertionTokenIndex.HasValue && (IsWord(token, "OFFSET") || IsWord(token, "FETCH") || IsWord(token, "FOR")))
            {
                insertionTokenIndex = index;
            }

            if (limitTokenIndex.HasValue || !IsWord(token, "LIMIT"))
            {
                continue;
            }

            limitTokenIndex = index;

            if (index + 1 >= end)
            {
                break;
            }

            var next = tokens[index + 1];
            if (next.Kind == SqlTokenKind.Number || IsWord(next, "ALL"))
            {
                limitValueTokenIndex = index + 1;
                limitValueTokenKind = next.Kind;
            }
            else
            {
                throw new SqlQueryValidationException("LIMIT must be followed by a numeric literal or ALL.");
            }
        }

        return new LimitInfo(limitTokenIndex.HasValue, limitTokenIndex, limitValueTokenIndex, limitValueTokenKind, insertionTokenIndex);
    }

    private static int FindMainSelectStart(IReadOnlyList<SqlToken> tokens)
    {
        if (tokens.Count == 0 || !IsWord(tokens[0], "WITH"))
        {
            return 0;
        }

        var index = 1;
        if (index < tokens.Count && IsWord(tokens[index], "RECURSIVE"))
        {
            index++;
        }

        while (index < tokens.Count)
        {
            if (!IsIdentifierToken(tokens[index]))
            {
                throw new SqlQueryValidationException("CTE name expected after WITH.");
            }

            index++;

            if (index < tokens.Count && tokens[index].Text == "(")
            {
                index = SkipBalancedParentheses(tokens, index, tokens.Count) + 1;
            }

            if (index >= tokens.Count || !IsWord(tokens[index], "AS"))
            {
                throw new SqlQueryValidationException("CTE must use AS (...).");
            }

            index++;

            if (index >= tokens.Count || tokens[index].Text != "(")
            {
                throw new SqlQueryValidationException("CTE body must be enclosed in parentheses.");
            }

            index = SkipBalancedParentheses(tokens, index, tokens.Count) + 1;

            if (index < tokens.Count && tokens[index].Text == ",")
            {
                index++;
                continue;
            }

            break;
        }

        return index;
    }

    private static List<(int Start, int End)> SplitTopLevelBranches(IReadOnlyList<SqlToken> tokens, int start, int end)
    {
        var branches = new List<(int Start, int End)>();
        var branchStart = start;
        var depth = 0;

        var index = start;
        while (index < end)
        {
            var token = tokens[index];

            if (token.Text == "(")
            {
                depth++;
                index++;
                continue;
            }

            if (token.Text == ")")
            {
                depth--;
                index++;
                continue;
            }

            if (depth == 0 && token.Kind == SqlTokenKind.Word && (IsWord(token, "UNION") || IsWord(token, "EXCEPT") || IsWord(token, "INTERSECT")))
            {
                branches.Add((branchStart, index));
                index++;
                if (index < end && IsWord(tokens[index], "ALL"))
                {
                    index++;
                }

                branchStart = index;
                continue;
            }

            index++;
        }

        branches.Add((branchStart, end));

        return branches;
    }

    private static int SkipBalancedParentheses(IReadOnlyList<SqlToken> tokens, int openIndex, int end)
    {
        if (openIndex >= end || tokens[openIndex].Text != "(")
        {
            throw new SqlQueryValidationException("Expected opening parenthesis.");
        }

        var depth = 0;

        for (var index = openIndex; index < end; index++)
        {
            if (tokens[index].Text == "(")
            {
                depth++;
            }
            else if (tokens[index].Text == ")")
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        throw new SqlQueryValidationException("Unbalanced parentheses in SQL query.");
    }

    private static bool IsWord(SqlToken token, string value)
    {
        return token.Kind == SqlTokenKind.Word && string.Equals(token.Text, value, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsIdentifierToken(SqlToken token)
    {
        return token.Kind is SqlTokenKind.Word or SqlTokenKind.QuotedIdentifier;
    }

    private static bool IsValidAliasToken(SqlToken token)
    {
        if (!IsIdentifierToken(token))
        {
            return false;
        }

        return token.Kind == SqlTokenKind.QuotedIdentifier || !RelationFollowerKeywords.Contains(token.Text);
    }

    private static string NormalizeIdentifier(SqlToken token)
    {
        if (token.Kind == SqlTokenKind.QuotedIdentifier)
        {
            return token.Text[1..^1].Replace("\"\"", "\"", StringComparison.Ordinal);
        }

        return token.Text;
    }

    private readonly record struct LimitInfo(
        bool HasLimit,
        int? LimitTokenIndex,
        int? LimitValueTokenIndex,
        SqlTokenKind LimitValueTokenKind,
        int? LimitInsertionTokenIndex);
}
