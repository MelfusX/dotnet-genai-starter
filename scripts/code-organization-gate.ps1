param(
    [int] $MaxFileLines = 200,
    [int] $MaxLogicalTypeLines = 200
)

$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$srcRoot = Join-Path $repoRoot "src"

function Get-RelativePath {
    param([string] $Path)

    return Resolve-Path -Relative $Path
}

function Get-FileLineCount {
    param([string] $Path)

    return (Get-Content -LiteralPath $Path).Count
}

function Get-FileNamespace {
    param([string] $Text)

    $namespaceMatch = [regex]::Match($Text, "(?m)^\s*namespace\s+([^;\r\n{]+)")
    if ($namespaceMatch.Success) {
        return $namespaceMatch.Groups[1].Value.Trim()
    }

    return "<global>"
}

$productionFiles = Get-ChildItem -Path $srcRoot -Recurse -Filter "*.cs" |
    Where-Object {
        $_.FullName -notmatch "[/\\]bin[/\\]" -and
        $_.FullName -notmatch "[/\\]obj[/\\]" -and
        $_.FullName -notmatch "[/\\]TestResults[/\\]" -and
        $_.FullName -notmatch "[/\\]tests?[/\\]"
    }

$findings = New-Object System.Collections.Generic.List[object]
$typeFiles = @{}
$typePattern = "(?m)^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:sealed|abstract|static|partial|readonly)\s+)*(?:class|record|struct|enum|interface)\s+([A-Za-z_][A-Za-z0-9_]*)"
$nestedPrivateTypePattern = "(?m)^\s+private\s+(?:(?:sealed|abstract|static|partial|readonly)\s+)*(?:class|record|struct|enum|interface)\s+([A-Za-z_][A-Za-z0-9_]*)"
$statusStringPattern = '"(Passed|Succeeded|Failed|Running|Canceled|TimedOut|Rejected|ValidationFailed|ApprovalRequired|NotExecuted|NotRequired|SimulatedApproved|Valid|Invalid)"'

foreach ($file in $productionFiles) {
    $text = Get-Content -Raw -LiteralPath $file.FullName
    $lineCount = Get-FileLineCount $file.FullName
    $relativePath = Get-RelativePath $file.FullName

    if ($lineCount -gt $MaxFileLines) {
        $findings.Add([pscustomobject]@{
            Rule = "file-lines"
            File = $relativePath
            Type = ""
            Line = ""
            Detail = "$lineCount lines; limit $MaxFileLines"
        })
    }

    $namespace = Get-FileNamespace $text
    foreach ($match in [regex]::Matches($text, $typePattern)) {
        $fullTypeName = "$namespace.$($match.Groups[1].Value)"
        if (-not $typeFiles.ContainsKey($fullTypeName)) {
            $typeFiles[$fullTypeName] = New-Object System.Collections.Generic.List[object]
        }

        $typeFiles[$fullTypeName].Add([pscustomobject]@{
            File = $relativePath
            Lines = $lineCount
        })
    }

    foreach ($match in [regex]::Matches($text, $nestedPrivateTypePattern)) {
        $lineNumber = ($text.Substring(0, $match.Index) -split "`n").Count
        $findings.Add([pscustomobject]@{
            Rule = "nested-private-type"
            File = $relativePath
            Type = $match.Groups[1].Value
            Line = $lineNumber
            Detail = "private nested type candidate"
        })
    }

    $statusMatches = Select-String -LiteralPath $file.FullName -Pattern $statusStringPattern -AllMatches
    foreach ($statusMatch in $statusMatches) {
        foreach ($match in $statusMatch.Matches) {
            $findings.Add([pscustomobject]@{
                Rule = "status-string-candidate"
                File = $relativePath
                Type = ""
                Line = $statusMatch.LineNumber
                Detail = $match.Value
            })
        }
    }
}

foreach ($entry in $typeFiles.GetEnumerator()) {
    $totalLines = ($entry.Value | Measure-Object Lines -Sum).Sum
    if ($totalLines -gt $MaxLogicalTypeLines) {
        $findings.Add([pscustomobject]@{
            Rule = "logical-type-lines"
            File = ($entry.Value.File -join "; ")
            Type = $entry.Key
            Line = ""
            Detail = "$totalLines aggregate lines; limit $MaxLogicalTypeLines"
        })
    }
}

if ($findings.Count -eq 0) {
    Write-Output "Code organization gate: no findings."
    exit 0
}

Write-Output "Code organization gate: $($findings.Count) finding(s)."
$findings |
    Sort-Object Rule, File, Type, Line |
    Format-Table Rule, File, Type, Line, Detail -AutoSize -Wrap

exit 1
